using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Persecución")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 0.5f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.2f;

    private Transform player;
    private Rigidbody2D rb;

    private bool isKnockedBack = false;
    private Coroutine knockbackCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con el tag Player.");
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack)
            return;

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

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

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        knockbackCoroutine = StartCoroutine(
            KnockbackRoutine(direction, force)
        );
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force)
    {
        isKnockedBack = true;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;

        isKnockedBack = false;
        knockbackCoroutine = null;
    }
}