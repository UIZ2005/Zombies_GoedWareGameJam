using UnityEngine;

public class SonidoBoton : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonSound;

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonSound);
    }
}