using UnityEngine;

/// <summary>
/// Interfaz para cualquier entidad que pueda recibir daño
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Recibe daño y retorna si sobrevivió
    /// </summary>
    bool TakeDamage(int damage, Vector2 knockbackDirection = default, float knockbackForce = 0f);
    
    /// <summary>
    /// Retorna la salud actual
    /// </summary>
    int GetCurrentHealth();
    
    /// <summary>
    /// Retorna la salud máxima
    /// </summary>
    int GetMaxHealth();
    
    /// <summary>
    /// Retorna true si está vivo
    /// </summary>
    bool IsAlive();
    
    /// <summary>
    /// Retorna posición para referencias externas
    /// </summary>
    Vector3 GetPosition();
}
