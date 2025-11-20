# 👥 El Equipo - Game Jam Fantasma

## Quiénes Somos

**Proyecto:** Game Jam Fantasma  
**Duración:** 3 semanas (que se sintieron como 3 días)  
**Período:** 21 de octubre - 9 de noviembre, 2025  
**Equipo:** 4 personas (amigos que querían hacer un juego)  
**"Metodología":** Caos organizado + Discord 24/7  

---

## Los Miembros

### 🎯 Alex - El que "sabía" Unity

**Rol:** Líder técnico (autoproclamado)  
**Experiencia previa:** Algunos tutoriales de YouTube y un proyecto de la uni  
**Contribución:** ~45% del código

#### Lo que hizo

- Armó la estructura del proyecto (copió de otro proyecto anterior)
- Hizo el GameManager con Singleton (porque todos los tutoriales lo usan)
- Implementó el sistema de eventos (esto sí estuvo bien)
- El PlayerController (que refactorizamos como 5 veces)
- La cámara que sigue al jugador
- MUCHA documentación (quizá demasiada)

**Lo que REALMENTE pasó:**
- Primera semana: "Vamos a hacerlo todo súper profesional"
- Segunda semana: "Ok, quizá muy profesional no..."
- Tercera semana: "QUE FUNCIONE PORFA"

**Tecnologías:**
- Unity (más o menos)
- C# (con ayuda de copilot)
- Git (causó 3 conflictos de merge)

**Su frase favorita:** "Pero en el tutorial funcionaba..."

---

### 💎 Saul - El solucionador de problemas

**Rol:** Dev senior (tiene más experiencia que el resto)  
**Experiencia:** Algunos proyectos personales + un trabajo freelance  
**Contribución:** ~35% del código

#### Lo que hizo

- TODO el sistema de enemigos
- El parallax (quedó re bien la verdad)
- Los bosses con sus fases
- Arregló como 20 bugs que los demás causamos
- Optimizó el juego cuando corría a 20 FPS

**Features:**
1. **Enemigos** - Hizo 4 tipos diferentes
   - Los que patrullan (tardó un día en que no se caigan de las plataformas)
   - Los que te persiguen
   - Los bosses

2. **Sistema de Parallax**
   - Múltiples capas
   - Se escala solo
   - Se ve profesional (gracias Saul)

3. **Detección de bordes**
   - Para que los enemigos no caigan (después de que cayeran como 100 veces)

**Momentos destacados:**
- "Ya arreglé tu código" (lo dijo como 10 veces)
- Resolvió el bug del parallax a las 3 AM
- Salvó el proyecto cuando Alex borró la escena principal sin querer

**Su frase favorita:** "¿Quién usó FindObjectOfType en el Update?"

---

### 🎮 Russel - El de los niveles

**Rol:** Developer junior  
**Experiencia:** Cursó programación 1 y 2  
**Contribución:** ~12% del código + los niveles

#### Lo que hizo

- Sistema de guardado (con JSON porque era lo más fácil)
- Generador de niveles desde CSV
- Algunos scripts de UI
- Testing (encontró muchos bugs)
- El CHANGELOG

**Sistemas:**
1. **Generador de niveles**
   - Lee archivos CSV
   - Pone las paredes y enemigos donde deben ir
   - Bueno... casi siempre donde deben ir

2. **Sistema de Guardado**
   - Guarda el progreso
   - A veces funciona al cargar
   - En la versión final ya funciona bien

**Momentos memorables:**
- Generó un nivel con 50 enemigos por accidente
- El guardado borró todo una vez (no volvió a pasar)
- Documentó todo en el CHANGELOG (el único que leyó bien el git commit)

**Aprendizajes:**
- JSON es tu amigo
- CSV es simple pero efectivo  
- Hacer backup antes de probar cosas random

**Su frase favorita:** "Creo que ya encontré el bug... nope"

---

### 🎨 Erik - El artista

**Rol:** Artista (y código cuando era necesario)  
**Experiencia:** Hace pixel art de hobby  
**Contribución:** TODO el arte + ~8% código

#### Lo que hizo

**Arte (lo importante):**
- Personaje principal: 29 frames de animación
- 4 tipos de enemigos
- 2 bosses con animaciones
- 4 capas de fondos parallax
- UI elements
- 70+ sprites en total

**"Código":**
- Script de pinchos
- Ayudó con el parallax
- Configuró los animators en Unity

**Pipeline:**
1. Dibujar en Piskel
2. Exportar
3. Importar en Unity
4. "¿Por qué se ve pixelado?" (configurar el import settings)
5. Repetir

**Desafíos:**
- Unity cambiaba la configuración de los sprites (????)
- Los sprites se veían mal hasta que Saul ayudó
- Hacer que 29 frames de animación se vean fluidos

**Logros:**
- El juego se ve bonito (gracias a él)
- Nadie se quejó del arte
- Los enemigos dan un poco de miedo

**Su frase favorita:** "¿Me pueden esperar para probar? Quiero ver cómo se ve"

---

## 📊 La Realidad del Trabajo

### Quién hizo qué (honestamente)

```
Arquitectura: Alex (80%) + Saul arreglando cosas (20%)
Enemigos: Saul (95%) + Alex copiando código (5%)  
Guardado: Russel (100%) después de 5 intentos
PlayerController: Alex (100%) después de 3 refactors
Arte: Erik (100%) y quedó genial
Documentación: Alex (90%) porque le gusta escribir
Bugs: TODOS (100% teamwork)
```

### Por semana

**Semana 1:**
- Alex: Setup del proyecto + primeros scripts
- Saul: Ayudó con la estructura
- Russel: Aprendió más Unity
- Erik: Hizo el 60% del arte

**Semana 2:**
- Alex: GameManager + EventManager
- Saul: Enemigos + Boss
- Russel: Sistema de guardado (intento 2)
- Erik: Animaciones

**Semana 3:**
- Alex: Documentación + arreglar bugs
- Saul: Parallax + optimización + más arreglos de bugs
- Russel: Testing + más testing
- Erik: Polish del arte
- TODOS: Panic mode los últimos 2 días

---

## 🏆 Lo que logramos (contra todo pronóstico)

### Cosas que salieron bien
✅ El juego funciona (la mayoría del tiempo)  
✅ No hay bugs críticos (que sepamos)  
✅ Los enemigos no se caen de las plataformas  
✅ El arte se ve increíble  
✅ El sistema de guardado funciona  
✅ Documentamos todo (quizá demasiado)

### Cosas que no salieron tan bien
❌ El código tiene partes... mejorables  
❌ Hay TODOs en el código (muchos)  
❌ Algunos assets no se usan  
❌ La carpeta "Legacy" tiene código que da miedo  
❌ Merge conflicts (nunca más)

---

## 💬 Reflexiones Honestas

### Alex
> "Aprendí que 'profesional' no significa 'sobre-documentar todo'. También que Git es más complejo de lo que pensaba. Y que Saul es un héroe por arreglar todo lo que rompí."

### Saul  
> "Fue divertido. También frustrante. Los enemigos cayéndose de las plataformas me quitó el sueño. Pero cuando finalmente funcionó... chef's kiss. Ah, y aprendí a no dejar que Alex haga commits a las 3 AM."

### Russel
> "Mi sistema de guardado borró todo el progreso una vez y casi lloro. Pero aprendí muchísimo. CSV es simple pero funciona. JSON igual. Y hacer testing es importante (ahora entiendo por qué)."

### Erik
> "Nunca pensé que hacer sprites para Unity fuera tan... específico. Cada vez que pensaba que terminaba, había que ajustar algo. Pero el equipo valoró cada sprite y eso fue genial. Ver mis dibujos moverse en el juego fue lo máximo."

---

## 🤝 Cómo trabajamos (o intentamos)

### "Metodología"
- Daily... cuando nos acordábamos
- Discord abierto 24/7
- "Code reviews" = "Saul, revisa esto"
- Testing = jugarlo y ver qué se rompe

### Herramientas
- **Discord:** Para todo
- **Git:** Para sufrir juntos
- **Unity:** Obvio
- **Stack Overflow:** El verdadero héroe
- **YouTube:** Por los tutoriales
- **Café:** Vital

### Horarios
- Alex: Madrugadas (commits a las 4 AM)
- Saul: Tardes-noches (el más consistente)
- Russel: Después de clases
- Erik: Fines de semana + noches

---

## 📈 Stats reales

| Persona | Commits | Bugs creados | Bugs arreglados | Café consumido |
|---------|---------|--------------|-----------------|----------------|
| Alex    | 12      | 30+          | 15              | Mucho          |
| Saul    | 10      | 5            | 40+             | Demasiado      |
| Russel  | 4       | 10           | 8               | Normal         |
| Erik    | 2       | 2            | 2               | Poco           |

---

## 🎯 Conclusión

Somos un equipo amateur que hizo su mejor esfuerzo. El resultado no es perfecto pero:
- Funciona
- Se ve bien
- Aprendimos un montón
- Nos divertimos (más o menos)
- Lo terminamos (milagro)

Y eso es lo que importa.

---

**Equipo formado:** Octubre 21, 2025  
**Proyecto "terminado":** Noviembre 9, 2025  
**Estado:** Vivos (apenas)  
**¿Lo volveríamos a hacer?:** Probablemente

---

*"El verdadero game jam fueron los amigos que hicimos en el camino... y los bugs que arreglamos juntos"*
