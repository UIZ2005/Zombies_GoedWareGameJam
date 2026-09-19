using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public enum WeaponType
    {
        Regla,
        Tijeras,
        Compas
    }

    [Header("Weapon Selection")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.Regla;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Player Direction")]
    [SerializeField] private float attackPointDistance = 0.7f;
    private Vector2 movement;

    [Header("Regla")]
    [SerializeField] private int reglaDamage = 20;
    [SerializeField] private float reglaCooldown = 1f;
    [SerializeField] private float reglaRange = 1.5f;
    [SerializeField] private float reglaArcAngle = 100f;
    [SerializeField] private float reglaKnockback = 8f;

    [Header("Tijeras")]
    [SerializeField] private int tijerasDamage = 35;
    [SerializeField] private float tijerasCooldown = 0.25f;
    [SerializeField] private float tijerasRange = 0.8f;
    [SerializeField] private float tijerasKnockback = 2f;

    [Header("Compas")]
    [SerializeField] private int compasDamage = 25;
    [SerializeField] private float compasCooldown = 1.4f;
    [SerializeField] private float compasRange = 2.5f;
    [SerializeField] private float compasKnockback = 4f;
    [SerializeField] private int compasMaxTargets = 3;

    [Header("Debug")]
    [SerializeField] private bool showAttackGizmos = true;

    private Vector2 facingDirection = Vector2.down;
    private float nextAttackTime;

    public void OnInteract(InputValue value)
    {
        Debug.Log("Se presionó Interact (E)");

        if (value.isPressed)
        {
            TryAttack();
        }
    }
    // =========================================
    // SELECCIÓN DE ARMAS
    // =========================================

    private void UpdateWeaponSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(WeaponType.Regla);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(WeaponType.Tijeras);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(WeaponType.Compas);
        }
    }

    private void ChangeWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;

        Debug.Log("Arma seleccionada: " + currentWeapon);
    }

    // =========================================
    // DIRECCIÓN DEL JUGADOR
    // =========================================

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    private void UpdateFacingDirection()
    {

        if (movement.sqrMagnitude > 0.01f)
        {
            facingDirection = movement.normalized;
        }

        if (attackPoint != null)
        {
            attackPoint.localPosition =
                facingDirection * attackPointDistance;
        }
    }

    // =========================================
    // ATAQUE GENERAL
    // =========================================

    private void TryAttack()
    {
        Debug.Log("Intenta atacar");
        if (Time.time < nextAttackTime)
            return;

        switch (currentWeapon)
        {
            case WeaponType.Regla:
                nextAttackTime = Time.time + reglaCooldown;
                Debug.Log("case regla");
                AttackRegla();
                break;

            case WeaponType.Tijeras:
                nextAttackTime = Time.time + tijerasCooldown;
                AttackTijeras();
                break;

            case WeaponType.Compas:
                nextAttackTime = Time.time + compasCooldown;
                AttackCompas();
                break;
        }
    }

    // =========================================
    // REGLA: ATAQUE EN ARCO
    // =========================================

    private void AttackRegla()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            reglaRange,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            Vector2 directionToEnemy =
                (enemy.transform.position - transform.position)
                .normalized;

            float angle = Vector2.Angle(
                facingDirection,
                directionToEnemy
            );

            if (angle > reglaArcAngle / 2f)
                continue;

            ApplyDamage(
                enemy,
                reglaDamage,
                reglaKnockback
            );
        }

        Debug.Log("Ataque con regla");
    }

    // =========================================
    // TIJERAS: ATAQUE CORTO
    // =========================================

    private void AttackTijeras()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            tijerasRange,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            ApplyDamage(
                enemy,
                tijerasDamage,
                tijerasKnockback
            );

            // Solo golpea al primer enemigo encontrado.
            break;
        }

        Debug.Log("Ataque con tijeras");
    }

    // =========================================
    // COMPÁS: ATAQUE EN LÍNEA RECTA
    // =========================================

    private void AttackCompas()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position,
            facingDirection,
            compasRange,
            enemyLayer
        );

        int targetsHit = 0;

        foreach (RaycastHit2D hit in hits)
        {
            if (targetsHit >= compasMaxTargets)
                break;

            ApplyDamage(
                hit.collider,
                compasDamage,
                compasKnockback
            );

            targetsHit++;
        }

        Debug.Log("Ataque con compás");
    }

    // =========================================
    // APLICAR DAÑO
    // =========================================

    private void ApplyDamage(
        Collider2D enemy,
        int damage,
        float knockback
    )
    {
        EnemyHealth enemyHealth =
            enemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
            return;

        Vector2 knockbackDirection =
            (enemy.transform.position - transform.position)
            .normalized;

        enemyHealth.TakeDamage(
            damage,
            knockbackDirection,
            knockback
        );
    }

    // =========================================
    // GIZMOS
    // =========================================

    private void OnDrawGizmosSelected()
    {
        if (!showAttackGizmos || attackPoint == null)
            return;

        Gizmos.color = Color.red;

        switch (currentWeapon)
        {
            case WeaponType.Regla:
                Gizmos.DrawWireSphere(
                    attackPoint.position,
                    reglaRange
                );
                break;

            case WeaponType.Tijeras:
                Gizmos.DrawWireSphere(
                    attackPoint.position,
                    tijerasRange
                );
                break;

            case WeaponType.Compas:
                Gizmos.DrawRay(
                    transform.position,
                    facingDirection * compasRange
                );
                break;
        }
    }
    private void Update()
    {
        UpdateFacingDirection();
    }
}