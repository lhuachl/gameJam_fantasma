# Mejoras al GeneradorNivel.cs

## 🎯 Problemas Resueltos

### 1. **Paredes Renderizándose en Negro Intermitentemente**

**Causa**: 
- El material compartido (`sharedMaterial`) se estaba compartiendo entre instancias, causando conflictos de estado
- El sorting order no era consistente
- No había reseteo de escala local antes de aplicar transformaciones

**Solución**:
- Crear **nueva instancia de Material** para cada sprite (no compartida)
- Usar `sr.material` en lugar de `sr.sharedMaterial`
- Resetear `localScale` a `Vector3.one` antes de aplicar escala
- Forzar `sortingOrder = 10` para paredes (visible, sobre fondo)

```csharp
sr.material = new Material(Shader.Find("Sprites/Default")); // Nueva instancia
sr.sortingLayerName = "Default";
sr.sortingOrder = 10;
```

---

### 2. **Fondos Rotos (No Escalan, Se Ven Negros)**

**Causa**:
- No se resetaba `localScale` antes de aplicar nueva escala (acumulación)
- SpriteRenderers múltiples no se procesaban consistentemente
- Sorting layer incorrecto causaba fondos invisibles

**Solución**:
- **Resetear scale**: `go.transform.localScale = Vector3.one` al instanciar
- Procesar **todos los SpriteRenderers** del fondo
- Establecer sorting consistente: `sortingOrder = -100` para fondos
- Mejor detección del tamaño del sprite con fallback

```csharp
// 1. Resetear escala
go.transform.localScale = Vector3.one;

// 2. Procesar fondos
foreach (var sr in srs)
{
    sr.sortingLayerName = "Default";
    sr.sortingOrder = -100;
    
    // Calcular y escalar correctamente
    var size = sr.bounds.size;
    if (size.x > 0.0001f && size.y > 0.0001f)
    {
        float sx = width / size.x;
        float sy = height / size.y;
        float s = Mathf.Max(sx, sy);
        go.transform.localScale = new Vector3(s, s, 1f);
    }
}
```

---

### 3. **Sistema de Enemigos Sin Categorización**

**Antes**: Solo había un tipo (`prefabEnemigo` para 'X')

**Después**: 4 tipos de enemigos categorizados

#### Nuevos Prefabs en Inspector:
- **`prefabEnemigoBasico`** (X) - Enemigo normal/estándar
- **`prefabEnemigoRapido`** (Y) - Enemigo rápido/ágil
- **`prefabEnemigoFuerte`** (Z) - Enemigo fuerte/tanque
- **`prefabEnemigoVolador`** (V) - Enemigo que vuela

#### Compatibilidad Retroactiva:
```csharp
case "X": 
    prefabACrear = prefabEnemigoBasico != null ? prefabEnemigoBasico : prefabEnemigo;
    break;
```
Si no asignas `prefabEnemigoBasico`, usará el antiguo `prefabEnemigo`.

---

## 📝 Cómo Usar en los Mapas CSV

### Símbolos Disponibles:

| Símbolo | Tipo | Prefab |
|---------|------|--------|
| `1` | Pared/Plataforma | `prefabPared` |
| `S` | Punto de inicio | `prefabInicio` |
| `E` | Puerta de salida | `prefabFinal` |
| `X` | Enemigo básico | `prefabEnemigoBasico` |
| `Y` | Enemigo rápido | `prefabEnemigoRapido` |
| `Z` | Enemigo fuerte | `prefabEnemigoFuerte` |
| `V` | Enemigo volador | `prefabEnemigoVolador` |
| `P` | Pincho/trampa | `prefabPincho` |
| `0` o vacío | Aire (sin generar) | - |

### Ejemplo de Mapa:
```csv
1,1,1,1,1,1,1,1,1,1
1,S,0,X,0,Y,0,0,0,1
1,0,0,P,0,Z,0,V,0,1
1,X,0,0,0,0,0,0,E,1
1,1,1,1,1,1,1,1,1,1
```

---

## 🔧 Cambios Técnicos Internos

### Eliminado:
- ❌ `cachedUnlitSpriteMat` (variable estática que causaba problemas de estado compartido)

### Mejorado:
- ✅ `EnsureUnlitSprite()` - Limpieza y asignación de material más robusta
- ✅ `EnsureUnlitForWalls()` - Mejor manejo de sorting layer
- ✅ `SpawnRandomBackgroundToFit()` - Reseteo de escala y procesamiento de todos los renderers
- ✅ `GenerarNivelDesdeAsset()` - Soporte para 4 tipos de enemigos + mejor logging

### Logging Mejorado:
```csharp
Debug.Log($"GeneradorNivel: Fondo escalado a ({s:F2}, {s:F2}) para cubrir nivel...");
Debug.LogWarning($"GeneradorNivel: Símbolo desconocido '{celda}' en posición ({x}, {y})");
```

---

## ⚠️ Checklist para Validar en el Inspector

- [ ] Asignar `prefabPared` 
- [ ] Asignar `prefabInicio`
- [ ] Asignar `prefabFinal`
- [ ] Asignar `prefabPincho`
- [ ] **Asignar `prefabEnemigoBasico`** (nuevo)
- [ ] **Asignar `prefabEnemigoRapido`** (nuevo, opcional)
- [ ] **Asignar `prefabEnemigoFuerte`** (nuevo, opcional)
- [ ] **Asignar `prefabEnemigoVolador`** (nuevo, opcional)
- [ ] Asignar fondos (o dejar que cargue desde Resources)
- [ ] El jugador está en la escena y tiene tag "Player"

---

## 🐛 Debugging

Si siguen viendo problemas:

1. **Fondos negros**: Revisar que el sprite tenga `Sprite Renderer` y no esté usando shaders "Lit"
2. **Paredes negras**: Hacer clic en la pared en el juego y revisar Console → debe decir "sortingOrder = 10"
3. **Enemigos no aparecen**: Asegurarse que los prefabs tienen sprites asignados
4. **Símbolos no reconocidos**: Ver Console para warnings y agregar el símbolo al switch

---

## 📊 Performance

- **Menos cálculos**: Se eliminó caché compartida que generaba corrutinas innecesarias
- **Material por instancia**: Cada sprite tiene su propio material (más memoria, pero más seguro)
- **Mejor escala**: Se calcula correctamente solo una vez al crear el nivel
