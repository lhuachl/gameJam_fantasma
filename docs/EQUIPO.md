# 👥 Equipo de Desarrollo - Game Jam Fantasma

## Información del Equipo

**Proyecto:** Game Jam Fantasma  
**Duración:** 3 semanas (21 días)  
**Período:** 21 de octubre - 9 de noviembre, 2025  
**Tamaño del equipo:** 4 desarrolladores  
**Metodología:** Ágil con sprints semanales  

---

## Miembros del Equipo

### 🎯 Alex - Lead Developer & Game Designer

**Rol Principal:** Líder Técnico y Diseñador de Juegos  
**Especialidades:** Arquitectura de software, Sistemas core, Gameplay programming  
**Contribución:** 45% del código total  

#### Áreas de Responsabilidad
- Arquitectura central del juego
- Diseño de patrones de software (Singleton, Pub/Sub)
- Implementación de sistemas core
- Liderazgo técnico y toma de decisiones
- Documentación técnica principal

#### Contribuciones Principales

**Sistemas Implementados:**
1. **GameManager** (~350 líneas)
   - Singleton centralizado para estado global
   - Sistema de guardado/carga JSON
   - Gestión de progresión entre niveles
   - Sistema de decisiones BUENO/MALO
   - Gestión de upgrades permanentes

2. **EventManager** (~200 líneas)
   - Patrón Publisher-Subscriber
   - 13 tipos de eventos del juego
   - Sistema de debugging con logs
   - Comunicación desacoplada entre sistemas

3. **PlayerController** (~350 líneas)
   - Movimiento fluido con Input System
   - Salto con detección de suelo mejorada
   - Dash intangible con invulnerabilidad (0.2s)
   - Sistema de ataque con detección de enemigos
   - Sistema de salud con muerte y respawn
   - Broadcasting de eventos

4. **CameraManager** (~150 líneas)
   - Seguimiento suave del jugador
   - Camera boundaries
   - Smoothing configurable
   - Lock opcional en ejes

5. **Interfaces del Sistema**
   - `IDamageable` - Entidades que reciben daño
   - `IEnemy` - Comportamiento de enemigos
   - `IBoss` - Comportamiento de jefes

**Documentación Creada:**
- 00_LEEME_PRIMERO.txt
- ARQUITECTURA_BRUTAL.txt
- COMIENZA_AQUI.txt
- README.md principal
- 10+ documentos técnicos adicionales

**Tecnologías Dominadas:**
- C# avanzado (.NET Framework 4.x)
- Unity Engine 2022.3+
- Design Patterns (Singleton, Observer, Factory)
- Input System (new Unity Input System)
- Git y control de versiones

**Logros Destacados:**
- ✅ Arquitectura escalable y mantenible
- ✅ Eliminó acoplamiento entre sistemas
- ✅ Redujo FindWithTag de 30+ a 0 instancias
- ✅ Estableció convenciones de código del equipo
- ✅ Lideró sesiones de code review

**Cita:**
> "La arquitectura correcta al inicio es la diferencia entre un prototipo y un juego escalable. Invertimos tiempo en hacer las cosas bien desde el principio."

---

### 💎 Saul - Senior Developer & Technical Artist

**Rol Principal:** Desarrollador Senior y Artista Técnico  
**Especialidades:** IA de enemigos, Sistemas visuales, Optimización de performance  
**Contribución:** 35% del código total  

#### Áreas de Responsabilidad
- Sistema completo de enemigos
- Parallax y sistemas visuales
- Optimización de rendimiento
- Arte técnico y shaders
- Resolución de problemas técnicos complejos

#### Contribuciones Principales

**Sistemas Implementados:**
1. **BaseEnemy** (~200 líneas)
   - Clase abstracta con lógica común de enemigos
   - Sistema de detección de bordes con raycasts
   - Detección de paredes y obstáculos
   - Sistema de flip automático del sprite
   - Integración con interfaces IDamageable e IEnemy

2. **PatrolEnemy** (~180 líneas)
   - Patrullaje inteligente de plataformas
   - Detección de bordes para no caer
   - Cambio de dirección en paredes
   - Sistema de wait time en puntos de patrulla
   - Edge/Wall detection configurable

3. **ChaseEnemy** (~220 líneas)
   - Persecución del jugador en rango de visión
   - Estado Idle con wander aleatorio
   - Chase speed multiplier para velocidad en persecución
   - Transición suave entre estados
   - Sistema de ataque cuando alcanza al jugador

4. **BackgroundManager** (~280 líneas)
   - Sistema de parallax multicapa
   - Infinite tiling horizontal automático
   - Escala automática a resolución objetivo (1920x1080)
   - Parallax factors configurables por capa
   - Optimización para múltiples fondos simultáneos

5. **Boss.cs** (contribución)
   - Sistema de fases de combate
   - Comportamientos únicos por fase
   - Transiciones entre fases
   - Ataques especiales por fase

6. **FinalBoss.cs**
   - 4 fases de combate únicas
   - Mecánicas especiales por fase
   - Spawn de enemigos en fases específicas
   - Patrón de ataques complejo

**Soluciones Técnicas:**
- Implementó detección de bordes para evitar caídas de enemigos
- Optimizó raycasts usando layers específicos
- Creó sistema de escala automática para diferentes resoluciones
- Resolvió bug de infinite tiling sin costuras
- Implementó object pooling básico para proyectiles

**Tecnologías Dominadas:**
- C# y Unity avanzado
- Physics2D y Raycasting
- Sistema de animaciones en Unity
- Shader programming básico
- Performance profiling

**Logros Destacados:**
- ✅ 4 tipos de enemigos completamente funcionales
- ✅ Sistema de parallax profesional multicapa
- ✅ IA de enemigos sin bugs de plataformas
- ✅ Optimización a 60 FPS estables
- ✅ Resolución de los desafíos técnicos más complejos

**Cita:**
> "Los detalles técnicos marcan la diferencia entre un juego que funciona y un juego que se siente pulido. La detección de bordes fue crítica para la experiencia del jugador."

---

### 🎮 Russel - Game Developer

**Rol Principal:** Desarrollador de Juegos  
**Especialidades:** Generación de niveles, Sistemas de guardado, UI  
**Contribución:** 12% del código total  

#### Áreas de Responsabilidad
- Generación procedimental de niveles
- Sistema de guardado/carga
- Elementos de UI
- Testing y QA
- Documentación de usuario

#### Contribuciones Principales

**Sistemas Implementados:**
1. **GeneradorNivel.cs** (~300 líneas)
   - Generación de niveles desde archivos CSV
   - Parsing de formato nivel (X, Y, Z, V, B, P)
   - Instanciación de paredes, enemigos, boss, y jugador
   - Soporte para múltiples tipos de tiles
   - Configuración de layers automática

2. **GameSaveData.cs** (~150 líneas)
   - Estructura serializable completa
   - Tracking de nivel actual y completados
   - Historial de decisiones BUENO/MALO
   - Diccionario de bosses derrotados
   - Stats del jugador (salud, daño, habilidades)
   - Tiempo de juego acumulado

3. **Guardadodepartida.cs** (legacy, contribución)
   - Sistema de guardado inicial
   - Serialización JSON
   - Gestión de archivos

4. **Continue.cs**
   - Carga de partida guardada
   - Restauración de estado del juego
   - Validación de archivos de guardado

5. **NewGame.cs**
   - Inicio de nueva partida
   - Inicialización de valores por defecto
   - Limpieza de datos previos

**Formatos CSV Diseñados:**
```
Nivel_1.csv formato:
X,Y,Z,V = Tipos de enemigos
B = Boss
P = Jugador
# = Pared/plataforma
```

**Documentación Creada:**
- CHANGELOG.md
- Parte del README.md
- MAP_STRUCTURE.txt
- Guías de formato CSV

**Tecnologías Dominadas:**
- C# y Unity básico-intermedio
- JSON serialization
- File I/O en Unity
- CSV parsing
- Unity UI

**Logros Destacados:**
- ✅ Sistema de niveles CSV funcional y flexible
- ✅ Guardado automático sin pérdida de datos
- ✅ Documentación clara para diseñadores de niveles
- ✅ Testing exhaustivo de sistemas de guardado

**Cita:**
> "Un buen sistema de guardado es invisible cuando funciona, pero esencial para la experiencia del jugador. La persistencia debe ser confiable."

---

### 🎨 Erik - Artist & Developer

**Rol Principal:** Artista del Juego y Desarrollador de Soporte  
**Especialidades:** Arte 2D, Animación, Sprites, Efectos visuales  
**Contribución:** 8% del código + 100% del arte  

#### Áreas de Responsabilidad
- Diseño visual del juego
- Creación de sprites
- Animaciones de personajes y enemigos
- Efectos visuales
- UI art
- Identidad visual del proyecto

#### Contribuciones Principales

**Assets Artísticos Creados:**

**Personaje Principal:**
- Idle Animation: 7 frames (MainCharacterChapter1Iddle1-7.png)
- Run Animation: 6 frames (corriendo.png spritesheet)
- Jump Animation: 4 frames (salto.png)
- Attack Animation: 5 frames (atack sprites)
- Dash Animation: 3 frames (dash.png)
- Parry Animation: 4 frames (parry.png)

**Total frames personaje:** 29 frames

**Enemigos:**
- Enemigo Básico: 8 frames walk animation
- Enemigo Ataque: Sprite sheet 4 frames
- Boss Sprites: 10+ frames múltiples fases
- Final Boss: 15+ frames animaciones especiales

**Total sprites enemigos:** 50+

**Backgrounds:**
- 4 capas de parallax con diferentes profundidades
- Resolución: 1920x1080 optimizado
- Fondos temáticos para diferentes zonas

**UI Elements:**
- Sprites de vida (hearts/health bar)
- Elementos de menú principal
- Iconos de habilidades y upgrades
- Efectos de transición entre niveles

**Efectos Visuales:**
- Partículas de dash
- Efectos de impacto
- Explosiones de enemigos derrotados
- Efectos de boss attacks

**Código Contribuido:**
- BackgroundParallaxFill.cs (mantenimiento)
- Pinchos.cs (trampas)
- Ajustes de animators en Unity
- Configuración de sprite atlases

**Software Utilizado:**
- Piskel (pixel art)
- Photoshop (sprites y efectos)
- Unity Animator (animaciones)
- Sprite editor tools

**Logros Destacados:**
- ✅ 50+ sprites únicos creados
- ✅ 25+ animaciones fluidas
- ✅ 4 capas de fondos parallax
- ✅ Identidad visual cohesiva del juego
- ✅ Assets optimizados para performance

**Pipeline de Trabajo:**
1. Concept art y bocetos iniciales
2. Pixel art/sprites en Piskel
3. Export PNG con transparencia
4. Import en Unity con settings optimizados
5. Creación de animaciones en Animator
6. Ajustes basados en feedback del equipo

**Cita:**
> "El arte da vida al código. Cada sprite y animación fue diseñado para complementar las mecánicas del juego y crear una experiencia visual memorable."

---

## 📊 Distribución de Trabajo

### Por Área de Desarrollo

```
Arquitectura & Core Systems:    Alex (80%)  + Saul (20%)
Sistema de Enemigos:            Saul (95%)  + Alex (5%)
Sistema de Guardado:            Russel (70%) + Alex (30%)
PlayerController:               Alex (100%)
Visual Systems (Parallax):      Saul (100%)
Arte y Animaciones:             Erik (100%)
Generación de Niveles:          Russel (90%) + Alex (10%)
Documentación Técnica:          Alex (70%)  + Russel (30%)
Testing & QA:                   Todo el equipo
```

### Por Semana

**Semana 1 (Setup & Prototipos):**
- Alex: 50% del trabajo técnico
- Saul: 25% del trabajo técnico
- Russel: 15% del trabajo técnico
- Erik: 60% del trabajo artístico

**Semana 2 (Core Systems & Bosses):**
- Alex: 45% del trabajo técnico (arquitectura)
- Saul: 40% del trabajo técnico (enemigos y boss)
- Russel: 10% del trabajo técnico (guardado)
- Erik: 30% del trabajo artístico

**Semana 3 (Polish & Entrega):**
- Alex: 40% del trabajo técnico (documentación)
- Saul: 35% del trabajo técnico (parallax y optimización)
- Russel: 15% del trabajo técnico (testing)
- Erik: 50% del trabajo artístico (polish)

---

## 🏆 Logros del Equipo

### Técnicos
✅ Arquitectura profesional con patrones de diseño  
✅ 4,500+ líneas de código limpio y documentado  
✅ 35+ scripts C# organizados  
✅ 13 eventos de sistema implementados  
✅ 3 interfaces bien definidas  
✅ 60 FPS estables en hardware de gama media  
✅ Zero bugs críticos en entrega final  

### Artísticos
✅ 50+ sprites únicos  
✅ 25+ animaciones fluidas  
✅ 4 capas de parallax profesionales  
✅ Identidad visual cohesiva  
✅ Assets optimizados para performance  

### Documentación
✅ 15+ archivos de documentación  
✅ 3,000+ líneas de docs técnicas  
✅ README completo (437 líneas)  
✅ CHANGELOG detallado (219 líneas)  
✅ Guías de setup y configuración  

---

## 💬 Reflexiones del Equipo

### Alex (Lead Developer)
> "Este proyecto demostró que una arquitectura sólida desde el inicio no es sobre sobre-ingeniería, sino sobre crear una base que facilita el desarrollo rápido y la escalabilidad. El EventManager eliminó tantos problemas de acoplamiento que cada nueva feature se volvió más fácil de implementar. Si volviera a empezar, haría exactamente lo mismo."

### Saul (Senior Developer)
> "Los desafíos técnicos fueron satisfactorios de resolver. La detección de bordes para enemigos parecía simple en papel, pero requirió varias iteraciones para hacerlo robusto. Ver a los enemigos patrullando sin caer de las plataformas fue muy gratificante. El sistema de parallax también fue un reto interesante - lograr infinite tiling sin costuras requirió matemáticas precisas."

### Russel (Developer)
> "Aprendí muchísimo sobre serialización y persistencia de datos en Unity. Al inicio, el sistema de guardado tenía bugs porque guardábamos datos en múltiples lugares. Alex me ayudó a centralizar todo en GameSaveData y desde entonces todo funcionó perfectamente. El sistema CSV para niveles también fue educativo - parsing y validación son más complejos de lo que parecen."

### Erik (Artist)
> "Fue mi primera experiencia en un Game Jam con un equipo tan técnico. Inicialmente me preocupaba que el arte se viera opacado por el código, pero el equipo valoró cada sprite y animación. La colaboración con Saul en el parallax fue especialmente buena - él entendió mi visión artística y la hizo realidad técnicamente. Ver mis sprites cobrando vida en el juego fue increíble."

---

## 🤝 Dinámica de Equipo

### Comunicación
- Daily stand-ups (15 minutos cada mañana)
- Sesiones de code review 2 veces por semana
- Playtesting en grupo cada viernes
- Chat de Discord para comunicación continua

### Herramientas Utilizadas
- **Git/GitHub:** Control de versiones y colaboración
- **Discord:** Comunicación en tiempo real
- **Unity Collaborate:** Sincronización de escenas
- **Google Docs:** Documentación compartida inicial
- **Trello:** Tracking de tareas (primera semana)

### Metodología
- Sprints semanales con objetivos claros
- Pair programming para features complejas
- Code reviews obligatorias antes de merge
- Testing compartido de nuevas features

---

## 📈 Impacto Individual

### Métricas de Commits (Estimado)

| Desarrollador | Commits | Líneas Añadidas | Líneas Eliminadas | Archivos Modificados |
|---------------|---------|-----------------|-------------------|----------------------|
| Alex          | 12      | 2,500+          | 800+              | 25+                  |
| Saul          | 10      | 1,800+          | 400+              | 18+                  |
| Russel        | 4       | 600+            | 200+              | 8+                   |
| Erik          | 2       | 150+            | 50+               | 50+ (assets)         |

**Nota:** Erik contribuyó principalmente assets (sprites, animaciones) que no se reflejan completamente en commits de código.

---

## 🎯 Palabras Finales

El equipo de **Game Jam Fantasma** demostró que:

✅ **Arquitectura + Arte = Producto Profesional**  
La combinación de código sólido (Alex, Saul) con arte de calidad (Erik) creó un juego que no solo funciona sino que se ve bien.

✅ **Liderazgo Técnico es Esencial**  
El liderazgo de Alex estableció estándares de código y arquitectura que el equipo siguió exitosamente.

✅ **Especialización + Colaboración = Eficiencia**  
Cada miembro se enfocó en su área fuerte mientras colaboraba cuando era necesario.

✅ **Documentación = Legado**  
La documentación exhaustiva asegura que el proyecto puede continuar desarrollándose en el futuro.

---

**Equipo formado:** Octubre 21, 2025  
**Proyecto completado:** Noviembre 9, 2025  
**Resultado:** Exitoso ✅  

---

*"Individualmente somos una gota. Juntos somos un océano." - Ryunosuke Satoro*

Este equipo lo demostró en 21 días.
