# 🎮 Game Jam Fantasma - Plataformero Lineal con Decisiones

[![Unity](https://img.shields.io/badge/Unity-2022+-blue.svg)](https://unity.com)
[![C#](https://img.shields.io/badge/C%23-Latest-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Un plataformero lineal 2D con sistema de decisiones branching que afectan la narrativa y los finales del juego. Desarrollado como parte de un Game Jam con arquitectura profesional y escalable.

## 📋 Tabla de Contenidos

- [Características Principales](#-características-principales)
- [Arquitectura del Proyecto](#-arquitectura-del-proyecto)
- [Instalación y Setup](#-instalación-y-setup)
- [Guía de Inicio Rápido](#-guía-de-inicio-rápido)
- [Sistemas Principales](#-sistemas-principales)
- [Estructura de Carpetas](#-estructura-de-carpetas)
- [Mecánicas de Juego](#-mecánicas-de-juego)
- [Sistema de Eventos](#-sistema-de-eventos)
- [Documentación Adicional](#-documentación-adicional)
- [Roadmap](#-roadmap)
- [Contribuciones](#-contribuciones)

## ✨ Características Principales

### 🎯 Gameplay
- **Plataformero Lineal**: Progresión nivel a nivel con mapas generados desde CSV
- **Sistema de Decisiones**: Elecciones BUENO/MALO que afectan la narrativa
- **Múltiples Finales**: Diferentes finales basados en las decisiones tomadas
- **Sistema de Upgrades**: Mejoras permanentes que persisten entre niveles
- **Boss Fights**: Jefes únicos cada 3 niveles con comportamientos específicos

### 🏗️ Arquitectura
- **GameManager Singleton**: Estado centralizado del juego
- **EventManager**: Sistema Pub/Sub para comunicación desacoplada
- **Sistema de Guardado JSON**: Persistencia automática del progreso
- **Interfaces y Abstracciones**: IDamageable, IEnemy, IBoss para código reutilizable
- **Jerarquía de Clases**: BaseEnemy y BaseBoss para facilitar extensibilidad

### ⚡ Mecánicas del Jugador
- **Movimiento Fluido**: Control responsive con Input System
- **Salto Mejorado**: Detección de suelo precisa
- **Dash Intangible**: Dash que otorga invulnerabilidad temporal
- **Sistema de Combate**: Ataque con detección de enemigos en rango
- **Sistema de Salud**: Daño, muerte y respawn

## 🏛️ Arquitectura del Proyecto

### Pilares Arquitectónicos

1. **Centralización de Estado**: `GameManager` como única fuente de verdad
2. **Comunicación por Eventos**: Sistemas desacoplados mediante `EventManager`
3. **Persistencia Unificada**: Un único archivo JSON para todo el progreso
4. **Abstracciones Reutilizables**: Interfaces y clases base para escalabilidad
5. **Organización Modular**: Carpetas por responsabilidad, no por tipo

### Ventajas de Esta Arquitectura

✅ **Escalabilidad**: Fácil agregar nuevos enemigos, bosses o features sin modificar código core
✅ **Mantenibilidad**: Bugs localizados en componentes específicos, no dispersos
✅ **Debugging**: Console logs muestran todos los eventos del sistema
✅ **Performance**: No más `FindWithTag` repetidos, referencias optimizadas
✅ **Testing**: Interfaces facilitan la creación de mocks y pruebas unitarias
✅ **Extensibilidad**: Nuevas features sin romper funcionalidad existente

## 🚀 Instalación y Setup

### Requisitos Previos

- Unity 2022.3 o superior
- .NET Framework 4.x o superior
- Input System Package (instalado automáticamente)

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/lhuachl/gameJam_fantasma.git
   cd gameJam_fantasma
   ```

2. **Abrir en Unity**
   - Abre Unity Hub
   - Click en "Add" y selecciona la carpeta del proyecto
   - Abre el proyecto con Unity 2022.3+

3. **Configurar la escena**
   - Abre la escena principal: `Scenes/MainScene.unity`
   - Verifica que existan los GameObjects:
     - `GameManager` (con script `Core/GameManager.cs`)
     - `EventManager` (vacío, el EventManager es estático)
     - `Player` (con script `Entities/Player/PlayerController.cs`)
     - `Camera` (con script `Managers/CameraManager.cs`)

4. **Compilar y ejecutar**
   - Presiona Play en Unity
   - Verifica en Console que aparezcan los logs:
     ```
     [GameManager] Inicializado | Ruta: ...
     [GameManager] Nuevo juego creado
     ```

## 🎮 Guía de Inicio Rápido

### Para Desarrolladores (30 Minutos)

1. **Lee la documentación clave** (10 min)
   - `Scripts/00_LEEME_PRIMERO.txt` - Resumen ejecutivo
   - `Scripts/QUICK_START.txt` - Setup paso a paso
   - `Scripts/COMIENZA_AQUI.txt` - Guía de refactorización

2. **Configura GameObjects en Unity** (10 min)
   - Sigue las instrucciones en `Scripts/QUICK_START.txt` sección "PASO 2"
   - Agrega GameManager y EventManager a la escena
   - Reemplaza PlayerController y CameraManager

3. **Verifica Funcionalidad** (10 min)
   - Presiona Play
   - Prueba movimiento (WASD), salto (Space), dash (Y), ataque (P)
   - Verifica logs en Console

### Para Diseñadores de Niveles

Los niveles se generan desde archivos CSV ubicados en `Resources/Levels/`:
- Formato: `Nivel_X.csv` donde X es el número de nivel
- Estructura: Coordenadas de paredes, enemigos (X, Y, Z, V), boss, jugador

Consulta `Scripts/MAP_STRUCTURE.txt` para detalles del formato CSV.

## 🔧 Sistemas Principales

### GameManager (Core/GameManager.cs)

El cerebro del juego. Gestiona:
- Estado global del juego (nivel actual, salud, upgrades)
- Carga y guardado de progreso (JSON)
- Progresión entre niveles
- Decisiones BUENO/MALO
- Upgrades permanentes

**Ejemplo de uso:**
```csharp
// Obtener nivel actual
int currentLevel = GameManager.Instance.GetCurrentLevel();

// Guardar progreso
GameManager.Instance.SaveGameState();

// Progresar al siguiente nivel
GameManager.Instance.ProgressToNextLevel();

// Registrar derrota de boss
GameManager.Instance.DefeatBoss("boss1");

// Agregar upgrade
GameManager.Instance.AddHealthUpgrade(20);
```

### EventManager (Utilities/Events/EventManager.cs)

Sistema Pub/Sub para comunicación entre sistemas sin acoplamiento.

**Eventos disponibles:**
- `BossDefeatedEvent` - Boss derrotado
- `LevelCompleteEvent` - Nivel completado
- `DecisionMadeEvent` - Decisión BUENO/MALO realizada
- `PlayerTakeDamageEvent` - Jugador recibe daño
- `PlayerDiedEvent` - Jugador muere
- `PlayerTookUpgradeEvent` - Jugador recoge upgrade
- `EnemyDefeatedEvent` - Enemigo derrotado
- `LevelLoadedEvent` - Nivel cargado
- Y más... (ver `Utilities/Events/GameEvents.cs`)

**Ejemplo de uso:**
```csharp
// Suscribirse a evento
EventManager.Subscribe<BossDefeatedEvent>(OnBossDefeated);

// Broadcast evento
EventManager.Broadcast(new BossDefeatedEvent 
{ 
    bossId = "boss1", 
    levelNumber = 3 
});

// Desuscribirse
EventManager.Unsubscribe<BossDefeatedEvent>(OnBossDefeated);
```

### Sistema de Guardado (Data/GameSaveData.cs)

Estructura serializable que contiene TODO el estado del juego:
- `currentLevel`: Nivel actual
- `completedLevels`: Niveles completados
- `decisionsPath`: Historial de decisiones BUENO/MALO
- `defeatedBosses`: Bosses derrotados (diccionario)
- `maxHealth`, `weaponDamage`: Stats del jugador
- `specialAbilities`: Habilidades desbloqueadas
- `playTime`: Tiempo de juego acumulado

Ubicación: `Application.persistentDataPath/gamesave.json`

## 📂 Estructura de Carpetas

```
Assets/Scripts/
│
├── 📂 Core/                    # Sistemas fundamentales
│   └── GameManager.cs          # Singleton de estado global
│
├── 📂 Data/                    # Estructuras de datos
│   └── GameSaveData.cs         # Clase serializable para guardado
│
├── 📂 Utilities/               # Utilidades reutilizables
│   ├── Interfaces/             # Contratos de interfaces
│   │   ├── IDamageable.cs      # Interfaz para entidades que reciben daño
│   │   ├── IEnemy.cs           # Interfaz para enemigos
│   │   └── IBoss.cs            # Interfaz para bosses
│   │
│   └── Events/                 # Sistema de eventos
│       ├── GameEvents.cs       # Definiciones de eventos (13 tipos)
│       └── EventManager.cs     # Pub/Sub manager
│
├── 📂 Entities/                # Entidades del juego
│   ├── Player/                 # Jugador
│   │   └── PlayerController.cs # Control del jugador (350 líneas)
│   │
│   ├── Enemy/                  # Enemigos
│   │   ├── Base/               # (Pendiente: BaseEnemy.cs)
│   │   └── Types/              # (Pendiente: tipos específicos)
│   │
│   └── Boss/                   # Bosses
│       └── (Pendiente: BaseBoss.cs + bosses específicos)
│
├── 📂 Managers/                # Gestores de sistemas
│   ├── CameraManager.cs        # Control de cámara suave
│   └── (Pendiente: LevelManager, AudioManager, HUDManager)
│
├── 📂 Level/                   # (Pendiente: generación y transiciones)
│
├── 📂 UI/                      # (Pendiente: MenuManager, HUDManager)
│
└── 📂 Legacy/                  # Scripts antiguos (para refactorizar)
    ├── ControlesPersonaje.cs   # ⚠️ Reemplazar con PlayerController
    ├── CameraFollowplayer.cs   # ⚠️ Reemplazar con CameraManager
    ├── GeneradorNivel.cs       # ⚠️ Refactorizar a LevelManager
    ├── Boss.cs                 # ⚠️ Refactorizar a BaseBoss
    ├── EnemyController.cs      # ⚠️ Refactorizar a BaseEnemy
    └── ...                     # Otros scripts legacy
```

**Leyenda:**
- ✅ = Implementado y probado
- ⚠️ = Requiere refactorización
- 📝 = Pendiente de implementar

## 🎯 Mecánicas de Juego

### Control del Jugador

| Acción | Tecla | Descripción |
|--------|-------|-------------|
| Mover | WASD / Flechas | Movimiento horizontal |
| Saltar | Space | Salto con detección de suelo |
| Dash | Y | Dash intangible con cooldown |
| Atacar | P | Ataque cuerpo a cuerpo |

### Sistema de Dash
- **Duración**: 0.2 segundos (configurable)
- **Cooldown**: 1 segundo (configurable)
- **Intangibilidad**: El jugador es invulnerable durante el dash
- **Elevación**: Permite elevar ligeramente en el eje Y

### Sistema de Combate
- Detección de enemigos en rango (CircleCollider2D)
- Daño configurable por arma
- Knockback al golpear enemigos
- Sistema de salud con muerte y respawn

### Progresión

```
Nivel 1 → Nivel 2 → Nivel 3 [BOSS] → Decisión (BUENO/MALO)
                                          ↓
                                    Nivel 4 → Nivel 5 → Nivel 6 [BOSS] → ...
```

- Cada 3 niveles: Boss fight
- Después de boss: Decisión que afecta la narrativa
- Upgrades recolectados se mantienen entre niveles
- JSON guarda todo el progreso automáticamente

## 📡 Sistema de Eventos

El EventManager desacopla completamente los sistemas del juego. No más `FindWithTag` o referencias cruzadas complejas.

### Flujo de Eventos Típico

```
1. Jugador derrota enemigo
   → EnemyController.Die() 
   → Broadcast: EnemyDefeatedEvent

2. HUDManager escucha el evento
   → Actualiza contador de enemigos
   → Actualiza score

3. LevelManager escucha el evento
   → Verifica si todos los enemigos están muertos
   → Desbloquea puerta final si es el caso

4. Jugador llega al final
   → Finish.OnTriggerEnter()
   → Broadcast: LevelCompleteEvent

5. GameManager escucha el evento
   → Guarda progreso
   → Carga siguiente nivel
```

### Debugging de Eventos

Activa `debugMode = true` en GameManager para ver todos los eventos en Console:
```
[EventManager] Subscrito a BossDefeatedEvent (1 handler)
[EventManager] Broadcast BossDefeatedEvent a 1 handler(s)
[GameManager] Boss derrotado: boss1 en nivel 3
```

## 📚 Documentación Adicional

El proyecto incluye documentación exhaustiva en la carpeta `Scripts/`:

### Documentación de Inicio
- `00_LEEME_PRIMERO.txt` - **LEER PRIMERO**: Resumen ejecutivo de la arquitectura
- `QUICK_START.txt` - Setup en Unity en 30 minutos
- `COMIENZA_AQUI.txt` - Guía de refactorización paso a paso
- `INDICE_ARCHIVOS.txt` - Navegación y orden de lectura

### Documentación Técnica
- `ARQUITECTURA_BRUTAL.txt` - Referencia técnica completa (400+ líneas)
- `ANALISIS_ARQUITECTURA_COMPLETO.md` - Análisis detallado del sistema
- `ARQUITECTURA_METROIDVANIA.md` - Diseño de arquitectura avanzada
- `MAP_STRUCTURE.txt` - Estructura del árbol de archivos

### Documentación de Implementación
- `ROADMAP_IMPLEMENTACION.md` - Plan de implementación por fases
- `GENERADOR_NIVEL_MEJORAS.md` - Mejoras al sistema de generación
- `DIAGRAMA_ARQUITECTURA.txt` - Diagrama visual de la arquitectura
- `INSTRUCCIONES_SETUP.txt` - Setup detallado

### Documentación de Progreso
- `GAMELOG.txt` - Bitácora del desarrollo
- `RESUMEN_EJECUTIVO.txt` - Resumen del proyecto
- `RESUMEN_ENTREGA.txt` - Descripción de entregables
- `ENTREGA_FINAL.txt` - Estado final del proyecto
- `LISTO_PARA_COMENZAR.txt` - Próximos pasos
- `PROXIMO_PASO_FASE1.txt` - Fase 1 de implementación

## 🗺️ Roadmap

### ✅ Fase 1: Fundamentos (COMPLETADO)
- [x] GameManager centralizado
- [x] EventManager con 13 eventos
- [x] GameSaveData con JSON
- [x] PlayerController mejorado
- [x] CameraManager suave
- [x] Interfaces: IDamageable, IEnemy, IBoss
- [x] Documentación completa

### 🟡 Fase 2: Refactorización (EN PROGRESO)
- [ ] MenuManager (consolidar NewGame + Continue)
- [ ] LevelManager (refactorizar GeneradorNivel)
- [ ] Refactorizar Boss.cs para usar eventos
- [ ] Refactorizar Finish.cs para usar eventos

### 📝 Fase 3: Jerarquías de Clases (PENDIENTE)
- [ ] BaseEnemy con lógica común
- [ ] 4 tipos de enemigos (BasicEnemy, FastEnemy, StrongEnemy, FlyingEnemy)
- [ ] BaseBoss con sistema de fases
- [ ] Boss1, Boss2, FinalBoss heredando de BaseBoss

### 📝 Fase 4: Managers Avanzados (PENDIENTE)
- [ ] AudioManager (música por zona y boss)
- [ ] HUDManager (salud, upgrades, score)
- [ ] LevelTransition (fades, cinemáticas)
- [ ] PauseManager (menú de pausa)

### 📝 Fase 5: Polish y Testing (PENDIENTE)
- [ ] Efectos visuales y partículas
- [ ] Animaciones pulidas
- [ ] Sound effects
- [ ] Balance de dificultad
- [ ] Testing completo

## 🤝 Contribuciones

Este proyecto fue desarrollado como parte de un Game Jam. Para contribuir:

1. Fork el repositorio
2. Crea una branch para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la branch (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

### Convenciones de Código

- **Nombres de clases**: PascalCase (`GameManager`, `PlayerController`)
- **Nombres de métodos**: PascalCase (`CreateNewGame()`, `TakeDamage()`)
- **Variables privadas**: camelCase con `_` (`_instance`, `_currentHealth`)
- **Variables públicas**: camelCase (`currentLevel`, `maxHealth`)
- **Constantes**: UPPER_SNAKE_CASE (`MAX_HEALTH`, `DEFAULT_SPEED`)
- **Comentarios**: En español para documentación, en inglés para código técnico

### Guidelines de Arquitectura

1. **Nunca** guardes estado en múltiples lugares - usa GameManager
2. **Siempre** usa EventManager para comunicación entre sistemas
3. **Implementa** interfaces relevantes (IDamageable, IEnemy, IBoss)
4. **Hereda** de clases base cuando tenga sentido (BaseEnemy, BaseBoss)
5. **Evita** `FindWithTag` y `Find` en tiempo de ejecución
6. **Centraliza** la lógica en managers, no la disperses

## 📄 Licencia

Este proyecto está bajo la licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 🙏 Agradecimientos

- Unity Technologies por el motor de juego
- GitHub Copilot por asistencia en arquitectura y documentación
- La comunidad de Game Jams por inspiración y feedback

---

**Desarrollado con ❤️ para Game Jam 2**

Para preguntas o soporte, consulta la documentación en `Scripts/` o abre un issue en GitHub.
