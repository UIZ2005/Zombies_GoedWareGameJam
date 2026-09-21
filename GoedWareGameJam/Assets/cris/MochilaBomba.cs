using UnityEngine;
using System.Collections;

public class MochilaBomba : MonoBehaviour
{
    [Header("Configuración de Explosión")]
    [SerializeField] private int dañoExplosion = 50; // Daño único y masivo
    [SerializeField] private float knockback = 12f; // Gran empuje por la onda expansiva
    [SerializeField] private float tiempoDetonacion = 2f; // Cuenta regresiva
    [SerializeField] private float tiempoDestruccion = 0.5f; // Cuánto dura el clip de la explosión antes de borrarse
    [SerializeField] private float multiplicadorEscala = 0.2f; // Crecimiento visual de la explosión
    
    [Header("Detección de Enemigos")]
    [SerializeField] private LayerMask enemyLayer; 

    private Animator anim;
    private CircleCollider2D areaColision;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        areaColision = GetComponent<CircleCollider2D>();
        
        // No necesita trigger, solo se usa para calcular el radio del Gizmo y el OverlapCircle
        areaColision.enabled = false; 
        
        // Empezamos la mecha inmediatamente al soltarla
        StartCoroutine(RutinaMechaYExplosion());
    }

    private IEnumerator RutinaMechaYExplosion()
    {
        // 1. Esperamos los 2 segundos (Aquí se reproduce tu primera animación de "Mochila_Tick")
        yield return new WaitForSeconds(tiempoDetonacion);

        // 2. Disparamos la animación de explosión (Asegúrate de tener un Trigger "Explotar" en tu Animator)
        anim.SetTrigger("Explotar");
        
        // 3. Aumentamos la escala para la explosión visual
        transform.localScale = transform.localScale * multiplicadorEscala;

        // 4. Aplicamos el daño masivo de una sola vez
        AplicarDañoMasivo();

        // 5. Esperamos a que termine de dibujarse la animación de explosión y destruimos el objeto
        yield return new WaitForSeconds(tiempoDestruccion);
        Destroy(gameObject);
    }

    private void AplicarDañoMasivo()
    {
        // Detectamos a todos los enemigos en el área aumentada
        Collider2D[] enemigosEnArea = Physics2D.OverlapCircleAll(transform.position, areaColision.radius * transform.localScale.x, enemyLayer);

        foreach (Collider2D enemigo in enemigosEnArea)
        {
            EnemyHealth enemyHealth = enemigo.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Empujamos a los enemigos desde el centro de la mochila hacia afuera
                Vector2 knockbackDir = (enemigo.transform.position - transform.position).normalized;
                enemyHealth.TakeDamage(dañoExplosion, knockbackDir, knockback);
            }
        }
    }
    
    // Gizmo amarillo para diferenciarlo visualmente en el editor
    private void OnDrawGizmosSelected()
    {
        if (areaColision != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, areaColision.radius * transform.localScale.x);
        }
    }
}