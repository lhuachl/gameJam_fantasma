using UnityEngine;

/// <summary>
/// Interfaz para bosses. Define su comportamiento único
/// </summary>
public interface IBoss : IDamageable
{
    /// <summary>
    /// ID único del boss (ej: "boss1", "finalBoss")
    /// </summary>
    string GetBossId();
    
    /// <summary>
    /// Entra en modo activo (combate comienza)
    /// </summary>
    void EnterBattle();
    
    /// <summary>
    /// Sale del combate
    /// </summary>
    void ExitBattle();
    
    /// <summary>
    /// Retorna fase actual (0-3 típicamente)
    /// </summary>
    int GetCurrentPhase();
}
