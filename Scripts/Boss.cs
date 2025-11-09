using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Stats del Jefe")]
    public int maxHealth = 20;
    private int currentHealth;
    public float velocidadMovimiento = 3f;

    [Header("Ataques")]
    public float attackCooldown = 5f;
    public GameObject enemigoPrefab; 
    public Transform puntoInvocacion; 
    public GameObject proyectilPrefab; 
    public Transform puntoDisparo; 
    public float velocidadPersecucion = 5f; 

    [Header("Detección")]
    public LayerMask groundLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;

    private Rigidbody2D rb;
    private Transform player;
    private bool mirandoDerecha = true;
    private bool isInvulnerable = false;
    private bool canAttack = true;

    private enum Estado { Paseando, Persiguiendo, Atacando, EnCooldown }
    private Estado estadoActual;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        estadoActual = Estado.Paseando;
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        while (true)
        {
            switch (estadoActual)
            {
                case Estado.Paseando:
                    yield return StartCoroutine(PasearState());
                    break;
                case Estado.Persiguiendo:
                    yield return StartCoroutine(PerseguirState());
                    break;
                case Estado.Atacando:
                    yield return StartCoroutine(AtacarState());
                    break;
                case Estado.EnCooldown:
                    yield return StartCoroutine(CooldownState());
                    break;
            }
        }
    }

    IEnumerator PasearState()
    {
        float direction = mirandoDerecha ? 1 : -1;
        while (estadoActual == Estado.Paseando)
        {
            moveAround(direction);

            if (DetectarPared())
            {
                direction *= -1;
                Girar();
            }

            if (canAttack)
            {
                estadoActual = Estado.Atacando;
            }
            yield return null;
        }
    }

    IEnumerator PerseguirState()
    {
        while (estadoActual == Estado.Persiguiendo)
        {
            if (player != null)
            {
                Vector3 direccionHaciaJugador = (player.position - transform.position).normalized;
                rb.linearVelocity = new Vector2(direccionHaciaJugador.x * velocidadPersecucion, rb.linearVelocity.y);

                if ((direccionHaciaJugador.x > 0 && !mirandoDerecha) || (direccionHaciaJugador.x < 0 && mirandoDerecha))
                {
                    Girar();
                }

                if (Vector2.Distance(transform.position, player.position) < 2f)
                {
                    golpeBaston();
                    estadoActual = Estado.EnCooldown;
                }
            }
            else
            {
                estadoActual = Estado.Paseando;
            }
            yield return null;
        }
    }

    IEnumerator AtacarState()
    {
        canAttack = false;
        rb.linearVelocity = Vector2.zero;

        randomAtack();

        yield return null;
    }

    IEnumerator CooldownState()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        if (estadoActual != Estado.Persiguiendo)
        {
            estadoActual = Estado.Paseando;
        }
    }

    void FireballAtack()
    {
        Debug.Log("Ataque a Distancia!");
        Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
        estadoActual = Estado.EnCooldown;
    }

    void invocarAtack()
    {
        Debug.Log("Invocando Enemigos!");
        Instantiate(enemigoPrefab, puntoInvocacion.position, puntoInvocacion.rotation);
        estadoActual = Estado.EnCooldown;
    }

    void golpeBaston()
    {
        Debug.Log("Ataque con Bastón!");
        rb.linearVelocity = Vector2.zero;
    }

    void moveAround(float direction)
    {
        rb.linearVelocity = new Vector2(direction * velocidadMovimiento, rb.linearVelocity.y);
    }

    void randomAtack()
    {
        List<int> ataquesPosibles = new List<int> { 0, 1, 2 };
        if (GameObject.FindGameObjectsWithTag("Enemy").Length > 1) 
        {
            ataquesPosibles.Remove(2); 
        }

        int ataqueElegido = ataquesPosibles[Random.Range(0, ataquesPosibles.Count)];

        switch (ataqueElegido)
        {
            case 0: 
                estadoActual = Estado.Persiguiendo;
                break;
            case 1: 
                FireballAtack();
                break;
            case 2: 
                invocarAtack();
                break;
        }
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
        yield return new WaitForSeconds(3f);
        isInvulnerable = false;
    }

    void Die()
    {
        Destroy(gameObject);
    }

    private bool DetectarPared()
    {
        return Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayer);
    }

    void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * (mirandoDerecha ? 1 : -1), wallCheck.position.y, wallCheck.position.z));
    }
}
