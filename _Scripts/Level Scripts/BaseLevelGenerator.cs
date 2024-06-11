using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using TWC;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    [Range(0, 1)]
    public float spawnWeight;
}

public class BaseLevelGenerator : MonoBehaviour
{
    private static BaseLevelGenerator _instance;
    public static BaseLevelGenerator Instance { get { return _instance; } }

    public TileWorldCreator tileWorldCreator;
    public GameObject loadingScreen;
    private NavMeshSurface navMeshSurface;
    public delegate void NavMeshReadyEvent();
    public static event NavMeshReadyEvent OnNavMeshReady;

    public GameObject playerPrefab; // Player prefab
    public Transform playerSpawnPoint;

    public List<EnemySpawnInfo> enemySpawnList; // List of enemies and their spawn weights
    public int baseEnemyCount = 5; // Base enemy count
    public LayerMask groundLayer; // Ground layer mask
    public float spawnAreaRadius = 50f; // Radius to search for spawn points

    void OnEnable()
    {
        tileWorldCreator.OnBlueprintLayersComplete += BlueprintMapReady;
        tileWorldCreator.OnBuildLayersComplete += MapReady;
    }

    void OnDisable()
    {
        tileWorldCreator.OnBlueprintLayersComplete -= BlueprintMapReady;
        tileWorldCreator.OnBuildLayersComplete -= MapReady;
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        GenerateDungeon();
    }

    public void ExitReached()
    {
        Debug.Log("Player has reached portal - generating new dungeon");

        StartCoroutine(GenerateDungeonIE());
    }

    IEnumerator GenerateDungeonIE()
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(5f);

        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        loadingScreen.SetActive(true);
        tileWorldCreator.SetCustomRandomSeed(System.Environment.TickCount);
        tileWorldCreator.ExecuteAllBlueprintLayers();
    }

    void BlueprintMapReady(TileWorldCreator _twc)
    {
        Debug.Log("Blueprint ready");
        tileWorldCreator.ExecuteAllBuildLayers(true);
    }

    void MapReady(TileWorldCreator _twc)
    {
        Debug.Log("Build complete");
        CreateAndBakeNavMeshSurface();
        loadingScreen.SetActive(false);
    }

    void CreateAndBakeNavMeshSurface()
    {
        GameObject navMeshSurfaceObject = new GameObject("NavMeshSurface");
        navMeshSurface = navMeshSurfaceObject.AddComponent<NavMeshSurface>();

        navMeshSurface.layerMask = LayerMask.GetMask("Ground", "Player");

        navMeshSurface.BuildNavMesh();

        OnNavMeshReady?.Invoke();

        StartCoroutine(SpawnPlayerAndEnemies());
    }

    IEnumerator SpawnPlayerAndEnemies()
    {
        yield return new WaitForSeconds(0.5f); // Ensure NavMesh is ready

        // Spawn player
        Vector3 playerPosition = GetRandomPositionOnNavMesh();
        if (NavMesh.SamplePosition(playerPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Instantiate(playerPrefab, hit.position, Quaternion.identity);
        }
        else // Fallback if no ground is hit
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }
        

        // Determine the number of enemies to spawn based on difficulty
        int enemyCount = baseEnemyCount; // You can modify this based on difficulty

        // Spawn enemies
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPosition = GetRandomPositionOnNavMesh();
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                GameObject enemyToSpawn = GetRandomEnemyPrefab();
                GameObject enemyInstance = Instantiate(enemyToSpawn, hit.position, Quaternion.identity);
                NavMeshAgent enemyAgent = enemyInstance.GetComponent<NavMeshAgent>();
                if (enemyAgent != null)
                {
                    enemyAgent.enabled = true; // Enable NavMeshAgent after positioning
                }
            }
            // Fallback if no ground is hit
            else
            {
                // Try again
                i--;
            }
        }
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-spawnAreaRadius, spawnAreaRadius),
            0,
            Random.Range(-spawnAreaRadius, spawnAreaRadius)
        );
        randomPoint += playerSpawnPoint.position; // Center around player spawn point
        randomPoint.y = 50; // Start high above the ground
        RaycastHit hit;
        if (Physics.Raycast(randomPoint, Vector3.down, out hit, 100f, groundLayer))
        {
            return hit.point;
        }
        return randomPoint; // Fallback if no ground is hit
    }

    GameObject GetRandomEnemyPrefab()
    {
        float totalWeight = 0;
        foreach (var enemy in enemySpawnList)
        {
            totalWeight += enemy.spawnWeight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0;

        foreach (var enemy in enemySpawnList)
        {
            cumulativeWeight += enemy.spawnWeight;
            if (randomValue < cumulativeWeight)
            {
                return enemy.enemyPrefab;
            }
        }

        return null; // Should never reach here if weights are set up correctly
    }
}
