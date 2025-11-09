# Guía de Configuración Rápida en Unity

## 🎮 Configuración de Enemigos (5 minutos)

### Paso 1: Configurar Layers

**Unity → Edit → Project Settings → Tags and Layers**

Añadir estos Layers:
```
Layer 8: Ground
Layer 9: Player  
Layer 10: Enemy
```

### Paso 2: Crear Prefab de PatrolEnemy

1. **Crear GameObject:** Hierarchy → Create Empty → Nombrar "PatrolEnemy"

2. **Añadir Componentes:**
   ```
   Add Component → Physics 2D → Rigidbody 2D
   ├─ Gravity Scale: 1
   ├─ Constraints: Freeze Rotation Z ✓
   
   Add Component → Physics 2D → Box Collider 2D
   ├─ Size: (0.8, 0.8)
   
   Add Component → Rendering → Sprite Renderer
   ├─ Sprite: [Tu sprite de enemigo]
   ├─ Sorting Layer: Default
   ├─ Order in Layer: 1
   
   Add Component → Scripts → Patrol Enemy
   ```

3. **Configurar PatrolEnemy Script:**
   ```
   === Stats ===
   Max Health: 3
   Attack Damage: 1
   Move Speed: 2.0
   
   === Vision & Attack ===
   Vision Range: 5.0
   Attack Range: 1.2
   Attack Cooldown: 1.0
   Player Layer: Nothing (o configura Layer Player si existe)
   
   === Platform Detection ===
   Ground Layer: Ground (IMPORTANTE!)
   Edge Check Distance: 0.6
   Wall Check Distance: 0.4
   Edge Check Offset: (0.4, 0.05)
   Wall Check Offset: (0.4, 0.1)
   
   === Patrol Settings ===
   Patrol Wait Time: 1.0
   Start Moving Right: false
   ```

4. **Guardar como Prefab:**
   - Arrastrar GameObject a carpeta `Prefab/`
   - Nombrar: `PatrolEnemyPrefab`

### Paso 3: Crear Prefab de ChaseEnemy

Similar a Paso 2, pero:
- Nombrar: "ChaseEnemy"
- Usar componente `Chase Enemy`
- Configurar:
  ```
  === Stats ===
  Move Speed: 2.5 (más rápido)
  
  === Chase Settings ===
  Chase Speed Multiplier: 1.5
  Idle Wander Chance: 0.3
  Wander Change Interval: 2.0
  ```

### Paso 4: Configurar Plataformas

**CRÍTICO:** Las plataformas deben tener Layer "Ground"

Para cada plataforma en tu escena:
```
1. Seleccionar GameObject de plataforma
2. Inspector → Layer → Ground
3. Verificar que tenga Collider 2D
```

### Paso 5: Crear Enemy Spawner

1. **Crear GameObject vacío** en posición de spawn
2. **Añadir componente** `Enemy Spawner`
3. **Configurar:**
   ```
   === Configuration ===
   Enemy Types: (Size: 2)
   
   [0] Enemy Spawn Config
       Enemy Prefab: PatrolEnemyPrefab
       Enemy Type: X
       Spawn Weight: 60
   
   [1] Enemy Spawn Config
       Enemy Prefab: ChaseEnemyPrefab
       Enemy Type: Y
       Spawn Weight: 40
   
   === Spawn Settings ===
   Initial Delay: 2.0
   Spawn Interval: 3.0
   Max Enemies: 5
   Spawn On Start: ✓
   
   === Fallback ===
   Use Code Generated Enemies: ✓
   Enemy Sprite: [Opcional: tu sprite]
   ```

---

## 🎨 Configuración de Parallax (5 minutos)

### Paso 1: Preparar Sprites de Fondo

**Requisitos:**
- Resolución: 1920x1080 o 960x540
- Formato: PNG con transparencia (opcional)
- Import Settings en Unity:
  - Texture Type: Sprite (2D and UI)
  - Pixels Per Unit: 100
  - Filter Mode: Bilinear
  - Compression: None (para calidad máxima)

### Paso 2: Crear GameObjects de Fondo

Para cada capa de fondo:

1. **Crear GameObject vacío:**
   ```
   Hierarchy → Create Empty → Nombrar "Fondo_Capa1"
   ```

2. **Añadir Sprite Renderer:**
   ```
   Add Component → Sprite Renderer
   ├─ Sprite: [Tu sprite de fondo]
   ├─ Sorting Layer: Background (crear si no existe)
   ├─ Order in Layer: -3 (para capa más lejana)
   ```

3. **Posicionar:**
   ```
   Transform:
   ├─ Position: (0, 0, -30) // Z negativo = atrás
   ├─ Rotation: (0, 0, 0)
   ├─ Scale: (1, 1, 1) // Se escalará automáticamente
   ```

Repetir para cada capa (Capa2, Capa3, etc.)

### Paso 3: Crear BackgroundManager

1. **Crear GameObject vacío:**
   ```
   Hierarchy → Create Empty → Nombrar "BackgroundManager"
   Position: (0, 0, 0)
   ```

2. **Añadir componente** `Background Manager`

3. **Configurar Capas:**
   ```
   === Layers Configuration ===
   Size: 3 (para 3 capas de ejemplo)
   
   [0] Parallax Layer (Cielo/Fondo lejano)
       Background Object: Fondo_Capa1
       Parallax Factor: 0.1
       Z Depth: -30
       Infinite Tiling X: ✓
       Follow Camera Y: false
   
   [1] Parallax Layer (Montañas/Capa media)
       Background Object: Fondo_Capa2
       Parallax Factor: 0.4
       Z Depth: -20
       Infinite Tiling X: ✓
       Follow Camera Y: false
   
   [2] Parallax Layer (Árboles/Capa cercana)
       Background Object: Fondo_Capa3
       Parallax Factor: 0.7
       Z Depth: -10
       Infinite Tiling X: ✓
       Follow Camera Y: false
   ```

4. **Configurar Resolución:**
   ```
   === Resolution Configuration ===
   Target Resolution:
   ├─ X: 1920
   ├─ Y: 1080
   
   Auto Scale To Screen: ✓
   ```

5. **Asignar Cámara:**
   ```
   === Camera Reference ===
   Main Camera: Main Camera (auto-detectado)
   ```

### Paso 4: Configurar Sorting Layers

**Unity → Edit → Project Settings → Tags and Layers**

```
Sorting Layers:
├─ Default
├─ Background (Order: -1000)
├─ Environment (Order: 0)
├─ Enemies (Order: 100)
├─ Player (Order: 200)
├─ UI (Order: 1000)
```

---

## ✅ Checklist de Verificación

### Enemigos:
- [ ] Layers "Ground" y "Player" creados
- [ ] Plataformas tienen Layer "Ground"
- [ ] Prefabs PatrolEnemy y ChaseEnemy creados
- [ ] EnemySpawner configurado con prefabs
- [ ] Ground Layer asignado en scripts de enemigos
- [ ] Probado en escena de prueba

### Parallax:
- [ ] Sprites de fondo importados (1920x1080 o 960x540)
- [ ] GameObjects de fondo creados
- [ ] BackgroundManager configurado
- [ ] Capas de parallax configuradas
- [ ] Resolución objetivo establecida
- [ ] Sorting Layers configurados
- [ ] Probado movimiento de cámara

---

## 🧪 Pruebas Rápidas

### Probar Enemigos:

1. **Play Mode**
2. **Mover jugador hacia enemigo**
3. **Verificar:**
   - ✓ Enemigo patrulla sin caer
   - ✓ Enemigo cambia dirección en bordes
   - ✓ Enemigo detecta y persigue jugador
   - ✓ Enemigo ataca en rango
   - ✓ Sprite voltea correctamente

### Probar Parallax:

1. **Play Mode**
2. **Mover cámara (o jugador)**
3. **Verificar:**
   - ✓ Fondos se mueven a diferentes velocidades
   - ✓ Capas más lejanas se mueven más lento
   - ✓ No hay huecos entre tiles
   - ✓ Fondos cubren toda la pantalla
   - ✓ Escala correcta en resolución objetivo

---

## 🐛 Soluciones Rápidas

### Enemigo cae de plataformas:
```
1. Verificar Layer "Ground" en plataformas
2. Asignar "Ground Layer" en enemigo
3. Aumentar "Edge Check Distance" a 0.8
```

### Enemigo no ataca:
```
1. Verificar que Attack Range > 0
2. Verificar que Attack Cooldown no sea muy alto
3. Verificar que Vision Range > Attack Range
```

### Parallax no funciona:
```
1. Verificar que BackgroundManager esté activo
2. Verificar que GameObjects estén asignados
3. Parallax Factor debe ser 0.1-0.9
4. Verificar que cámara se mueva
```

### Fondos no escalan:
```
1. Auto Scale To Screen: ✓
2. Sprites deben ser 1920x1080 o 960x540
3. Target Resolution correcta
```

---

## 📞 Comandos Útiles de Depuración

En el script del enemigo, el sistema muestra raycast debug:
- **Verde:** Camino libre
- **Rojo:** Obstáculo detectado

Para ver gizmos en Scene View:
```
Scene View → Gizmos → ✓ Activado
```

---

## 🎯 Valores Recomendados por Tipo de Juego

### Plataformas Clásico:
```
PatrolEnemy:
  Move Speed: 2.0
  Vision Range: 6.0
  Edge Check Distance: 0.6

ChaseEnemy:
  Move Speed: 2.5
  Chase Speed Multiplier: 1.5
  Vision Range: 8.0
```

### Metroidvania:
```
PatrolEnemy:
  Move Speed: 1.5
  Vision Range: 5.0
  Patrol Wait Time: 1.5

ChaseEnemy:
  Move Speed: 3.0
  Chase Speed Multiplier: 2.0
  Vision Range: 10.0
```

### Acción Rápida:
```
PatrolEnemy:
  Move Speed: 3.0
  Vision Range: 4.0
  Patrol Wait Time: 0.5

ChaseEnemy:
  Move Speed: 4.0
  Chase Speed Multiplier: 1.8
  Idle Wander Chance: 0.5
```

---

**¡Listo para comenzar!** 🚀

Sigue estos pasos en orden y tendrás el sistema funcionando en minutos.
