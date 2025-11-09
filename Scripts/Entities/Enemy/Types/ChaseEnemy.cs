using UnityEngine;

/// <summary>
/// Enemigo que persigue activamente al jugador.
/// - Más agresivo que el PatrolEnemy
/// - Persigue al jugador en cuanto lo ve
/// - Puede moverse más rápido
/// - No patrulla cuando no ve al jugador, se queda quieto o deambula
/// </summary>
public class ChaseEnemy : BaseEnemy
{
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeedMultiplier = 1.5f;
    [SerializeField] private float idleWanderChance = 0.3f;
    [SerializeField] private float wanderChangeInterval = 2f;
    
    private float wanderTimer = 0f;
    private bool isWandering = false;
    
    protected override char enemyType => 'Y'; // Tipo rápido

    protected override void Start()
    {
        base.Start();
        
        // Dirección aleatoria inicial
        facingDirection = Random.value > 0.5f ? 1 : -1;
    }

    protected override void UpdateBehavior()
    {
        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            IdleBehavior();
        }
    }

    /// <summary>
    /// Persigue al jugador con velocidad aumentada
    /// </summary>
    private void ChasePlayer()
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
        
        // Perseguir con velocidad aumentada
        int directionToPlayer = player.position.x > transform.position.x ? 1 : -1;
        
        if (CanMoveInDirection(directionToPlayer))
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(directionToPlayer * moveSpeed * chaseSpeedMultiplier, rb.linearVelocity.y);
            }
            
            facingDirection = directionToPlayer;
            UpdateSpriteDirection();
        }
        else
        {
            // Si hay obstáculo, intentar saltar (opcional)
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }

    /// <summary>
    /// Comportamiento cuando está inactivo: deambular ocasionalmente o quedarse quieto
    /// </summary>
    private void IdleBehavior()
    {
        wanderTimer += Time.fixedDeltaTime;
        
        if (wanderTimer >= wanderChangeInterval)
        {
            wanderTimer = 0f;
            
            // Decidir si deambular
            if (Random.value < idleWanderChance)
            {
                isWandering = true;
                facingDirection = Random.value > 0.5f ? 1 : -1;
            }
            else
            {
                isWandering = false;
            }
        }
        
        if (isWandering)
        {
            // Deambular lentamente
            if (CanMoveInDirection(facingDirection))
            {
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(facingDirection * moveSpeed * 0.5f, rb.linearVelocity.y);
                }
                UpdateSpriteDirection();
            }
            else
            {
                // Cambiar dirección si hay obstáculo
                facingDirection = -facingDirection;
            }
        }
        else
        {
            // Quedarse quieto
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
    }
}
