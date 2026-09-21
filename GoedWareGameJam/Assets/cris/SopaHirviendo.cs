using UnityEngine;
using System.Collections;

public class SopaHirviendo : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    [SerializeField] private float velocidadLanzamiento = 12f; // Quizás la olla vuela un poco más lento por ser pesada

    [Header("Configuración de Daño y Fuego")]
    [SerializeField] private int dañoPorTick = 10;
    [SerializeField] private float knockback = 0f; // El fuego no empuja, solo quema
    [SerializeField] private float tiempoVidaFuego = 5f; 
    [SerializeField] private float tiempoFade = 1.5f;
    [SerializeField] private float tiempoEntreDaños = 0.5f;
    [SerializeField] private float multiplicadorEscala = 4f; // Ajusta qué tanto crece el charco de fuego
    
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
            transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, velocidadLanzamiento * Time.deltaTime);

            if (Vector3.Distance(transform.position, posicionObjetivo) <= 0.1f)
            {
                Detonar();
            }
        }
    }

    private void Detonar()
    {
        estaEnVuelo = false;
        
        // Usamos el mismo trigger "Explotar" para mantener la consistencia en el Animator
        anim.SetTrigger("Explotar"); 
        
        // Crecemos la escala para el charco de fuego
        transform.localScale = transform.localScale * multiplicadorEscala;
        
        StartCoroutine(RutinaDañoContinuo());
        StartCoroutine(RutinaFuego());
    }

    private IEnumerator RutinaDañoContinuo()
    {
        float tiempoSolido = tiempoVidaFuego - tiempoFade;
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < tiempoSolido)
        {
            Collider2D[] enemigosEnArea = Physics2D.OverlapCircleAll(transform.position, areaColision.radius * transform.localScale.x, enemyLayer);

            foreach (Collider2D enemigo in enemigosEnArea)
            {
                EnemyHealth enemyHealth = enemigo.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    Vector2 knockbackDir = (enemigo.transform.position - transform.position).normalized;
                    enemyHealth.TakeDamage(dañoPorTick, knockbackDir, knockback);
                }
            }

            yield return new WaitForSeconds(tiempoEntreDaños);
            tiempoTranscurrido += tiempoEntreDaños;
        }
    }

    private IEnumerator RutinaFuego()
    {
        float tiempoSolido = tiempoVidaFuego - tiempoFade;
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
    
    private void OnDrawGizmosSelected()
    {
        if (areaColision != null)
        {
            Gizmos.color = Color.red; // Fuego en rojo para diferenciarlo
            Gizmos.DrawWireSphere(transform.position, areaColision.radius * transform.localScale.x);
        }
    }
}