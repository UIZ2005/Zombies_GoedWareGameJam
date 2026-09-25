using System.Collections;
using System.Collections.Generic;
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
    public int Nweapon;

    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Dictionary<WeaponType,GameObject> armas = new Dictionary<WeaponType,GameObject>();
    [SerializeField] private GameObject[] armasGO;

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

    private void Awake()
    {
        armas.Add(WeaponType.Regla, armasGO[0]);
        armas.Add(WeaponType.Tijeras, armasGO[1]);
        armas.Add(WeaponType.Compas, armasGO[2]);
    }
    public void OnInteract(InputValue value)
    {

        if (value.isPressed)
        {
            TryAttack();
        }
    }
    // =========================================
    // SELECCIÓN DE ARMAS
    // =========================================

    public void OnCrouch()
    {
        UpdateWeaponSelection(Nweapon);
    }
    private void UpdateWeaponSelection(int n)
    {
        if (n == 0) return;

        if (n==1)
        {
            ChangeWeapon(WeaponType.Regla);
        }

        if (n == 2)
        {
            ChangeWeapon(WeaponType.Tijeras);
        }

        if (n == 3)
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
            // 2. Calcular el ángulo en grados basado en la dirección
            float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

            // 3. Aplicar la rotación en el eje Z (para 2D)
            attackPoint.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    // =========================================
    // ATAQUE GENERAL
    // =========================================

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        
        switch (currentWeapon)
        {
            case WeaponType.Regla:
                nextAttackTime = Time.time + reglaCooldown;
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
        StartCoroutine(animacionArma(nextAttackTime));
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

    IEnumerator animacionArma(float timeAction)
    {
        armas[currentWeapon].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        armas[currentWeapon].SetActive(false);
        yield return null;
    }
}