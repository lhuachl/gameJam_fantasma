# Utilities - Código Reutilizable

Esta carpeta contiene código reutilizable, abstracciones e infraestructura que puede ser usada por múltiples sistemas del juego.

## 📂 Estructura

```
Utilities/
├── Events/          # Sistema de eventos Pub/Sub
│   ├── EventManager.cs
│   └── GameEvents.cs
│
└── Interfaces/      # Contratos e interfaces
    ├── IDamageable.cs
    ├── IEnemy.cs
    └── IBoss.cs
```

## 📡 Events/ - Sistema de Eventos

Sistema de comunicación desacoplada tipo Pub/Sub que permite a los sistemas comunicarse sin conocerse directamente.

### EventManager.cs
**Gestor centralizado de eventos** (95 líneas)

#### Características
- ✅ Pattern Pub/Sub estático
- ✅ Type-safe con genéricos
- ✅ Debug mode para visualizar suscripciones
- ✅ Fácil Subscribe/Unsubscribe
- ✅ Broadcast a múltiples handlers

#### API

```csharp
// Suscribirse a un evento
EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);

// Desuscribirse de un evento
EventManager.Unsubscribe<BossDefeatedEvent>(OnBossDefeated);

// Broadcast un evento
EventManager.Broadcast(new BossDefeatedEvent 
{ 
    bossId = "boss1", 
    levelNumber = 3 
});

// Debug: Ver suscripciones
EventManager.DebugPrintSubscribers();

// Limpiar todos los suscriptores
EventManager.ClearAllSubscribers();
```

#### Ejemplo Completo

```csharp
public class LevelManager : MonoBehaviour
{
    private void Start()
    {
        // Suscribirse en Start
        EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);
        EventManager.Subscribe<LevelCompleteEvent>(OnLevelComplete);
    }
    
    private void OnDestroy()
    {
        // IMPORTANTE: Desuscribirse en OnDestroy
        EventManager.Unsubscribe<BossDefeatedEvent>(OnBossDefeated);
        EventManager.Unsubscribe<LevelCompleteEvent>(OnLevelComplete);
    }
    
    private void OnBossDefeated(BossDefeatedEvent e)
    {
        Debug.Log($"Boss {e.bossId} derrotado en nivel {e.levelNumber}");
        UnlockFinalDoor();
    }
    
    private void OnLevelComplete(LevelCompleteEvent e)
    {
        Debug.Log($"Nivel {e.levelNumber} completado");
        ShowDecisionUI();
    }
}
```

### GameEvents.cs
**Definiciones de eventos** (110 líneas)

13 tipos de eventos predefinidos, todos heredan de `GameEvent`:

#### Boss y Nivel
- `BossDefeatedEvent` - Boss derrotado
  - `bossId`: string
  - `levelNumber`: int
  
- `LevelCompleteEvent` - Nivel completado
  - `levelNumber`: int
  
- `DecisionMadeEvent` - Decisión BUENO/MALO realizada
  - `chosenPath`: string ("good" o "bad")
  - `levelNumber`: int

#### Jugador
- `PlayerTakeDamageEvent` - Jugador recibe daño
  - `damageAmount`: int
  - `remainingHealth`: int
  - `damagePosition`: Vector2
  
- `PlayerDiedEvent` - Jugador muere
  - `lastLevel`: int
  - `deathPosition`: Vector2
  
- `PlayerTookUpgradeEvent` - Jugador recoge upgrade
  - `upgradeId`: string
  - `upgradeConfig`: UpgradeConfig
  
- `PlayerJumpedEvent` - Jugador salta
  - `jumpPosition`: Vector2
  
- `PlayerDashedEvent` - Jugador hace dash
  - `startPosition`: Vector2
  - `endPosition`: Vector2
  - `isIntangible`: bool
  
- `PlayerAttackedEvent` - Jugador ataca
  - `attackPosition`: Vector2
  - `damageDealt`: int

#### Enemigos
- `EnemyDefeatedEvent` - Enemigo derrotado
  - `enemyType`: char (X, Y, Z, V)
  - `deathPosition`: Vector2
  
- `EnemySpawnedEvent` - Enemigo spawneado
  - `enemyType`: char
  - `spawnPosition`: Vector2

#### Sistema
- `LevelLoadedEvent` - Nivel cargado
  - `levelNumber`: int
  - `levelName`: string
  
- `LevelUnloadingEvent` - Nivel descargándose
  - `currentLevel`: int

#### Audio
- `MusicChangeRequestedEvent` - Cambio de música solicitado
  - `musicClipName`: string
  - `fadeDuration`: float
  
- `SFXPlayRequestedEvent` - Efecto de sonido solicitado
  - `sfxClipName`: string
  - `playPosition`: Vector2

#### Guardado
- `GameSavedEvent` - Juego guardado
  - `saveLocation`: string

## 🔌 Interfaces/ - Contratos

Interfaces que definen contratos claros para componentes del juego.

### IDamageable.cs
**Interfaz para entidades que reciben daño** (20 líneas)

```csharp
public interface IDamageable
{
    bool TakeDamage(int damage, Vector2 knockbackDir, float knockbackForce);
    int GetCurrentHealth();
    int GetMaxHealth();
    bool IsAlive();
    Vector3 GetPosition();
}
```

**Implementado por:**
- PlayerController
- BaseEnemy (cuando se implemente)
- BaseBoss (cuando se implemente)

### IEnemy.cs
**Interfaz para enemigos** (20 líneas)

Hereda de `IDamageable` y agrega:

```csharp
public interface IEnemy : IDamageable
{
    char GetEnemyType();  // X, Y, Z, V
    bool CanSeePlayer();
    void AttackPlayer();
    bool IsAttacking();
}
```

**Para implementar en:**
- BaseEnemy
- BasicEnemy, FastEnemy, StrongEnemy, FlyingEnemy

### IBoss.cs
**Interfaz para bosses** (18 líneas)

Hereda de `IDamageable` y agrega:

```csharp
public interface IBoss : IDamageable
{
    string GetBossId();
    void EnterBattle();
    void ExitBattle();
    int GetCurrentPhase();
}
```

**Para implementar en:**
- BaseBoss
- Boss1, Boss2, FinalBoss

## 🎯 Ventajas del Sistema

### EventManager
✅ **Desacoplamiento Total**: Sistemas no se conocen entre sí
✅ **Fácil Debugging**: Logs muestran quién escucha qué
✅ **Escalable**: Agregar nuevos eventos es trivial
✅ **Type-Safe**: Genéricos evitan errores de casteo
✅ **Performance**: Dictionary lookup es O(1)

### Interfaces
✅ **Contratos Claros**: API definida explícitamente
✅ **Testeable**: Fácil crear mocks para testing
✅ **Polimorfismo**: Tratar diferentes tipos de forma uniforme
✅ **Documentación**: Interfaz como documentación viviente
✅ **Mantenibilidad**: Cambios controlados por contrato

## 📝 Guías de Uso

### Crear un Nuevo Evento

1. En `GameEvents.cs`, agrega tu clase de evento:
```csharp
public class MyCustomEvent : GameEvent
{
    public string myData;
    public int myNumber;
}
```

2. Úsalo en cualquier parte:
```csharp
EventManager.Broadcast(new MyCustomEvent 
{ 
    myData = "test", 
    myNumber = 42 
});
```

### Implementar una Interfaz

```csharp
public class MyEnemy : MonoBehaviour, IEnemy
{
    // Implementar todos los métodos de IDamageable
    public bool TakeDamage(int damage, Vector2 dir, float force)
    {
        // Tu lógica de daño
        return true;
    }
    
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;
    public Vector3 GetPosition() => transform.position;
    
    // Implementar métodos de IEnemy
    public char GetEnemyType() => 'X';
    public bool CanSeePlayer() => /* detección */ true;
    public void AttackPlayer() { /* ataque */ }
    public bool IsAttacking() => isAttacking;
}
```

## ⚠️ Mejores Prácticas

### EventManager
✅ **HACER**:
- Desuscribirse en `OnDestroy()`
- Suscribirse en `Start()` o `Awake()`
- Usar eventos para comunicación cross-system
- Activar debug mode durante desarrollo

❌ **NO HACER**:
- No olvidar desuscribirse (memory leaks)
- No suscribirse múltiples veces al mismo evento
- No usar eventos para comunicación interna de un sistema
- No abusar de eventos para todo (a veces referencias directas son mejores)

### Interfaces
✅ **HACER**:
- Implementar TODOS los métodos
- Usar para polimorfismo
- Crear interfaces pequeñas y enfocadas
- Documentar comportamiento esperado

❌ **NO HACER**:
- No crear interfaces gigantes
- No agregar lógica en interfaces (solo contratos)
- No duplicar métodos en múltiples interfaces sin herencia

## 📚 Referencias

- Ver `ARQUITECTURA_BRUTAL.txt` para diseño completo
- Ver `Core/GameManager.cs` para uso de eventos
- Ver `Entities/Player/PlayerController.cs` para implementación de IDamageable

---

**Última actualización**: 2025-11-09
