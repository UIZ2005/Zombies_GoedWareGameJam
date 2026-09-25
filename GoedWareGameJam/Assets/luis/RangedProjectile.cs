using UnityEngine;

public class RangedProjectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private float damage;

    private bool canSlow;
    private float slowChance;
    private float slowDuration;
    private float slowMultiplier;

    private bool pierceEnemies;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Destroy(gameObject, 2f);
    }
    public void Initialize(
        Vector2 direction,
        float speed,
        float damage,
        bool canSlow,
        float slowChance,
        float slowDuration,
        bool pierceEnemies,
        float slowMultiplier = 0.5f
    )
    {
        this.damage = damage;
        this.canSlow = canSlow;
        this.slowChance = slowChance;
        this.slowDuration = slowDuration;
        this.pierceEnemies = pierceEnemies;
        this.slowMultiplier = slowMultiplier;

        rb.linearVelocity =
            direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy =
            collision.GetComponent<EnemyHealth>();

        if (enemy == null)
            return;

        Vector2 knockbackDirection =
            (enemy.transform.position -
             transform.position).normalized;

        // Las armas a distancia también pueden empujar
        enemy.TakeDamage(
            Mathf.RoundToInt(damage),
            knockbackDirection,
            0f
        );

        // Ralentización de las ligas
        if (canSlow)
        {
            if (Random.value <= slowChance)
            {
               /* enemy.ApplySlow(
                    slowDuration,
                    slowMultiplier
                );*/
            }
        }

        // Cerbatana atraviesa enemigos
        if (!pierceEnemies)
        {
            Destroy(gameObject);
        }
    }
}