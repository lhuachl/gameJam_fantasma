using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // Importar el nuevo Input System

public class ControlesPersonaje : MonoBehaviour
{
    [Header("Stats")]
    public float velocidad = 5.0f;
    public float fuerzaSalto = 5.0f;
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Dash")]
    public float dashSpeedX = 24f;
    public float dashSpeedY = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool canParry = true;

    [Header("Checks")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public Transform wallCheck;
    public float wallCheckDistance = 0.6f;

    [Header("Death")]
    public float deathY = -20f;

    private Rigidbody2D rb;
    private bool mirandoDerecha = true;
    private bool isGrounded;
    private bool isTouchingWall;

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 3f;
    public Transform inicio;
    private bool warnedMissingGroundLayer = false;

    // Variables para el nuevo Input System (definidas en código, sin asset generado)
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction attackAction;
    private InputAction parryAction;
    private Vector2 moveInput;

    private void Awake()
    {
        // MOVE (stick y teclado WASD + flechas)
        moveAction = new InputAction("Move", InputActionType.Value, binding: "<Gamepad>/leftStick");
        var wasd = moveAction.AddCompositeBinding("2DVector");
        wasd.With("Up", "<Keyboard>/w");
        wasd.With("Down", "<Keyboard>/s");
        wasd.With("Left", "<Keyboard>/a");
        wasd.With("Right", "<Keyboard>/d");
        var arrows = moveAction.AddCompositeBinding("2DVector");
        arrows.With("Up", "<Keyboard>/upArrow");
        arrows.With("Down", "<Keyboard>/downArrow");
        arrows.With("Left", "<Keyboard>/leftArrow");
        arrows.With("Right", "<Keyboard>/rightArrow");
        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        // JUMP (Espacio / botón sur)
        jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.performed += ctx => HandleJump();

        // DASH (tecla Y)
        dashAction = new InputAction("Dash", InputActionType.Button, "<Keyboard>/y");
        dashAction.performed += ctx => { if (canDash) StartCoroutine(Dash()); };

        // ATTACK (tecla P)
        attackAction = new InputAction("Attack", InputActionType.Button, "<Keyboard>/p");
        attackAction.performed += ctx => { if (canAttack) StartCoroutine(Attack()); };

        // PARRY (tecla I)
        parryAction = new InputAction("Parry", InputActionType.Button, "<Keyboard>/i");
        parryAction.performed += ctx => { if (canParry) StartCoroutine(Parry()); };
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
        attackAction.Enable();
        parryAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.Disable();
        dashAction.Disable();
        jumpAction.Disable();
        moveAction.Disable();
        parryAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                Debug.LogWarning("ControlesPersonaje: Rigidbody2D no estaba en este objeto; se usó el del padre.");
            }
        }
        if (rb == null)
        {
            Debug.LogError("ControlesPersonaje: No se encontró Rigidbody2D en este objeto ni en el padre. No se puede mover.");
            enabled = false;
            return;
        }
        currentHealth = maxHealth;
        if (groundCheck == null)
        {
            var found = transform.Find("groundCheck");
            if (found != null)
            {
                groundCheck = found;
            }
            else
            {
                GameObject gc = new GameObject("groundCheck");
                gc.transform.SetParent(transform);
                gc.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                groundCheck = gc.transform;
                Debug.LogWarning("ControlesPersonaje: 'groundCheck' no estaba asignado. Se creó automáticamente como hijo.");
            }
        }
        if (rb != null)
        {
            // Evitar que el rigidbody rote al chocar
            rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
            rb.rotation = 0f;
            // Auto-configurar capa Ground si existe
            int groundMask = LayerMask.GetMask("Ground");
            if (groundLayer.value == 0 && groundMask != 0)
            {
                groundLayer = groundMask;
                Debug.Log("ControlesPersonaje: 'groundLayer' auto-asignado a capa 'Ground'.");
            }
            if (rb.bodyType != RigidbodyType2D.Dynamic)
            {
                Debug.LogWarning("ControlesPersonaje: Rigidbody2D.bodyType no es Dynamic. Cambia a Dynamic para permitir movimiento por velocidad.");
            }
            if ((rb.constraints & RigidbodyConstraints2D.FreezePositionX) != 0)
            {
                Debug.LogWarning("ControlesPersonaje: Rigidbody2D tiene Freeze Position X activado. Desactívalo para permitir movimiento horizontal.");
            }
        }
    }

    void Update()
    {
        if (transform.position.y <= deathY)
        {
            Die();
            return;
        }
        if (isDashing) return;
        if (moveAction != null && moveAction.enabled)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
        // Fallback: sistema de input antiguo si el nuevo no devuelve nada
        if (Mathf.Approximately(moveInput.x, 0f))
        {
            float legacyX = Input.GetAxisRaw("Horizontal");
            if (!Mathf.Approximately(legacyX, 0f))
            {
                moveInput = new Vector2(legacyX, moveInput.y);
            }
        }
        // Fallback para salto con sistema antiguo
        if ((jumpAction == null || !jumpAction.enabled) && Input.GetKeyDown(KeyCode.Space))
        {
            HandleJump();
        }
        // Fallback para dash con sistema antiguo (tecla Y)
        if ((dashAction == null || !dashAction.enabled) && Input.GetKeyDown(KeyCode.Y))
        {
            if (canDash) StartCoroutine(Dash());
        }
        // Fallback para ataque con sistema antiguo (tecla P)
        if ((attackAction == null || !attackAction.enabled) && Input.GetKeyDown(KeyCode.P))
        {
            if (canAttack) StartCoroutine(Attack());
        }
        // Fallback para parry con sistema antiguo (tecla I)
        if ((parryAction == null || !parryAction.enabled) && Input.GetKeyDown(KeyCode.I))
        {
            if (canParry) StartCoroutine(Parry());
        }
        CheckSurroundings();
    }

    void FixedUpdate()
    {
        if (isDashing) return;
        movimiento2D(moveInput.x);
        // Diagnóstico ligero
        // if (Mathf.Abs(moveInput.x) > 0.01f) Debug.Log($"MoveX={moveInput.x}, velX={rb.linearVelocity.x}");
    }

    private void HandleJump()
    {
        // Salto sólo si está en el suelo y la velocidad vertical es casi 0 (evita saltos en aire)
        if (isGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.2f)
        {
            salto();
        }
        else if (isTouchingWall && !isGrounded)
        {
            Walljump();
        }
    }

    private void CheckSurroundings()
    {
        int mask = groundLayer.value;

        // Suelo
        if (mask != 0)
        {
            if (groundCheck != null)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, mask);
            }
            else
            {
                // Fallback: raycast corto hacia abajo desde el centro
                isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckRadius * 2f, mask);
            }
        }
        else
        {
            if (!warnedMissingGroundLayer)
            {
                Debug.LogWarning("ControlesPersonaje: 'groundLayer' no está configurado. Usando fallback por velocidad vertical para grounded.");
                warnedMissingGroundLayer = true;
            }
            // Fallback: grounded si casi no cambia Y (más estricto)
            isGrounded = Mathf.Abs(rb.linearVelocity.y) < 0.01f;
        }

        // Pared
        if (mask != 0)
        {
            if (wallCheck != null)
            {
                isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, mask);
            }
            else
            {
                isTouchingWall = Physics2D.Raycast(transform.position, transform.right, wallCheckDistance, mask);
            }
        }
        else
        {
            isTouchingWall = false;
        }
    }

    void movimiento2D(float movimiento)
    {
        rb.linearVelocity = new Vector2(movimiento * velocidad, rb.linearVelocity.y);

        if ((movimiento > 0 && !mirandoDerecha) || (movimiento < 0 && mirandoDerecha))
        {
            girar();
        }
    }

    void girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.Rotate(0f, 180f, 0f);
    }

    void salto()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
    }

    void Walljump()
    {
        float jumpDirection = mirandoDerecha ? -1f : 1f;
        rb.linearVelocity = new Vector2(jumpDirection * velocidad, fuerzaSalto);
        girar(); // Girar al saltar de la pared
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        isInvulnerable = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        // Movimiento en eje X según la dirección que está mirando, eje Y hacia arriba
        float dashX = transform.right.x * dashSpeedX;
        float dashY = dashSpeedY;
        rb.linearVelocity = new Vector2(dashX, dashY);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(invulnerabilityDuration - dashDuration);
        isInvulnerable = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator Attack()
    {
        canAttack = false;
        Debug.Log("Attack!");

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, attackRange);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Hit an enemy!");
                var enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator Parry()
    {
        if (!canParry) yield break;
        canParry = false;
        bool prevInv = isInvulnerable;
        isInvulnerable = true;
        yield return new WaitForSeconds(0.15f);
        isInvulnerable = prevInv;
        yield return new WaitForSeconds(1f);
        canParry = true;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        StartCoroutine(BecomeInvulnerable());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    void Die()
    {
        Debug.Log("Player has died.");
        resetear();
        currentHealth = maxHealth; 
    }

    void resetear()
    {
        if (inicio != null) {
            transform.position = inicio.position;
        } else {
             Debug.LogError("ControlesPersonaje: El objeto 'inicio' no está asignado en el Inspector para resetear la posición.");
        }
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * transform.right.x, wallCheck.position.y, wallCheck.position.z));
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackRange * transform.right.x, transform.position.y, transform.position.z));
    }
}