using UnityEngine;

public class Pinchos : MonoBehaviour
{
    public int damage = 1;
    public float knockbackX = 8f;
    public float knockbackY = 6f;

    void OnTriggerEnter2D(Collider2D other)
    {
        TryAffect(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryAffect(collision.collider.gameObject);
    }

    void TryAffect(GameObject go)
    {
        if (go == null) return;
        if (!(go.CompareTag("Player") || go.name == "Player")) return;

        var ctrl = go.GetComponent<ControlesPersonaje>();
        if (ctrl != null)
        {
            ctrl.TakeDamage(damage);
        }

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float dir = Mathf.Sign(go.transform.position.x - transform.position.x);
            if (Mathf.Approximately(dir, 0f)) dir = 1f;
            rb.linearVelocity = new Vector2(dir * knockbackX, knockbackY);
        }
    }
}
