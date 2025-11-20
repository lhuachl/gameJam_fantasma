# 📊 Cómo fue el desarrollo (la verdad)

## Resumen

**Proyecto:** Game Jam Fantasma  
**Duración:** 3 semanas  
**Fecha:** 21 octubre - 9 noviembre, 2025  
**Equipo:** 4 amigos  
**Meta:** Hacer un juego  
**Resultado:** Lo logramos (sorprendentemente)

---

## Semana 1: "Esto va a estar fácil"

### Día 1-2: Planning
- Alex armó el proyecto en Unity
- Hicimos brainstorming de mecánicas
- Erik empezó a dibujar el personaje
- "Vamos a hacer las cosas profesionales" - Alex

**Decisiones importantes:**
- Motor: Unity (porque era lo que sabíamos)
- Estilo: Pixel art (porque Erik sabe hacerlo)
- Género: Plataformero (clásico)

### Día 3-5: Primeros prototipos
- Alex hizo movimiento básico del jugador
- Saul empezó con enemigos simples
- Erik terminó sprites del personaje
- Russel investigó cómo generar niveles

**Logros:**
✅ El personaje se mueve
✅ El personaje salta
✅ Hay enemigos (que se caen de las plataformas)
✅ Se ve... ok

**Problemas:**
- Los enemigos caen de las plataformas
- La cámara no sigue bien
- No hay sistema de daño todavía
- Git dio problemas (primer merge conflict)

### Día 6-7: Arreglar cosas
- Saul pasó UN DÍA ENTERO haciendo que los enemigos no caigan
- Alex implementó la cámara
- Russel hizo primer prototipo de nivel con CSV
- Erik hizo sprites de enemigos

**Estado al final de semana 1:**
- Juego básico funcional
- 8 commits
- ~800 líneas de código
- Muchas ganas todavía

---

## Semana 2: "Ok, esto es más difícil"

### Día 8-10: Sistema de combate
- Alex añadió ataque al jugador
- Saul hizo que enemigos detecten al jugador
- Sistema de vida implementado
- Erik hizo animaciones de ataque

**Bugs descubiertos:**
- Enemigos se caen a veces todavía
- El jugador puede atacar infinito de rápido
- La vida no se guarda
- Unity crasheó 3 veces

### Día 11-13: Arquitectura "profesional"
- Alex: "Necesitamos eventos!"
- Implementó EventManager
- Creó GameManager con Singleton
- Saul hizo el primer Boss

**Quote del día:** "¿Por qué usamos FindObjectOfType 30 veces en Update?" - Saul

**Refactor grande:**
- Centralizamos estado en GameManager
- Quitamos FindObjectOfType (casi todos)
- Implementamos sistema de eventos
- El código quedó más limpio

### Día 14: Review y panic
- Playtesting en equipo
- Encontramos 20+ bugs
- Lista de TODOs creció
- "¿Va a estar listo?" - Todos

**Decisiones difíciles:**
- No va a haber sonido (no hay tiempo)
- Menus simples (muy simples)
- 3 niveles en vez de 5

**Estado al final de semana 2:**
- Boss funcional
- Sistema de eventos funcionando
- Guardado implementado (más o menos)
- 15 commits más
- Menos confianza que semana 1

---

## Semana 3: "QUE FUNCIONE"

### Día 15-16: Polish que no es polish
- Alex añadió dash intangible (después de 5 intentos)
- Saul optimizó rendimiento
- Russel testeó todo
- Erik hizo más animaciones

**Bug del día:** El dash hacía al jugador invisible permanentemente

### Día 17-18: Visual upgrades
- Saul implementó parallax
- "Wow, esto sí se ve bien" - Todos
- Erik terminó todos los sprites
- Sistema de niveles mejorado

**Momentos memorables:**
- El parallax funcionó a la primera (milagro)
- Alex borró la escena principal sin querer
- Saul lo arregló del backup
- Crisis evitada

### Día 19-20: CRUNCH TIME
- Todos trabajando al mismo tiempo
- Discord abierto 24/7
- "¿Alguien durmió?" - Nadie
- Final Boss implementado
- Sistema de decisiones agregado

**Bugs encontrados:**
1. Guardado borra todo (arreglado)
2. Enemigos vuelan a veces (arreglado)
3. Cámara sale del mapa (arreglado)
4. Player se queda trabado (arreglado)
5. Boss no muere (arreglado)
6. Y 10 más...

### Día 21: Entrega
- Testing final
- Alex escribió documentación
- Bug fixing de último minuto
- Build final
- "Lo logramos!"

**Commits de las últimas horas:**
- "Fix critical bug"
- "Fix another critical bug"
- "Please work"
- "Final build"
- "Actually final build"

---

## Qué salió bien

✅ **Terminamos** - Eso solo ya es un logro
✅ **Funciona** - El juego corre sin crashear (casi siempre)
✅ **Se ve bien** - Gracias Erik
✅ **Código decente** - No es espagueti total
✅ **Aprendimos** - Un montón

## Qué salió mal

❌ **No hay sonido** - No alcanzó el tiempo
❌ **Menús básicos** - Funcionan pero se ven feos
❌ **Algunos bugs** - Menores, pero ahí están
❌ **Documentación tardía** - La hicimos después
❌ **Crunch** - Los últimos días fueron duros

---

## Desafíos técnicos

### 1. Enemigos cayendo
**Problema:** Los enemigos se caían de las plataformas
**Intentos:** 5
**Tiempo:** 1 día completo
**Solución:** Raycasts de detección de bordes (gracias Saul)

### 2. Sistema de guardado
**Problema:** Guardaba en lugares random, a veces borraba todo
**Intentos:** 3
**Tiempo:** 2 días
**Solución:** Un solo archivo JSON centralizado

### 3. Rendimiento
**Problema:** 20 FPS con muchos enemigos
**Intentos:** Varios
**Tiempo:** Medio día
**Solución:** Optimizaciones de Saul

### 4. Dash intangible
**Problema:** El dash no daba invulnerabilidad bien
**Intentos:** 5+
**Tiempo:** Tarde entera
**Solución:** Flag isDashing que se checa en TakeDamage

### 5. Merge conflicts
**Problema:** Git es complicado
**Intentos:** Muchos
**Tiempo:** Demasiado
**Solución:** Saul explicó Git mejor

---

## Métricas finales

### Código
- Líneas de código: ~4,500
- Scripts: 35+
- Commits: 28
- Branches: 3 (main, y 2 de features)
- Merge conflicts: 6

### Arte
- Sprites: 50+
- Animaciones: 25+
- Capas parallax: 4
- Horas de dibujo: Muchas

### Gameplay
- Niveles: 3 (menos de lo planeado)
- Enemigos: 4 tipos
- Bosses: 2
- Mecánicas: 5 (mover, saltar, dash, atacar, morir)

### Time
- Horas trabajadas: ~300 (entre todos)
- Noches sin dormir: 3
- Reuniones: ~15
- Panic attacks: Varios

---

## Lecciones aprendidas

### Técnicas
1. **Eventos > Referencias directas** - Menos acoplamiento
2. **Una fuente de verdad** - GameManager centralizado
3. **Git es importante** - Pero difícil
4. **Testing temprano** - Ahorra tiempo después
5. **Backups** - SIEMPRE

### De Proyecto
1. **Planear mejor** - Subestimamos el tiempo
2. **Scope pequeño** - Es mejor terminar algo simple
3. **Comunicación** - Discord 24/7 ayudó
4. **Especialización** - Cada uno en su área
5. **Equipo** - Es más fácil con ayuda

### Personales
1. **Alex:** "No todo necesita documentación exhaustiva"
2. **Saul:** "Ser el debugger del equipo es agotador"
3. **Russel:** "Testing es importante (ahora lo entiendo)"
4. **Erik:** "Programadores y artistas piensan diferente"

---

## Stats curiosas

- Commits a las 3+ AM: 7
- Veces que Unity crasheó: 15+
- "It works on my machine": 8
- Bugs encontrados en producción: 2
- Pizzas consumidas: 6
- Litros de café: Muchos
- Veces que quisimos rendirnos: 3
- Veces que nos alegramos de no rendirnos: 1 (cuando terminamos)

---

## Palabras finales

Hicimos un juego en 3 semanas. No es perfecto. Tiene bugs. Le falta sonido. Los menús son básicos. Pero funciona. Y lo terminamos. Y aprendimos un montón.

Para un equipo amateur en su primer game jam serio, estamos orgullosos del resultado.

¿Lo volveríamos a hacer? Probablemente. Pero con más tiempo y café.

---

## Agradecimientos

- **YouTube** - Por los tutoriales
- **Stack Overflow** - Por las respuestas
- **Unity** - Por ser gratis
- **Café** - Por existir
- **Discord** - Por mantenernos conectados
- **Nuestras familias** - Por aguantarnos
- **Nosotros** - Por no rendirnos

---

**Informe escrito por:** El equipo  
**Fecha:** Después de entregar  
**Estado emocional:** Exhaustos pero felices  
**Precisión:** ~85%

*"No es mucho, pero es trabajo honesto"*
