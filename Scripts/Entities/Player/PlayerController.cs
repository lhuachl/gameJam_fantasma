using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador del jugador - Maneja movimiento, saltos, dash, ataque
/// Usa el nuevo Input System configurado en código
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour, IDamageable
{
    // ═══════════════════════════════════════════════════════════
    // STATS BÁSICOS
    // ═══════════════════════════════════════════════════════════
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeedX = 24f;
    [SerializeField] private float dashSpeedY = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Death")]
    [SerializeField] private float deathYThreshold = -20f;
    [SerializeField] private Transform respawnPoint;

    // ═══════════════════════════════════════════════════════════
    // COMPONENTES
    // ═══════════════════════════════════════════════════════════

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    // ═══════════════════════════════════════════════════════════
    // ESTADO
    // ═══════════════════════════════════════════════════════════

    private int currentHealth;
    private int maxHealth;
    private bool isGrounded;
    private bool isFacingRight = true;
    private bool isDashing = false;
    private bool canDash = true;
    private bool canAttack = true;
    private bool isIntangible = false;

    // INPUT
    private Vector2 moveInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction attackAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        SetupInputActions();
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        dashAction?.Enable();
        attackAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        dashAction?.Disable();
        attackAction?.Disable();
    }

    private void Start()
    {
        maxHealth = GameManager.Instance.GetMaxHealth();
        currentHealth = maxHealth;
        
        if (groundCheck == null)
            Debug.LogWarning("[PlayerController] ¡groundCheck no asignado!");
    }

    private void Update()
    {
        // Física
        CheckGround();
        ApplyDrag();
        
        // Movimiento
        Move();
        
        // Verificar muerte
        if (transform.position.y < deathYThreshold)
            TakeDamage(currentHealth, Vector2.zero, 0);
    }

    // ═══════════════════════════════════════════════════════════
    // INPUT SETUP
    // ═══════════════════════════════════════════════════════════

    private void SetupInputActions()
    {
        // MOVE (stick + WASD + flechas)
        moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        // JUMP (Espacio / botón sur)
        jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.performed += ctx => Jump();

        // DASH (Y / botón norte)
        dashAction = new InputAction("Dash", InputActionType.Button, "<Keyboard>/y");
        dashAction.AddBinding("<Gamepad>/buttonNorth");
        dashAction.performed += ctx => AttemptDash();

        // ATTACK (P / botón oeste)
        attackAction = new InputAction("Attack", InputActionType.Button, "<Keyboard>/p");
        attackAction.AddBinding("<Gamepad>/buttonWest");
        attackAction.performed += ctx => AttemptAttack();
    }

    // ═══════════════════════════════════════════════════════════
    // MOVIMIENTO
    // ═══════════════════════════════════════════════════════════

    private void Move()
    {
        if (isDashing)
            return; // No permitir movimiento durante dash

        float targetVelocityX = moveInput.x * moveSpeed;
        rb.velocity = new Vector2(targetVelocityX, rb.velocity.y);

        // Rotar sprite según dirección
        if (moveInput.x > 0 && !isFacingRight)
            Flip();
        else if (moveInput.x < 0 && isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !isFacingRight;
    }

    // ═══════════════════════════════════════════════════════════
    // SALTO
    // ═══════════════════════════════════════════════════════════

    private void Jump()
    {
        if (!isGrounded)
            return;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        EventManager.Broadcast(new PlayerJumpedEvent { jumpPosition = transform.position });
    }

    // ═══════════════════════════════════════════════════════════
    // DASH (INTANGIBLE)
    // ═══════════════════════════════════════════════════════════

    private void AttemptDash()
    {
        if (canDash)
            StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;
        isIntangible = true;

        // Desactivar collider para intangibilidad
        col.enabled = false;

        Vector2 dashDirection = isFacingRight ? Vector2.right : Vector2.left;
        Vector2 dashVelocity = new Vector2(dashDirection.x * dashSpeedX, dashSpeedY);

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.velocity = dashVelocity;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reactivar collider
        col.enabled = true;
        isDashing = false;
        isIntangible = false;

        EventManager.Broadcast(new PlayerDashedEvent 
        { 
            dashStartPosition = transform.position - (Vector3)dashDirection * 2,
            dashEndPosition = transform.position,
            isIntangible = true
        });

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    // ═══════════════════════════════════════════════════════════
    // ATAQUE
    // ═══════════════════════════════════════════════════════════

    private void AttemptAttack()
    {
        if (canAttack)
            StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        canAttack = false;

        int damage = GameManager.Instance.GetWeaponDamage();
        Vector2 attackDir = isFacingRight ? Vector2.right : Vector2.left;
        Vector3 attackPos = transform.position + (Vector3)attackDir * attackRange;

        // Detectar enemigos en rango
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, attackRange);
        foreach (var hit in hitEnemies)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("Boss"))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage, attackDir, 5f);
                }
            }
        }

        EventManager.Broadcast(new PlayerAttackedEvent 
        { 
            attackPosition = attackPos, 
            damageDealt = damage 
        });

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ═══════════════════════════════════════════════════════════
    // FÍSICA
    // ═══════════════════════════════════════════════════════════

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundHit != null;
    }

    private void ApplyDrag()
    {
        rb.drag = isGrounded ? groundDrag : airDrag;
    }

    // ═══════════════════════════════════════════════════════════
    // INTERFAZ IDAMAGEABLE
    // ═══════════════════════════════════════════════════════════

    public bool TakeDamage(int damage, Vector2 knockbackDirection = default, float knockbackForce = 0f)
    {
        // No se puede recibir daño durante dash
        if (isIntangible)
            return true;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        GameManager.Instance.SetCurrentHealth(currentHealth);

        // Knockback
        if (knockbackForce > 0)
        {
            rb.velocity = knockbackDirection.normalized * knockbackForce;
        }

        EventManager.Broadcast(new PlayerTakeDamageEvent 
        { 
            damageAmount = damage, 
            remainingHealth = currentHealth,
            damageSourcePosition = transform.position
        });

        if (currentHealth <= 0)
        {
            Die();
            return false;
        }

        return true;
    }

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsAlive() => currentHealth > 0;
    public Vector3 GetPosition() => transform.position;

    private void Die()
    {
        EventManager.Broadcast(new PlayerDiedEvent 
        { 
            lastLevelAttempted = GameManager.Instance.GetCurrentLevel(),
            deathPosition = transform.position
        });

        // Respawnear o volver al nivel
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // ═══════════════════════════════════════════════════════════
    // GETTERS
    // ═══════════════════════════════════════════════════════════

    public bool IsIntangible => isIntangible;
    public bool IsDashing => isDashing;
    public bool IsGrounded => isGrounded;
    public bool IsFacingRight => isFacingRight;
}
