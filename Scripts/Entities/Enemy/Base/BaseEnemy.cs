using UnityEngine;
using System.Collections;

/// <summary>
/// Clase base abstracta para todos los enemigos del juego.
/// Implementa funcionalidad común: salud, daño, detección de jugador, movimiento básico.
/// </summary>
public abstract class BaseEnemy : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 3;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int attackDamage = 1;
    [SerializeField] protected float moveSpeed = 2f;
    
    [Header("Vision & Attack")]
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float attackRange = 1.2f;
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected LayerMask playerLayer;
    
    [Header("Platform Detection")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected float edgeCheckDistance = 0.6f;
    [SerializeField] protected float wallCheckDistance = 0.4f;
    [SerializeField] protected Vector2 edgeCheckOffset = new Vector2(0.4f, 0.05f);
    [SerializeField] protected Vector2 wallCheckOffset = new Vector2(0.4f, 0.1f);
    
    // Referencias
    protected Rigidbody2D rb;
    protected Transform player;
    protected SpriteRenderer spriteRenderer;
    
    // Estado
    protected bool isAttacking = false;
    protected bool canAttack = true;
    protected int facingDirection = -1; // -1 = izquierda, 1 = derecha
    protected bool isAlive = true;
    
    // Tipo de enemigo (X, Y, Z, V)
    protected abstract char enemyType { get; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1f;
        }
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        FindPlayer();
    }

    protected virtual void FixedUpdate()
    {
        if (!isAlive || isAttacking) return;
        
        UpdateBehavior();
    }

    /// <summary>
    /// Actualiza el comportamiento del enemigo cada frame
    /// </summary>
    protected abstract void UpdateBehavior();

    /// <summary>
    /// Encuentra al jugador en la escena
    /// </summary>
    protected virtual void FindPlayer()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Player");
        }
        
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    #region IEnemy Implementation
    
    public char GetEnemyType()
    {
        return enemyType;
    }

    public virtual bool CanSeePlayer()
    {
        if (player == null || !isAlive) return false;
        
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > visionRange) return false;
        
        // Verificar si el jugador está en la dirección que mira el enemigo
        float dx = player.position.x - transform.position.x;
        int playerDirection = dx > 0 ? 1 : -1;
        
        // Solo ver al jugador si está en la dirección que miramos
        if (playerDirection != facingDirection && facingDirection != 0) return false;
        
        return true;
    }

    public virtual void AttackPlayer()
    {
        if (!canAttack || !isAlive) return;
        
        StartCoroutine(AttackCoroutine());
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    protected virtual IEnumerator AttackCoroutine()
    {
        canAttack = false;
        isAttacking = true;
        
        // Detener movimiento durante ataque
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        
        // Animación de preparación
        yield return new WaitForSeconds(0.2f);
        
        // Ejecutar ataque
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange + 0.1f)
            {
                var damageable = player.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockbackDir = (player.position - transform.position).normalized;
                    damageable.TakeDamage(attackDamage, knockbackDir, 5f);
                }
            }
        }
        
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    #endregion

    #region IDamageable Implementation
    
    public virtual bool TakeDamage(int damage, Vector2 knockbackDirection = default, float knockbackForce = 0f)
    {
        if (!isAlive) return false;
        
        currentHealth -= damage;
        
        // Aplicar knockback
        if (knockbackForce > 0 && rb != null)
        {
            rb.linearVelocity = knockbackDirection * knockbackForce;
        }
        
        if (currentHealth <= 0)
        {
            Die();
            return false;
        }
        
        return true;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    #endregion

    #region Movement & Platform Detection
    
    /// <summary>
    /// Mueve al enemigo en una dirección verificando obstáculos
    /// </summary>
    protected virtual void MoveInDirection(int direction)
    {
        if (!CanMoveInDirection(direction))
        {
            // Si no puede moverse, cambiar dirección
            facingDirection = -direction;
            return;
        }
        
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }
        
        facingDirection = direction;
        UpdateSpriteDirection();
    }

    /// <summary>
    /// Verifica si puede moverse en una dirección (detecta bordes y paredes)
    /// </summary>
    protected virtual bool CanMoveInDirection(int direction)
    {
        // Verificar si hay suelo adelante (para no caer)
        if (!HasGroundAhead(direction))
        {
            return false;
        }
        
        // Verificar si hay pared adelante
        if (HasWallAhead(direction))
        {
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Detecta si hay suelo adelante del enemigo
    /// </summary>
    protected virtual bool HasGroundAhead(int direction)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction * edgeCheckOffset.x, edgeCheckOffset.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, edgeCheckDistance, groundLayer);
        
        // Debug visual
        Debug.DrawRay(origin, Vector2.down * edgeCheckDistance, hit.collider != null ? Color.green : Color.red);
        
        return hit.collider != null;
    }

    /// <summary>
    /// Detecta si hay una pared adelante del enemigo
    /// </summary>
    protected virtual bool HasWallAhead(int direction)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction * wallCheckOffset.x, wallCheckOffset.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(direction, 0), wallCheckDistance, groundLayer);
        
        // Debug visual
        Debug.DrawRay(origin, new Vector2(direction, 0) * wallCheckDistance, hit.collider != null ? Color.red : Color.green);
        
        return hit.collider != null;
    }

    /// <summary>
    /// Actualiza la dirección del sprite según la dirección que mira
    /// </summary>
    protected virtual void UpdateSpriteDirection()
    {
        if (facingDirection == 0 || spriteRenderer == null) return;
        
        Vector3 scale = transform.localScale;
        scale.x = facingDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    #endregion

    #region Death
    
    protected virtual void Die()
    {
        isAlive = false;
        
        // Desactivar físicas
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        
        // Desactivar colisiones
        var colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        
        // Destruir después de un pequeño delay
        Destroy(gameObject, 0.1f);
    }

    #endregion

    #region Collision Handling
    
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el jugador salta encima del enemigo, morir
        if (collision.collider.CompareTag("Player") || collision.collider.name == "Player")
        {
            foreach (var contact in collision.contacts)
            {
                // Si el contacto viene desde arriba (normal apunta hacia abajo)
                if (contact.normal.y < -0.5f)
                {
                    Die();
                    
                    // Dar un pequeño rebote al jugador
                    var playerRb = collision.collider.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);
                    }
                    
                    break;
                }
            }
        }
    }

    #endregion

    #region Debug
    
    protected virtual void OnDrawGizmosSelected()
    {
        // Rango de visión
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    #endregion
}
