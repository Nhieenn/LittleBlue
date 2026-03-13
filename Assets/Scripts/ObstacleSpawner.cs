using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private float baseSpawnRate = 2f;
    [SerializeField] private float minRandomDelay = -0.5f;
    [SerializeField] private float maxRandomDelay = 1f;

    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            float randomDelay = Random.Range(minRandomDelay, maxRandomDelay);
            nextSpawnTime = Time.time + baseSpawnRate + randomDelay;
        }
    }

    private void SpawnObstacle()
    {
        Instantiate(obstaclePrefab, transform.position, Quaternion.identity);
    }
}