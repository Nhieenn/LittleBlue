using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private float groundWidth;
    private Vector2 startPosition;

    void Start()
    {
        groundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPosition = transform.position;
    }

    void Update()
    {
        float newPosition = Mathf.Repeat(Time.time * speed, groundWidth);
        transform.position = startPosition + Vector2.left * newPosition;
    }
}