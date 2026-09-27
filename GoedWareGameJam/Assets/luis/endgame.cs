using UnityEngine;
using UnityEngine.SceneManagement;

public class endgame : MonoBehaviour
{
    public string nombreEscena;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

            SceneManager.LoadScene(nombreEscena);
    }
}
