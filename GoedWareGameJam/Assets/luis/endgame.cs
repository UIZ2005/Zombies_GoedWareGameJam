using UnityEngine;
using UnityEngine.SceneManagement;

public class endgame : MonoBehaviour
{
    public string nombreEscena;
    public bool isend = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(isend)
            SceneManager.LoadScene(nombreEscena);
    }

    public void cambiarescena(string Escena)
    {
        SceneManager.LoadScene(Escena);
    }
}
