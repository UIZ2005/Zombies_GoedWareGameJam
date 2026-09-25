using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerExplosives : MonoBehaviour
{
    public enum ExplosiveType
    {
        Ninguno, // Estado inicial por si no tiene armas explosivas
        Botella,
        Mochila,
        Sopa
    }

    [Header("Explosive Selection")]
    [SerializeField] private ExplosiveType currentExplosive = ExplosiveType.Ninguno;
    
    // Variable que recibe el número desde el script popsNear
    public int NExplosive; 

    [Header("References")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject botellaPrefab;
    [SerializeField] private GameObject mochilaPrefab;
    [SerializeField] private GameObject sopaPrefab;

    [Header("Botella de Gaseosa")]
    [SerializeField] private float botellaCooldown = 5f;

    [Header("Mochila Bomba")]
    [SerializeField] private float mochilaCooldown = 3f;

    [Header("Sopa Hirviendo")]
    [SerializeField] private float sopaCooldown = 6f;

    private float nextThrowTime;

    // =========================================
    // RECOGER ARMA (Conectado a la tecla 'C' o tu botón de Crouch)
    // =========================================
    public void OnCrouch()
    {
        UpdateExplosiveSelection(NExplosive);
    }

    private void UpdateExplosiveSelection(int n)
    {
        if (n == 0) return; // Si no hay nada cerca, no hace nada

        if (n == 1) ChangeExplosive(ExplosiveType.Botella);
        if (n == 2) ChangeExplosive(ExplosiveType.Mochila);
        if (n == 3) ChangeExplosive(ExplosiveType.Sopa);
    }

    private void ChangeExplosive(ExplosiveType newExplosive)
    {
        currentExplosive = newExplosive;
        Debug.Log("Explosivo recogido y equipado: " + currentExplosive);
    }

    // =========================================
    // ATACAR (Conectado a la tecla 'R')
    // =========================================
    public void OnExplosiveAttack(InputValue value)
    {
        if (value.isPressed) TryThrow();
    }

    // =========================================
    // LANZAMIENTO GENERAL
    // =========================================
    private void TryThrow()
    {
        // Si no tiene ningún explosivo recogido, no puede lanzar
        if (currentExplosive == ExplosiveType.Ninguno) return;

        if (Time.time < nextThrowTime) return;

        switch (currentExplosive)
        {
            case ExplosiveType.Botella:
                nextThrowTime = Time.time + botellaCooldown;
                ThrowBotella();
                break;
            case ExplosiveType.Mochila:
                nextThrowTime = Time.time + mochilaCooldown;
                ThrowMochila(); 
                break;
            case ExplosiveType.Sopa:
                nextThrowTime = Time.time + sopaCooldown;
                ThrowSopa();
                break;
        }
    }

    // =========================================
    // BOTELLA DE GASEOSA
    // =========================================
    private void ThrowBotella()
    {
        if (botellaPrefab == null || throwPoint == null) return;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;
        GameObject botellaInstance = Instantiate(botellaPrefab, throwPoint.position, Quaternion.identity);
        BotellaGaseosa botellaScript = botellaInstance.GetComponent<BotellaGaseosa>();
        if (botellaScript != null) botellaScript.InicializarLanzamiento(mouseWorldPosition);
    }

    // =========================================
    // MOCHILA BOMBA (MINA TERRESTRE)
    // =========================================
    private void ThrowMochila()
    {
        if (mochilaPrefab == null || throwPoint == null) return;
        Instantiate(mochilaPrefab, throwPoint.position, Quaternion.identity);
    }

    // =========================================
    // SOPA HIRVIENDO
    // =========================================
    private void ThrowSopa()
    {
        if (sopaPrefab == null || throwPoint == null) return;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;
        GameObject sopaInstance = Instantiate(sopaPrefab, throwPoint.position, Quaternion.identity);
        SopaHirviendo sopaScript = sopaInstance.GetComponent<SopaHirviendo>();
        if (sopaScript != null) sopaScript.InicializarLanzamiento(mouseWorldPosition);
    }
}