using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Rigidbody2D rb;
    Transform player;
    public float velocidad = 2.0f;
    public float visionDistancia = 5f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;
    public LayerMask groundLayer;
    public float edgeCheckDistance = 0.6f;
    public float wallCheckDistance = 0.4f;

    bool canAttack = true;
    bool isAttacking = false;
    int dir = -1; // -1 izquierda, 1 derecha
    float patrolChangeTimer = 0f;
    float patrolChangeInterval = 2.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
            rb.rotation = 0f;
        }
        GameObject maybePlayer = GameObject.FindWithTag("Player");
        if (maybePlayer == null)
        {
            GameObject byName = GameObject.Find("Player");
            player = byName != null ? byName.transform : null;
        }
        else
        {
            player = maybePlayer.transform;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        bool seesPlayer = PuedeVerJugador();
        if (seesPlayer)
        {
            float dist = Mathf.Abs(player.position.x - transform.position.x);
            dir = player.position.x > transform.position.x ? 1 : -1;
            if (dist <= attackRange)
            {
                if (canAttack) StartCoroutine(Attack());
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            else
            {
                MoverConSeguridad(dir);
            }
        }
        else
        {
            Patrullar();
        }

        FlipSegunDireccion();
    }

    void MoverConSeguridad(int direction)
    {
        if (!HaySueloAdelante(direction) || HayParedAdelante(direction))
        {
            dir = -direction;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        rb.linearVelocity = new Vector2(direction * velocidad, rb.linearVelocity.y);
    }

    void Patrullar()
    {
        patrolChangeTimer += Time.fixedDeltaTime;
        if (patrolChangeTimer >= patrolChangeInterval)
        {
            patrolChangeTimer = 0f;
            patrolChangeInterval = Random.Range(1.5f, 3.5f);
            int choice = Random.Range(0, 3); // 0: parar, 1: izquierda, 2: derecha
            if (choice == 0) dir = 0; else dir = (choice == 1 ? -1 : 1);
        }

        if (dir == 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        MoverConSeguridad(dir);
    }

    bool PuedeVerJugador()
    {
        if (player == null) return false;
        float dx = player.position.x - transform.position.x;
        if (Mathf.Abs(dx) > visionDistancia) return false;
        int facing = dir == 0 ? 1 : dir;
        if (Mathf.Sign(dx) != facing) return false;
        return true;
    }

    bool HaySueloAdelante(int direction)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction * 0.4f, 0.05f);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, edgeCheckDistance, groundLayer);
        return hit.collider != null;
    }

    bool HayParedAdelante(int direction)
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(direction * 0.4f, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(direction, 0), wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    void FlipSegunDireccion()
    {
        if (dir == 0) return;
        Vector3 escala = transform.localScale;
        escala.x = dir > 0 ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    IEnumerator Attack()
    {
        canAttack = false;
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        if (player != null && Mathf.Abs(player.position.x - transform.position.x) <= attackRange + 0.1f)
        {
            var ctrl = player.GetComponent<ControlesPersonaje>();
            if (ctrl != null) ctrl.TakeDamage(1);
        }
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(int dmg)
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.name == "Player")
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}

