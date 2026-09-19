using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(
        int damage,
        Vector2 knockbackDirection,
        float knockbackForce
    )
    {
        currentHealth -= damage;

        if (rb != null)
        {
            rb.AddForce(
                knockbackDirection.normalized * knockbackForce,
                ForceMode2D.Impulse
            );
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}