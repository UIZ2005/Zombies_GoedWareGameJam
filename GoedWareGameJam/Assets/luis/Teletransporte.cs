using System.Collections;
using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    public GameObject salida;
    [Header("Objeto a activar tras teletransportar")]
    public GameObject objetoAActivar; // <- NUEVO: Objeto que se activará al teletransportar
    
    public float tiempoTransicion = 1f;
    private GameObject player1;
    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player1 = FindAnyObjectByType<MovePlayer>().gameObject;
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(teleport(collision.gameObject));
        }
    }

    public void teletransporte()
    {
        StartCoroutine(teleport(player1));
    }

    IEnumerator teleport(GameObject player)
    {
        player.GetComponent<MovePlayer>().ActivarQuieto();
        //anim.SetBool("enter", true);
        yield return new WaitForSecondsRealtime(0.5f);
        
        // 1. Teletransportar al jugador
        player.transform.position = salida.transform.position;

        // 2. Activar el GameObject deseado justo después del teletransporte
        if (objetoAActivar != null)
        {
            objetoAActivar.SetActive(true);
        }

        yield return new WaitForSecondsRealtime(tiempoTransicion);
        player.GetComponent<MovePlayer>().DesactivarQuieto();
        //anim.SetBool("enter", false);

        yield return null;
    }
}