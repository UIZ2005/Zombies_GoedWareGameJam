using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VidaJugador : MonoBehaviour
{
    public int vidasMaximas = 3;
    private int vidasActuales;

    public AudioSource audioSource;
    public AudioClip popCorazon;

    public Image[] corazones;

    // Hijos del niño en orden: Normal, MedioZombie, Zombie
    public GameObject[] aspectos;
    private int aspectoActual = 0;

    public float duracionParpadeo = 0.8f;
    public float intervaloParpadeo = 0.1f;

    public float tiempoInvencible = 1f;
    private float contadorInvencible = 0f;

    void Start()
    {
        vidasActuales = vidasMaximas;
        ActualizarCorazones();

        // Solo el primer aspecto empieza activo
        for (int i = 0; i < aspectos.Length; i++)
            aspectos[i].SetActive(i == 0);
    }

    void Update()
    {
        if (contadorInvencible > 0)
            contadorInvencible -= Time.deltaTime;
    }

    public void PerderVida()
    {
        if (contadorInvencible > 0) return;

        vidasActuales--;
        contadorInvencible = tiempoInvencible;
        ActualizarCorazones();
        audioSource.PlayOneShot(popCorazon);

        if (vidasActuales <= 0)
        {
            Morir();
            return;
        }

        // 2 vidas -> aspecto 1, 1 vida -> aspecto 2
        StartCoroutine(CambiarAspecto(vidasMaximas - vidasActuales));
    }

    IEnumerator CambiarAspecto(int nuevo)
    {
        GameObject viejo = aspectos[aspectoActual];
        GameObject siguiente = aspectos[nuevo];

        SpriteRenderer srViejo = viejo.GetComponent<SpriteRenderer>();
        SpriteRenderer srNuevo = siguiente.GetComponent<SpriteRenderer>();

        siguiente.SetActive(true);

        // Parpadeo: alterna entre el sprite viejo y el nuevo
        float tiempo = 0f;
        bool mostrarNuevo = false;
        while (tiempo < duracionParpadeo)
        {
            mostrarNuevo = !mostrarNuevo;
            srViejo.enabled = !mostrarNuevo;
            srNuevo.enabled = mostrarNuevo;

            yield return new WaitForSeconds(intervaloParpadeo);
            tiempo += intervaloParpadeo;
        }

        // Queda solo el nuevo
        srNuevo.enabled = true;
        srViejo.enabled = true;
        viejo.SetActive(false);
        aspectoActual = nuevo;
    }

    void ActualizarCorazones()
    {
        for (int i = 0; i < corazones.Length; i++)
            corazones[i].gameObject.SetActive(i < vidasActuales);
    }

    void Morir()
    {
        Debug.Log("murio");
        // aqui se deberia activar la animacion de morir, de convertirse en zombie, cuando la tenga finalizada
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
            PerderVida();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
            PerderVida();
    }
}