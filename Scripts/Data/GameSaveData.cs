using System.Collections.Generic;

/// <summary>
/// Contiene todo el estado del juego que necesita persistencia
/// </summary>
[System.Serializable]
public class GameSaveData
{
    // === PROGRESIÓN ===
    public int currentLevel = 1;
    public List<int> completedLevels = new List<int>();
    public bool pendingBoss = false;
    
    // === DECISIONES (para finales branching) ===
    public List<string> decisionsPath = new List<string>(); // ["good", "bad", "good"]
    public int goodEndings = 0;
    public int badEndings = 0;
    
    // === BOSSES ===
    public Dictionary<string, bool> defeatedBosses = new Dictionary<string, bool>()
    {
        { "boss1", false },
        { "boss2", false },
        { "boss3", false },
        { "finalBoss", false }
    };
    
    // === UPGRADES (PERMANENTES) ===
    public int maxHealthUpgrades = 0;
    public int weaponDamageUpgrades = 0;
    public List<string> specialAbilities = new List<string>();
    
    // === STATS DEL JUGADOR ===
    public int currentHealth = 3;
    public int maxHealth = 3;
    public int weaponDamage = 1;
    
    // === MISCELÁNEA ===
    public float playTime = 0f;
    public string lastPlayedDate = "";
    
    public GameSaveData() { }
    
    /// <summary>
    /// Reset completo para nuevo juego
    /// </summary>
    public void ResetToDefaults()
    {
        currentLevel = 1;
        completedLevels.Clear();
        pendingBoss = false;
        decisionsPath.Clear();
        goodEndings = 0;
        badEndings = 0;
        
        foreach (var key in defeatedBosses.Keys)
            defeatedBosses[key] = false;
        
        maxHealthUpgrades = 0;
        weaponDamageUpgrades = 0;
        specialAbilities.Clear();
        
        currentHealth = 3;
        maxHealth = 3;
        weaponDamage = 1;
        playTime = 0f;
        lastPlayedDate = "";
    }
}

/// <summary>
/// Configuración de upgrades disponibles en el juego
/// </summary>
[System.Serializable]
public class UpgradeConfig
{
    public string upgradeId;
    public string upgradeDisplayName;
    public int healthBonus;
    public int damageBonus;
    public string specialAbility;
    
    public UpgradeConfig(string id, string displayName, int health = 0, int damage = 0, string ability = "")
    {
        upgradeId = id;
        upgradeDisplayName = displayName;
        healthBonus = health;
        damageBonus = damage;
        specialAbility = ability;
    }
}
