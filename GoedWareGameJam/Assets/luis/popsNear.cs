using Unity.VisualScripting;
using UnityEngine;

public class popsNear : MonoBehaviour
{
    public GameObject canva;
    public int numeroDelarma;
    
    [Header("¿Es un explosivo?")]
    public bool esExplosivo;
    [Header("¿Es un Melee?")]
    public bool esMelee=false;

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
            
            // Si es explosivo le pasamos el número a PlayerExplosives
            if (esExplosivo) 
            {
                player.GetComponent<PlayerExplosives>().NExplosive = numeroDelarma;
            }
            // Si no, se lo pasamos a PlayerCombat como siempre
            else if(esMelee)
            {
                player.GetComponent<PlayerCombat>().Nweapon = numeroDelarma;
            }
            else
            {
                player.GetComponent<PlayerRangedCombat>().Nweapon = numeroDelarma;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canva.SetActive(false);
            
            if (esExplosivo)
            {
                player.GetComponent<PlayerExplosives>().NExplosive = 0;
            }
            else
            {
                player.GetComponent<PlayerCombat>().Nweapon = 0;
            }
        }
    }
}