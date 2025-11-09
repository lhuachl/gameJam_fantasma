using UnityEngine;

/// <summary>
/// Interfaz para enemigos. Define comportamiento esperado
/// </summary>
public interface IEnemy : IDamageable
{
    /// <summary>
    /// Tipo de enemigo (X=básico, Y=rápido, Z=fuerte, V=volador)
    /// </summary>
    char GetEnemyType();
    
    /// <summary>
    /// Detecta si el jugador está en rango de visión
    /// </summary>
    bool CanSeePlayer();
    
    /// <summary>
    /// Ataca al jugador si es posible
    /// </summary>
    void AttackPlayer();
    
    /// <summary>
    /// Retorna true si el enemigo está atacando actualmente
    /// </summary>
    bool IsAttacking();
}
