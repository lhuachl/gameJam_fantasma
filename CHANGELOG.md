# Changelog

Todos los cambios notables en este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Versionado Semántico](https://semver.org/lang/es/).

## [No Liberado]

### 🎯 Planeado
- Sistema de BaseEnemy con 4 tipos de enemigos (Basic, Fast, Strong, Flying)
- Sistema de BaseBoss con jerarquía de herencia
- MenuManager consolidando NewGame y Continue
- AudioManager para música por zona y boss
- HUDManager para UI dinámica
- Sistema de transiciones de nivel con cinemáticas
- Sistema de upgrades visuales en UI

## [0.2.0] - 2025-11-09

### 📚 Agregado
- **README.md completo** en la raíz del proyecto con:
  - Descripción detallada del proyecto
  - Guía de instalación y setup
  - Documentación de arquitectura
  - Estructura de carpetas explicada
  - Sistema de eventos documentado
  - Roadmap de desarrollo
  - Guías de contribución
- **CHANGELOG.md** para trackear el historial del proyecto
- Documentación consolidada en carpeta Scripts/ con 15+ archivos

### 🏗️ Arquitectura
- **GameManager.cs** (Core/) - Singleton centralizado para estado global
  - Gestión de guardado/carga en JSON
  - Sistema de progresión entre niveles
  - Gestión de decisiones BUENO/MALO
  - Sistema de upgrades permanentes (salud, arma, habilidades)
  - Métodos públicos: `CreateNewGame()`, `SaveGameState()`, `LoadGameState()`, `DefeatBoss()`, `ProgressToNextLevel()`

- **EventManager.cs** (Utilities/Events/) - Sistema Pub/Sub para comunicación desacoplada
  - Subscribe/Unsubscribe/Broadcast pattern
  - Debug mode para visualizar eventos en Console
  - Sin acoplamiento entre sistemas
  
- **GameEvents.cs** (Utilities/Events/) - 13 eventos predefinidos
  - `BossDefeatedEvent`, `LevelCompleteEvent`, `DecisionMadeEvent`
  - `PlayerTakeDamageEvent`, `PlayerDiedEvent`, `PlayerTookUpgradeEvent`
  - `PlayerJumpedEvent`, `PlayerDashedEvent`, `PlayerAttackedEvent`
  - `EnemyDefeatedEvent`, `EnemySpawnedEvent`
  - `LevelLoadedEvent`, `LevelUnloadingEvent`
  - `MusicChangeRequestedEvent`, `SFXPlayRequestedEvent`

- **GameSaveData.cs** (Data/) - Estructura serializable para guardado
  - Tracking de nivel actual y completados
  - Historial de decisiones BUENO/MALO
  - Diccionario de bosses derrotados
  - Stats del jugador (salud, daño, habilidades)
  - Tiempo de juego acumulado

### 🎮 Sistemas de Jugador
- **PlayerController.cs** (Entities/Player/) - Control mejorado del jugador (350 líneas)
  - Input System integrado
  - Movimiento fluido con aceleración
  - Salto con detección de suelo mejorada
  - **Dash intangible** con invulnerabilidad temporal
  - Sistema de ataque con detección de enemigos
  - Implementa interfaz `IDamageable`
  - Broadcasting de eventos: Jump, Dash, Attack, TakeDamage, Die

- **CameraManager.cs** (Managers/) - Cámara suave siguiendo al jugador
  - Smoothing configurable
  - Offset personalizable
  - Lock opcional en ejes X/Y
  - Auto-detección de jugador si no asignado

### 🔌 Interfaces
- **IDamageable.cs** (Utilities/Interfaces/) - Contrato para entidades que reciben daño
  - `TakeDamage(int damage, Vector2 knockback, float knockbackForce): bool`
  - `GetCurrentHealth(): int`
  - `GetMaxHealth(): int`
  - `IsAlive(): bool`
  - `GetPosition(): Vector3`

- **IEnemy.cs** (Utilities/Interfaces/) - Contrato para enemigos (hereda IDamageable)
  - `GetEnemyType(): char` (X, Y, Z, V)
  - `CanSeePlayer(): bool`
  - `AttackPlayer(): void`
  - `IsAttacking(): bool`

- **IBoss.cs** (Utilities/Interfaces/) - Contrato para bosses (hereda IDamageable)
  - `GetBossId(): string`
  - `EnterBattle(): void`
  - `ExitBattle(): void`
  - `GetCurrentPhase(): int`

### 📁 Organización
- Estructura de carpetas profesional:
  - `Core/` - Sistemas fundamentales (GameManager)
  - `Data/` - Estructuras de datos (GameSaveData)
  - `Utilities/` - Interfaces y eventos reutilizables
  - `Entities/` - Player, Enemy, Boss (jerarquías)
  - `Managers/` - Gestores de sistemas (Camera, Level, Audio, HUD)
  - `Level/` - Generación y transiciones de niveles
  - `UI/` - Interfaces de usuario

### 🐛 Correcciones
- SaveData ya no está duplicada en múltiples archivos (bug histórico resuelto)
- GameManager persiste entre escenas con DontDestroyOnLoad
- Sistema de eventos reemplaza `FindWithTag` repetidos

### ♻️ Refactorizado
- Scripts legacy movidos conceptualmente a Legacy/ (pendiente organización física)
  - `ControlesPersonaje.cs` → Reemplazar con `PlayerController.cs`
  - `CameraFollowplayer.cs` → Reemplazar con `CameraManager.cs`
  - `GeneradorNivel.cs` → Refactorizar a `LevelManager.cs`
  - `Boss.cs` → Refactorizar con `BaseBoss.cs`
  - `EnemyController.cs` → Refactorizar con `BaseEnemy.cs`

### 📖 Documentación Creada
Documentación exhaustiva en `Scripts/`:

**Inicio y Setup:**
- `00_LEEME_PRIMERO.txt` - Resumen ejecutivo (5 min)
- `QUICK_START.txt` - Setup en Unity (30 min)
- `COMIENZA_AQUI.txt` - Guía de refactorización
- `INDICE_ARCHIVOS.txt` - Navegación de docs

**Técnica:**
- `ARQUITECTURA_BRUTAL.txt` - Referencia técnica completa (400+ líneas)
- `ANALISIS_ARQUITECTURA_COMPLETO.md` - Análisis de sistemas
- `ARQUITECTURA_METROIDVANIA.md` - Diseño avanzado
- `MAP_STRUCTURE.txt` - Árbol de archivos anotado
- `DIAGRAMA_ARQUITECTURA.txt` - Diagramas visuales

**Implementación:**
- `ROADMAP_IMPLEMENTACION.md` - Plan por fases
- `GENERADOR_NIVEL_MEJORAS.md` - Mejoras del generador
- `INSTRUCCIONES_SETUP.txt` - Setup detallado

**Progreso:**
- `GAMELOG.txt` - Bitácora de desarrollo
- `RESUMEN_EJECUTIVO.txt` - Resumen del proyecto
- `RESUMEN_ENTREGA.txt` - Entregables
- `ENTREGA_FINAL.txt` - Estado final
- `LISTO_PARA_COMENZAR.txt` - Próximos pasos
- `PROXIMO_PASO_FASE1.txt` - Fase 1

## [0.1.0] - 2025-11-07

### 🎮 Agregado
- Proyecto inicial de Unity configurado
- Sistema básico de plataformero 2D
- `ControlesPersonaje.cs` - Control básico del jugador
- `CameraFollowplayer.cs` - Cámara siguiendo al jugador
- `GeneradorNivel.cs` - Generación de niveles desde CSV
- `EnemyController.cs` - Lógica básica de enemigos
- `Boss.cs` - Implementación básica de boss
- `FinalBoss.cs` - Boss final
- `NewGame.cs` - Iniciar nuevo juego
- `Continue.cs` - Continuar partida guardada
- `Finish.cs` - Final de nivel
- `Guardadodepartida.cs` - Sistema de guardado básico (legacy)
- Scripts auxiliares:
  - `BackgroundParallaxFill.cs` - Parallax de fondo
  - `BulletScript.cs` - Proyectiles
  - `GeneradordeBichos.cs` - Spawner de enemigos
  - `ParalaxController.cs` - Control de parallax
  - `Pinchos.cs` - Trampas de pinchos
  - `TilemapSetup.cs` - Setup de tilemaps

### 🎨 Assets
- Sprites de jugador, enemigos y bosses
- Animaciones básicas (carpeta `Animations/`)
- Imágenes UI (carpeta `Images/`)
- Input System configurado (`InputSystem_Actions.inputactions`)
- Prefabs de entidades (carpeta `Prefab/`)
- Configuración de Render Pipeline (URP)
- TextMesh Pro integrado

### ⚙️ Configuración
- Unity 2D Pipeline con URP
- Input System package
- Configuración de layers y tags
- Escenas básicas del juego

### 🐛 Problemas Conocidos (En v0.1.0)
- SaveData duplicada en 4 lugares diferentes (causa pérdida de datos)
- Sistemas fuertemente acoplados (muchos `FindWithTag`)
- Sin interfaz común para enemigos y bosses
- Intangibilidad en dash no funcional
- Código duplicado en varios scripts
- Difícil de mantener y escalar

---

## Tipos de Cambios

- **Agregado** para nuevas características.
- **Cambiado** para cambios en funcionalidad existente.
- **Deprecado** para características que serán removidas pronto.
- **Removido** para características removidas.
- **Correcciones** para corrección de bugs.
- **Seguridad** para vulnerabilidades.

## Versionado

Este proyecto utiliza [Versionado Semántico](https://semver.org/lang/es/):

- **MAJOR**: Cambios incompatibles en API
- **MINOR**: Nueva funcionalidad compatible con versiones anteriores
- **PATCH**: Correcciones de bugs compatibles con versiones anteriores

---

**Nota**: Las fechas están en formato YYYY-MM-DD (ISO 8601).

Para ver el historial completo de commits, consulta: [GitHub Commits](https://github.com/lhuachl/gameJam_fantasma/commits)
