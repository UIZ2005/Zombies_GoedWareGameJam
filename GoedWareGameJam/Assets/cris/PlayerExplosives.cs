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
    [SerializeField] private GameObject mochilaPrefab; // NUEVO: Referencia a la mochila
    [SerializeField] private GameObject sopaPrefab;

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
                ThrowMochila(); // NUEVO: Llamamos a la función de la mochila
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
        
        // A diferencia de la botella, la mochila solo se suelta en la posición actual del jugador
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