# 🏗️ ARQUITECTURA DE SOFTWARE PARA METROIDVANIA

## 📌 ¿Por Qué Metroidvania Cambia Todo?

### Características Clave de un Metroidvania:
1. **Mundo Abierto Interconectado** - No son niveles lineales, es UN mapa grande
2. **Backtracking** - El jugador vuelve a zonas anteriores
3. **Progresión No-Lineal** - Acceso dinámico a áreas (gated por habilidades)
4. **Upgrades y Powerups** - Armas, movimientos, salud, etc.
5. **Múltiples Jefes** - Distribuidos por el mapa, no al final de niveles
6. **Interconexión de Salas** - Transiciones suaves o por puertas
7. **Guardado Frecuente** - Save rooms / checkpoints estratégicos
8. **Exploración** - Secretos, colectibles, atajos

### Tu Proyecto Actual:
```
Linear Level 1 → Linear Level 2 → Linear Level 3 → Boss → Linear Level 4...
     ❌ ESTO NO ESCALA A METROIDVANIA
```

### Lo Que Necesitas:
```
          [Boss 1]
            ↙ ↖
     [Zona A] - [Zona B]
            ↖ ↙
          [Boss 2]
     
     Con múltiples caminos, atajos, upgrades gateados por habilidades
```

---

## 🎯 ARQUITECTURA PROPUESTA PARA METROIDVANIA

### 1. **Gestión de Mundo - WorldManager**

```csharp
public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    
    [SerializeField] private List<Room> allRooms;
    [SerializeField] private List<Boss> allBosses;
    
    private Dictionary<string, Room> roomsById;
    private Dictionary<string, Boss> bossesById;
    
    public Room GetRoom(string roomId) => roomsById[roomId];
    public Boss GetBoss(string bossId) => bossesById[bossId];
    
    public List<Room> GetConnectedRooms(string currentRoomId)
    {
        // Retorna las salas conectadas a la actual
    }
}

[System.Serializable]
public class Room
{
    public string roomId;           // "ForestA", "CaveB", etc.
    public string displayName;      // "Dark Forest"
    public Vector3 playerSpawnPos;
    public List<string> connectedRoomIds;
    public List<Enemy> enemies;
    public List<Collectible> collectibles;
    public RoomType type;           // Normal, BossArena, SaveRoom, TreasureRoom
}

public enum RoomType
{
    Normal,
    BossArena,
    SaveRoom,
    TreasureRoom,
    HubZone
}
```

---

### 2. **Sistema de Progresión - Progression Manager**

```csharp
public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }
    
    // Habilidades del jugador que abren nuevas áreas
    private Dictionary<string, bool> abilities = new();
    
    // Upgrades de stat
    private Dictionary<string, int> statUpgrades = new();
    
    // Jefes derrotados
    private HashSet<string> defeatedBosses = new();
    
    // Colectibles encontrados
    private HashSet<string> foundCollectibles = new();
    
    public bool HasAbility(string abilityId) => abilities.TryGetValue(abilityId, out var has) && has;
    
    public void UnlockAbility(string abilityId)
    {
        abilities[abilityId] = true;
        EventManager.Broadcast(new AbilityUnlockedEvent { abilityId = abilityId });
    }
    
    public void DefeatBoss(string bossId)
    {
        defeatedBosses.Add(bossId);
        EventManager.Broadcast(new BossDefeatedEvent { bossId = bossId });
    }
    
    public bool IsBossDefeated(string bossId) => defeatedBosses.Contains(bossId);
}

// Ejemplo de habilidades que podría tener:
public static class AbilityIds
{
    public const string DOUBLE_JUMP = "double_jump";
    public const string WALL_SLIDE = "wall_slide";
    public const string DASH = "dash";
    public const string CHARGE_ATTACK = "charge_attack";
    public const string ICE_BEAM = "ice_beam";
    public const string MISSILES = "missiles";
}
```

---

### 3. **Sistema de Puertas/Gateado - AccessControl**

```csharp
public interface IAccessible
{
    AccessRequirement GetAccessRequirement();
    bool CanAccess(ProgressionManager progression);
}

[System.Serializable]
public class AccessRequirement
{
    public AccessType type;
    public string requirementId;  // "double_jump", "red_key", "boss_defeated"
    public int minimumLevel;      // Para upgrades progresivos
}

public enum AccessType
{
    FreeAccess,
    RequiresAbility,
    RequiresItem,
    RequiresBossDefeated,
    RequiresKey,
    LevelGated
}

// Uso en puertas, paredes, etc.
public class LockedDoor : MonoBehaviour, IAccessible
{
    [SerializeField] private AccessRequirement requirement;
    private SpriteRenderer spriteRenderer;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        
        var progression = ProgressionManager.Instance;
        if (CanAccess(progression))
        {
            // Abre la puerta
            spriteRenderer.color = Color.green;
            GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            // Muestra mensaje
            UIManager.ShowMessage($"Se requiere: {requirement.requirementId}");
        }
    }
    
    public AccessRequirement GetAccessRequirement() => requirement;
    
    public bool CanAccess(ProgressionManager progression)
    {
        return requirement.type switch
        {
            AccessType.FreeAccess => true,
            AccessType.RequiresAbility => progression.HasAbility(requirement.requirementId),
            AccessType.RequiresBossDefeated => progression.IsBossDefeated(requirement.requirementId),
            _ => false
        };
    }
}
```

---

### 4. **Sistema de Guardado - SaveManager (mejorado)**

```csharp
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    [System.Serializable]
    public class GameSave
    {
        public PlayerSaveData playerData;
        public WorldSaveData worldData;
        public ProgressionSaveData progressionData;
    }
    
    [System.Serializable]
    public class PlayerSaveData
    {
        public int currentHealth;
        public int maxHealth;
        public Vector3 lastCheckpointPosition;
        public string currentRoomId;
        public int playTime;  // en segundos
    }
    
    [System.Serializable]
    public class WorldSaveData
    {
        public List<string> defeatedBosses;
        public List<string> collectiblesFound;
        public List<RoomModification> roomModifications;
    }
    
    [System.Serializable]
    public class ProgressionSaveData
    {
        public Dictionary<string, bool> unlockedAbilities;
        public Dictionary<string, int> statUpgrades;
        public int totalPlayTime;
    }
    
    public void SaveGame(string slotName)
    {
        var save = new GameSave
        {
            playerData = CapturePlayerData(),
            worldData = CaptureWorldData(),
            progressionData = CaptureProgressionData()
        };
        
        string json = JsonConvert.SerializeObject(save);
        string path = GetSavePath(slotName);
        File.WriteAllText(path, json);
        
        EventManager.Broadcast(new GameSavedEvent { slotName = slotName });
    }
    
    public GameSave LoadGame(string slotName)
    {
        string path = GetSavePath(slotName);
        if (!File.Exists(path)) return null;
        
        string json = File.ReadAllText(path);
        var save = JsonConvert.DeserializeObject<GameSave>(json);
        
        EventManager.Broadcast(new GameLoadedEvent { slotName = slotName });
        return save;
    }
    
    public List<string> GetSaveSlots()
    {
        // Retorna nombres de guardos disponibles
    }
}
```

---

### 5. **Sistema de Eventos Centralizado - EventManager**

```csharp
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    private Dictionary<System.Type, System.Delegate> eventDictionary = new();
    
    public static void Subscribe<T>(System.Action<T> handler) where T : GameEvent
    {
        Instance.eventDictionary.TryGetValue(typeof(T), out var @delegate);
        @delegate = System.Delegate.Combine(@delegate, handler);
        Instance.eventDictionary[typeof(T)] = @delegate;
    }
    
    public static void Unsubscribe<T>(System.Action<T> handler) where T : GameEvent
    {
        if (Instance.eventDictionary.TryGetValue(typeof(T), out var @delegate))
        {
            @delegate = System.Delegate.Remove(@delegate, handler);
            Instance.eventDictionary[typeof(T)] = @delegate;
        }
    }
    
    public static void Broadcast<T>(T gameEvent) where T : GameEvent
    {
        if (Instance.eventDictionary.TryGetValue(typeof(T), out var @delegate))
        {
            (@delegate as System.Action<T>)?.Invoke(gameEvent);
        }
    }
}

// Base class para todos los eventos
public abstract class GameEvent { }

// Eventos específicos
public class BossDefeatedEvent : GameEvent { public string bossId; }
public class AbilityUnlockedEvent : GameEvent { public string abilityId; }
public class CollectibleFoundEvent : GameEvent { public string collectibleId; }
public class RoomEnteredEvent : GameEvent { public string roomId; }
public class RoomExitedEvent : GameEvent { public string roomId; }
public class PlayerDiedEvent : GameEvent { public string lastRoomId; }
public class GameSavedEvent : GameEvent { public string slotName; }
public class GameLoadedEvent : GameEvent { public string slotName; }
```

---

### 6. **Enemigos Escalables - Enemy Hierarchy**

```csharp
public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHealth = 10;
    protected int currentHealth;
    protected Rigidbody2D rb;
    protected Transform player;
    
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }
    
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }
    
    protected virtual void Die()
    {
        Destroy(gameObject);
        EventManager.Broadcast(new EnemyDefeatedEvent { enemyType = GetType().Name });
    }
}

// Enemigos específicos
public class BasicEnemy : BaseEnemy
{
    [SerializeField] private float patrolSpeed = 2f;
    
    protected override void Start()
    {
        base.Start();
        // Lógica específica de BasicEnemy
    }
}

public class FastEnemy : BaseEnemy
{
    [SerializeField] private float chaseSpeed = 4f;
    // Más rápido, menor vida
}

public class StrongEnemy : BaseEnemy
{
    [SerializeField] private int armor = 2;
    
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(Mathf.Max(1, damage - armor));
    }
}

public class FlyingEnemy : BaseEnemy
{
    [SerializeField] private float flightHeight = 3f;
    // Se mueve en 3D, diferente IA
}
```

---

### 7. **Jugador Mejorado - Player System**

```csharp
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    
    private Rigidbody2D rb;
    private ProgressionManager progression;
    
    public event System.Action OnHealthChanged;
    public event System.Action OnDeath;
    
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        progression = ProgressionManager.Instance;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        OnDeath?.Invoke();
        EventManager.Broadcast(new PlayerDiedEvent 
        { 
            lastRoomId = GetCurrentRoomId() 
        });
        // Respawn en último checkpoint
    }
    
    public void Jump()
    {
        if (!progression.HasAbility(AbilityIds.DOUBLE_JUMP))
        {
            // Solo un salto normal
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        // Con double jump, permite 2 saltos
    }
}
```

---

### 8. **Gestión de Salas - Room Transitions**

```csharp
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    
    private string currentRoomId;
    private Room currentRoom;
    
    public void TransitionToRoom(string nextRoomId)
    {
        // Guardar estado de la sala actual
        if (!string.IsNullOrEmpty(currentRoomId))
        {
            SaveRoomState(currentRoomId);
            EventManager.Broadcast(new RoomExitedEvent { roomId = currentRoomId });
        }
        
        // Cargar nueva sala
        currentRoomId = nextRoomId;
        currentRoom = WorldManager.Instance.GetRoom(nextRoomId);
        
        // Spawnear enemigos, colectibles, etc.
        LoadRoomState(nextRoomId);
        
        // Posicionar jugador
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentRoom.playerSpawnPos;
        }
        
        EventManager.Broadcast(new RoomEnteredEvent { roomId = nextRoomId });
    }
    
    private void SaveRoomState(string roomId)
    {
        // Guardar enemigos, objetos, estado de la sala
    }
    
    private void LoadRoomState(string roomId)
    {
        // Cargar o recrear enemigos, objetos, etc.
    }
}
```

---

### 9. **Estructura de Carpetas Recomendada**

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── EventManager.cs
│   ├── GameEvents.cs (todos los eventos)
│   └── Constants.cs
│
├── Managers/
│   ├── SaveManager.cs
│   ├── WorldManager.cs
│   ├── RoomManager.cs
│   ├── ProgressionManager.cs
│   └── UIManager.cs
│
├── Player/
│   ├── PlayerController.cs
│   ├── PlayerAbilities.cs
│   ├── PlayerInventory.cs
│   └── PlayerStats.cs
│
├── Entities/
│   ├── Interfaces/
│   │   ├── IDamageable.cs
│   │   ├── IEnemy.cs
│   │   └── IAccessible.cs
│   │
│   ├── Enemy/
│   │   ├── BaseEnemy.cs
│   │   ├── BasicEnemy.cs
│   │   ├── FastEnemy.cs
│   │   ├── StrongEnemy.cs
│   │   └── FlyingEnemy.cs
│   │
│   ├── Boss/
│   │   ├── BaseBoss.cs
│   │   ├── Boss1.cs
│   │   └── FinalBoss.cs
│   │
│   └── Collectible/
│       ├── BaseCollectible.cs
│       ├── HealthPotion.cs
│       ├── AmmoPickup.cs
│       └── AbilityPickup.cs
│
├── World/
│   ├── Room.cs
│   ├── RoomTransition.cs
│   ├── LockedDoor.cs
│   ├── SavePoint.cs
│   └── WorldData.cs
│
├── UI/
│   ├── HUD.cs
│   ├── PauseMenu.cs
│   ├── MainMenu.cs
│   ├── MapUI.cs
│   └── InventoryUI.cs
│
├── Config/
│   ├── GameConfig.cs (ScriptableObject)
│   ├── RoomConfigs/ (uno por sala)
│   └── EnemyConfigs/ (uno por tipo)
│
└── Utilities/
    ├── CameraFollow.cs
    ├── AudioManager.cs
    └── Extensions.cs
```

---

## 🔄 FLUJO DE EJECUCIÓN - METROIDVANIA

```
STARTUP
  ↓
GameManager.Awake()
  ├─ Inicializa SaveManager
  ├─ Inicializa EventManager
  ├─ Inicializa ProgressionManager
  └─ Inicializa WorldManager
      ↓
      WorldManager carga todas las salas (con IDs, conexiones)
        ↓
        SaveManager carga último guardado (si existe)
          ├─ Restaura ProgressionManager (habilidades, upgrades, bosses derrotados)
          ├─ Restaura PlayerController (salud, posición)
          └─ Restaura RoomManager (última sala visitada)
              ↓
              RoomManager carga la sala actual
                ├─ Instantiate enemigos según config
                ├─ Instantiate colectibles
                └─ Posiciona jugador en checkpoint
                    ↓
                    JUEGO EN MARCHA
                    
EVENTOS DURANTE JUEGO:

Jugador entra a sala nueva
  ↓
RoomManager.TransitionToRoom()
  ├─ Salva estado de sala anterior
  ├─ Carga nueva sala
  └─ Broadcast RoomEnteredEvent

Jugador derrota enemigo
  ↓
Enemy.Die()
  └─ Broadcast EnemyDefeatedEvent

Jugador derrota boss
  ↓
Boss.Die()
  ├─ ProgressionManager.DefeatBoss()
  └─ Broadcast BossDefeatedEvent
    ├─ Unlock new ability
    ├─ Update accessible areas

Jugador toma powerup
  ↓
Collectible.OnTriggerEnter()
  ├─ ProgressionManager.UnlockAbility()
  └─ Broadcast AbilityUnlockedEvent
    ├─ PlayerAbilities updatean
    ├─ HUD actualiza

Jugador en Save Point
  ↓
SavePoint.OnTriggerStay()
  └─ Press ENTER → SaveManager.SaveGame()
    └─ Broadcast GameSavedEvent

Jugador muere
  ↓
PlayerController.Die()
  ├─ Broadcast PlayerDiedEvent
  └─ RoomManager respawns en último checkpoint
```

---

## 🎯 VENTAJAS DE ESTA ARQUITECTURA

✅ **Escalable** - Agregar nuevas salas, enemigos, bosses es trivial
✅ **Mantenible** - Cada clase tiene UNA responsabilidad
✅ **Desacoplada** - Los sistemas no se conocen entre sí, usan eventos
✅ **Testeable** - Podés testear cada sistema independientemente
✅ **Flexible** - Cambiar gateado (puerta cerrada → requiere habilidad) es una línea
✅ **Progresión Clara** - Todo trackea qué se desbloqueó, cuándo, dónde
✅ **Guardado Robusto** - Un único SaveManager, sincronizado siempre

---

## 🚀 PRIMEROS PASOS (Orden de Ejecución)

1. **GameManager + SaveManager** (centralizar todo)
2. **EventManager** (comunicación entre sistemas)
3. **ProgressionManager** (rastrear habilidades/bosses)
4. **WorldManager** (cargar salas)
5. **RoomManager** (transiciones)
6. **BaseEnemy + tipos** (refactorizar enemigos)
7. **PlayerController mejorado** (con habilidades)
8. **UI** (HUD, mapa, inventario)

---

## 💡 DIFERENCIAS vs NIVEL LINEAL

| Aspecto | Nivel Lineal | Metroidvania |
|---------|-------------|-------------|
| Guardado | Uno al final de nivel | Múltiples checkpoints |
| Progresión | Lineal (1→2→3) | No-lineal (cualquier orden) |
| Acceso | Todo desbloqueado | Gated por habilidades |
| Enemigos | Desaparecen al pasar | Persisten, pueden backtrackear |
| Bosses | Distribuidos/secuencial | Esparcidos por el mapa |
| Mapa | Niveles independientes | Un mundo interconectado |
| Guardado JSON | { nivel } | { habilidades, bosses, posición, etc } |

¿Esto tiene más sentido ahora? ¿Quieres que empecemos a implementar?
