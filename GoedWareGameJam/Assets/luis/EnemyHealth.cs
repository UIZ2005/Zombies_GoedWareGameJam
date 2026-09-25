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
        if (isDead || isKnockedBack || player == null)
            return;

        // Si fue empujado fuera del mapa azul, intenta reconectarse
        if (!agent.isOnNavMesh)
        {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, maxDistanceToNavMesh, UnityEngine.AI.NavMesh.AllAreas))
            {
                agent.Warp(hit.position); // Vuelve a anclarse al mapa
            }
            return; // Espera a estar seguro en el mapa antes de moverse
        }

        // Persecución inteligente estándar
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        if (isDead) return;

        currentHealth -= damage;

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

        Destroy(gameObject);
    }
}