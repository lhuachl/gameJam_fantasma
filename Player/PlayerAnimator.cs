using UnityEngine;

/// <summary>
/// Sistema de animación del jugador - Escucha eventos del PlayerController y actualiza el Animator
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Animator Parameters")]
    private Animator animator;
    private PlayerController playerController;
    private Rigidbody2D rb;

    // Estado de animación
    private bool wasGrounded = true;

    // Animation parameters
    private readonly int IsRunningHash = Animator.StringToHash("IsRunning");
    private readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int IsJumpingHash = Animator.StringToHash("IsJumping");
    private readonly int IsFallingHash = Animator.StringToHash("IsFalling");
    private readonly int OnAirHash = Animator.StringToHash("OnAir");
    private readonly int OnDashPressedHash = Animator.StringToHash("OnDashPressed");
    private readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private readonly int IsParryingHash = Animator.StringToHash("IsParrying");
    private readonly int OnJumpHash = Animator.StringToHash("OnJump");
    private readonly int OnLandHash = Animator.StringToHash("OnLand");
    private readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    private readonly int OnParryHash = Animator.StringToHash("OnParry");
    private readonly int OnTakeDamageHash = Animator.StringToHash("OnTakeDamage");
    private readonly int OnDieHash = Animator.StringToHash("OnDie");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            Debug.LogError("[PlayerAnimator] No se encontró Animator en el GameObject");
        }

        if (playerController == null)
        {
            Debug.LogError("[PlayerAnimator] No se encontró PlayerController en el GameObject");
        }
    }

    private void OnEnable()
    {
        // Suscribirse a eventos del jugador
        EventManager.Instance.Subscribe<PlayerJumpedEvent>(OnPlayerJumped);
        EventManager.Instance.Subscribe<PlayerDashedEvent>(OnPlayerDashed);
        EventManager.Instance.Subscribe<PlayerAttackedEvent>(OnPlayerAttacked);
        EventManager.Instance.Subscribe<PlayerTakeDamageEvent>(OnPlayerTakeDamage);
        EventManager.Instance.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void OnDisable()
    {
        // Desuscribirse de eventos
        EventManager.Instance.Unsubscribe<PlayerJumpedEvent>(OnPlayerJumped);
        EventManager.Instance.Unsubscribe<PlayerDashedEvent>(OnPlayerDashed);
        EventManager.Instance.Unsubscribe<PlayerAttackedEvent>(OnPlayerAttacked);
        EventManager.Instance.Unsubscribe<PlayerTakeDamageEvent>(OnPlayerTakeDamage);
        EventManager.Instance.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
    }

    private void Update()
    {
        if (animator == null || playerController == null) return;

        UpdateMovementAnimations();
        UpdateGroundAnimations();
        UpdateAirAnimations();
    }

    /// <summary>
    /// Actualiza las animaciones de movimiento horizontal
    /// </summary>
    private void UpdateMovementAnimations()
    {
        // Obtener la velocidad horizontal
        float speedX = Mathf.Abs(rb.linearVelocity.x);
        
        // Actualizar parámetro IsRunning
        bool isRunning = speedX > 0.1f;
        animator.SetBool(IsRunningHash, isRunning);

        // Actualizar dirección del sprite (si el PlayerController no lo hace)
        if (speedX > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (speedX < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    /// <summary>
    /// Actualiza las animaciones relacionadas con el suelo
    /// </summary>
    private void UpdateGroundAnimations()
    {
        // Verificar si el jugador está en el suelo (usando el mismo método que PlayerController)
        bool isGrounded = playerController.IsGrounded();
        
        // Actualizar parámetro IsGrounded
        animator.SetBool(IsGroundedHash, isGrounded);

        // Detectar aterrizaje (transición de not grounded a grounded)
        if (isGrounded && !wasGrounded)
        {
            animator.SetTrigger(OnLandHash);
        }

        wasGrounded = isGrounded;
    }

    /// <summary>
    /// Actualiza las animaciones de aire
    /// </summary>
    private void UpdateAirAnimations()
    {
        bool isGrounded = playerController.IsGrounded();
        
        // Actualizar parámetro OnAir (opuesto a IsGrounded)
        animator.SetBool(OnAirHash, !isGrounded);

        // Actualizar parámetro IsJumping (velocidad Y positiva y no en suelo)
        bool isJumping = !isGrounded && rb.linearVelocity.y > 0.1f;
        animator.SetBool(IsJumpingHash, isJumping);

        // Actualizar parámetro IsFalling (velocidad Y negativa y no en suelo)
        bool isFalling = !isGrounded && rb.linearVelocity.y < -0.1f;
        animator.SetBool(IsFallingHash, isFalling);
    }

    // ═══════════════════════════════════════════════════════════
    // MANEJADORES DE EVENTOS
    // ═══════════════════════════════════════════════════════════

    private void OnPlayerJumped(PlayerJumpedEvent evt)
    {
        animator.SetTrigger(OnJumpHash);
        Debug.Log("[PlayerAnimator] Salto detectado");
    }

    private void OnPlayerDashed(PlayerDashedEvent evt)
    {
        animator.SetBool(OnDashPressedHash, true);
        // El dash dura 0.2 segundos, así que lo apagamos después
        StartCoroutine(EndDashAnimation());
        Debug.Log("[PlayerAnimator] Dash detectado");
    }

    private void OnPlayerAttacked(PlayerAttackedEvent evt)
    {
        animator.SetTrigger(OnAttackHash);
        animator.SetBool(IsAttackingHash, true);
        // El ataque dura 0.5 segundos (attackCooldown)
        StartCoroutine(EndAttackAnimation());
        Debug.Log("[PlayerAnimator] Ataque detectado");
    }

    private void OnPlayerTakeDamage(PlayerTakeDamageEvent evt)
    {
        animator.SetTrigger(OnTakeDamageHash);
        Debug.Log("[PlayerAnimator] Daño recibido");
    }

    private void OnPlayerDied(PlayerDiedEvent evt)
    {
        animator.SetTrigger(OnDieHash);
        Debug.Log("[PlayerAnimator] Muerte detectada");
    }

    // ═══════════════════════════════════════════════════════════
    // CORRUTINAS DE ANIMACIÓN
    // ═══════════════════════════════════════════════════════════

    private System.Collections.IEnumerator EndDashAnimation()
    {
        yield return new WaitForSeconds(0.2f); // Duración del dash
        animator.SetBool(OnDashPressedHash, false);
    }

    private System.Collections.IEnumerator EndAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f); // attackCooldown
        animator.SetBool(IsAttackingHash, false);
    }

    // ═══════════════════════════════════════════════════════════
    // MÉTODOS PÚBLICOS PARA ACCESO DESDE OTROS SCRIPTS
    // ═══════════════════════════════════════════════════════════

    public void SetParryAnimation(bool isParrying)
    {
        animator.SetBool(IsParryingHash, isParrying);
    }

    public void TriggerParry()
    {
        animator.SetTrigger(OnParryHash);
    }
}