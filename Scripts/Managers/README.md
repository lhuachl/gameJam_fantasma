# Managers - Gestores de Sistemas

Esta carpeta contiene los managers que gestionan sistemas específicos del juego. Cada manager es responsable de un aspecto particular de la funcionalidad del juego.

## 📂 Contenido Actual

### CameraManager.cs ✅
**Control de cámara suave** (35 líneas)

Manager simple que hace que la cámara siga al jugador con movimiento suave.

#### Características
- ✅ Seguimiento suave (smoothing)
- ✅ Offset configurable
- ✅ Lock opcional en ejes X/Y
- ✅ Auto-detección de jugador si no asignado

#### Configuración

```csharp
[SerializeField] Referencias:
- target: Transform              // Transform del jugador (auto-detecta si null)

[SerializeField] Parámetros:
- offset: Vector3 = (0, 1, -10) // Offset de la cámara
- smoothSpeed: float = 0.1f      // Velocidad de smoothing (menor = más suave)
- lockX: bool = false            // Bloquear movimiento en X
- lockY: bool = false            // Bloquear movimiento en Y
```

#### Ejemplo de Uso

1. Adjunta `CameraManager.cs` a tu GameObject Camera
2. Asigna el Player en el campo `target` (o déjalo null para auto-detección)
3. Ajusta `offset` según necesites (Z negativo para que se vea)
4. Ajusta `smoothSpeed` (0.05 = muy suave, 0.5 = rápido)

```csharp
// No necesitas llamar métodos, funciona automáticamente
// Pero puedes cambiar propiedades en runtime:
cameraManager.offset = new Vector3(2, 3, -10);
cameraManager.smoothSpeed = 0.2f;
```

#### Cómo Funciona

```csharp
void LateUpdate()
{
    if (target == null) return;
    
    // Calcular posición deseada
    Vector3 desiredPosition = target.position + offset;
    
    // Aplicar locks si están activos
    if (lockX) desiredPosition.x = transform.position.x;
    if (lockY) desiredPosition.y = transform.position.y;
    
    // Interpolar suavemente
    Vector3 smoothedPosition = Vector3.Lerp(
        transform.position, 
        desiredPosition, 
        smoothSpeed
    );
    
    transform.position = smoothedPosition;
}
```

## 📝 Managers Pendientes de Implementación

### LevelManager.cs (Pendiente)
**Generación y gestión de niveles**

Refactorización de `Legacy/GeneradorNivel.cs` con nueva arquitectura.

#### Responsabilidades Planeadas
- Cargar nivel desde CSV
- Instanciar prefabs (paredes, enemigos, boss, jugador)
- Posicionar elementos según CSV
- Suscribirse a eventos (BossDefeated, EnemyDefeated)
- Desbloquear puerta final cuando se cumplan condiciones
- Broadcast LevelLoadedEvent al cargar

#### API Planeada

```csharp
public class LevelManager : MonoBehaviour
{
    // Referencias
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject[] enemyPrefabs; // X, Y, Z, V
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject finishPrefab;
    
    // Métodos públicos
    public void LoadLevel(int levelNumber)
    {
        // Cargar CSV: Resources/Levels/Nivel_{levelNumber}.csv
        // Parsear y generar nivel
        // Broadcast LevelLoadedEvent
    }
    
    public void UnloadLevel()
    {
        // Limpiar nivel actual
        // Broadcast LevelUnloadingEvent
    }
    
    // Event handlers
    private void OnBossDefeated(BossDefeatedEvent e)
    {
        // Desbloquear puerta final
        UnlockFinalDoor();
    }
    
    private void OnEnemyDefeated(EnemyDefeatedEvent e)
    {
        // Actualizar contador de enemigos
        // Si todos muertos, hacer algo (opcional)
    }
}
```

#### Formato CSV Esperado

```csv
# Formato: TipoElemento,PosX,PosY
W,0,0      # Wall
W,1,0
X,5,2      # Enemigo tipo X
Y,10,3     # Enemigo tipo Y
B,15,0     # Boss
P,0,1      # Jugador (spawn)
F,20,0     # Finish (meta)
```

---

### AudioManager.cs (Pendiente)
**Gestión de música y efectos de sonido**

Manager para audio con soporte de música por zona y boss.

#### Responsabilidades Planeadas
- Reproducir música de fondo por nivel/zona
- Cambiar música cuando aparece boss
- Crossfade entre tracks
- Reproducir efectos de sonido
- Control de volumen global

#### API Planeada

```csharp
public class AudioManager : MonoBehaviour
{
    // Singleton
    public static AudioManager Instance { get; private set; }
    
    // Audio Sources
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    // Audio Clips
    [SerializeField] private AudioClip[] levelMusic;
    [SerializeField] private AudioClip[] bossMusic;
    [SerializeField] private AudioClip[] sfxClips;
    
    // Métodos públicos
    public void PlayLevelMusic(int levelNumber, float fadeDuration = 1f)
    {
        // Crossfade a música de nivel
    }
    
    public void PlayBossMusic(string bossId, float fadeDuration = 1f)
    {
        // Crossfade a música de boss
    }
    
    public void PlaySFX(string sfxName, Vector2? position = null)
    {
        // Reproducir efecto de sonido
        // Si position != null, usar AudioSource.PlayClipAtPoint
    }
    
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    
    // Event handlers
    private void OnMusicChangeRequested(MusicChangeRequestedEvent e)
    {
        // Cambiar música según evento
    }
    
    private void OnSFXPlayRequested(SFXPlayRequestedEvent e)
    {
        // Reproducir SFX según evento
    }
}
```

#### Eventos que Escucha

- `MusicChangeRequestedEvent` - Cambio de música solicitado
- `SFXPlayRequestedEvent` - Efecto de sonido solicitado
- `BossDefeatedEvent` - Volver a música de nivel
- `LevelLoadedEvent` - Iniciar música del nivel

#### Ejemplo de Uso

```csharp
// Desde cualquier script:
AudioManager.Instance.PlaySFX("player_jump");
AudioManager.Instance.PlayLevelMusic(3);

// O usando eventos:
EventManager.Broadcast(new SFXPlayRequestedEvent 
{ 
    sfxClipName = "enemy_death",
    playPosition = enemyPosition 
});
```

---

### HUDManager.cs (Pendiente)
**Gestión de interfaz de usuario en juego**

Manager para actualizar HUD dinámicamente.

#### Responsabilidades Planeadas
- Mostrar health bar del jugador
- Mostrar upgrades recolectados
- Mostrar contador de enemigos
- Boss health bar (cuando está en combate)
- Score/puntos

#### API Planeada

```csharp
public class HUDManager : MonoBehaviour
{
    // Referencias UI
    [SerializeField] private Slider playerHealthBar;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private Text enemyCountText;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject upgradeIconPrefab;
    [SerializeField] private Transform upgradeContainer;
    
    // Métodos públicos
    public void UpdatePlayerHealth(int current, int max)
    {
        playerHealthBar.value = (float)current / max;
    }
    
    public void UpdateBossHealth(int current, int max)
    {
        bossHealthBar.gameObject.SetActive(true);
        bossHealthBar.value = (float)current / max;
    }
    
    public void HideBossHealth()
    {
        bossHealthBar.gameObject.SetActive(false);
    }
    
    public void ShowUpgrade(string upgradeId, Sprite icon)
    {
        // Instanciar icono en upgradeContainer
    }
    
    public void UpdateEnemyCount(int remaining)
    {
        enemyCountText.text = $"Enemigos: {remaining}";
    }
    
    // Event handlers
    private void OnPlayerTakeDamage(PlayerTakeDamageEvent e)
    {
        UpdatePlayerHealth(e.remainingHealth, maxHealth);
    }
    
    private void OnPlayerTookUpgrade(PlayerTookUpgradeEvent e)
    {
        ShowUpgrade(e.upgradeId, e.upgradeConfig.icon);
    }
    
    private void OnEnemyDefeated(EnemyDefeatedEvent e)
    {
        // Actualizar contador
    }
}
```

#### Eventos que Escucha

- `PlayerTakeDamageEvent` - Actualizar health bar
- `PlayerTookUpgradeEvent` - Mostrar upgrade
- `EnemyDefeatedEvent` - Actualizar contador
- `BossDefeatedEvent` - Ocultar boss health bar
- `LevelLoadedEvent` - Reset UI

---

### PauseManager.cs (Opcional - Pendiente)
**Gestión del menú de pausa**

Manager para pausar el juego.

#### Responsabilidades
- Pausar/despausar juego (Time.timeScale)
- Mostrar menú de pausa
- Botones: Continuar, Reiniciar, Salir

#### API Planeada

```csharp
public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    private bool isPaused = false;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}
```

---

### MenuManager.cs (Pendiente)
**Gestión del menú principal**

Consolidación de `Legacy/NewGame.cs` y `Legacy/Continue.cs`.

#### Responsabilidades
- Botón "Nuevo Juego"
- Botón "Continuar"
- Botón "Opciones" (opcional)
- Botón "Salir"

#### API Planeada

```csharp
public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    
    void Start()
    {
        // Deshabilitar continuar si no hay save
        continueButton.interactable = GameManager.Instance.HasSaveFile();
    }
    
    public void OnNewGameClicked()
    {
        GameManager.Instance.CreateNewGame();
        SceneManager.LoadScene("Level_1");
    }
    
    public void OnContinueClicked()
    {
        if (GameManager.Instance.HasSaveFile())
        {
            GameManager.Instance.LoadGameState();
            int level = GameManager.Instance.GetCurrentLevel();
            SceneManager.LoadScene($"Level_{level}");
        }
    }
    
    public void OnOptionsClicked()
    {
        // Abrir menú de opciones
    }
    
    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
```

## 🎯 Principios de Diseño

### Para Todos los Managers

1. **Singleton cuando sea apropiado**: AudioManager, HUDManager
2. **Suscribirse a eventos**: Usar EventManager en lugar de referencias directas
3. **Una responsabilidad**: Cada manager se enfoca en un aspecto
4. **API pública clara**: Métodos descriptivos y fáciles de usar
5. **Configuración en Inspector**: SerializeField para flexibilidad

### Ciclo de Vida Típico

```csharp
public class ExampleManager : MonoBehaviour
{
    void Start()
    {
        // Suscribirse a eventos
        EventManager.Subscribe<SomeEvent>(OnSomeEvent);
        
        // Inicialización
        Initialize();
    }
    
    void OnDestroy()
    {
        // IMPORTANTE: Desuscribirse
        EventManager.Unsubscribe<SomeEvent>(OnSomeEvent);
    }
    
    private void OnSomeEvent(SomeEvent e)
    {
        // Manejar evento
    }
}
```

## 📚 Referencias

- Ver `Core/GameManager.cs` para gestión de estado global
- Ver `Utilities/Events/` para sistema de eventos
- Ver `Legacy/` para scripts antiguos de referencia
- Ver `COMIENZA_AQUI.txt` para guías de refactorización

---

**Última actualización**: 2025-11-09
