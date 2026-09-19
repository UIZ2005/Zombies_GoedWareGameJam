using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerExplosives : MonoBehaviour
{
    public enum ExplosiveType
    {
        Botella,
        Mochila,
        Sopa
    }

    [Header("Explosive Selection")]
    [SerializeField] private ExplosiveType currentExplosive = ExplosiveType.Botella;

    [Header("References")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject botellaPrefab;

    [Header("Botella de Gaseosa")]
    [SerializeField] private float botellaCooldown = 5f;

    [Header("Mochila Bomba")]
    [SerializeField] private float mochilaCooldown = 3f;

    [Header("Sopa Hirviendo")]
    [SerializeField] private float sopaCooldown = 6f;

    private float nextThrowTime;

    // =========================================
    // NEW INPUT SYSTEM: ACCIONES
    // =========================================

    public void OnSelectExplosive1(InputValue value)
    {
        if (value.isPressed) ChangeExplosive(ExplosiveType.Botella);
    }

    public void OnSelectExplosive2(InputValue value)
    {
        if (value.isPressed) ChangeExplosive(ExplosiveType.Mochila);
    }

    public void OnSelectExplosive3(InputValue value)
    {
        if (value.isPressed) ChangeExplosive(ExplosiveType.Sopa);
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed) TryThrow();
    }

    private void ChangeExplosive(ExplosiveType newExplosive)
    {
        currentExplosive = newExplosive;
        Debug.Log("Explosivo seleccionado: " + currentExplosive);
    }

    // =========================================
    // LANZAMIENTO GENERAL
    // =========================================

    private void TryThrow()
    {
        if (Time.time < nextThrowTime) return;

        switch (currentExplosive)
        {
            case ExplosiveType.Botella:
                nextThrowTime = Time.time + botellaCooldown;
                ThrowBotella();
                break;
            case ExplosiveType.Mochila:
                nextThrowTime = Time.time + mochilaCooldown;
                Debug.Log("Lanzando Mochila...");
                break;
            case ExplosiveType.Sopa:
                nextThrowTime = Time.time + sopaCooldown;
                Debug.Log("Lanzando Sopa...");
                break;
        }
    }

    // =========================================
    // BOTELLA DE GASEOSA
    // =========================================

    private void ThrowBotella()
    {
        if (botellaPrefab == null || throwPoint == null) return;

        // Leer la posición exacta del ratón en pantalla y convertirla al mundo 2D
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f; // Asegurar que esté en el plano 2D

        // Instanciar y lanzar
        GameObject botellaInstance = Instantiate(botellaPrefab, throwPoint.position, Quaternion.identity);
        BotellaGaseosa botellaScript = botellaInstance.GetComponent<BotellaGaseosa>();
        
        if (botellaScript != null)
        {
            botellaScript.InicializarLanzamiento(mouseWorldPosition);
        }
    }
}