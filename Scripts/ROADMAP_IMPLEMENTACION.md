# 🎮 ANÁLISIS DE ARQUITECTURA - Plataformero Lineal con Decisiones Branching

## ✅ Lo Que Ya Tienes Claro

### Estructura del Juego
```
Nivel 1 (CSV) → Nivel 2 (CSV) → Nivel 3 (CSV) [BOSS PAR] → Nivel 4 → ...
                                                      ↓
                         [Decisión: BUENO vs MALO → Final Desbloqueado]
                         
Cada nivel es un MAPA DE PLATAFORMERO (no una sala de metroidvania)
Progresión LINEAL pero con RAMAS de finales
```

### Mecánicas del Jugador
- Salto normal + Dash mejorado
- Dash = **intangible a enemigos** (i-frames)
- Dash eleva ligeramente (eje Y configurable)
- Estos parámetros ya están en ControlesPersonaje.cs ✅

### Bosses
- Al final de cada zona PAR (nivel 3, 6, 9, etc.)
- Heredan de clase abstracta `BaseBoss`
- Boss1 y Boss2 diferentes según progresión
- Pueden extender de `BaseEnemy` o ser independientes

### Sistema de Guardado
- Trackea: `nivelActual`, `finalBueno`, `finalMalo`, `boss1`, `boss2`, `pendingBoss`
- Decisiones binarias BUENO/MALO abren finales diferentes
- JSON centralizado

---

## 🔴 LO QUE FALTA

### 1. **CLASE ABSTRACTA BaseBoss** 🔴
```csharp
// NO EXISTE
// Necesitas:
public abstract class BaseBoss : MonoBehaviour, IDamageable
{
    public abstract void Enter();           // Animación entrada
    public abstract void Phase1();          // Primer patrón
    public abstract void Phase2();          // Segundo patrón (opcional)
    public abstract void OnDefeated();      // Lógica de victoria
}
```

**Problema**: Boss.cs y FinalBoss.cs no tienen estructura común
**Impacto**: No puedes intercambiar bosses sin código hardcodeado

---

### 2. **INTERFAZ IDamageable + IEnemy** 🔴
```csharp
// NO EXISTE
// Necesitas:
public interface IDamageable
{
    void TakeDamage(int damage);
    int GetHealth();
}

public interface IEnemy : IDamageable
{
    void OnDetectPlayer();
    void OnLosePlayer();
}
```

**Problema**: EnemyController, Boss, FinalBoss implementan TakeDamage de forma diferente
**Impacto**: Código duplicado, difícil mantener

---

### 3. **INTANGIBILIDAD EN DASH** 🔴
```csharp
// En ControlesPersonaje.cs - Dash()
isDashing = true;  // ✅ EXISTE
isInvulnerable = true;  // ✅ EXISTE

// PERO:
// - EnemyController.cs no CHEQUEA isInvulnerable
// - Boss.cs no CHEQUEA isInvulnerable
// - Los enemigos atacan igual durante dash
```

**Problema**: La intangibilidad no está implementada en enemigos
**Impacto**: El dash no es útil como escape, enemigos ignoran i-frames

---

### 4. **SISTEMA DE EVENTOS** 🔴
```csharp
// NO EXISTE
// Necesitas:
EventManager.Broadcast(new BossDefeatedEvent { bossId = "boss1" });
EventManager.Subscribe<BossDefeatedEvent>(HandleBossDefeat);
```

**Problema**: GeneradorNivel espera que Boss() llame a OnBossDefeated()
**Impacto**: Acoplamiento fuerte, difícil agregar lógica

---

### 5. **MANAGER CENTRALIZADO** 🔴
```csharp
// NO EXISTE GameManager
// Necesitas:
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SaveData CurrentSave { get; private set; }
    
    public void SaveProgress(int levelCompleted, string finalType)
    {
        // Maneja TODO el guardado
    }
}
```

**Problema**: Lógica de guardado está esparcida en NewGame, Continue, Finish, GeneradorNivel
**Impacto**: Bug histórico: SaveData inconsistente entre archivos

---

### 6. **CLASS HIERARCHY DE ENEMIGOS** 🔴
```csharp
// TIENES: EnemyController.cs (solo tipo genérico)
// NECESITAS:
BaseEnemy
├── BasicEnemy (X en CSV)
├── FastEnemy (Y en CSV)
├── StrongEnemy (Z en CSV)
└── FlyingEnemy (V en CSV)

BaseBoss
├── Boss1
└── FinalBoss
```

**Problema**: GeneradorNivel instancia prefabEnemigo pero no puede variar comportamiento
**Impacto**: Todos los enemigos se comportan igual

---

### 7. **INVULNERABILIDAD POST-HIT** 🔴
```csharp
// En ControlesPersonaje.cs:
public void TakeDamage(int damage)
{
    if (isInvulnerable) return;
    currentHealth -= damage;
    StartCoroutine(BecomeInvulnerable());  // 3 segundos
}
```

**Problema**: Esto está bien, pero enemigos pueden atacar durante los 3 segundos
**Impacto**: Jugador puede morir sin escapatoria

---

### 8. **TRANSICIONES DE NIVEL** 🔴
```csharp
// NO TIENE
// Necesitas:
public class LevelTransition : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TransitionToNextLevel();
        }
    }
}
```

**Problema**: Finish.cs es solo para cinemáticas de final, no para siguiente nivel
**Impacto**: No hay transición clara entre niveles

---

### 9. **CAMERA FOLLOW** 🔴
```csharp
// Existe CameraFollowplayer.cs pero sin constraints
// Necesita:
- Limitar bounds por nivel (no seguir fuera del mapa)
- Smooth camera follow
- Parallax support (para fondos)
```

---

### 10. **AUDIO MANAGER** 🔴
```csharp
// NO EXISTE
// Necesita:
- Música por zona
- SFX para saltos, ataques, daño
- Boss theme
- UI sounds
```

---

## 🏗️ ARQUITECTURA PROPUESTA (Adaptada a tu caso)

```
Assets/Scripts/
├── Core/
│   ├── GameManager.cs ← CENTRALIZAR TODO
│   ├── EventManager.cs
│   ├── SaveSystem.cs
│   └── GameState.cs
│
├── Interfaces/
│   ├── IDamageable.cs
│   └── IEnemy.cs
│
├── Player/
│   ├── PlayerController.cs (mejorado)
│   ├── PlayerDash.cs (separar lógica dash)
│   └── PlayerStats.cs
│
├── Entities/
│   ├── Enemy/
│   │   ├── BaseEnemy.cs
│   │   ├── BasicEnemy.cs
│   │   ├── FastEnemy.cs
│   │   ├── StrongEnemy.cs
│   │   └── FlyingEnemy.cs
│   │
│   └── Boss/
│       ├── BaseBoss.cs ← CREAR
│       ├── Boss1.cs (hereda de BaseBoss)
│       └── FinalBoss.cs (hereda de BaseBoss)
│
├── Level/
│   ├── LevelGenerator.cs (renombrar GeneradorNivel)
│   ├── LevelTransition.cs ← CREAR
│   ├── LevelConfig.cs (ScriptableObject)
│   └── RoomData.cs
│
├── UI/
│   ├── HUDManager.cs
│   ├── HealthDisplay.cs
│   ├── LevelDisplay.cs
│   └── CinematicUI.cs
│
└── Managers/
    ├── AudioManager.cs ← CREAR
    └── InputManager.cs
```

---

## 🎯 PRIORIDADES DE IMPLEMENTACIÓN

### FASE 1: Centralización (2 horas) - **CRÍTICO**
```
1. GameManager.cs + SaveManager.cs
2. EventManager.cs
3. Refactorizar NewGame.cs, Continue.cs, Finish.cs
   → Todos usan GameManager.SaveProgress()
   
RESULTADO: SaveData nunca más se pierde
```

### FASE 2: Interfaces (1 hora)
```
1. IDamageable.cs
2. IEnemy.cs
3. Implementar en EnemyController, Boss, FinalBoss

RESULTADO: Código limpio, intercambiable
```

### FASE 3: BaseBoss (2 horas)
```
1. Crear BaseBoss.cs (parecido a BaseEnemy)
2. Refactorizar Boss.cs → hereda de BaseBoss
3. Refactorizar FinalBoss.cs → hereda de BaseBoss
4. Implementar OnBossDefeated() evento

RESULTADO: Bosses consistentes, escalables
```

### FASE 4: Intangibilidad en Dash (1 hora)
```
1. Agregar field a ControlesPersonaje: isIntangible
2. En EnemyController.Attack(): if (player.isIntangible) return;
3. En Boss.Attack(): if (player.isIntangible) return;

RESULTADO: Dash es útil defensivamente
```

### FASE 5: Clase Hierarchy de Enemigos (2 horas)
```
1. BaseEnemy.cs (extraer de EnemyController)
2. BasicEnemy, FastEnemy, StrongEnemy, FlyingEnemy
3. Actualizar GeneradorNivel para usar prefabs correctos

RESULTADO: Variedad de enemigos sin duplicación
```

### FASE 6: Transiciones (1 hora)
```
1. LevelTransition.cs para pasar de nivel
2. Fade negro + carga siguiente CSV
3. Integrar con GameManager

RESULTADO: Flujo limpio entre niveles
```

---

## 🔧 CÓDIGO QUE NECESITA CAMBIOS

### 1. ControlesPersonaje.cs - Agregar intangibilidad
```csharp
// ACTUALIZAR:
private bool isIntangible = false;

IEnumerator Dash()
{
    canDash = false;
    isDashing = true;
    isIntangible = true;  // ← AGREGAR
    // ... resto del dash
    isIntangible = false;
}

// Exponer públicamente:
public bool IsIntangible => isIntangible;
```

### 2. EnemyController.cs - Respetar intangibilidad
```csharp
// EN Attack():
IEnumerator Attack()
{
    canAttack = false;
    isAttacking = true;
    yield return new WaitForSeconds(0.2f);
    
    if (player != null)
    {
        var ctrl = player.GetComponent<ControlesPersonaje>();
        if (ctrl != null && ctrl.IsIntangible) 
        {
            // Jugador está en dash, no atacar
            isAttacking = false;
            yield return new WaitForSeconds(attackCooldown);
            canAttack = true;
            yield break;
        }
        
        // Atacar normalmente si no es intangible
        if (Mathf.Abs(player.position.x - transform.position.x) <= attackRange + 0.1f)
        {
            ctrl.TakeDamage(1);
        }
    }
    
    yield return new WaitForSeconds(0.1f);
    isAttacking = false;
    yield return new WaitForSeconds(attackCooldown);
    canAttack = true;
}
```

### 3. Boss.cs - Misma intangibilidad
```csharp
// EN randomAtack() o PerseguirState():
if (player != null)
{
    var ctrl = player.GetComponent<ControlesPersonaje>();
    if (ctrl != null && ctrl.IsIntangible)
    {
        // Skip ataque, solo esquivar
        return;
    }
    // Atacar normalmente
}
```

---

## 📊 COMPARACIÓN: ANTES vs DESPUÉS

| Aspecto | Antes | Después |
|---------|-------|---------|
| **SaveData** | 4 definiciones | 1 en GameManager |
| **Enemigos** | Solo EnemyController | BaseEnemy + 4 tipos |
| **Bosses** | Boss.cs sin patrón | BaseBoss + Boss1, FinalBoss |
| **Intangibilidad** | No funciona | Dash = safe desde enemigos |
| **Eventos** | Acoplamiento fuerte | Desacoplados con EventManager |
| **Líneas de código** | 500+ duplicadas | 300 centralizadas |
| **Mantenibilidad** | Frágil | Robusta |

---

## ❓ PREGUNTAS ANTES DE EMPEZAR

1. **¿Cuántos niveles planeas?**
   - Si es < 10: estructura simple está bien
   - Si es > 10: necesitas LevelConfig ScriptableObject

2. **¿Diferentes tipos de Bosses?**
   - Boss1: bruja con hechizos
   - Boss2: dragón volador
   - Final Boss: hybrid
   
   O todos iguales con variaciones?

3. **¿Upgrades del jugador permanentes?**
   - ¿Pueden llevar salud extra/arma mejorada al siguiente nivel?
   - O cada nivel empieza de cero?

4. **¿Música por nivel o globalmente?**
   - Cada zona su tema
   - O misma música

5. **¿Dificultad progresiva?**
   - ¿Los enemigos mejoran en niveles posteriores?
   - ¿Stats diferentes por nivel?

---

## 🚀 PRÓXIMOS PASOS

Recomiendo empezar por **FASE 1: GameManager + EventManager**

Una vez que centralices el guardado, TODO lo demás es más fácil.

¿Começamos por ahí?
