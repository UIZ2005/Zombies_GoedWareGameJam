using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyType
    {
        public string enemyName;
        public GameObject prefab;

        [Min(0)]
        public float spawnWeight = 1f;
    }

    [Header("Jugador y cámara")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;

    [Header("Tipos de enemigos")]
    [SerializeField] private EnemyType[] enemyTypes = new EnemyType[3];

    [Header("Configuración del Spawn")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float extraDistance = 2f;
    [SerializeField] private int maxEnemies = 20;

    [Header("Depuración")]
    [SerializeField] private bool showSpawnGizmo = true;

    private float spawnTimer;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (player == null || playerCamera == null)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;

            if (GetEnemyCount() < maxEnemies)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject selectedPrefab = SelectEnemy();

        if (selectedPrefab == null)
            return;

        Vector2 spawnPosition = GetSpawnPosition();

        Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private GameObject SelectEnemy()
    {
        float totalWeight = 0f;

        // Sumar todos los pesos
        foreach (EnemyType enemy in enemyTypes)
        {
            if (enemy.prefab != null)
                totalWeight += enemy.spawnWeight;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogWarning(
                "No hay pesos de spawn válidos."
            );

            return null;
        }

        // Elegir un valor aleatorio
        float randomValue = Random.Range(0f, totalWeight);

        foreach (EnemyType enemy in enemyTypes)
        {
            if (enemy.prefab == null)
                continue;

            randomValue -= enemy.spawnWeight;

            if (randomValue <= 0f)
                return enemy.prefab;
        }

        return null;
    }

    private Vector2 GetSpawnPosition()
    {
        // Mitad del alto y ancho visibles de la cámara
        float cameraHeight = playerCamera.orthographicSize;
        float cameraWidth =
            cameraHeight * playerCamera.aspect;

        // Distancia hasta las esquinas de la cámara
        float cameraRadius = Mathf.Sqrt(
            cameraWidth * cameraWidth +
            cameraHeight * cameraHeight
        );

        // Radio de spawn fuera de la cámara
        float spawnRadius = cameraRadius + extraDistance;

        // Ángulo aleatorio
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);

        Vector2 direction = new Vector2(
            Mathf.Cos(randomAngle),
            Mathf.Sin(randomAngle)
        );

        return (Vector2)player.position +
               direction * spawnRadius;
    }

    private int GetEnemyCount()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        return enemies.Length;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showSpawnGizmo || player == null || playerCamera == null)
            return;

        float cameraHeight = playerCamera.orthographicSize;
        float cameraWidth =
            cameraHeight * playerCamera.aspect;

        float cameraRadius = Mathf.Sqrt(
            cameraWidth * cameraWidth +
            cameraHeight * cameraHeight
        );

        float spawnRadius = cameraRadius + extraDistance;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            player.position,
            spawnRadius
        );
    }
}