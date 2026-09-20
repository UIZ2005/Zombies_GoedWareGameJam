using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Persecución")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 1.1f;

    private Transform player;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.3f;

    private bool isKnockedBack = false;
    private Coroutine knockbackCoroutine;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private Rigidbody2D rb;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "No se encontró un objeto con el tag Player."
            );
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isKnockedBack)
            return;

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > stoppingDistance)
        {
            Vector2 direction =
                (player.position - transform.position).normalized;

            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(
        int damage,
        Vector2 knockbackDirection,
        float knockbackForce
    )
    {
        if (isDead)
            return;

        currentHealth -= damage;

        // Aplicar knockback
        if (rb != null && knockbackForce > 0)
        {
            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
            }

            knockbackCoroutine = StartCoroutine(
                ApplyKnockback(knockbackDirection, knockbackForce)
            );
        }

        // Comprobar vida
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator ApplyKnockback(
        Vector2 direction,
        float force
    )
    {
        isKnockedBack = true;

        // Detener el movimiento de persecución
        rb.linearVelocity = Vector2.zero;

        // Aplicar empuje
        rb.AddForce(
            direction.normalized * force,
            ForceMode2D.Impulse
        );

        // Esperar mientras dura el knockback
        yield return new WaitForSeconds(knockbackDuration);

        // Detener el desplazamiento restante
        rb.linearVelocity = Vector2.zero;

        isKnockedBack = false;
        knockbackCoroutine = null;
    }

    private void Die()
    {
        isDead = true;

        rb.linearVelocity = Vector2.zero;

        Destroy(gameObject);
    }
}