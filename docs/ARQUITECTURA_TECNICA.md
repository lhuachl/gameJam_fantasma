# 🏗️ Cómo está armado el juego (más o menos)

## TL;DR

Usamos Unity. Hicimos Singletons porque los tutoriales lo recomiendan. Hay un sistema de eventos que la verdad ayudó mucho. Y si, hay una carpeta "Legacy" que mejor no abrir.

---

## Índice (por si te interesa)

1. [Lo Básico](#lo-básico)
2. [Patrones que usamos](#patrones-que-usamos)
3. [Los Sistemas Principales](#los-sistemas-principales)
4. [Cómo se comunican las cosas](#cómo-se-comunican-las-cosas)
5. [Las Interfaces](#las-interfaces)
6. [El Sistema de Guardado](#el-sistema-de-guardado)
7. [Organización del Código](#organización-del-código)
8. [Por qué hicimos lo que hicimos](#por-qué-hicimos-lo-que-hicimos)

---

## Lo Básico

### Los "Principios" que seguimos

Intentamos hacer las cosas bien:

1. **GameManager centralizado** - Todo el estado del juego en un lugar
2. **Sistema de eventos** - Para que las cosas no dependan directamente unas de otras
3. **Interfaces** - Para que el código sea reutilizable (o eso dijeron)
4. **JSON para guardar** - Porque es fácil de debuggear
5. **Carpetas organizadas** - Para no perder los archivos

### Qué salió bien

✅ El juego funciona  
✅ No hay (muchos) bugs  
✅ El código está organizado  
✅ Podemos agregar features sin romper todo  
✅ El sistema de eventos fue una gran idea

### Qué no salió tan bien

❌ Hay código duplicado en algunos lados  
❌ La carpeta "Legacy" tiene cosas raras  
❌ Algunos scripts son muy largos  
❌ No todo está perfectamente optimizado  
❌ Hay TODOs que nunca se hicieron

---

## Patrones que usamos

### 1. Singleton (GameManager)

**Por qué:** Necesitábamos una forma fácil de acceder al estado del juego desde cualquier lado.

**Cómo funciona:**
```csharp
// Código simplificado
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
- Fácil de usar desde cualquier script
- Persiste entre escenas
- Una sola instancia garantizada

**Desventajas:**
- Es un "anti-pattern" según algunos
- Dificulta el testing (no nos importó mucho)
- Puede volverse un "god object"

### 2. Pub/Sub (EventManager)

**Por qué:** Para que los scripts no se llamen directamente entre sí.

**Ejemplo:**
```csharp
// Publicar un evento
EventManager.Broadcast(new EnemyDefeatedEvent { enemyType = 'X' });

// Suscribirse a un evento
EventManager.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);

// Handler
private void OnEnemyDefeated(EnemyDefeatedEvent eventData)
{
    Debug.Log("Enemigo derrotado!");
    // Hacer algo...
}
```

**Ventajas:**
- Los sistemas no se conocen entre sí
- Fácil agregar nuevos listeners
- El código es más limpio

**Por qué funcionó:**
- Eliminó como 30 FindObjectOfType() en Updates
- Podíamos agregar features sin tocar código existente
- Debugging fue más fácil

### 3. Herencia (BaseEnemy)

**Por qué:** Todos los enemigos comparten lógica común.

```csharp
// Clase base
public abstract class BaseEnemy : MonoBehaviour, IEnemy, IDamageable
{
    // Lógica común
    protected void CheckForEdge() { ... }
    protected void Flip() { ... }
    public bool TakeDamage(int damage, ...) { ... }
    
    // Cada enemigo implementa esto diferente
    protected abstract void UpdateAI();
}

// Enemigo específico
public class PatrolEnemy : BaseEnemy
{
    protected override void UpdateAI()
    {
        // Comportamiento de patrulla
    }
}
```

**Qué aprendimos:**
- Herencia es útil cuando hay comportamiento compartido
- Pero no abuses de ella
- A veces composición es mejor (no tuvimos tiempo)

---

## Los Sistemas Principales

### 1. GameManager

**Qué hace:** Guarda todo el estado del juego

**Responsabilidades:**
- Nivel actual
- Vida del jugador
- Decisiones tomadas
- Bosses derrotados
- Upgrades desbloqueados
- Guardar/Cargar progreso

**Métodos importantes:**
```csharp
GameManager.Instance.SaveGameState();        // Guarda todo
GameManager.Instance.LoadGameState();         // Carga todo
GameManager.Instance.ProgressToNextLevel();  // Siguiente nivel
GameManager.Instance.DefeatBoss("boss1");    // Boss derrotado
```

**Problemas que tuvimos:**
- Al principio guardábamos cosas en múltiples lugares (mal)
- Alex lo centralizó todo aquí (bien)
- Ahora funciona (yay)

### 2. EventManager

**Qué hace:** Permite que los scripts se comuniquen sin conocerse

**Eventos que tenemos:**
1. BossDefeatedEvent
2. LevelCompleteEvent  
3. PlayerTakeDamageEvent
4. PlayerDiedEvent
5. EnemyDefeatedEvent
6. Y como 8 más...

**Ejemplo de uso:**
```csharp
// En Boss.cs cuando muere
EventManager.Broadcast(new BossDefeatedEvent 
{ 
    bossId = "boss1",
    levelNumber = 3 
});

// En GameManager escuchando
void Start()
{
    EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);
}

void OnBossDefeated(BossDefeatedEvent data)
{
    Debug.Log($"Boss {data.bossId} derrotado!");
    saveData.defeatedBosses[data.bossId] = true;
    SaveGameState();
}
```

### 3. PlayerController

**Qué hace:** Controla al jugador (obvio)

**Mecánicas:**
- Movimiento (WASD)
- Salto (Space)
- Dash (Shift) - con invulnerabilidad!
- Ataque (clic o P)

**Código importante:**
```csharp
// El dash hace intangible al jugador
if (isDashing)
{
    // Jugador no puede recibir daño
    return false;
}
```

**Evolución:**
- Versión 1: Movimiento básico
- Versión 2: Agregamos salto mejorado
- Versión 3: Dash intangible (después de muchos bugs)
- Versión 4: Refactor para usar eventos
- Versión actual: Funciona bien

### 4. Sistema de Enemigos

**BaseEnemy** - La clase base

Tiene la lógica que todos comparten:
- Detección de bordes (para no caerse)
- Detección de paredes
- Sistema de flip del sprite
- TakeDamage
- Die()

**PatrolEnemy** - El enemigo que patrulla

```csharp
// Pseudocódigo
void UpdateAI()
{
    Mover en dirección actual
    
    if (detecta borde)
        Voltear
        
    if (detecta pared)
        Voltear
}
```

El truco fue hacer que detecte el borde ANTES de caerse. Tardamos como un día en lograr eso.

**ChaseEnemy** - El que te persigue

Tiene 3 estados:
- Idle: Anda random
- Chase: Te persigue si te ve
- Attack: Te ataca si está cerca

### 5. Sistema de Parallax

**BackgroundManager** - Por Saul

Características:
- Múltiples capas de fondo
- Cada capa se mueve a diferente velocidad
- Infinite scrolling (se repite sin que se note)
- Escala automática a 1920x1080

**Cómo funciona:**
```
Capa lejana: se mueve lento (parallax factor 0.2)
Capa media: se mueve medio (parallax factor 0.5)
Capa cercana: se mueve rápido (parallax factor 0.8)
```

Esto da sensación de profundidad. Quedó bien.

### 6. CameraManager

**Qué hace:** La cámara sigue al jugador suavemente

**Features:**
- Smoothing (no se mueve instantáneo)
- Boundaries (no sale del mapa)
- Offset configurable

Código simple pero efectivo.

---

## Cómo se comunican las cosas

### Flujo típico de gameplay

```
1. Jugador presiona tecla
   ↓
2. PlayerController lo procesa
   ↓
3. Se mueve el Rigidbody2D
   ↓
4. (Opcionalmente) Se emite un evento
   ↓
5. Otros sistemas escuchan y reaccionan
```

### Ejemplo: Derrotar un enemigo

```
1. Jugador ataca
   ↓
2. PlayerController detecta colisión
   ↓
3. enemy.TakeDamage(10, knockback, force)
   ↓
4. Enemigo muere
   ↓
5. EventManager.Broadcast(EnemyDefeatedEvent)
   ↓
6. HUDManager actualiza score (si lo escucha)
7. LevelManager verifica si quedan enemigos (si lo escucha)
```

Sin eventos, tendríamos que hacer FindObjectOfType en cada paso. Eso es lento y feo.

---

## Las Interfaces

### IDamageable

Para todo lo que puede recibir daño:

```csharp
interface IDamageable
{
    bool TakeDamage(int damage, Vector2 knockback, float force);
    int GetCurrentHealth();
    int GetMaxHealth();
    bool IsAlive();
    Vector3 GetPosition();
}
```

**Quién lo implementa:**
- Player
- Enemigos
- Bosses
- Objetos destructibles

### IEnemy

Para enemigos:

```csharp
interface IEnemy : IDamageable
{
    char GetEnemyType();
    bool CanSeePlayer();
    void AttackPlayer();
}
```

### IBoss

Para bosses:

```csharp
interface IBoss : IDamageable
{
    string GetBossId();
    void EnterBattle();
    int GetCurrentPhase();
}
```

**Por qué interfaces:**
- El código puede trabajar con "cualquier IDamageable" sin importar si es player o enemigo
- Más flexible
- C# no tiene herencia múltiple de todas formas

---

## El Sistema de Guardado

### GameSaveData

```csharp
[Serializable]
public class GameSaveData
{
    // Progreso
    public int currentLevel = 1;
    public List<int> completedLevels = new List<int>();
    
    // Decisiones
    public List<DecisionRecord> decisionsPath = new List<DecisionRecord>();
    
    // Bosses
    public Dictionary<string, bool> defeatedBosses = new Dictionary<string, bool>();
    
    // Stats
    public int maxHealth = 100;
    public int currentHealth = 100;
    
    // Habilidades
    public List<string> specialAbilities = new List<string>();
    
    // Metadata
    public float playTime = 0f;
    public DateTime lastSaveTime;
}
```

**Dónde se guarda:**
- Application.persistentDataPath + "/gamesave.json"
- En Windows: C:/Users/[usuario]/AppData/LocalLow/[company]/[juego]/
- Es JSON así que puedes editarlo si quieres (para testing)

**Cuándo se guarda:**
- Al completar nivel
- Al derrotar boss
- Cada 5 minutos (autosave)
- Al salir del juego

**Problemas que tuvimos:**
- Versión 1 de Russel borraba todo a veces
- Versión 2 guardaba en lugares random
- Versión final funciona bien

---

## Organización del Código

### Estructura de carpetas

```
Scripts/
├── Core/
│   ├── GameManager.cs
│   ├── EventManager.cs
│   └── GameEvents.cs
│
├── Data/
│   └── GameSaveData.cs
│
├── Entities/
│   ├── Player/
│   │   └── PlayerController.cs
│   │
│   ├── Enemy/
│   │   ├── Base/
│   │   │   └── BaseEnemy.cs
│   │   └── Types/
│   │       ├── PatrolEnemy.cs
│   │       └── ChaseEnemy.cs
│   │
│   └── Boss/
│       ├── Boss.cs
│       └── FinalBoss.cs
│
├── Managers/
│   ├── CameraManager.cs
│   ├── BackgroundManager.cs
│   └── LevelManager.cs
│
├── Utilities/
│   └── Interfaces/
│       ├── IDamageable.cs
│       ├── IEnemy.cs
│       └── IBoss.cs
│
└── Legacy/
    └── (cosas viejas que mejor no tocar)
```

**Regla simple:**
- Si es un sistema core → Core/
- Si guarda datos → Data/
- Si es una entidad del juego → Entities/
- Si maneja algo → Managers/
- Si es genérico → Utilities/
- Si da miedo → Legacy/

---

## Por qué hicimos lo que hicimos

### ¿Por qué Singleton para GameManager?

**Pros:**
- ✅ Fácil de acceder desde cualquier lado
- ✅ Persiste entre escenas
- ✅ Todos los tutoriales lo usan
- ✅ Simple de implementar

**Cons:**
- ❌ Es técnicamente un anti-pattern
- ❌ Puede crecer demasiado
- ❌ Dificulta testing unitario

**Veredicto:** Para un game jam está bien. Para un proyecto grande, quizá usaríamos otra cosa.

### ¿Por qué eventos en vez de referencias directas?

**Antes:**
```csharp
// Acoplamiento fuerte (malo)
FindObjectOfType<HUDManager>().UpdateScore(10);
FindObjectOfType<LevelManager>().CheckEnemiesDefeated();
```

**Después:**
```csharp
// Desacoplado (bueno)
EventManager.Broadcast(new EnemyDefeatedEvent { enemyType = 'X' });
// Quien quiera escuchar, escucha
```

**Por qué es mejor:**
- No hay dependencias directas
- Fácil agregar/remover listeners
- Más fácil de debuggear
- Más rápido (no hay FindObjectOfType)

### ¿Por qué JSON y no binario?

**Razones:**
- ✅ Puedes leer el archivo de guardado
- ✅ Fácil de debuggear
- ✅ Puedes editarlo manualmente para testing
- ✅ Simple de implementar

**Desventajas:**
- ❌ Más grande que binario
- ❌ El jugador puede hacer trampa (no nos importó)

---

## Métricas (porque sí)

### Complejidad
- GameManager: Moderada (tiene bastantes métodos)
- EventManager: Baja (es simple)
- PlayerController: Media-Alta (tiene muchos estados)
- BaseEnemy: Media (maneja varias cosas)

### Acoplamiento
- **Antes:** Alto (FindObjectOfType por todos lados)
- **Después:** Bajo (eventos para todo)

### Bugs encontrados
- **Semana 1:** 15+
- **Semana 2:** 20+
- **Semana 3:** 10+
- **Al entregar:** 2 (conocidos)

---

## Conclusión

Hicimos un juego funcional en 3 semanas. La arquitectura no es perfecta pero:

✅ Funciona  
✅ Es mantenible  
✅ Pudimos agregar features fácilmente  
✅ No es un desastre de código  
✅ Aprendimos mucho

Para un game jam amateur, está bastante bien.

---

## Cosas que haríamos diferente

Si lo hiciéramos de nuevo:

1. Usar un sistema de inyección de dependencias (quizá)
2. Escribir tests (definitivamente)
3. Planear mejor la arquitectura desde el inicio
4. No hacer commits a las 4 AM
5. Hacer más code reviews
6. Documentar mientras programamos, no después

Pero hey, para ser nuestro primer game jam serio, no está mal.

---

**Documento escrito por:** Alex (con ayuda del equipo)  
**Cuando:** Después de entregar (entre el cansancio)  
**Versión:** 1.0 (aka "la única")

*"Si compilas en la primera, algo está mal"* - Ley de Murphy del desarrollo
