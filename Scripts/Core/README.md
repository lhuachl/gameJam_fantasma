# Core - Sistemas Fundamentales

Esta carpeta contiene los sistemas centrales del juego que gestionan el estado global y la arquitectura base.

## 📦 Contenido

### GameManager.cs
**Singleton de Estado Global** (265 líneas)

El cerebro del juego. Gestiona todo el estado global y persiste entre escenas.

#### Responsabilidades
- ✅ Gestión de guardado/carga en JSON
- ✅ Progresión entre niveles
- ✅ Sistema de decisiones BUENO/MALO
- ✅ Gestión de upgrades permanentes
- ✅ Tracking de bosses derrotados
- ✅ Stats del jugador (salud, daño, habilidades)

#### Características
- **Singleton Pattern**: Único en toda la aplicación
- **DontDestroyOnLoad**: Persiste entre escenas
- **JSON Serialization**: Guarda en `Application.persistentDataPath`
- **Debug Mode**: Logs detallados en Console
- **Event Broadcasting**: Integrado con EventManager

#### API Pública

```csharp
// Acceso al singleton
GameManager.Instance

// Gestión de juego
void CreateNewGame()                    // Crea nuevo juego con defaults
void SaveGameState()                    // Guarda progreso en JSON
void LoadGameState()                    // Carga progreso desde JSON
bool HasSaveFile()                      // Verifica si existe save

// Progresión
void ProgressToNextLevel()              // Avanza al siguiente nivel
void MakeLevelDecision(string decision) // Registra decisión BUENO/MALO
void DefeatBoss(string bossId)          // Marca boss como derrotado
bool IsBossDefeated(string bossId)      // Verifica si boss fue derrotado

// Upgrades
void AddHealthUpgrade(int amount)       // Agrega upgrade de salud
void AddWeaponUpgrade(int amount)       // Agrega upgrade de daño
void AddSpecialAbility(string ability)  // Agrega habilidad especial
bool HasAbility(string ability)         // Verifica si tiene habilidad

// Getters
int GetCurrentLevel()                   // Nivel actual
int GetMaxHealth()                      // Salud máxima
int GetWeaponDamage()                   // Daño del arma
GameSaveData GetCurrentSave()           // SaveData completo (read-only)
```

#### Ejemplo de Uso

```csharp
// Crear nuevo juego
GameManager.Instance.CreateNewGame();

// Obtener nivel actual
int currentLevel = GameManager.Instance.GetCurrentLevel();

// Derrotar boss
GameManager.Instance.DefeatBoss("boss1");

// Hacer decisión
GameManager.Instance.MakeLevelDecision("good"); // o "bad"

// Agregar upgrade
GameManager.Instance.AddHealthUpgrade(20);

// Progresar al siguiente nivel
GameManager.Instance.ProgressToNextLevel();

// Guardar progreso (automático en varios momentos)
GameManager.Instance.SaveGameState();
```

#### Eventos que Broadcast

- `GameSavedEvent` - Cuando se guarda el juego
  - `saveLocation`: string - Ubicación del save

#### Flujo Típico

```
1. Awake()
   └─ Singleton setup
   └─ DontDestroyOnLoad
   └─ Intentar cargar save existente
   └─ Si no existe, CreateNewGame()

2. Durante el juego
   └─ Sistemas llaman a métodos públicos
   └─ GameManager actualiza currentSave
   └─ SaveGameState() periódicamente

3. Entre niveles
   └─ ProgressToNextLevel()
   └─ SaveGameState()
   └─ SceneManager.LoadScene(nextLevel)

4. Al cerrar
   └─ SaveGameState() automático
```

## 🎯 Principios de Diseño

1. **Única Fuente de Verdad**: Todo el estado del juego vive aquí
2. **No Duplicación**: SaveData nunca está en múltiples lugares
3. **Persistencia Automática**: DontDestroyOnLoad garantiza persistencia
4. **API Clara**: Métodos públicos descriptivos y fáciles de usar
5. **Debugging**: Modo debug con logs detallados

## ⚠️ Reglas Importantes

### ✅ HACER
- Usar `GameManager.Instance` para acceder al estado global
- Llamar a `SaveGameState()` después de cambios importantes
- Usar `GetCurrentSave()` para leer datos (no modificar directamente)
- Activar `debugMode = true` cuando estés desarrollando

### ❌ NO HACER
- No crear múltiples instancias de GameManager
- No guardar estado del juego en otros scripts
- No modificar `currentSave` directamente desde fuera
- No depender de `FindWithTag("GameManager")`

## 🔄 Relación con Otros Sistemas

```
GameManager
    ├─ EventManager (broadcasts eventos)
    ├─ GameSaveData (contiene el estado)
    ├─ PlayerController (consulta stats)
    ├─ LevelManager (consulta nivel actual)
    ├─ MenuManager (crea/carga juegos)
    ├─ Boss scripts (registran derrotas)
    └─ Finish script (progresa niveles)
```

## 🐛 Debugging

### Ver logs en Console
```csharp
[GameManager] Inicializado | Ruta: C:/Users/.../gamesave.json
[GameManager] Nuevo juego creado
[GameManager] Juego guardado en: C:/Users/.../gamesave.json
[GameManager] Boss derrotado: boss1 en nivel 3
[GameManager] Progresó a nivel: 4
```

### Verificar SaveData
La ruta del archivo JSON se muestra en el primer log. Puedes abrir ese archivo para ver el contenido:

```json
{
  "currentLevel": 3,
  "completedLevels": [1, 2],
  "pendingBoss": true,
  "decisionsPath": ["good", "bad"],
  "defeatedBosses": {
    "boss1": true
  },
  ...
}
```

## 📚 Referencias

- Ver `Data/GameSaveData.cs` para estructura completa del save
- Ver `Utilities/Events/GameEvents.cs` para eventos disponibles
- Ver documentación en `ARQUITECTURA_BRUTAL.txt` para detalles técnicos

---

**Última actualización**: 2025-11-09
