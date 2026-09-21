using Unity.VisualScripting;
using UnityEngine;

public class popsNear : MonoBehaviour
{
    public GameObject canva;
    public int numeroDelarma;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canva.SetActive(true);
            player.GetComponent<PlayerCombat>().Nweapon = numeroDelarma;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canva.SetActive(false);
            player.GetComponent<PlayerCombat>().Nweapon = 0;
        }
    }
}
