using UnityEngine;

/// <summary>
/// Enemigo que patrulla en plataformas.
/// - Patrulla de izquierda a derecha en su plataforma
/// - Cambia de dirección al llegar a un borde o pared
/// - Persigue al jugador si lo ve
/// - Ataca cuando está en rango
/// </summary>
public class PatrolEnemy : BaseEnemy
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolWaitTime = 1f;
    [SerializeField] private bool startMovingRight = false;
    
    private float waitTimer = 0f;
    private bool isWaiting = false;
    
    protected override char enemyType => 'X'; // Tipo básico

    protected override void Start()
    {
        base.Start();
        
        // Establecer dirección inicial
        facingDirection = startMovingRight ? 1 : -1;
    }

    protected override void UpdateBehavior()
    {
        // Si está esperando, no moverse
        if (isWaiting)
        {
            waitTimer -= Time.fixedDeltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
            }
            
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            return;
        }
        
        // Verificar si ve al jugador
        if (CanSeePlayer())
        {
            ChaseAndAttackPlayer();
        }
        else
        {
            Patrol();
        }
    }

    /// <summary>
    /// Comportamiento de patrulla: moverse en una dirección hasta encontrar un obstáculo
    /// </summary>
    private void Patrol()
    {
        // Verificar si puede continuar en la dirección actual
        if (!CanMoveInDirection(facingDirection))
        {
            // Cambiar dirección y esperar un momento
            facingDirection = -facingDirection;
            isWaiting = true;
            waitTimer = patrolWaitTime;
            
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            return;
        }
        
        // Moverse en la dirección actual
        MoveInDirection(facingDirection);
    }

    /// <summary>
    /// Persigue al jugador y ataca si está en rango
    /// </summary>
    private void ChaseAndAttackPlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Si está en rango de ataque, atacar
        if (distanceToPlayer <= attackRange)
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            
            AttackPlayer();
            return;
        }
        
        // Si no está en rango, perseguir
        int directionToPlayer = player.position.x > transform.position.x ? 1 : -1;
        MoveInDirection(directionToPlayer);
    }
}
