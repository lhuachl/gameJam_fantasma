using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float Speed = 10f;

    private Rigidbody2D rb;
    private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = direction * Speed;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con algo, simplemente se destruye
        Destroy(gameObject);
    }
}