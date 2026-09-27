using UnityEngine;

public class ActivarOBJ : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] Zombies;
    private AudioManager AudioManager;
    public int npista;
    private bool press = false;
    void Start()
    {
        AudioManager = FindAnyObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !press)
        {
            foreach (GameObject z in Zombies)
            {
                if(z!=null)
                z.SetActive(true);
            }
            AudioManager.seleccionAudio(npista);
            press = true;
        }
    }
}
