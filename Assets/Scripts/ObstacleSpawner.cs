using UnityEngine;
using UnityEngine.Pool;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float baseSpawnRate = 2f;
    [SerializeField] private float minRandomDelay = -0.5f;
    [SerializeField] private float maxRandomDelay = 1f;

    public static ObstacleSpawner Instance { get; private set; }
    private ObjectPool<GameObject> pool;
    private float nextSpawnTime;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(obstaclePrefab, transform.position, Quaternion.identity),
            actionOnGet: (obj) => { obj.transform.position = transform.position; obj.SetActive(true); },
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 5,
            maxSize: 10
        );
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            pool.Get();
            nextSpawnTime = Time.time + baseSpawnRate + Random.Range(minRandomDelay, maxRandomDelay);
        }
    }

    public void ReleaseObstacle(GameObject obj)
    {
        pool.Release(obj);
    }
}