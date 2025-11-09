# Sistema de Enemigos y Fondos Parallax Mejorados

## 📋 Resumen de Mejoras

Este documento describe las mejoras implementadas en el sistema de enemigos y fondos parallax del juego.

### ✅ Mejoras Implementadas

1. **Sistema de Enemigos Estructurado**
   - Clase base abstracta `BaseEnemy` con funcionalidad común
   - Tipos especializados: `PatrolEnemy` y `ChaseEnemy`
   - Detección mejorada de bordes y paredes para patrullaje en plataformas
   - Sistema de visión y persecución del jugador
   - Implementación completa de interfaces `IEnemy` e `IDamageable`

2. **Gestor de Fondos Parallax**
   - `BackgroundManager` centralizado para gestionar múltiples capas
   - Soporte para resoluciones 1920x1080 y 960x540 (mitad)
   - Escalado automático según resolución de pantalla
   - Configuración flexible de factores de parallax por capa
   - Integración con `BackgroundParallaxFill` existente

3. **Spawner de Enemigos Mejorado**
   - `EnemySpawner` con soporte para múltiples tipos de enemigos
   - Sistema de pesos probabilísticos para variedad
   - Límite configurable de enemigos simultáneos
   - Generación por código como fallback

---

## 🎮 Sistema de Enemigos

### BaseEnemy (Clase Base)

**Ubicación:** `Scripts/Entities/Enemy/Base/BaseEnemy.cs`

Clase base abstracta que proporciona:

- **Salud y Daño:** Implementa `IDamageable` con sistema de salud
- **Movimiento:** Lógica de movimiento con detección de obstáculos
- **Detección de Plataformas:**
  - `HasGroundAhead()`: Detecta si hay suelo adelante (previene caídas)
  - `HasWallAhead()`: Detecta paredes
  - `CanMoveInDirection()`: Verifica si puede moverse sin caer
- **Visión y Ataque:**
  - `CanSeePlayer()`: Detecta al jugador en rango de visión
  - `AttackPlayer()`: Sistema de ataque con cooldown
- **Sprites:** Voltea el sprite según la dirección de movimiento

#### Parámetros Configurables

```csharp
[SerializeField] protected int maxHealth = 3;
[SerializeField] protected int attackDamage = 1;
[SerializeField] protected float moveSpeed = 2f;
[SerializeField] protected float visionRange = 5f;
[SerializeField] protected float attackRange = 1.2f;
[SerializeField] protected float attackCooldown = 1f;
[SerializeField] protected LayerMask groundLayer;
[SerializeField] protected float edgeCheckDistance = 0.6f;
[SerializeField] protected float wallCheckDistance = 0.4f;
```

### PatrolEnemy (Enemigo de Patrulla)

**Ubicación:** `Scripts/Entities/Enemy/Types/PatrolEnemy.cs`

**Comportamiento:**
- Patrulla de izquierda a derecha en su plataforma
- Cambia de dirección al llegar a un borde o pared
- Se detiene brevemente al cambiar de dirección
- Persigue al jugador si lo detecta en su rango de visión
- Ataca cuando el jugador está en rango

**Parámetros Adicionales:**
```csharp
[SerializeField] private float patrolWaitTime = 1f;
[SerializeField] private bool startMovingRight = false;
```

**Tipo:** 'X' (básico)

### ChaseEnemy (Enemigo Perseguidor)

**Ubicación:** `Scripts/Entities/Enemy/Types/ChaseEnemy.cs`

**Comportamiento:**
- Más agresivo que PatrolEnemy
- Persigue activamente al jugador cuando lo ve
- Se mueve más rápido durante la persecución (1.5x velocidad base)
- Cuando está inactivo, puede deambular lentamente o quedarse quieto
- No patrulla de forma estructurada como PatrolEnemy

**Parámetros Adicionales:**
```csharp
[SerializeField] private float chaseSpeedMultiplier = 1.5f;
[SerializeField] private float idleWanderChance = 0.3f;
[SerializeField] private float wanderChangeInterval = 2f;
```

**Tipo:** 'Y' (rápido)

### Detección de Plataformas

El sistema usa **Raycasts** para detectar el entorno:

1. **Detección de Bordes:**
   - Lanza un ray hacia abajo desde la posición adelante del enemigo
   - Si no detecta suelo, el enemigo cambia de dirección
   - Previene que los enemigos caigan de las plataformas

2. **Detección de Paredes:**
   - Lanza un ray horizontal en la dirección de movimiento
   - Si detecta una pared, el enemigo cambia de dirección
   - Usa el `groundLayer` para detectar obstáculos sólidos

3. **Visualización de Debug:**
   - Rays verdes = camino libre
   - Rays rojos = obstáculo detectado

#### Configuración en Unity

Para que la detección funcione correctamente:

1. Asignar el **Layer "Ground"** a todas las plataformas y paredes
2. En el Inspector del enemigo, configurar:
   - `Ground Layer`: Seleccionar "Ground"
   - `Edge Check Distance`: 0.6 (distancia de detección de bordes)
   - `Wall Check Distance`: 0.4 (distancia de detección de paredes)

---

## 🎨 Sistema de Fondos Parallax

### BackgroundManager

**Ubicación:** `Scripts/Managers/BackgroundManager.cs`

Gestor centralizado que controla múltiples capas de fondo con efecto parallax.

#### Características

1. **Gestión de Múltiples Capas:**
   - Cada capa tiene su propio factor de parallax
   - Profundidad Z configurable
   - Tiling infinito horizontal opcional
   - Seguimiento de cámara en Y opcional

2. **Soporte de Resoluciones:**
   - **1920x1080** (resolución completa HD)
   - **960x540** (mitad de la resolución HD)
   - Escalado automático de fondos según la resolución objetivo

3. **Integración Automática:**
   - Configura automáticamente `BackgroundParallaxFill` en cada capa
   - Añade el componente si no existe
   - Gestiona parámetros de parallax y tiling

#### Configuración de Capa (ParallaxLayer)

```csharp
[Serializable]
public class ParallaxLayer
{
    public GameObject backgroundObject;      // GameObject del fondo
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;     // 0 = pegado a cámara, 1 = estático
    public float zDepth = -10f;              // Profundidad (negativo = atrás)
    public bool infiniteTilingX = true;      // Tiling infinito horizontal
    public bool followCameraY = false;       // Seguir cámara verticalmente
}
```

#### Cómo Usar en Unity

1. **Crear un GameObject vacío** en la escena llamado "BackgroundManager"
2. **Añadir el script** `BackgroundManager`
3. **Configurar las capas:**
   - Añadir elementos a la lista "Layers"
   - Arrastrar cada GameObject de fondo a su capa
   - Configurar el factor de parallax (más bajo = más rápido)
   - Configurar la profundidad Z (más negativo = más atrás)
4. **Configurar la resolución objetivo:**
   - 1920x1080 para resolución completa
   - 960x540 para mitad de resolución
5. **Asignar la cámara principal** (opcional, se detecta automáticamente)

#### Ejemplo de Configuración

Para un fondo con 3 capas:

```
Capa 0 (Cielo):
  - Background Object: Fondo1
  - Parallax Factor: 0.1 (muy lento, casi estático)
  - Z Depth: -30
  - Infinite Tiling X: true

Capa 1 (Montañas):
  - Background Object: Fondo2
  - Parallax Factor: 0.3 (medio)
  - Z Depth: -20
  - Infinite Tiling X: true

Capa 2 (Árboles):
  - Background Object: Fondo3
  - Parallax Factor: 0.6 (más rápido)
  - Z Depth: -10
  - Infinite Tiling X: true
```

#### API Pública

```csharp
// Añadir capa en runtime
backgroundManager.AddLayer(backgroundObj, parallaxFactor: 0.5f, zDepth: -15f);

// Eliminar capa
backgroundManager.RemoveLayer(backgroundObj);

// Cambiar resolución
backgroundManager.SetTargetResolution(960, 540);

// Cambiar factor de parallax de una capa
backgroundManager.SetLayerParallaxFactor(layerIndex: 0, factor: 0.2f);

// Obtener número de capas
int count = backgroundManager.GetLayerCount();
```

---

## 🎯 EnemySpawner Mejorado

**Ubicación:** `Scripts/Entities/Enemy/EnemySpawner.cs`

### Características

1. **Soporte para Múltiples Tipos:**
   - Configura diferentes prefabs de enemigos
   - Asigna pesos probabilísticos a cada tipo
   - Generación aleatoria basada en pesos

2. **Control de Población:**
   - Límite máximo de enemigos simultáneos
   - Limpieza automática de enemigos muertos

3. **Generación Flexible:**
   - Usa prefabs cuando están disponibles
   - Fallback a generación por código si no hay prefabs

### Configuración en Unity

```
Enemy Types:
  - Enemy Prefab: PatrolEnemyPrefab
    Enemy Type: X
    Spawn Weight: 60 (60% de probabilidad)
  
  - Enemy Prefab: ChaseEnemyPrefab
    Enemy Type: Y
    Spawn Weight: 40 (40% de probabilidad)

Initial Delay: 2.0s
Spawn Interval: 3.0s
Max Enemies: 5
Spawn On Start: true
```

### API Pública

```csharp
// Generar enemigo manualmente
spawner.SpawnEnemy();

// Detener generación
spawner.StopSpawning();

// Reanudar generación
spawner.ResumeSpawning();

// Limpiar todos los enemigos
spawner.ClearAllEnemies();

// Obtener conteo
int count = spawner.GetActiveEnemyCount();
```

---

## 🔧 Migración desde el Sistema Antiguo

### EnemyController Legacy → Nuevos Enemigos

**Antes:**
```
GameObject con:
- EnemyController.cs
```

**Después:**
```
GameObject con:
- PatrolEnemy.cs o ChaseEnemy.cs
- Rigidbody2D (automático)
- BoxCollider2D (automático)
- SpriteRenderer (automático)
```

**Pasos de Migración:**

1. Duplicar el prefab del enemigo antiguo
2. Eliminar el componente `EnemyController`
3. Añadir `PatrolEnemy` o `ChaseEnemy`
4. Configurar parámetros en el Inspector:
   - Max Health, Attack Damage, Move Speed
   - Vision Range, Attack Range
   - Ground Layer (importante!)
5. Probar en una escena de prueba

### GeneradordeBichos → EnemySpawner

**Antes:**
```csharp
public class GeneradordeBichos : MonoBehaviour
{
    public GameObject EnemyPrefab;
    // ...
}
```

**Después:**
```csharp
public class EnemySpawner : MonoBehaviour
{
    // Configurar múltiples tipos con pesos
}
```

**Pasos de Migración:**

1. Reemplazar `GeneradordeBichos` con `EnemySpawner`
2. Configurar la lista "Enemy Types"
3. Asignar prefabs y pesos
4. Configurar intervalos y límites

---

## 📝 Notas Importantes

### Layers Requeridos

Asegúrate de tener configurados estos Layers en Unity:

1. **Ground** - Para plataformas y paredes (usado por enemigos)
2. **Player** - Para el jugador (usado por detección)
3. **Enemy** - Para enemigos (opcional, para organización)

### Configuración de Físicas

Los enemigos requieren:

- **Rigidbody2D:**
  - Gravity Scale: 1.0
  - Constraints: Freeze Rotation Z
  
- **Collider2D:**
  - BoxCollider2D o CapsuleCollider2D
  - Is Trigger: false

### Performance

- **Detección por Raycast:** Más eficiente que Physics2D.OverlapCircle
- **Límite de Enemigos:** Configura `maxEnemies` en EnemySpawner
- **Parallax Layers:** 3-5 capas es óptimo para performance

---

## 🐛 Solución de Problemas

### Los enemigos caen de las plataformas

**Solución:**
1. Verificar que las plataformas tengan el Layer "Ground"
2. Asignar "Ground Layer" en el Inspector del enemigo
3. Ajustar `edgeCheckDistance` (aumentar si las plataformas son pequeñas)

### Los enemigos no ven al jugador

**Solución:**
1. Verificar que el jugador tenga el tag "Player"
2. Aumentar `visionRange` en el Inspector
3. Verificar que el enemigo esté mirando hacia el jugador

### Fondos no escalan correctamente

**Solución:**
1. Verificar la resolución objetivo en BackgroundManager
2. Activar "Auto Scale To Screen"
3. Asegurarse de que los sprites tengan el tamaño correcto (1920x1080 o 960x540)

### Fondos no hacen parallax

**Solución:**
1. Verificar que BackgroundManager esté activo
2. Comprobar que cada capa tenga un GameObject asignado
3. Verificar que el `parallaxFactor` no sea 0 o 1
4. Asegurar que la cámara se mueva

---

## 📚 Referencias

- **Interfaces:** `Scripts/Utilities/Interfaces/`
- **Legacy Code:** `Scripts/Legacy/` (no modificar, se mantiene para referencia)
- **Documentación del Proyecto:** `Scripts/README.md`

---

## ✅ Checklist de Integración

Usa esta checklist al integrar el nuevo sistema:

- [ ] Configurar Layers (Ground, Player, Enemy)
- [ ] Crear prefabs de PatrolEnemy y ChaseEnemy
- [ ] Reemplazar spawners antiguos con EnemySpawner
- [ ] Configurar BackgroundManager con las capas de fondo
- [ ] Configurar resolución objetivo (1920x1080 o 960x540)
- [ ] Probar detección de bordes en diferentes plataformas
- [ ] Probar persecución del jugador
- [ ] Verificar que el parallax funcione correctamente
- [ ] Ajustar parámetros de balance (velocidad, daño, etc.)

---

**Última actualización:** 2025-11-09
**Versión:** 1.0
