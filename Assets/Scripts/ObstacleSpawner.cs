using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ObstacleConfig
    {
        public GameObject prefab;
        public float spawnY;
    }

    [SerializeField] private ObstacleConfig[] obstacles;
    [SerializeField] private float baseSpawnRate = 2f;
    [SerializeField] private float minRandomDelay = -0.5f;
    [SerializeField] private float maxRandomDelay = 1f;

    public static ObstacleSpawner Instance { get; private set; }

    private Dictionary<GameObject, ObjectPool<GameObject>> pools;
    private float nextSpawnTime;

    void Awake()
    {
        Instance = this;
        pools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    }

    void Start()
    {
        foreach (var config in obstacles)
        {
            GameObject currentPrefab = config.prefab;
            float currentSpawnY = config.spawnY;

            pools[currentPrefab] = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(currentPrefab);
                    obj.GetComponent<Obstacle>().prefabSource = currentPrefab;
                    return obj;
                },
                actionOnGet: (obj) => {
                    obj.transform.position = new Vector2(transform.position.x, currentSpawnY);
                    obj.SetActive(true);
                },
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 5,
                maxSize: 10
            );
        }
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        if (Time.time >= nextSpawnTime)
        {
            int randomIndex = Random.Range(0, obstacles.Length);
            GameObject selectedPrefab = obstacles[randomIndex].prefab;

            pools[selectedPrefab].Get();

            float delay = baseSpawnRate + Random.Range(minRandomDelay, maxRandomDelay);
            nextSpawnTime = Time.time + (delay / GameManager.Instance.gameSpeed);
        }
    }

    public void ReleaseObstacle(GameObject obj, GameObject prefabSource)
    {
        if (pools.ContainsKey(prefabSource))
        {
            pools[prefabSource].Release(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}