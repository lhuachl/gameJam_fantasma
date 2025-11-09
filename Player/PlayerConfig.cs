using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/Player Config")]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    
    [Header("Attack")]
    public float attackRange = 1f;
    public int attackDamage = 1;
    public float attackCooldown = 0.5f;
    
    [Header("Health")]
    public int maxHealth = 3;
    public float invulnerabilityDuration = 1f;
    public float deathY = -20f;
    
    [Header("References")]
    public Transform groundCheck;
}