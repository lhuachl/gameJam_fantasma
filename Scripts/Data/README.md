# Data - Estructuras de Datos

Esta carpeta contiene las estructuras de datos serializables que representan el estado del juego y configuraciones.

## 📦 Contenido

### GameSaveData.cs ✅
**Estructura serializable para guardado del juego** (55 líneas)

Clase que contiene TODO el estado del juego que se persiste en JSON.

#### Estructura

```csharp
[System.Serializable]
public class GameSaveData
{
    // Progresión
    public int currentLevel;                    // Nivel actual (1-based)
    public List<int> completedLevels;           // Niveles completados
    public bool pendingBoss;                    // Hay un boss pendiente?
    
    // Decisiones narrativas
    public List<string> decisionsPath;          // Historial de decisiones ("good"/"bad")
    public int goodEndings;                     // Cantidad de finales buenos
    public int badEndings;                      // Cantidad de finales malos
    
    // Bosses
    public Dictionary<string, bool> defeatedBosses; // Bosses derrotados
    
    // Stats del jugador
    public int maxHealthUpgrades;               // Upgrades de salud
    public int weaponDamageUpgrades;            // Upgrades de daño
    public List<string> specialAbilities;       // Habilidades especiales
    
    // Stats actuales
    public int currentHealth;                   // Salud actual
    public int maxHealth;                       // Salud máxima
    public int weaponDamage;                    // Daño del arma
    
    // Meta
    public float playTime;                      // Tiempo de juego en segundos
    public string lastPlayedDate;               // Última fecha de juego
    
    // Constructor
    public GameSaveData()
    {
        ResetToDefaults();
    }
    
    // Resetear a valores por defecto
    public void ResetToDefaults()
    {
        currentLevel = 1;
        completedLevels = new List<int>();
        pendingBoss = false;
        decisionsPath = new List<string>();
        goodEndings = 0;
        badEndings = 0;
        defeatedBosses = new Dictionary<string, bool>();
        maxHealthUpgrades = 0;
        weaponDamageUpgrades = 0;
        specialAbilities = new List<string>();
        currentHealth = 100;
        maxHealth = 100;
        weaponDamage = 10;
        playTime = 0f;
        lastPlayedDate = "";
    }
}
```

#### Ubicación del Archivo JSON

El archivo se guarda en:
```
Application.persistentDataPath + "/gamesave.json"
```

**Rutas por plataforma:**
- Windows: `C:/Users/[Usuario]/AppData/LocalLow/[Compañía]/[Juego]/gamesave.json`
- macOS: `~/Library/Application Support/[Compañía]/[Juego]/gamesave.json`
- Linux: `~/.config/unity3d/[Compañía]/[Juego]/gamesave.json`

#### Ejemplo de JSON

```json
{
  "currentLevel": 3,
  "completedLevels": [1, 2],
  "pendingBoss": true,
  "decisionsPath": ["good", "bad"],
  "goodEndings": 1,
  "badEndings": 1,
  "defeatedBosses": {
    "boss1": true,
    "boss2": false
  },
  "maxHealthUpgrades": 2,
  "weaponDamageUpgrades": 1,
  "specialAbilities": ["double_jump", "dash"],
  "currentHealth": 120,
  "maxHealth": 140,
  "weaponDamage": 15,
  "playTime": 3600.5,
  "lastPlayedDate": "2025-11-09T02:30:00"
}
```

#### Uso con GameManager

GameManager gestiona toda la serialización/deserialización:

```csharp
// Crear nuevo juego
GameManager.Instance.CreateNewGame();
// Internamente crea GameSaveData y llama ResetToDefaults()

// Guardar
GameManager.Instance.SaveGameState();
// Serializa currentSave a JSON y guarda en disco

// Cargar
GameManager.Instance.LoadGameState();
// Lee JSON del disco y deserializa a currentSave

// Acceder (read-only)
GameSaveData save = GameManager.Instance.GetCurrentSave();
int level = save.currentLevel;
```

#### Campos Importantes

##### Progresión
- `currentLevel`: Nivel en el que está el jugador (1-based)
- `completedLevels`: Lista de niveles ya completados
- `pendingBoss`: Flag para saber si hay un boss pendiente de derrotar

##### Decisiones
- `decisionsPath`: Array de strings "good" o "bad" con las decisiones tomadas
- `goodEndings`: Cantidad de veces que eligió el final bueno
- `badEndings`: Cantidad de veces que eligió el final malo

##### Bosses
- `defeatedBosses`: Dictionary con `"boss1": true`, `"boss2": false`, etc.

##### Stats
- `maxHealthUpgrades`: Cantidad de upgrades de salud recogidos
- `weaponDamageUpgrades`: Cantidad de upgrades de daño recogidos
- `specialAbilities`: Array de strings con IDs de habilidades ("dash", "double_jump", etc.)

##### Salud y Daño
- `currentHealth`: Salud actual del jugador
- `maxHealth`: Salud máxima = 100 + (maxHealthUpgrades * 20)
- `weaponDamage`: Daño del arma = 10 + (weaponDamageUpgrades * 5)

## 📝 Clases de Configuración

### UpgradeConfig ✅
**Configuración de upgrades** (en GameSaveData.cs)

Estructura para definir upgrades:

```csharp
[System.Serializable]
public class UpgradeConfig
{
    public string upgradeId;              // ID único ("health_1", "weapon_1")
    public string upgradeDisplayName;     // Nombre para mostrar
    public int healthBonus;               // Bonus de salud
    public int damageBonus;               // Bonus de daño
    public string specialAbility;         // Habilidad especial (opcional)
}
```

#### Ejemplo de Uso

```csharp
// Definir upgrade
UpgradeConfig healthUpgrade = new UpgradeConfig
{
    upgradeId = "health_upgrade_1",
    upgradeDisplayName = "Corazón Extra",
    healthBonus = 20,
    damageBonus = 0,
    specialAbility = ""
};

// Cuando el jugador lo recoge
GameManager.Instance.AddHealthUpgrade(healthUpgrade.healthBonus);

// Broadcast evento
EventManager.Broadcast(new PlayerTookUpgradeEvent
{
    upgradeId = healthUpgrade.upgradeId,
    upgradeConfig = healthUpgrade
});
```

## 🔄 Flujo de Datos

### Crear Nuevo Juego
```
1. Usuario presiona "New Game"
2. MenuManager.OnNewGameClicked()
3. GameManager.CreateNewGame()
   └─ new GameSaveData()
   └─ ResetToDefaults()
   └─ SaveGameState() → JSON
4. LoadScene("Level_1")
```

### Guardar Progreso
```
1. Evento importante ocurre (boss derrotado, nivel completado)
2. GameManager actualiza currentSave
   - currentSave.defeatedBosses["boss1"] = true
   - currentSave.currentLevel = 4
3. GameManager.SaveGameState()
   └─ JsonUtility.ToJson(currentSave)
   └─ File.WriteAllText(savePath, json)
4. EventManager.Broadcast(GameSavedEvent)
```

### Cargar Progreso
```
1. Usuario presiona "Continue"
2. MenuManager.OnContinueClicked()
3. GameManager.LoadGameState()
   └─ File.ReadAllText(savePath)
   └─ JsonUtility.FromJson<GameSaveData>(json)
   └─ currentSave = deserializedData
4. int level = currentSave.currentLevel
5. LoadScene($"Level_{level}")
```

### Durante el Juego
```
Jugador mata boss
    └─ Boss.Die()
        └─ EventManager.Broadcast(BossDefeatedEvent)
            └─ GameManager.OnBossDefeated()
                └─ currentSave.defeatedBosses["boss1"] = true
                └─ SaveGameState()

Jugador recoge upgrade
    └─ UpgradePickup.OnTriggerEnter()
        └─ GameManager.AddHealthUpgrade(20)
            └─ currentSave.maxHealth += 20
            └─ currentSave.maxHealthUpgrades++
            └─ SaveGameState()
```

## 🎯 Ventajas de Esta Estructura

✅ **Centralizada**: Todo el estado en UN solo lugar
✅ **Serializable**: Fácil convertir a/desde JSON
✅ **Versionable**: Fácil agregar nuevos campos sin romper saves viejos
✅ **Debuggeable**: Puedes abrir el JSON y ver el estado
✅ **Portable**: Copia el JSON entre máquinas

## ⚠️ Notas Importantes

### Al Agregar Nuevos Campos

1. Agregar el campo a `GameSaveData`
2. Inicializarlo en `ResetToDefaults()`
3. Si es necesario, agregar getter/setter en GameManager
4. Los saves viejos serán compatibles (campos nuevos tendrán valores default)

### Compatibilidad con Versiones Antiguas

JsonUtility es permisivo:
- Campos nuevos en código pero no en JSON → Usan valor default
- Campos en JSON pero no en código → Son ignorados

### No Hacer

❌ **No** modifies `currentSave` directamente desde fuera de GameManager
❌ **No** serialices manualmente (deja que GameManager lo haga)
❌ **No** guardes estado en variables estáticas fuera de GameManager
❌ **No** crees múltiples instancias de GameSaveData

### Hacer

✅ **Usar** métodos de GameManager para modificar estado
✅ **Llamar** SaveGameState() después de cambios importantes
✅ **Consultar** GetCurrentSave() para leer (read-only)
✅ **Testear** con diferentes saves (crear JSONs manualmente)

## 🐛 Debugging

### Ver el JSON

1. Ejecuta el juego en Unity
2. Mira la Console: `[GameManager] Inicializado | Ruta: ...`
3. Copia esa ruta y ábrela en un editor de texto
4. Verás el JSON completo del save

### Resetear el Save

Simplemente elimina el archivo `gamesave.json` y el juego creará uno nuevo.

### Crear Save Personalizado

1. Copia el JSON actual
2. Modifícalo manualmente
3. Guárdalo como `gamesave.json`
4. Inicia el juego con "Continue"

## 📚 Referencias

- Ver `Core/GameManager.cs` para cómo se usa GameSaveData
- Ver `Utilities/Events/GameEvents.cs` para eventos relacionados
- Ver documentación Unity de [JsonUtility](https://docs.unity3d.com/ScriptReference/JsonUtility.html)
- Ver [Application.persistentDataPath](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html)

---

**Última actualización**: 2025-11-09
