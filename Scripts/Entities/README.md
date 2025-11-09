# Entities - Entidades del Juego

Esta carpeta contiene las entidades principales del juego: jugador, enemigos y bosses. Cada tipo de entidad está organizada en su propia subcarpeta.

## 📂 Estructura

```
Entities/
├── Player/          # Jugador
│   └── PlayerController.cs (✅ Implementado)
│
├── Enemy/           # Enemigos
│   ├── Base/        # (Pendiente: BaseEnemy.cs)
│   └── Types/       # (Pendiente: tipos específicos)
│
└── Boss/            # Bosses
    └── (Pendiente: BaseBoss.cs + bosses específicos)
```

## 🎮 Player/ - Control del Jugador

### PlayerController.cs ✅
**Control completo del jugador** (350 líneas)

Sistema robusto que gestiona todas las mecánicas del jugador.

#### Características Implementadas
- ✅ **Input System integrado** - Configurado en código, no requiere assets externos
- ✅ **Movimiento fluido** - Con aceleración y desaceleración natural
- ✅ **Salto mejorado** - Detección de suelo precisa con GroundCheck
- ✅ **Dash intangible** - Dash con invulnerabilidad temporal
- ✅ **Sistema de combate** - Ataque con detección de enemigos en rango
- ✅ **Sistema de salud** - TakeDamage, muerte y respawn
- ✅ **Implementa IDamageable** - Interfaz estándar para recibir daño
- ✅ **Broadcasting de eventos** - Integrado con EventManager

#### Controles

| Acción | Tecla(s) | Input System |
|--------|----------|--------------|
| Mover | WASD / Flechas | "Move" (Vector2) |
| Saltar | Space | "Jump" |
| Dash | Y | "Dash" |
| Atacar | P | "Attack" |

#### Configuración en Inspector

```
[SerializeField] Parámetros de Movimiento:
- moveSpeed: float = 5f            // Velocidad de movimiento
- jumpForce: float = 10f           // Fuerza de salto
- dashForce: float = 15f           // Fuerza de dash
- dashDuration: float = 0.2f       // Duración del dash
- dashCooldown: float = 1f         // Cooldown entre dashes

[SerializeField] Parámetros de Combate:
- attackDamage: int = 10           // Daño base del ataque
- attackRange: float = 1.5f        // Rango del ataque
- attackCooldown: float = 0.5f     // Cooldown entre ataques

[SerializeField] Referencias:
- groundCheck: Transform           // Transform para detección de suelo
- groundLayer: LayerMask           // Layer del suelo
- deathYThreshold: float = -20f    // Y para muerte por caída

[SerializeField] Salud:
- maxHealth: int = 100             // Salud máxima
```

#### API Pública

```csharp
// IDamageable implementation
bool TakeDamage(int damage, Vector2 knockbackDir, float knockbackForce)
int GetCurrentHealth()
int GetMaxHealth()
bool IsAlive()
Vector3 GetPosition()

// Propiedades de estado
bool IsGrounded { get; }
bool IsDashing { get; }
bool IsIntangible { get; }
bool IsAttacking { get; }
```

#### Eventos que Broadcast

- `PlayerJumpedEvent` - Cuando el jugador salta
  - `jumpPosition`: Vector2
  
- `PlayerDashedEvent` - Cuando el jugador hace dash
  - `startPosition`: Vector2
  - `endPosition`: Vector2
  - `isIntangible`: bool
  
- `PlayerAttackedEvent` - Cuando el jugador ataca
  - `attackPosition`: Vector2
  - `damageDealt`: int
  
- `PlayerTakeDamageEvent` - Cuando el jugador recibe daño
  - `damageAmount`: int
  - `remainingHealth`: int
  - `damagePosition`: Vector2
  
- `PlayerDiedEvent` - Cuando el jugador muere
  - `lastLevel`: int
  - `deathPosition`: Vector2

#### Ejemplo de Uso

```csharp
// En otro script, detectar cuando el jugador recibe daño
void Start()
{
    EventManager.Subscribe<PlayerTakeDamageEvent>(OnPlayerDamaged);
}

void OnPlayerDamaged(PlayerTakeDamageEvent e)
{
    Debug.Log($"Jugador recibió {e.damageAmount} daño");
    UpdateHealthBar(e.remainingHealth);
}

void OnDestroy()
{
    EventManager.Unsubscribe<PlayerTakeDamageEvent>(OnPlayerDamaged);
}
```

#### Sistema de Dash Intangible

El dash otorga invulnerabilidad temporal:

1. Jugador presiona tecla de Dash (Y)
2. `isDashing = true`
3. `isIntangible = true`
4. Collider se desactiva (opcional)
5. Durante `dashDuration` segundos, el jugador no puede recibir daño
6. `TakeDamage()` retorna false si `isIntangible == true`
7. Después de `dashDuration`, vuelve a la normalidad
8. Cooldown de `dashCooldown` segundos antes de poder volver a hacer dash

```csharp
// Los enemigos deben verificar:
if (player.IsIntangible)
{
    // No aplicar daño
    return;
}
```

## 👾 Enemy/ - Enemigos

### Estructura Planeada

#### Base/
**BaseEnemy.cs** (Pendiente de implementación)

Clase abstracta que contiene lógica común para todos los enemigos:

```csharp
public abstract class BaseEnemy : MonoBehaviour, IEnemy
{
    // Stats comunes
    protected int maxHealth;
    protected int currentHealth;
    protected float moveSpeed;
    protected int attackDamage;
    
    // Comportamiento común
    protected virtual void Patrol() { }
    protected virtual bool DetectPlayer() { }
    protected virtual void ChasePlayer() { }
    protected virtual void AttackPlayer() { }
    
    // IDamageable implementation
    public virtual bool TakeDamage(int damage, Vector2 dir, float force) { }
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;
    public Vector3 GetPosition() => transform.position;
    
    // IEnemy implementation
    public abstract char GetEnemyType();
    public virtual bool CanSeePlayer() { }
    public virtual void AttackPlayer() { }
    public virtual bool IsAttacking() { }
    
    protected virtual void Die()
    {
        EventManager.Broadcast(new EnemyDefeatedEvent
        {
            enemyType = GetEnemyType(),
            deathPosition = transform.position
        });
        Destroy(gameObject);
    }
}
```

#### Types/
**Tipos específicos de enemigos** (Pendientes)

Cada tipo hereda de BaseEnemy y customiza comportamiento:

1. **BasicEnemy (X)** - Enemigo estándar
   - Patrullaje simple
   - Velocidad normal
   - Ataque cuerpo a cuerpo
   
2. **FastEnemy (Y)** - Enemigo rápido
   - `moveSpeed *= 1.5f`
   - `maxHealth /= 1.5f`
   - Detección mejorada
   - Ataque rápido
   
3. **StrongEnemy (Z)** - Enemigo fuerte
   - `moveSpeed *= 0.7f`
   - `maxHealth *= 1.5f`
   - `attackDamage *= 1.5f`
   - Ataque lento pero poderoso
   
4. **FlyingEnemy (V)** - Enemigo volador
   - `rb.gravityScale = 0`
   - Patrón de vuelo
   - Ataque a distancia (opcional)

## 👑 Boss/ - Jefes

### Estructura Planeada

#### BaseBoss.cs (Pendiente)

Clase abstracta con sistema de fases:

```csharp
public abstract class BaseBoss : MonoBehaviour, IBoss
{
    // Stats
    protected int maxHealth;
    protected int currentHealth;
    protected int currentPhase;
    
    // State machine
    protected enum BossState { Waiting, InBattle, Defeated }
    protected BossState state;
    
    // IBoss implementation
    public abstract string GetBossId();
    public abstract void EnterBattle();
    public abstract void ExitBattle();
    public int GetCurrentPhase() => currentPhase;
    
    // Fases
    protected virtual void Phase1() { }
    protected virtual void Phase2() { }
    protected virtual void Phase3() { }
    
    protected virtual void TransitionPhase()
    {
        currentPhase++;
        // Transición visual, cambio de patrones
    }
    
    protected virtual void Die()
    {
        state = BossState.Defeated;
        
        EventManager.Broadcast(new BossDefeatedEvent
        {
            bossId = GetBossId(),
            levelNumber = GameManager.Instance.GetCurrentLevel()
        });
        
        GameManager.Instance.DefeatBoss(GetBossId());
        
        // Animación de muerte
        Destroy(gameObject, 2f);
    }
}
```

#### Bosses Específicos (Pendientes)

1. **Boss1.cs** - Primer jefe
   - Hereda de BaseBoss
   - 3 fases
   - Ataques: Fireball, ComboAttack, GroundSlam
   
2. **Boss2.cs** - Segundo jefe
   - Hereda de BaseBoss
   - 3 fases
   - Mecánicas diferentes a Boss1
   
3. **FinalBoss.cs** - Jefe final
   - Hereda de BaseBoss
   - 4+ fases
   - Combinación de ataques previos

## 🎯 Principios de Diseño

### Para Todas las Entidades

1. **Implementar Interfaces**: Usar IDamageable, IEnemy, IBoss
2. **Usar Eventos**: Broadcast eventos en lugar de referencias directas
3. **Abstracciones**: Heredar de clases base cuando tenga sentido
4. **Respetar Intangibilidad**: Verificar IsIntangible antes de aplicar daño
5. **Estado Centralizado**: Consultar GameManager cuando sea necesario

### Para Enemigos

1. **Detección de Jugador**: Usar Raycast o CircleCollider2D
2. **Patrullaje**: Movimiento entre waypoints o aleatorio
3. **Ataque**: Cooldown entre ataques
4. **Muerte**: Broadcast EnemyDefeatedEvent

### Para Bosses

1. **Sistema de Fases**: Cambiar comportamiento según salud
2. **Música Específica**: Broadcast MusicChangeRequestedEvent al entrar
3. **UI de Salud**: Health bar específica para boss
4. **Cinemáticas**: Entrada y salida del boss

## 📝 Ejemplo: Crear Nuevo Enemigo

```csharp
using UnityEngine;

public class BasicEnemy : BaseEnemy
{
    public override char GetEnemyType() => 'X';
    
    protected override void Patrol()
    {
        // Lógica de patrullaje simple
        // Moverse entre dos puntos
    }
    
    protected override bool DetectPlayer()
    {
        // Usar CircleCollider2D o Raycast
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            detectionRange, 
            playerLayer
        );
        return hits.Length > 0;
    }
    
    protected override void ChasePlayer()
    {
        // Perseguir al jugador
        Vector2 direction = (playerPos - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }
}
```

## 🐛 Testing

### Checklist para PlayerController
- [ ] Movimiento responde a WASD y flechas
- [ ] Salto solo funciona cuando está en el suelo
- [ ] Dash tiene cooldown y hace intangible
- [ ] Ataque detecta enemigos en rango
- [ ] TakeDamage reduce salud correctamente
- [ ] Muerte trigger cuando health <= 0
- [ ] Eventos se broadcastean correctamente

### Checklist para Enemigos
- [ ] Implementa IEnemy correctamente
- [ ] Patrullaje funciona sin jugador
- [ ] Detecta jugador en rango
- [ ] Persigue jugador correctamente
- [ ] Ataque respeta cooldown
- [ ] TakeDamage funciona con knockback
- [ ] Muerte broadcast evento
- [ ] Respeta intangibilidad del jugador

### Checklist para Bosses
- [ ] Implementa IBoss correctamente
- [ ] Sistema de fases funciona
- [ ] Cada fase tiene comportamiento único
- [ ] Transiciones de fase son smooth
- [ ] Broadcast BossDefeatedEvent al morir
- [ ] Registra derrota en GameManager
- [ ] Música cambia al entrar/salir

## 📚 Referencias

- Ver `Utilities/Interfaces/` para definiciones de interfaces
- Ver `Utilities/Events/GameEvents.cs` para eventos disponibles
- Ver `Legacy/` para scripts antiguos de referencia
- Ver `ARQUITECTURA_BRUTAL.txt` para diseño completo

---

**Última actualización**: 2025-11-09
