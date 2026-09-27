using System.Collections;
using UnityEngine;

public class Teletransporte : MonoBehaviour
{
    public GameObject salida;
    //public Animator anim;
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
        audioManager.seleccionAudio(4);
        player.GetComponent<MovePlayer>().ActivarQuieto();
        //anim.SetBool("enter", true);
        yield return new WaitForSecondsRealtime(0.5f);
        player.transform.position = salida.transform.position;

        yield return new WaitForSecondsRealtime(tiempoTransicion);
        player.GetComponent<MovePlayer>().DesactivarQuieto();
        //anim.SetBool("enter", false);


        yield return null;
    }
}
