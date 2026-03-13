using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float groundWidth = 20f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            // Tạo miếng đất mới khi miếng cũ đi qua điểm kích hoạt
            Instantiate(groundPrefab, new Vector3(spawnPoint.position.x + groundWidth, spawnPoint.position.y, 0), Quaternion.identity);
        }
    }
}