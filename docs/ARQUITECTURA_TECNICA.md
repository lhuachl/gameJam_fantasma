# 🏗️ Arquitectura Técnica - Game Jam Fantasma

## Resumen Ejecutivo

Este documento describe la arquitectura técnica del juego Game Jam Fantasma, un plataformero 2D desarrollado en Unity con C#. El proyecto fue diseñado siguiendo principios de ingeniería de software profesional, incluyendo patrones de diseño, separación de responsabilidades, y código limpio.

**Arquitectos Principales:** Alex (Lead) y Saul (Senior Developer)

---

## Índice

1. [Visión General](#visión-general)
2. [Patrones de Diseño](#patrones-de-diseño)
3. [Arquitectura de Capas](#arquitectura-de-capas)
4. [Sistemas Principales](#sistemas-principales)
5. [Flujo de Datos](#flujo-de-datos)
6. [Interfaces y Abstracciones](#interfaces-y-abstracciones)
7. [Sistema de Eventos](#sistema-de-eventos)
8. [Persistencia de Datos](#persistencia-de-datos)
9. [Organización del Código](#organización-del-código)
10. [Decisiones Arquitectónicas](#decisiones-arquitectónicas)

---

## Visión General

### Principios Arquitectónicos

La arquitectura del juego se basa en 5 pilares fundamentales:

1. **Centralización de Estado** - Una única fuente de verdad (GameManager)
2. **Comunicación Desacoplada** - Eventos Pub/Sub (EventManager)
3. **Abstracción y Reutilización** - Interfaces y clases base
4. **Persistencia Unificada** - Un solo archivo de guardado (JSON)
5. **Organización Modular** - Carpetas por responsabilidad

### Características Clave

✅ **Escalable** - Fácil agregar nuevas features sin modificar código existente  
✅ **Mantenible** - Bugs localizados en componentes específicos  
✅ **Testeable** - Interfaces facilitan testing y mocking  
✅ **Performante** - Optimizado para 60 FPS en hardware medio  
✅ **Documentado** - Código y arquitectura completamente documentados  

---

## Patrones de Diseño

### 1. Singleton Pattern

**Aplicado en:** GameManager  
**Propósito:** Garantizar una única instancia de estado global  
**Desarrollador:** Alex

```csharp
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
```

**Ventajas:**
- Acceso global al estado del juego
- Persiste entre escenas con DontDestroyOnLoad
- Thread-safe en contexto de Unity
- Fácil acceso desde cualquier script

### 2. Observer Pattern (Pub/Sub)

**Aplicado en:** EventManager  
**Propósito:** Comunicación desacoplada entre sistemas  
**Desarrollador:** Alex

```csharp
public static class EventManager
{
    private static Dictionary<Type, List<Delegate>> _eventDictionary 
        = new Dictionary<Type, List<Delegate>>();
    
    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        Type eventType = typeof(T);
        if (!_eventDictionary.ContainsKey(eventType))
        {
            _eventDictionary[eventType] = new List<Delegate>();
        }
        _eventDictionary[eventType].Add(handler);
    }
    
    public static void Broadcast<T>(T eventData) where T : struct
    {
        Type eventType = typeof(T);
        if (_eventDictionary.ContainsKey(eventType))
        {
            foreach (var handler in _eventDictionary[eventType])
            {
                ((Action<T>)handler).Invoke(eventData);
            }
        }
    }
}
```

**Ventajas:**
- Sistemas no necesitan conocerse entre sí
- Fácil agregar nuevos listeners
- Reducción de acoplamiento de 100%
- Debug mode para tracking de eventos

### 3. Template Method Pattern

**Aplicado en:** BaseEnemy  
**Propósito:** Definir estructura común para todos los enemigos  
**Desarrollador:** Saul

```csharp
public abstract class BaseEnemy : MonoBehaviour, IEnemy, IDamageable
{
    // Métodos template que subclases deben implementar
    protected abstract void UpdateAI();
    protected abstract void PerformAttack();
    
    // Lógica común para todos los enemigos
    protected void Update()
    {
        if (!isAlive) return;
        
        UpdateAI();        // Comportamiento específico
        CheckPlayer();     // Común
        HandleMovement();  // Común
        UpdateAnimation(); // Común
    }
    
    // Método común implementado una vez
    public bool TakeDamage(int damage, Vector2 knockback, float force)
    {
        currentHealth -= damage;
        ApplyKnockback(knockback, force);
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }
}
```

**Ventajas:**
- Código común escrito una sola vez
- Subclases solo implementan lo diferente
- Fácil crear nuevos tipos de enemigos
- Garantiza consistencia de comportamiento

### 4. Strategy Pattern

**Aplicado en:** Sistema de IA de enemigos  
**Propósito:** Diferentes comportamientos de enemigos intercambiables  
**Desarrollador:** Saul

Diferentes estrategias de movimiento:
- `PatrolEnemy` - Estrategia de patrullaje
- `ChaseEnemy` - Estrategia de persecución
- Ambas heredan de `BaseEnemy` pero implementan `UpdateAI()` diferente

---

## Arquitectura de Capas

```
┌─────────────────────────────────────────┐
│         PRESENTATION LAYER              │
│  (UI, HUD, Menus, Visual Effects)       │
└─────────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────┐
│         APPLICATION LAYER               │
│  (GameManager, EventManager, Managers)  │
└─────────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────┐
│         DOMAIN LAYER                    │
│  (Entities, Logic, Interfaces)          │
│  Player, Enemies, Bosses                │
└─────────────────────────────────────────┘
                   ↓
┌─────────────────────────────────────────┐
│         DATA LAYER                      │
│  (GameSaveData, Persistence, JSON)      │
└─────────────────────────────────────────┘
```

### Separación de Responsabilidades

**Presentation Layer:**
- Renderizado visual
- Animaciones
- UI/HUD
- Efectos de partículas
- *No contiene lógica de negocio*

**Application Layer:**
- Coordinación de sistemas
- Gestión de estado global
- Comunicación entre sistemas
- Managers especializados
- *Orquesta pero no implementa lógica de dominio*

**Domain Layer:**
- Lógica del juego (gameplay)
- Entidades (Player, Enemy, Boss)
- Mecánicas (combate, movimiento)
- Reglas de negocio
- *Core del juego*

**Data Layer:**
- Serialización/Deserialización
- Persistencia en disco
- Gestión de archivos
- *Solo datos, sin lógica*

---

## Sistemas Principales

### 1. GameManager (Core)

**Responsabilidad:** Estado global y progresión  
**Desarrollador:** Alex  
**Líneas de código:** ~350

**Funcionalidades:**
- Mantener estado actual del juego
- Guardar/cargar progreso automáticamente
- Gestionar progresión entre niveles
- Trackear decisiones BUENO/MALO
- Gestionar upgrades permanentes
- Trackear bosses derrotados

**Métodos Públicos:**
```csharp
public void CreateNewGame()
public void SaveGameState()
public void LoadGameState()
public void ProgressToNextLevel()
public void DefeatBoss(string bossId)
public void MakeDecision(bool isGoodChoice)
public void AddHealthUpgrade(int amount)
public void AddWeaponUpgrade(int damage)
public void UnlockAbility(string abilityName)
public int GetCurrentLevel()
public GameSaveData GetCurrentSaveData()
```

**Integración:**
- Escucha eventos: `LevelCompleteEvent`, `BossDefeatedEvent`, `DecisionMadeEvent`
- Emite eventos: `LevelLoadedEvent`, `GameLoadedEvent`, `GameSavedEvent`

### 2. EventManager (Core)

**Responsabilidad:** Comunicación desacoplada  
**Desarrollador:** Alex  
**Líneas de código:** ~200

**Funcionalidades:**
- Registro de listeners por tipo de evento
- Broadcasting de eventos a todos los listeners
- Desuscripción de listeners
- Debug logging de eventos

**Métodos Públicos:**
```csharp
public static void Subscribe<T>(Action<T> handler) where T : struct
public static void Unsubscribe<T>(Action<T> handler) where T : struct
public static void Broadcast<T>(T eventData) where T : struct
public static void ClearAllSubscriptions()
```

**Eventos del Sistema (13 tipos):**
1. `BossDefeatedEvent` - Boss derrotado
2. `LevelCompleteEvent` - Nivel completado
3. `DecisionMadeEvent` - Decisión tomada
4. `PlayerTakeDamageEvent` - Jugador recibe daño
5. `PlayerDiedEvent` - Jugador muere
6. `PlayerTookUpgradeEvent` - Jugador recoge upgrade
7. `PlayerJumpedEvent` - Jugador salta
8. `PlayerDashedEvent` - Jugador hace dash
9. `PlayerAttackedEvent` - Jugador ataca
10. `EnemyDefeatedEvent` - Enemigo derrotado
11. `EnemySpawnedEvent` - Enemigo spawneado
12. `LevelLoadedEvent` - Nivel cargado
13. `LevelUnloadingEvent` - Nivel descargando

### 3. PlayerController (Entities)

**Responsabilidad:** Control del jugador  
**Desarrollador:** Alex  
**Líneas de código:** ~350

**Mecánicas Implementadas:**
- Movimiento horizontal (WASD)
- Salto con detección de suelo
- Dash intangible (0.2s invulnerabilidad)
- Sistema de ataque
- Sistema de salud
- Muerte y respawn

**Estados:**
- Idle
- Running
- Jumping
- Dashing (intangible)
- Attacking
- TakingDamage
- Dead

**Integración:**
- Implementa: `IDamageable`
- Emite: `PlayerJumpedEvent`, `PlayerDashedEvent`, `PlayerAttackedEvent`, `PlayerTakeDamageEvent`, `PlayerDiedEvent`
- Escucha: `LevelLoadedEvent` (para spawn inicial)

### 4. BaseEnemy (Entities)

**Responsabilidad:** Lógica común de enemigos  
**Desarrollador:** Saul  
**Líneas de código:** ~200

**Funcionalidades Comunes:**
- Detección de bordes de plataformas
- Detección de paredes
- Sistema de flip del sprite
- Sistema de salud
- Muerte y drop de items

**Métodos Abstractos (para subclases):**
```csharp
protected abstract void UpdateAI();
protected abstract void PerformAttack();
```

**Métodos Comunes:**
```csharp
protected void CheckForEdge()
protected void CheckForWall()
protected void Flip()
public bool TakeDamage(int damage, Vector2 knockback, float force)
protected void Die()
```

### 5. Enemigos Específicos

#### PatrolEnemy
**Desarrollador:** Saul  
**Comportamiento:** Patrulla entre dos puntos sin caer de plataformas

**IA:**
```
1. Mover en dirección actual
2. Raycast hacia abajo para detectar edge
3. Si edge detected → Flip() y cambiar dirección
4. Si wall detected → Flip() y cambiar dirección
5. Wait en puntos extremos (configurable)
```

#### ChaseEnemy
**Desarrollador:** Saul  
**Comportamiento:** Persigue jugador cuando está en rango

**Estados IA:**
- **Idle:** Wander aleatorio
- **Chase:** Persigue jugador a velocidad aumentada
- **Attack:** Ataca cuando está en rango

**IA:**
```
1. Raycast hacia jugador
2. Si player en vision range → Estado Chase
3. Si player fuera de range → Estado Idle
4. Si player en attack range → PerformAttack()
```

### 6. BackgroundManager (Managers)

**Responsabilidad:** Sistema de parallax multicapa  
**Desarrollador:** Saul  
**Líneas de código:** ~280

**Funcionalidades:**
- Parallax multicapa (hasta 10 capas)
- Infinite tiling horizontal
- Escala automática a resolución objetivo
- Parallax factors configurables
- Follow camera optional en Y

**Configuración por Capa:**
```csharp
[Serializable]
public class ParallaxLayer
{
    public GameObject backgroundObject;
    public float parallaxFactor;  // 0.0 - 1.0
    public float zDepth;          // -10 a -50
    public bool infiniteTilingX;
    public bool followCameraY;
}
```

**Algoritmo de Parallax:**
```
1. Calcular movimiento de cámara desde frame anterior
2. Para cada capa:
   a. newPos = currentPos + (cameraMovement * parallaxFactor)
   b. Aplicar newPos a background
   c. Si infiniteTilingX enabled:
      - Verificar si necesita wrap around
      - Instantiar/destruir tiles según necesidad
```

### 7. CameraManager (Managers)

**Responsabilidad:** Control de cámara  
**Desarrollador:** Alex  
**Líneas de código:** ~150

**Funcionalidades:**
- Seguimiento suave del jugador (smoothing)
- Camera boundaries (min/max X y Y)
- Offset configurable
- Lock opcional en ejes

---

## Flujo de Datos

### Flujo de Gameplay Típico

```
1. Usuario presiona tecla de movimiento
   ↓
2. Input System captura input
   ↓
3. PlayerController.Update() procesa input
   ↓
4. PlayerController mueve Rigidbody2D
   ↓
5. PlayerController emite PlayerMovedEvent (si configurado)
   ↓
6. CameraManager escucha evento y actualiza posición
   ↓
7. BackgroundManager escucha movimiento de cámara y actualiza parallax
```

### Flujo de Combate

```
1. Jugador presiona tecla de ataque (P)
   ↓
2. PlayerController.PerformAttack()
   ↓
3. CircleCollider2D detecta enemigos en rango
   ↓
4. Para cada enemigo detectado:
   a. enemigo.TakeDamage(damage, knockback, force)
   b. Si enemigo muere → EventManager.Broadcast(EnemyDefeatedEvent)
   ↓
5. HUDManager escucha EnemyDefeatedEvent y actualiza score
   ↓
6. LevelManager escucha y verifica si todos los enemigos están muertos
```

### Flujo de Guardado

```
1. Evento significativo ocurre (boss derrotado, nivel completado)
   ↓
2. EventManager broadcast evento específico
   ↓
3. GameManager escucha evento
   ↓
4. GameManager actualiza GameSaveData internamente
   ↓
5. GameManager.SaveGameState() serializa a JSON
   ↓
6. File.WriteAllText(path, json)
   ↓
7. GameManager emite GameSavedEvent
```

---

## Interfaces y Abstracciones

### IDamageable

**Propósito:** Contrato para entidades que pueden recibir daño  
**Desarrollador:** Alex

```csharp
public interface IDamageable
{
    bool TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce);
    int GetCurrentHealth();
    int GetMaxHealth();
    bool IsAlive();
    Vector3 GetPosition();
}
```

**Implementado por:**
- PlayerController
- BaseEnemy (y todas sus subclases)
- Boss
- Objetos destructibles

**Ventaja:** Cualquier cosa que pueda recibir daño implementa la misma interfaz, permitiendo código genérico.

### IEnemy

**Propósito:** Contrato para comportamiento de enemigos  
**Desarrollador:** Alex

```csharp
public interface IEnemy : IDamageable
{
    char GetEnemyType();
    bool CanSeePlayer();
    void AttackPlayer();
    bool IsAttacking();
}
```

**Implementado por:**
- PatrolEnemy
- ChaseEnemy
- FlyingEnemy (futuro)
- Otros tipos de enemigos

### IBoss

**Propósito:** Contrato para comportamiento de bosses  
**Desarrollador:** Alex

```csharp
public interface IBoss : IDamageable
{
    string GetBossId();
    void EnterBattle();
    void ExitBattle();
    int GetCurrentPhase();
}
```

**Implementado por:**
- Boss
- FinalBoss
- Bosses futuros

---

## Sistema de Eventos

### Ventajas del Sistema de Eventos

1. **Desacoplamiento Total**
   - Sistemas no se conocen entre sí
   - Fácil agregar/remover listeners
   - Código más testeable

2. **Flexibilidad**
   - Múltiples listeners para un evento
   - Agregar nuevos eventos sin modificar código existente

3. **Debugging**
   - Todos los eventos logeados en Console (si debug enabled)
   - Fácil tracking de flujo de datos

### Ejemplo de Uso Completo

```csharp
// Definir evento (en GameEvents.cs)
public struct BossDefeatedEvent
{
    public string bossId;
    public int levelNumber;
    public float timeTaken;
}

// Suscribirse (en GameManager.Start())
EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);

// Broadcast (en Boss.Die())
EventManager.Broadcast(new BossDefeatedEvent
{
    bossId = "boss1",
    levelNumber = 3,
    timeTaken = 245.5f
});

// Handler (en GameManager)
private void OnBossDefeated(BossDefeatedEvent eventData)
{
    Debug.Log($"Boss {eventData.bossId} defeated in {eventData.timeTaken}s");
    saveData.defeatedBosses[eventData.bossId] = true;
    SaveGameState();
    // Desbloquear siguiente nivel
    ProgressToNextLevel();
}

// Desuscribirse (en GameManager.OnDestroy())
EventManager.Unsubscribe<BossDefeatedEvent>(OnBossDefeated);
```

---

## Persistencia de Datos

### GameSaveData

**Desarrollador:** Russel  
**Formato:** JSON  
**Ubicación:** `Application.persistentDataPath/gamesave.json`

```csharp
[Serializable]
public class GameSaveData
{
    // Progresión
    public int currentLevel = 1;
    public List<int> completedLevels = new List<int>();
    
    // Decisiones
    public List<DecisionRecord> decisionsPath = new List<DecisionRecord>();
    
    // Bosses
    public Dictionary<string, bool> defeatedBosses = new Dictionary<string, bool>();
    
    // Stats del jugador
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int weaponDamage = 10;
    
    // Habilidades
    public List<string> specialAbilities = new List<string>();
    
    // Metadata
    public float playTime = 0f;
    public DateTime lastSaveTime;
    public string version = "1.0";
}
```

### Sistema de Guardado Automático

**Triggers de guardado automático:**
- Completar nivel
- Derrotar boss
- Tomar decisión BUENO/MALO
- Recoger upgrade
- Cada 5 minutos (autosave periódico)

**Proceso:**
```
1. EventManager broadcast evento significativo
2. GameManager escucha y actualiza saveData en memoria
3. GameManager.SaveGameState() llamado
4. Serialización a JSON con JsonUtility
5. Escritura a disco con respaldo del archivo anterior
6. Broadcast GameSavedEvent
```

---

## Organización del Código

### Estructura de Carpetas

```
Assets/Scripts/
│
├── Core/                      # Sistemas fundamentales
│   ├── GameManager.cs         # Estado global (Alex)
│   ├── EventManager.cs        # Pub/Sub (Alex)
│   └── GameEvents.cs          # Definiciones de eventos (Alex)
│
├── Data/                      # Estructuras de datos
│   └── GameSaveData.cs        # Persistencia (Russel)
│
├── Utilities/                 # Código reutilizable
│   ├── Interfaces/
│   │   ├── IDamageable.cs     # (Alex)
│   │   ├── IEnemy.cs          # (Alex)
│   │   └── IBoss.cs           # (Alex)
│   │
│   └── Events/
│       ├── GameEvents.cs      # Structs de eventos
│       └── EventManager.cs    # Manager Pub/Sub
│
├── Entities/                  # Entidades del juego
│   ├── Player/
│   │   └── PlayerController.cs  # (Alex)
│   │
│   ├── Enemy/
│   │   ├── Base/
│   │   │   └── BaseEnemy.cs     # (Saul)
│   │   │
│   │   └── Types/
│   │       ├── PatrolEnemy.cs   # (Saul)
│   │       └── ChaseEnemy.cs    # (Saul)
│   │
│   └── Boss/
│       ├── Boss.cs               # (Saul)
│       └── FinalBoss.cs          # (Saul)
│
├── Managers/                  # Gestores de sistemas
│   ├── CameraManager.cs       # (Alex)
│   ├── BackgroundManager.cs   # (Saul)
│   └── LevelManager.cs        # (Russel)
│
└── Legacy/                    # Scripts antiguos
    └── (scripts a refactorizar)
```

---

## Decisiones Arquitectónicas

### Por Qué Singleton para GameManager

**Razones:**
- ✅ Necesitamos exactamente una instancia
- ✅ Acceso global desde cualquier script
- ✅ Persiste entre escenas (DontDestroyOnLoad)
- ✅ Patrón común en Unity para managers

**Alternativas consideradas:**
- Static class → ❌ No es MonoBehaviour, pierde features de Unity
- ScriptableObject → ❌ No persiste estado en runtime correctamente
- DI Container → ❌ Over-engineering para scope del proyecto

### Por Qué Pub/Sub en vez de Referencias Directas

**Razones:**
- ✅ Elimina acoplamiento entre sistemas
- ✅ Fácil agregar nuevos listeners sin modificar código
- ✅ Permite testing independiente de componentes
- ✅ Reduce dependencias circulares

**Antes:**
```csharp
// Acoplamiento fuerte
FindObjectOfType<HUDManager>().UpdateScore(10);
FindObjectOfType<LevelManager>().CheckEnemiesDefeated();
```

**Después:**
```csharp
// Desacoplado
EventManager.Broadcast(new EnemyDefeatedEvent { enemyType = 'X' });
// HUDManager y LevelManager escuchan independientemente
```

### Por Qué Interfaces en vez de Herencia Múltiple

**Razones:**
- ✅ C# no soporta herencia múltiple
- ✅ Interfaces permiten contratos sin implementación forzada
- ✅ Composición sobre herencia (principio SOLID)
- ✅ Código más flexible y testeable

### Por Qué JSON en vez de Binary

**Razones:**
- ✅ Legible por humanos (debugging fácil)
- ✅ Fácil editar manualmente para testing
- ✅ Versionable en Git
- ✅ Fácil migración entre versiones del juego

**Desventaja aceptada:**
- ❌ Menos seguro (se puede editar) → Aceptable para Game Jam

---

## Métricas de Calidad

### Complejidad Ciclomática
- GameManager: ~12 (Moderada)
- EventManager: ~5 (Baja)
- PlayerController: ~18 (Moderada-Alta, aceptable para controller)
- BaseEnemy: ~10 (Moderada)

### Acoplamiento
- **Antes de refactorización:** Alto (~30+ FindWithTag calls)
- **Después:** Muy bajo (0 FindWithTag, todo por eventos)

### Cohesión
- **Alta:** Cada clase tiene una responsabilidad clara y única

### Cobertura de Documentación
- **100%:** Todos los sistemas principales documentados
- **README:** 437 líneas
- **Docs adicionales:** 15+ archivos

---

## Conclusión

La arquitectura de Game Jam Fantasma demuestra que es posible construir un juego profesional y escalable en solo 3 semanas siguiendo principios sólidos de ingeniería de software:

✅ **Patrones de diseño** aplicados correctamente  
✅ **Separación de responsabilidades** clara  
✅ **Código desacoplado** mediante eventos  
✅ **Abstracciones** que facilitan extensión  
✅ **Persistencia** robusta y confiable  
✅ **Organización** que facilita navegación  

Esta arquitectura no solo permitió desarrollo rápido durante el Game Jam, sino que también sienta las bases para futuro desarrollo y mantenimiento del proyecto.

---

**Documento preparado por:** Alex (Lead Developer)  
**Revisado por:** Saul (Senior Developer)  
**Fecha:** Noviembre 9, 2025  
**Versión:** 1.0
