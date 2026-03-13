using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 5f;
    private float groundWidth;
    private float currentDistance;
    private Vector2 startPosition;

    void Start()
    {
        groundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPosition = transform.position;
    }

    void Update()
    {
        float currentSpeed = baseSpeed * GameManager.Instance.gameSpeed;
        currentDistance += currentSpeed * Time.deltaTime;

        float newPosition = Mathf.Repeat(currentDistance, groundWidth);
        transform.position = startPosition + Vector2.left * newPosition;
    }
}