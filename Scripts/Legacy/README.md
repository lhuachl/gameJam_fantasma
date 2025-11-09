# Legacy - Scripts Pendientes de Refactorización

⚠️ **ADVERTENCIA**: Los scripts en esta carpeta están marcados para refactorización o reemplazo. No se recomienda modificarlos directamente.

## 🎯 Propósito de esta Carpeta

Esta carpeta contiene los scripts originales del proyecto que serán:
- **Reemplazados** por nuevas implementaciones con mejor arquitectura
- **Refactorizados** para usar el sistema de eventos y GameManager
- **Consolidados** cuando varios scripts hacen lo mismo

## 📦 Scripts y su Estado

### 🔄 Para Reemplazar Completamente

#### ControlesPersonaje.cs → PlayerController.cs ✅
**Estado**: REEMPLAZADO
- ✅ Nueva versión en `Entities/Player/PlayerController.cs`
- ✅ Input System integrado
- ✅ Dash intangible implementado
- ✅ Sistema de eventos integrado
- ✅ Implementa IDamageable

**Acción**: Usar `PlayerController.cs` en todos los GameObjects del jugador

---

#### CameraFollowplayer.cs → CameraManager.cs ✅
**Estado**: REEMPLAZADO
- ✅ Nueva versión en `Managers/CameraManager.cs`
- ✅ Smoothing mejorado
- ✅ Configuración más flexible
- ✅ Auto-detección de jugador

**Acción**: Usar `CameraManager.cs` en la cámara principal

---

### 🔧 Para Refactorizar con Nueva Arquitectura

#### GeneradorNivel.cs → LevelManager.cs
**Estado**: PENDIENTE REFACTORIZACIÓN
- ⏳ Necesita usar `GameManager.Instance.GetCurrentLevel()`
- ⏳ Debe suscribirse a eventos (BossDefeatedEvent)
- ⏳ Eliminar referencias a SaveData local
- ⏳ Broadcast LevelLoadedEvent al cargar

**Cambios requeridos**:
```csharp
// ANTES:
SaveData saveData = FindObjectOfType<SaveData>();
int level = saveData.nivelActual;

// DESPUÉS:
int level = GameManager.Instance.GetCurrentLevel();

// AGREGAR:
EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);
EventManager.Broadcast(new LevelLoadedEvent { levelNumber = level });
```

---

#### EnemyController.cs → BaseEnemy.cs
**Estado**: PENDIENTE REFACTORIZACIÓN
- ⏳ Extraer lógica común a `BaseEnemy.cs` (clase abstracta)
- ⏳ Implementar `IEnemy` interface
- ⏳ Usar eventos en lugar de FindWithTag
- ⏳ Respetar intangibilidad del jugador durante dash
- ⏳ Broadcast EnemyDefeatedEvent al morir

**Plan**:
1. Crear `Entities/Enemy/Base/BaseEnemy.cs` (abstracta)
2. Extraer: patrullaje, detección, ataque, daño
3. Crear tipos específicos: BasicEnemy, FastEnemy, StrongEnemy, FlyingEnemy
4. Cada tipo hereda y override comportamientos específicos

---

#### Boss.cs → BaseBoss.cs + Boss1.cs
**Estado**: PENDIENTE REFACTORIZACIÓN
- ⏳ Extraer lógica común a `BaseBoss.cs` (clase abstracta)
- ⏳ Implementar `IBoss` interface
- ⏳ Sistema de fases genérico
- ⏳ Broadcast BossDefeatedEvent al morir
- ⏳ Boss específico como Boss1.cs heredando de BaseBoss

**Cambios requeridos**:
```csharp
// AGREGAR en Die():
EventManager.Broadcast(new BossDefeatedEvent 
{
    bossId = "boss1",
    levelNumber = GameManager.Instance.GetCurrentLevel()
});

GameManager.Instance.DefeatBoss("boss1");
```

---

#### FinalBoss.cs → FinalBoss.cs (refactorizado)
**Estado**: PENDIENTE REFACTORIZACIÓN
- ⏳ Heredar de `BaseBoss`
- ⏳ Implementar `IBoss`
- ⏳ Usar eventos
- ⏳ Broadcast BossDefeatedEvent al morir

---

### 🔀 Para Consolidar

#### NewGame.cs + Continue.cs → MenuManager.cs
**Estado**: PENDIENTE CONSOLIDACIÓN
- ⏳ Crear `UI/MenuManager.cs` que contenga ambos
- ⏳ Métodos: `OnNewGameClicked()`, `OnContinueClicked()`
- ⏳ Usar `GameManager.Instance.CreateNewGame()`
- ⏳ Usar `GameManager.Instance.LoadGameState()`

**Código objetivo**:
```csharp
public class MenuManager : MonoBehaviour
{
    public void OnNewGameClicked()
    {
        GameManager.Instance.CreateNewGame();
        SceneManager.LoadScene("Level_1");
    }
    
    public void OnContinueClicked()
    {
        if (GameManager.Instance.HasSaveFile())
        {
            GameManager.Instance.LoadGameState();
            int level = GameManager.Instance.GetCurrentLevel();
            SceneManager.LoadScene($"Level_{level}");
        }
    }
}
```

---

#### Guardadodepartida.cs → OBSOLETO
**Estado**: OBSOLETO
- ❌ Funcionalidad completamente reemplazada por GameManager
- ❌ SaveData duplicada causaba bugs
- ❌ No usar

**Acción**: Ignorar este script, usar `GameManager.Instance.SaveGameState()`

---

### 📝 Scripts Auxiliares (Revisar si se Necesitan)

#### Finish.cs
**Estado**: NECESITA REFACTORIZACIÓN MENOR
- ⏳ Agregar broadcast de `LevelCompleteEvent`
- ⏳ Mostrar UI de decisión BUENO/MALO
- ⏳ Usar GameManager para registrar decisión

```csharp
// AGREGAR:
EventManager.Broadcast(new LevelCompleteEvent 
{ 
    levelNumber = GameManager.Instance.GetCurrentLevel() 
});
```

---

#### BackgroundParallaxFill.cs
**Estado**: MANTENER
- ✅ Funcionalidad auxiliar que no necesita refactorización
- ✅ No depende de arquitectura core

---

#### BulletScript.cs
**Estado**: REVISAR
- ❓ Ver si usa IDamageable correctamente
- ❓ Agregar broadcast de eventos si es necesario

---

#### GeneradordeBichos.cs
**Estado**: REVISAR
- ❓ Refactorizar para usar eventos
- ❓ Broadcast EnemySpawnedEvent

---

#### ParalaxController.cs
**Estado**: MANTENER
- ✅ Funcionalidad auxiliar que no necesita refactorización

---

#### Pinchos.cs
**Estado**: REVISAR
- ❓ Verificar que usa IDamageable.TakeDamage()
- ❓ Respetar intangibilidad del jugador

---

#### TilemapSetup.cs
**Estado**: MANTENER
- ✅ Setup de tilemaps, no necesita refactorización

---

## 🚀 Plan de Migración

### Fase 1: Reemplazos Simples (HOY - 1 hora)
1. ✅ Reemplazar `ControlesPersonaje.cs` con `PlayerController.cs` en Player GameObject
2. ✅ Reemplazar `CameraFollowplayer.cs` con `CameraManager.cs` en Camera
3. ✅ Verificar que funciona presionando Play

### Fase 2: Refactorización de Menu (HOY - 30 min)
1. Crear `UI/MenuManager.cs`
2. Consolidar lógica de NewGame.cs y Continue.cs
3. Actualizar botones en la escena de menú
4. Probar flujo completo: New Game → Play → Continue

### Fase 3: Refactorización de Nivel (ESTA SEMANA - 2 hrs)
1. Crear `Managers/LevelManager.cs` basado en GeneradorNivel.cs
2. Usar GameManager.Instance para estado
3. Suscribirse a eventos relevantes
4. Refactorizar Finish.cs para usar eventos

### Fase 4: Refactorización de Boss (ESTA SEMANA - 2 hrs)
1. Crear `Entities/Boss/BaseBoss.cs` (abstracta)
2. Refactorizar Boss.cs → Boss1.cs heredando de BaseBoss
3. Refactorizar FinalBoss.cs heredando de BaseBoss
4. Agregar broadcasts de eventos

### Fase 5: Refactorización de Enemigos (PRÓXIMA SEMANA - 3 hrs)
1. Crear `Entities/Enemy/Base/BaseEnemy.cs` (abstracta)
2. Crear 4 tipos: BasicEnemy, FastEnemy, StrongEnemy, FlyingEnemy
3. Migrar lógica de EnemyController.cs
4. Agregar broadcasts de eventos

### Fase 6: Cleanup (DESPUÉS)
1. Eliminar scripts obsoletos (Guardadodepartida.cs)
2. Revisar y actualizar scripts auxiliares
3. Testing completo

## ⚠️ Reglas Durante la Migración

### ✅ HACER
- Mantener scripts legacy intactos hasta que nuevas versiones estén probadas
- Probar cada cambio inmediatamente después de hacerlo
- Usar debug mode para verificar eventos
- Documentar cualquier problema encontrado

### ❌ NO HACER
- No modificar scripts legacy (hacer nuevas versiones)
- No borrar scripts hasta que estés 100% seguro
- No refactorizar múltiples sistemas a la vez
- No romper funcionalidad existente

## 📊 Estado de Migración

| Script | Estado | Prioridad | Tiempo Estimado |
|--------|--------|-----------|-----------------|
| ControlesPersonaje.cs | ✅ Reemplazado | Alta | 0 hrs |
| CameraFollowplayer.cs | ✅ Reemplazado | Alta | 0 hrs |
| NewGame.cs + Continue.cs | ⏳ Pendiente | Alta | 0.5 hrs |
| GeneradorNivel.cs | ⏳ Pendiente | Alta | 2 hrs |
| Boss.cs | ⏳ Pendiente | Media | 2 hrs |
| FinalBoss.cs | ⏳ Pendiente | Media | 1 hr |
| EnemyController.cs | ⏳ Pendiente | Baja | 3 hrs |
| Finish.cs | ⏳ Pendiente | Alta | 0.5 hrs |
| Guardadodepartida.cs | ❌ Obsoleto | N/A | 0 hrs |

**Progreso Total**: 2/9 scripts (22%)

## 📚 Referencias

- Ver `COMIENZA_AQUI.txt` para guía detallada de refactorización
- Ver `ARQUITECTURA_BRUTAL.txt` para arquitectura objetivo
- Ver `QUICK_START.txt` para setup paso a paso

---

**Última actualización**: 2025-11-09
