using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de la escena a la que quieres cambiar.")]
    [SerializeField] private string nombreDeEscena;

    // Método para cambiar de escena usando el nombre (Ideal para botones o eventos)
    public void CambiarAEscenaPorNombre()
    {
        if (!string.IsNullOrEmpty(nombreDeEscena))
        {
            SceneManager.LoadScene(nombreDeEscena);
        }
        else
        {
            Debug.LogWarning("No se ha especificado el nombre de la escena.", this);
        }
    }

    // Método alternativo por si prefieres usar el índice numérico de la escena (Build Settings)
    public void CambiarAEscenaPorIndice(int indiceEscena)
    {
        SceneManager.LoadScene(indiceEscena);
    }

    // Opcional: Cambiar de escena automáticamente al entrar en un Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CambiarAEscenaPorNombre();
        }
    }
}