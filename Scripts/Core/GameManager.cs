using System.IO;
using UnityEngine;

/// <summary>
/// Singleton que maneja TODO el estado del juego
/// - Carga/guarda datos
/// - Gestiona upgrades
/// - Controla progresión
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    [SerializeField] private bool debugMode = true;
    
    private GameSaveData currentSave;
    private string savePath;

    private void Awake()
    {
        // Singleton protection
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Definir ruta de guardado
        savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        
        if (debugMode)
            Debug.Log($"[GameManager] Inicializado | Ruta: {savePath}");

        // Intentar cargar juego previo, si no existe crear nuevo
        if (File.Exists(savePath))
            LoadGameState();
        else
            CreateNewGame();
    }

    // ═══════════════════════════════════════════════════════════
    // GESTIÓN DE GUARDADO
    // ═══════════════════════════════════════════════════════════

    public void CreateNewGame()
    {
        currentSave = new GameSaveData();
        currentSave.ResetToDefaults();
        currentSave.lastPlayedDate = System.DateTime.Now.ToString();
        
        SaveGameState();
        
        if (debugMode)
            Debug.Log("[GameManager] Nuevo juego creado");
        
        EventManager.Broadcast(new GameSavedEvent { saveLocation = "NewGame" });
    }

    public void SaveGameState()
    {
        if (currentSave == null)
        {
            Debug.LogError("[GameManager] Intento de guardar sin save data");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(currentSave, true);
            File.WriteAllText(savePath, json);
            
            if (debugMode)
                Debug.Log($"[GameManager] Guardado exitoso en: {savePath}");
            
            EventManager.Broadcast(new GameSavedEvent { saveLocation = savePath });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameManager] Error al guardar: {ex.Message}");
        }
    }

    public void LoadGameState()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("[GameManager] Archivo de guardado no encontrado");
            CreateNewGame();
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            currentSave = JsonUtility.FromJson<GameSaveData>(json);
            
            if (debugMode)
                Debug.Log($"[GameManager] Juego cargado desde: {savePath}");
            
            EventManager.Broadcast(new GameSavedEvent { saveLocation = "Loaded" });
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameManager] Error al cargar: {ex.Message}");
            CreateNewGame();
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(savePath);
    }

    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            if (debugMode)
                Debug.Log("[GameManager] Archivo de guardado eliminado");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // GESTIÓN DE PROGRESIÓN
    // ═══════════════════════════════════════════════════════════

    public int GetCurrentLevel()
    {
        return currentSave.currentLevel;
    }

    public void ProgressToNextLevel()
    {
        currentSave.currentLevel++;
        
        if (!currentSave.completedLevels.Contains(currentSave.currentLevel - 1))
            currentSave.completedLevels.Add(currentSave.currentLevel - 1);
        
        SaveGameState();
        
        if (debugMode)
            Debug.Log($"[GameManager] Progresó a nivel: {currentSave.currentLevel}");
    }

    public void MakeLevelDecision(string decision)
    {
        currentSave.decisionsPath.Add(decision);
        
        if (decision == "good")
            currentSave.goodEndings++;
        else if (decision == "bad")
            currentSave.badEndings++;
        
        SaveGameState();
        
        EventManager.Broadcast(new DecisionMadeEvent 
        { 
            chosenPath = decision, 
            levelNumber = currentSave.currentLevel 
        });
        
        if (debugMode)
            Debug.Log($"[GameManager] Decisión: {decision} en nivel {currentSave.currentLevel}");
    }

    // ═══════════════════════════════════════════════════════════
    // GESTIÓN DE BOSSES
    // ═══════════════════════════════════════════════════════════

    public void DefeatBoss(string bossId)
    {
        if (currentSave.defeatedBosses.ContainsKey(bossId))
        {
            currentSave.defeatedBosses[bossId] = true;
            SaveGameState();
            
            EventManager.Broadcast(new BossDefeatedEvent 
            { 
                bossId = bossId, 
                levelNumber = currentSave.currentLevel 
            });
            
            if (debugMode)
                Debug.Log($"[GameManager] Boss derrotado: {bossId}");
        }
    }

    public bool IsBossDefeated(string bossId)
    {
        return currentSave.defeatedBosses.ContainsKey(bossId) && currentSave.defeatedBosses[bossId];
    }

    public void SetPendingBoss(bool pending)
    {
        currentSave.pendingBoss = pending;
        SaveGameState();
    }

    public bool HasPendingBoss()
    {
        return currentSave.pendingBoss;
    }

    // ═══════════════════════════════════════════════════════════
    // GESTIÓN DE UPGRADES
    // ═══════════════════════════════════════════════════════════

    public void AddHealthUpgrade()
    {
        currentSave.maxHealthUpgrades++;
        currentSave.maxHealth = 3 + (currentSave.maxHealthUpgrades * 2);
        SaveGameState();
        
        if (debugMode)
            Debug.Log($"[GameManager] Upgrade de salud aplicado. Max Health: {currentSave.maxHealth}");
    }

    public void AddWeaponUpgrade()
    {
        currentSave.weaponDamageUpgrades++;
        currentSave.weaponDamage = 1 + currentSave.weaponDamageUpgrades;
        SaveGameState();
        
        if (debugMode)
            Debug.Log($"[GameManager] Upgrade de daño aplicado. Daño: {currentSave.weaponDamage}");
    }

    public void AddSpecialAbility(string abilityId)
    {
        if (!currentSave.specialAbilities.Contains(abilityId))
        {
            currentSave.specialAbilities.Add(abilityId);
            SaveGameState();
            
            if (debugMode)
                Debug.Log($"[GameManager] Habilidad especial agregada: {abilityId}");
        }
    }

    // ═══════════════════════════════════════════════════════════
    // GETTERS PARA STATS
    // ═══════════════════════════════════════════════════════════

    public int GetMaxHealth() => currentSave.maxHealth;
    public int GetCurrentHealth() => currentSave.currentHealth;
    public int GetWeaponDamage() => currentSave.weaponDamage;
    public GameSaveData GetCurrentSave() => currentSave;

    public void SetCurrentHealth(int health)
    {
        currentSave.currentHealth = Mathf.Clamp(health, 0, currentSave.maxHealth);
        SaveGameState();
    }

    // ═══════════════════════════════════════════════════════════
    // DEBUG
    // ═══════════════════════════════════════════════════════════

    public void PrintGameState()
    {
        Debug.Log("=== GAME STATE ===");
        Debug.Log($"Nivel: {currentSave.currentLevel}");
        Debug.Log($"Salud: {currentSave.currentHealth}/{currentSave.maxHealth}");
        Debug.Log($"Daño: {currentSave.weaponDamage}");
        Debug.Log($"Upgrades Salud: {currentSave.maxHealthUpgrades}");
        Debug.Log($"Upgrades Daño: {currentSave.weaponDamageUpgrades}");
        Debug.Log($"Bosses Derrotados: {string.Join(", ", currentSave.defeatedBosses.Keys)}");
    }
}
