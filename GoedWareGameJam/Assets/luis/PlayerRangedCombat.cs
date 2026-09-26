using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRangedCombat : MonoBehaviour
{
    public enum RangedWeapon
    {
        Cerbatana,
        PistolaLigas,
        Libros
    }

    [Header("Arma actual")]
    [SerializeField] private RangedWeapon currentWeapon = RangedWeapon.Cerbatana;
    [SerializeField] private GameObject[] currentWeapon_GO;
    public int Nweapon;


    [Header("Direcci�n")]
    [SerializeField] private Transform firePoint;

    private Vector2 facingDirection = Vector2.down;

    private float nextFireTime = 0f;


    [Header("Cerbatana de Bol�grafo")]
    [SerializeField] private GameObject penProjectile;
    [SerializeField] private float penDamage = 10f;
    [SerializeField] private float penSpeed = 25f;
    [SerializeField] private float penCooldown = 1.2f;

    [Header("Pistola de Ligas")]
    [SerializeField] private GameObject rubberProjectile;
    [SerializeField] private float rubberDamage = 10f;
    [SerializeField] private float rubberSpeed = 12f;
    [SerializeField] private float rubberCooldown = 0.08f;
    [SerializeField, Range(0f, 1f)] private float slowChance = 0.3f;
    [SerializeField] private float slowDuration = 1f;
    [SerializeField] private float slowMultiplier = 0.5f;

    [Header("Lanzamiento de Libros")]
    [SerializeField] private GameObject[] bookProjectiles;
    [SerializeField] private float bookDamage = 100f;
    [SerializeField] private float bookSpeed = 8f;
    [SerializeField] private float bookCooldown = 0.8f;
    [SerializeField] private float bookSpreadAngle = 25f;
    [SerializeField] private float bookLifetime = 0.5f;

  

    private void Update()
    {
        UpdateFacingDirection();

    }
    public void OnCrouch()
    {
        updateWeapon(Nweapon);
    }
    public void updateWeapon(int n)
    {
        if (n == 0) return;

        if (n == 1)
        {
            ChangeWeapon(RangedWeapon.Cerbatana);
            currentWeapon_GO[0].SetActive(true);
            currentWeapon_GO[1].SetActive(false);
            currentWeapon_GO[2].SetActive(false);

        }
                

        if (n == 2)
        {
            ChangeWeapon(RangedWeapon.PistolaLigas);
            currentWeapon_GO[2].SetActive(true);
            currentWeapon_GO[1].SetActive(false);
            currentWeapon_GO[0].SetActive(false);
        }
                

        if (n == 3)
        {
            ChangeWeapon(RangedWeapon.Libros);
            currentWeapon_GO[1].SetActive(true);
            currentWeapon_GO[0].SetActive(false);
            currentWeapon_GO[2].SetActive(false);
        }  
    }

    public void OnFire(InputValue value)
    {
       if (Time.time >= nextFireTime)
        {
            Shoot();
        }
    }
    private void UpdateFacingDirection()
    {
        // Obtener posici�n del mouse en pantalla
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        // Convertir la posici�n del mouse a coordenadas del mundo
        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        mouseWorldPosition.z = 0f;

        // Direcci�n desde el jugador hacia el mouse
        facingDirection =
            ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;

        // Mover el FirePoint hacia donde apunta el mouse
        if (firePoint != null)
        {
            firePoint.localPosition =
                facingDirection * 0.6f;

            // Rotar el FirePoint hacia el mouse
            float angle =
                Mathf.Atan2(
                    facingDirection.y,
                    facingDirection.x
                ) * Mathf.Rad2Deg;

            firePoint.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }
    }


    // =========================================================
    // DISPARO
    // =========================================================

    private void Shoot()
    {
        switch (currentWeapon)
        {
            case RangedWeapon.Cerbatana:
                ShootPen();
                break;

            case RangedWeapon.PistolaLigas:
                ShootRubber();
                break;

            case RangedWeapon.Libros:
                ShootBooks();
                break;
        }
    }


    // =========================================================
    // CERBATANA
    // =========================================================

    private void ShootPen()
    {
        if (penProjectile == null)
            return;

        GameObject projectile = Instantiate(
            penProjectile,
            firePoint.position,
            firePoint.rotation
        );

        RangedProjectile projectileScript =
            projectile.GetComponent<RangedProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(
                facingDirection,
                penSpeed,
                penDamage,
                false,
                0f,
                0f,
                true
            );
        }

        nextFireTime = Time.time + penCooldown;
    }


    // =========================================================
    // PISTOLA DE LIGAS
    // =========================================================

    private void ShootRubber()
    {
        if (rubberProjectile == null)
            return;

        GameObject projectile = Instantiate(
            rubberProjectile,
            firePoint.position,
            firePoint.rotation
        );

        RangedProjectile projectileScript =
            projectile.GetComponent<RangedProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(
                facingDirection,
                rubberSpeed,
                rubberDamage,
                true,
                slowChance,
                slowDuration,
                false,
                slowMultiplier
            );
        }

        nextFireTime = Time.time + rubberCooldown;
    }


    // =========================================================
    // LIBROS
    // =========================================================

    private void ShootBooks()
    {
        if (bookProjectiles == null)
            return;

        // Libro central
        CreateBookProjectile(bookProjectiles[1], facingDirection);

        // Libro superior
        Vector2 upperDirection =
            RotateVector(
                facingDirection,
                bookSpreadAngle
            );

        CreateBookProjectile(bookProjectiles[0], upperDirection);

        // Libro inferior
        Vector2 lowerDirection =
            RotateVector(
                facingDirection,
                -bookSpreadAngle
            );

        CreateBookProjectile(bookProjectiles[2], lowerDirection);

        nextFireTime = Time.time + bookCooldown;
    }


    private void CreateBookProjectile(GameObject bookProjectile, Vector2 direction)
    {
        GameObject projectile = Instantiate(
            bookProjectile,
            firePoint.position,
            Quaternion.identity
        );

        RangedProjectile projectileScript =
            projectile.GetComponent<RangedProjectile>();

        if (projectileScript != null)
        {
            projectileScript.Initialize(
                direction,
                bookSpeed,
                bookDamage,
                false,
                0f,
                bookLifetime,
                false
            );
        }

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        projectile.transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }


    private Vector2 RotateVector(
        Vector2 vector,
        float angle
    )
    {
        float radians = angle * Mathf.Deg2Rad;

        float x =
            vector.x * Mathf.Cos(radians) -
            vector.y * Mathf.Sin(radians);

        float y =
            vector.x * Mathf.Sin(radians) +
            vector.y * Mathf.Cos(radians);

        return new Vector2(x, y).normalized;
    }


    // =========================================================
    // CAMBIAR ARMA
    // =========================================================

    private void ChangeWeapon(RangedWeapon newWeapon)
    {
        currentWeapon = newWeapon;

        Debug.Log(
            "Arma a distancia: " + currentWeapon
        );
    }
}







