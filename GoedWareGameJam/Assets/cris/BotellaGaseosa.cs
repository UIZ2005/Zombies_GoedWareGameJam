using UnityEngine;
using System.Collections;

public class BotellaGaseosa : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    [SerializeField] private float velocidadLanzamiento = 15f;

    [Header("Configuración de Daño y Nube")]
    [SerializeField] private int dañoPorTick = 10;
    [SerializeField] private float knockback =1f; 
    [SerializeField] private float tiempoVidaNube = 6f; 
    [SerializeField] private float tiempoFade = 2f;
    [SerializeField] private float tiempoEntreDaños = 0.5f;
    
    [Header("Detección de Enemigos")]
    [SerializeField] private LayerMask enemyLayer;

    private Vector3 posicionObjetivo;
    private bool estaEnVuelo = true;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D areaColision;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        areaColision = GetComponent<CircleCollider2D>();
        
        areaColision.enabled = false;
        areaColision.isTrigger = true;
    }

    public void InicializarLanzamiento(Vector3 objetivo)
    {
        posicionObjetivo = objetivo;
        estaEnVuelo = true;
    }

    private void Update()
    {
        if (estaEnVuelo)
        {
            // Mover hacia el mouse
            transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidadLanzamiento * Time.deltaTime);

            // Al llegar al objetivo, detonar
            if (Vector3.Distance(transform.position, posicionObjetivo) <= 0.1f)
            {
                Detonar();
            }
        }
    }

    private void Detonar()
    {
        estaEnVuelo = false;
        anim.SetTrigger("Explotar"); 
        
        // Iniciamos el ciclo de vida de la nube y el daño continuo
        StartCoroutine(RutinaDañoContinuo());
        StartCoroutine(RutinaNubeToxica());
    }

    // NUEVO: Corrutina que aplica daño de área por pulsos (sin depender de OnTriggerStay2D)
    private IEnumerator RutinaDañoContinuo()
    {
        float tiempoSolido = tiempoVidaNube - tiempoFade;
        float tiempoTranscurrido = 0f;

        // Mientras la nube no empiece a desaparecer
        while (tiempoTranscurrido < tiempoSolido)
        {
            // Escanea a todos los enemigos dentro del radio del CircleCollider2D
            Collider2D[] enemigosEnArea = Physics2D.OverlapCircleAll(transform.position, areaColision.radius, enemyLayer);

            foreach (Collider2D enemigo in enemigosEnArea)
            {
                EnemyHealth enemyHealth = enemigo.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Vector2 knockbackDir = (enemigo.transform.position - transform.position).normalized;
                    enemyHealth.TakeDamage(dañoPorTick, knockbackDir, knockback);
                }
            }

            // Esperar el tiempo indicado antes de hacer el siguiente daño
            yield return new WaitForSeconds(tiempoEntreDaños);
            tiempoTranscurrido += tiempoEntreDaños;
        }
    }

    private IEnumerator RutinaNubeToxica()
    {
        float tiempoSolido = tiempoVidaNube - tiempoFade;
        yield return new WaitForSeconds(tiempoSolido);

        float tiempoTranscurrido = 0f;
        Color colorOriginal = spriteRenderer.color;

        while (tiempoTranscurrido < tiempoFade)
        {
            tiempoTranscurrido += Time.deltaTime;
            float nuevoAlpha = Mathf.Lerp(1f, 0f, tiempoTranscurrido / tiempoFade);
            spriteRenderer.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, nuevoAlpha);
            yield return null;
        }

        Destroy(gameObject);
    }
    
    // Para visualizar el área de daño en la escena (opcional)
    private void OnDrawGizmosSelected()
    {
        if (areaColision != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, areaColision.radius);
        }
    }
}