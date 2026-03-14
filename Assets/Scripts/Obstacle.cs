using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float destroyX = -15f;

    [HideInInspector] public GameObject prefabSource;

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;
        float currentSpeed = baseSpeed * GameManager.Instance.gameSpeed;
        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        if (transform.position.x <= destroyX)
        {
            ObstacleSpawner.Instance.ReleaseObstacle(gameObject, prefabSource);
        }
    }
}