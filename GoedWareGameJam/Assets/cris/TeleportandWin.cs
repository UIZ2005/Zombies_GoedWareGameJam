using UnityEngine;

public class TeleportandWin : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    [Tooltip("El objeto o punto al que será teletransportado el jugador.")]
    [SerializeField] private Transform teleportDestination;

    [Header("Interfaz (UI)")]
    [Tooltip("El Canvas que se activará al ganar.")]
    [SerializeField] private GameObject winCanvas;

    [Header("Audio de Fondo")]
    [Tooltip("El GameObject o componente AudioSource del audio de fondo que se desactivará.")]
    [SerializeField] private GameObject audioFondoObjeto;
    [Tooltip("(Opcional) Si prefieres hacer un fade out suave al ganar.")]
    [SerializeField] private AudioSource audioFondoSource;
    [Tooltip("Tiempo que tardará en apagarse la música de fondo suavemente (en segundos).")]
    [SerializeField] private float fadeOutTime = 1f;

    [Header("Audio de Victoria")]
    [Tooltip("El componente AudioSource que reproducirá el sonido.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("El clip de sonido de ganaste.")]
    [SerializeField] private AudioClip winSound;

    [Header("Etiqueta del Jugador")]
    [Tooltip("Tag para identificar que es el jugador.")]
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que entró tiene la etiqueta del jugador
        if (collision.CompareTag(playerTag))
        {
            // 1. Teletransportar al jugador
            if (teleportDestination != null)
            {
                collision.transform.position = teleportDestination.position;
            }
            else
            {
                Debug.LogWarning("No se ha asignado un destino de teletransporte.", this);
            }

            // 2. Activar el Canvas de victoria
            if (winCanvas != null)
            {
                winCanvas.SetActive(true);
            }

            // 3. Desactivar o apagar suavemente el audio de fondo
            if (audioFondoSource != null)
            {
                // Si asignaste el AudioSource, hacemos un desvanecimiento (Fade Out) suave
                StartCoroutine(FadeOutAudio(audioFondoSource, fadeOutTime));
            }
            else if (audioFondoObjeto != null)
            {
                // Si solo asignaste el GameObject, lo desactivamos directamente
                audioFondoObjeto.SetActive(false);
            }

            // 4. Reproducir el sonido de ganaste
            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
        }
    }

    // Corrutina para bajar el volumen poco a poco antes de apagarlo
    private System.Collections.IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Restauramos el volumen por si se reinicia la escena luego
        if (audioFondoObjeto != null)
        {
            audioFondoObjeto.SetActive(false);
        }
    }
}