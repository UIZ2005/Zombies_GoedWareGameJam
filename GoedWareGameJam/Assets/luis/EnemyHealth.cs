using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Persecución")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stoppingDistance = 1.1f;
    [SerializeField] private float maxDistanceToNavMesh = 2f; // Radio para buscar el mapa perdido

    private Transform player;
    private UnityEngine.AI.NavMeshAgent agent; 

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.3f;

    private bool isKnockedBack = false;
    private Coroutine knockbackCoroutine;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 1f;

    private Rigidbody2D rb;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
        agent.speed = speed;
        agent.stoppingDistance = stoppingDistance;

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Lo hacemos inmune a empujones de caminar para evitar el tartamudeo
        rb.bodyType = RigidbodyType2D.Kinematic;
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

    private void Update()
    {
        if (isDead || player == null)
            return;

        if (!agent.isOnNavMesh)
        {
            UnityEngine.AI.NavMeshHit hit;

            if (UnityEngine.AI.NavMesh.SamplePosition(
                transform.position,
                out hit,
                maxDistanceToNavMesh,
                UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }

            UpdateAnimation(Vector2.zero);
            return;
        }

        if (isKnockedBack)
        {
            UpdateAnimation(Vector2.zero);
            return;
        }

        agent.SetDestination(player.position);

        // La dirección/velocidad ahora viene del NavMeshAgent
        UpdateAnimation(agent.velocity);
    }

    private void UpdateAnimation(Vector2 velocity)
    {
        if (animator == null || spriteRenderer == null)
            return;

        // -----------------------------------------
        // QUIETO
        // -----------------------------------------

        if (velocity.sqrMagnitude < 0.01f)
        {
            animator.SetBool("walk", false);
            animator.SetBool("walkUp", false);

            return;
        }

        // -----------------------------------------
        // CAMINANDO
        // -----------------------------------------

        animator.SetBool("walk", true);

        // -----------------------------------------
        // HORIZONTAL
        // -----------------------------------------

        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            animator.SetBool("walkUp", false);

            // Derecha
            if (velocity.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            // Izquierda
            else
            {
                spriteRenderer.flipX = true;
            }
        }

        // -----------------------------------------
        // VERTICAL
        // -----------------------------------------

        else
        {
            // Hacia arriba
            if (velocity.y > 0)
            {
                animator.SetBool("walkUp", true);
            }
            // Hacia abajo
            else
            {
                animator.SetBool("walkUp", false);
            }
        }
    }
    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthTransparency();


        if (rb != null && knockbackForce > 0)
        {
            if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = StartCoroutine(ApplyKnockback(knockbackDirection, knockbackForce));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void UpdateHealthTransparency()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("No hay SpriteRenderer asignado.");
            return;
        }

        // Remap de vida:
        // currentHealth 0    -> 0
        // currentHealth 100  -> 1
        float healthNormalized = Mathf.InverseLerp(
            0f,
            maxHealth,
            currentHealth
        );

        // Remap:
        // 0   -> 0.3
        // 1   -> 1
        float alpha = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            healthNormalized
        );

        Color color = spriteRenderer.color;
        color.a = alpha;

        spriteRenderer.color = color;

        Debug.Log(
            $"HP: {currentHealth}/{maxHealth} | Normalizado: {healthNormalized} | Alpha: {alpha}"
        );
    }

    private IEnumerator ApplyKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        
        agent.isStopped = true; 
        
        // Se vuelve dinámico SOLO durante el impacto para poder salir volando
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        
        // Vuelve a su estado estable para seguir persiguiendo
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        agent.isStopped = false; 
        isKnockedBack = false;
        knockbackCoroutine = null;
    }

    private void Die()
    {
        isDead = true;
        
        if (agent != null) agent.isStopped = true;
        rb.linearVelocity = Vector2.zero;

        StartCoroutine(dieAnimation());
    }

    IEnumerator dieAnimation()
    {

        animator.SetBool("die", true);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        yield return null;
    }
}