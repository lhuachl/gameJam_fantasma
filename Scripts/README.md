# Scripts Directory

Esta carpeta contiene todos los scripts C# del proyecto organizados en una arquitectura modular y escalable.

## 📁 Estructura de Carpetas

### 🎯 Core/
**Sistemas fundamentales del juego**
- `GameManager.cs` - Singleton que gestiona el estado global del juego
  - Carga/guarda progreso en JSON
  - Gestiona nivel actual, upgrades, decisiones
  - Única fuente de verdad para el estado del juego

### 📊 Data/
**Estructuras de datos serializables**
- `GameSaveData.cs` - Clase que contiene todo el estado del juego
  - Nivel actual y niveles completados
  - Decisiones BUENO/MALO
  - Bosses derrotados
  - Stats del jugador (salud, daño, habilidades)
  - Tiempo de juego

### 🔧 Utilities/
**Código reutilizable y abstracciones**

#### Interfaces/
Contratos para componentes del juego:
- `IDamageable.cs` - Para entidades que reciben daño
- `IEnemy.cs` - Para enemigos (hereda IDamageable)
- `IBoss.cs` - Para bosses (hereda IDamageable)

#### Events/
Sistema de comunicación desacoplada:
- `EventManager.cs` - Pub/Sub manager estático
- `GameEvents.cs` - 13 tipos de eventos predefinidos

### 🎮 Entities/
**Entidades del juego (jugador, enemigos, bosses)**

#### Player/
- `PlayerController.cs` - Control completo del jugador
  - Movimiento, salto, dash intangible, ataque
  - Input System integrado
  - Implementa IDamageable

#### Enemy/ (En desarrollo)
- `Base/` - Para BaseEnemy.cs (clase abstracta)
- `Types/` - Para tipos específicos (BasicEnemy, FastEnemy, etc.)

#### Boss/ (En desarrollo)
- Para BaseBoss.cs y bosses específicos

### 🎛️ Managers/
**Gestores de sistemas específicos**
- `CameraManager.cs` - Cámara suave siguiendo al jugador
- (Pendiente) `LevelManager.cs` - Generación de niveles desde CSV
- (Pendiente) `AudioManager.cs` - Música y efectos de sonido
- (Pendiente) `HUDManager.cs` - Interfaz de usuario

### 🗺️ Level/
**Generación y transiciones de niveles**
- (Pendiente) `LevelTransition.cs` - Fades y cinemáticas
- (Pendiente) Refactorizar generador de niveles

### 🖼️ UI/
**Interfaces de usuario**
- (Pendiente) `MenuManager.cs` - Menú principal
- (Pendiente) `HUDManager.cs` - HUD del juego

### 📦 Legacy/
**Scripts antiguos pendientes de refactorización**

Scripts que serán reemplazados o refactorizados:
- `ControlesPersonaje.cs` → Reemplazado por `PlayerController.cs`
- `CameraFollowplayer.cs` → Reemplazado por `CameraManager.cs`
- `GeneradorNivel.cs` → Refactorizar a `LevelManager.cs`
- `EnemyController.cs` → Refactorizar con `BaseEnemy.cs`
- `Boss.cs` → Refactorizar con `BaseBoss.cs`
- `FinalBoss.cs` → Refactorizar heredando de `BaseBoss.cs`
- `NewGame.cs` + `Continue.cs` → Consolidar en `MenuManager.cs`
- `Guardadodepartida.cs` → Obsoleto (funcionalidad en GameManager)

Scripts auxiliares:
- `BackgroundParallaxFill.cs` - Parallax de fondo
- `BulletScript.cs` - Sistema de proyectiles
- `GeneradordeBichos.cs` - Spawner de enemigos
- `ParalaxController.cs` - Control de parallax
- `Pinchos.cs` - Trampas
- `TilemapSetup.cs` - Setup de tilemaps
- `Finish.cs` - Trigger de fin de nivel

## 🚀 Guía Rápida

### Para empezar a desarrollar:
1. Lee `00_LEEME_PRIMERO.txt` para entender la arquitectura
2. Lee `QUICK_START.txt` para setup en Unity
3. Usa `GameManager.Instance` para acceder al estado global
4. Usa `EventManager` para comunicación entre sistemas
5. Implementa interfaces relevantes en tus clases

### Para agregar un nuevo enemigo:
1. Hereda de `BaseEnemy` (cuando esté implementado)
2. Implementa `IEnemy`
3. Override métodos necesarios
4. Usa eventos para comunicarte con otros sistemas

### Para agregar un nuevo boss:
1. Hereda de `BaseBoss` (cuando esté implementado)
2. Implementa `IBoss`
3. Define fases y patrones de ataque
4. Broadcast `BossDefeatedEvent` al morir

## 📖 Documentación Adicional

Consulta los archivos de documentación en esta carpeta:
- `ARQUITECTURA_BRUTAL.txt` - Referencia técnica completa
- `MAP_STRUCTURE.txt` - Estructura visual del proyecto
- `COMIENZA_AQUI.txt` - Guía de refactorización
- Y muchos más...

## 🎯 Principios de Arquitectura

1. **Centralización**: GameManager es la única fuente de verdad
2. **Desacoplamiento**: EventManager para comunicación entre sistemas
3. **Abstracciones**: Interfaces para contratos claros
4. **Herencia**: Clases base para reutilización de código
5. **Organización**: Una responsabilidad por clase
6. **Performance**: Evitar `FindWithTag` en runtime

## ⚠️ Notas Importantes

- No modifiques archivos en `Legacy/` - están marcados para refactorización
- Siempre usa `GameManager.Instance` para estado global
- Nunca guardes estado en variables estáticas fuera de managers
- Usa eventos en lugar de referencias directas cuando sea posible
- Mantén los .meta files sincronizados con Unity

---

Para más información, consulta el README.md principal en la raíz del proyecto.
