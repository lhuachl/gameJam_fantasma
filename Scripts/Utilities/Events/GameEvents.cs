using UnityEngine;

/// <summary>
/// Clase base para todos los eventos del juego
/// </summary>
public abstract class GameEvent
{
    public float timestamp = Time.time;
}

// === EVENTOS DE PROGRESIÓN ===

public class BossDefeatedEvent : GameEvent
{
    public string bossId;
    public int levelNumber;
}

public class LevelCompleteEvent : GameEvent
{
    public int levelNumber;
}

public class DecisionMadeEvent : GameEvent
{
    public string chosenPath; // "good" o "bad"
    public int levelNumber;
}

public class GameSavedEvent : GameEvent
{
    public string saveLocation;
}

// === EVENTOS DEL JUGADOR ===

public class PlayerTakeDamageEvent : GameEvent
{
    public int damageAmount;
    public int remainingHealth;
    public Vector2 damageSourcePosition;
}

public class PlayerDiedEvent : GameEvent
{
    public int lastLevelAttempted;
    public Vector2 deathPosition;
}

public class PlayerTookUpgradeEvent : GameEvent
{
    public string upgradeId;
    public UpgradeConfig upgradeConfig;
}

// === EVENTOS DE JUGADOR SECUNDARIOS ===

public class PlayerJumpedEvent : GameEvent
{
    public Vector3 jumpPosition;
}

public class PlayerDashedEvent : GameEvent
{
    public Vector3 dashStartPosition;
    public Vector3 dashEndPosition;
    public bool isIntangible = true;
}

public class PlayerAttackedEvent : GameEvent
{
    public Vector3 attackPosition;
    public int damageDealt;
}

// === EVENTOS DE ENEMIGOS ===

public class EnemyDefeatedEvent : GameEvent
{
    public char enemyType;
    public Vector3 deathPosition;
}

public class EnemySpawnedEvent : GameEvent
{
    public char enemyType;
    public Vector3 spawnPosition;
}

// === EVENTOS DE NIVEL ===

public class LevelLoadedEvent : GameEvent
{
    public int levelNumber;
    public string levelName;
}

public class LevelUnloadingEvent : GameEvent
{
    public int currentLevel;
}

// === EVENTOS DE AUDIO ===

public class MusicChangeRequestedEvent : GameEvent
{
    public string musicClipName;
    public float fadeDuration = 0.5f;
}

public class SFXPlayRequestedEvent : GameEvent
{
    public string sfxClipName;
    public Vector3 playPosition;
}
