using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Gestor mejorado de generación de enemigos.
/// Soporta múltiples tipos de enemigos y configuración flexible.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnConfig
    {
        [Tooltip("Prefab del enemigo a generar")]
        public GameObject enemyPrefab;
        
        [Tooltip("Tipo de enemigo (X=Patrol, Y=Chase)")]
        public char enemyType = 'X';
        
        [Tooltip("Peso de probabilidad (mayor = más probable)")]
        [Range(1, 100)]
        public int spawnWeight = 50;
    }

    [Header("Configuración de Spawn")]
    [SerializeField] private List<EnemySpawnConfig> enemyTypes = new List<EnemySpawnConfig>();
    
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private bool spawnOnStart = true;
    
    [Header("Fallback (si no hay prefabs)")]
    [SerializeField] private bool useCodeGeneratedEnemies = true;
    [SerializeField] private Sprite enemySprite;
    
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private int totalSpawnWeight = 0;

    void Start()
    {
        // Calcular peso total para probabilidades
        foreach (var config in enemyTypes)
        {
            totalSpawnWeight += config.spawnWeight;
        }
        
        if (spawnOnStart)
        {
            InvokeRepeating(nameof(SpawnEnemy), initialDelay, spawnInterval);
        }
    }

    void Update()
    {
        // Limpiar lista de enemigos muertos
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    /// <summary>
    /// Genera un enemigo en la posición del spawner
    /// </summary>
    public void SpawnEnemy()
    {
        // Verificar límite de enemigos
        if (spawnedEnemies.Count >= maxEnemies)
        {
            return;
        }
        
        GameObject enemy = null;
        
        // Intentar generar desde prefabs configurados
        if (enemyTypes.Count > 0 && totalSpawnWeight > 0)
        {
            enemy = SpawnFromConfig();
        }
        
        // Fallback: generar enemigo por código
        if (enemy == null && useCodeGeneratedEnemies)
        {
            enemy = CreateCodeGeneratedEnemy();
        }
        
        if (enemy != null)
        {
            spawnedEnemies.Add(enemy);
            Debug.Log($"EnemySpawner: Enemigo generado. Total activos: {spawnedEnemies.Count}");
        }
    }

    /// <summary>
    /// Genera un enemigo basado en la configuración con probabilidades
    /// </summary>
    private GameObject SpawnFromConfig()
    {
        if (enemyTypes.Count == 0) return null;
        
        // Seleccionar tipo de enemigo según peso
        int randomWeight = Random.Range(0, totalSpawnWeight);
        int currentWeight = 0;
        
        EnemySpawnConfig selectedConfig = null;
        foreach (var config in enemyTypes)
        {
            currentWeight += config.spawnWeight;
            if (randomWeight < currentWeight)
            {
                selectedConfig = config;
                break;
            }
        }
        
        if (selectedConfig == null || selectedConfig.enemyPrefab == null)
        {
            return null;
        }
        
        // Instanciar enemigo
        GameObject enemy = Instantiate(selectedConfig.enemyPrefab, transform.position, Quaternion.identity);
        
        // Configurar sorting order para que se vea correctamente
        var renderers = enemy.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var renderer in renderers)
        {
            renderer.sortingOrder = 1;
        }
        
        return enemy;
    }

    /// <summary>
    /// Crea un enemigo básico por código (fallback)
    /// </summary>
    private GameObject CreateCodeGeneratedEnemy()
    {
        // Decidir aleatoriamente qué tipo crear
        bool createPatrolEnemy = Random.value > 0.5f;
        
        GameObject enemy = new GameObject(createPatrolEnemy ? "PatrolEnemy" : "ChaseEnemy");
        enemy.transform.position = transform.position;
        
        // Añadir componentes básicos
        var rb = enemy.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        var collider = enemy.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(0.8f, 0.8f);
        
        // Añadir sprite renderer
        var spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        
        // Intentar cargar sprite
        if (enemySprite != null)
        {
            spriteRenderer.sprite = enemySprite;
        }
        else
        {
            // Buscar sprite en Resources
            var loadedSprite = Resources.Load<Sprite>("Images/op");
            if (loadedSprite != null)
            {
                spriteRenderer.sprite = loadedSprite;
            }
        }
        
        // Añadir script de enemigo apropiado
        if (createPatrolEnemy)
        {
            var patrolEnemy = enemy.AddComponent<PatrolEnemy>();
            // Las propiedades se configurarán con valores por defecto
        }
        else
        {
            var chaseEnemy = enemy.AddComponent<ChaseEnemy>();
            // Las propiedades se configurarán con valores por defecto
        }
        
        Debug.Log($"EnemySpawner: Enemigo creado por código - {enemy.name}");
        return enemy;
    }

    /// <summary>
    /// Detiene la generación de enemigos
    /// </summary>
    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnEnemy));
    }

    /// <summary>
    /// Reinicia la generación de enemigos
    /// </summary>
    public void ResumeSpawning()
    {
        CancelInvoke(nameof(SpawnEnemy));
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    /// <summary>
    /// Destruye todos los enemigos generados
    /// </summary>
    public void ClearAllEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
    }

    /// <summary>
    /// Obtiene el número de enemigos activos
    /// </summary>
    public int GetActiveEnemyCount()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return spawnedEnemies.Count;
    }

    void OnDrawGizmos()
    {
        // Dibujar el punto de spawn
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}
