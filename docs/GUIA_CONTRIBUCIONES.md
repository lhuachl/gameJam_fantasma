# 📝 Quién hizo qué (más o menos)

## Resumen

Intentamos llevar cuenta de quién hizo qué. Probablemente olvidamos cosas.

---

## Alex - ~45% del código

### Lo que sí hizo
- GameManager (el singleton ese)
- EventManager (esto sí quedó bien)
- PlayerController (después de 3 refactors)
- CameraManager
- Las interfaces (IDamageable, IEnemy, IBoss)
- MUCHA documentación

### Commits principales
```
- "Initial commit" (setup del proyecto)
- "Add GameManager singleton" 
- "Implement EventManager" (lo más útil)
- "Refactor PlayerController to use events"
- "Add dash with invulnerability" (after many bugs)
- "Documentation overload" (literal)
```

### Líneas de código
- ~2500 líneas
- 12 commits
- 25+ archivos modificados
- Incontables horas debuggeando

---

## Saul - ~35% del código

### Lo que hizo
- TODO el sistema de enemigos
- BaseEnemy con detección de bordes
- PatrolEnemy (tardó un día en que no cayeran)
- ChaseEnemy con IA
- Boss y FinalBoss
- Sistema de Parallax (quedó profesional)
- BackgroundManager
- Arregló como 40 bugs de otros

### Commits principales
```
- "Add BaseEnemy class"
- "Implement edge detection for enemies" (gracias Saul)
- "Add PatrolEnemy" 
- "Add ChaseEnemy with AI states"
- "Implement parallax system"
- "Fix infinite tiling bug" (a las 3 AM)
- "Performance optimizations"
```

### Logros
- Los enemigos NO se caen de las plataformas
- El parallax se ve increíble
- 60 FPS en la mayoría de hardware
- Salvó el proyecto varias veces

---

## Russel - ~12% del código

### Lo que hizo
- GeneradorNivel desde CSV
- Sistema de guardado (después de varios intentos)
- GameSaveData
- NewGame/Continue scripts
- Testing (encontró muchos bugs)
- CHANGELOG.md

### Commits principales
```
- "Add level generator from CSV"
- "Implement save system" (attempt 1)
- "Fix save system" (attempt 2)
- "Save system finally works" (attempt 3)
- "Add CHANGELOG"
```

### Aprendizajes
- JSON es tu amigo
- Hacer backups ANTES de probar cosas
- El testing es importante
- CSV es simple pero funciona

---

## Erik - ~8% código + 100% arte

### Código
- Pinchos.cs
- Ayuda con BackgroundParallaxFill
- Configuración de Animators

### Arte (lo importante)
**Personaje:**
- 29 frames de animación
- Idle (7 frames)
- Run (6 frames)
- Jump (4 frames)
- Attack (5 frames)
- Dash (3 frames)
- Parry (4 frames)

**Enemigos:**
- 4 tipos diferentes
- 50+ sprites
- Animaciones walk, attack, hurt

**Bosses:**
- 2 bosses con animaciones múltiples
- 25+ frames totales

**Backgrounds:**
- 4 capas de parallax
- 1920x1080 cada una
- Optimizados por Saul

### Commits
```
- "Add character sprites"
- "Add enemy animations"
```

(La mayoría del arte no se ve en commits porque son assets binarios)

---

## Distribución Real

### Por área
```
Arquitectura:    Alex (80%) + Saul arreglando (20%)
Enemigos:        Saul (100%) 
Guardado:        Russel (100%)
Player:          Alex (100%)
Parallax:        Saul (100%)
Arte:            Erik (100%)
Bugs:            TODOS (colaboración team)
```

### Por semana

**Semana 1: Setup**
- Alex: Setup + primeros scripts
- Saul: Ayuda con estructura
- Russel: Aprender Unity
- Erik: 60% del arte

**Semana 2: Core**
- Alex: GameManager + Events
- Saul: Enemigos + Boss
- Russel: Sistema guardado
- Erik: Animaciones

**Semana 3: Polish**
- Alex: Documentación
- Saul: Parallax + Optimization
- Russel: Testing
- Erik: Polish arte
- TODOS: PANIC MODE

---

## Bugs Creados vs Arreglados

| Persona | Creados | Arreglados | Ratio |
|---------|---------|------------|-------|
| Alex    | 30+     | 15         | 0.5   |
| Saul    | 5       | 40+        | 8.0   |
| Russel  | 10      | 8          | 0.8   |
| Erik    | 2       | 2          | 1.0   |

Conclusión: Saul es el MVP

---

## Sistemas por persona

### Alex
- Core/GameManager.cs (~350 líneas)
- Core/EventManager.cs (~200 líneas)
- Entities/Player/PlayerController.cs (~350 líneas)
- Managers/CameraManager.cs (~150 líneas)
- Utilities/Interfaces/* (~100 líneas)

### Saul
- Entities/Enemy/Base/BaseEnemy.cs (~200 líneas)
- Entities/Enemy/Types/PatrolEnemy.cs (~180 líneas)
- Entities/Enemy/Types/ChaseEnemy.cs (~220 líneas)
- Managers/BackgroundManager.cs (~280 líneas)
- Entities/Boss/Boss.cs (~250 líneas)
- Entities/Boss/FinalBoss.cs (~300 líneas)

### Russel
- Legacy/GeneradorNivel.cs (~300 líneas)
- Data/GameSaveData.cs (~150 líneas)
- Legacy/Guardadodepartida.cs (~100 líneas)
- Legacy/Continue.cs (~80 líneas)
- Legacy/NewGame.cs (~70 líneas)

### Erik
- Scripts/Pinchos.cs (~50 líneas)
- 70+ assets (sprites, animaciones)

---

## Lo que aprendimos

### Alex
- Singleton no es tan malo como dicen
- Los eventos son tu amigo
- Documentar es importante
- No hacer commits a las 4 AM
- Git es complicado

### Saul
- Detección de bordes es más difícil de lo que parece
- Parallax require matemáticas
- Optimización importa
- Ser el "arregla-bugs" del equipo es agotador pero satisfactorio

### Russel
- JSON > XML
- Siempre hacer backups
- Testing es importante (ahora lo entiendo)
- CSV es genial para game design

### Erik
- Unity tiene configuraciones muy específicas para sprites
- Colaborar con programadores es diferente
- Pixel art toma tiempo
- Ver tus dibujos moverse es genial

---

## Stats finales

- **Commits totales:** 28
- **Líneas de código:** ~4,500
- **Scripts:** 35+
- **Sprites:** 50+
- **Animaciones:** 25+
- **Horas de sueño perdidas:** Muchas
- **Tazas de café:** Incontables
- **Veces que se rompió todo:** 5+
- **Veces que Saul lo arregló:** 5+

---

## Conclusión

Cada uno hizo su parte. Algunos hicieron más, otros menos, pero todos contribuimos. El proyecto salió adelante y aprendimos un montón.

Para ser un equipo amateur en su primer game jam serio, no está mal.

---

**Documento hecho por:** Alex  
**Verificado por:** Nadie (confiamos)  
**Fecha:** Cuando terminamos  
**Precisión:** ~80%

*"Los números son aproximados, el esfuerzo fue real"*
