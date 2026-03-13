using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float groundWidth = 30f;

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x <= -groundWidth)
        {
            transform.position = (Vector2)transform.position + new Vector2(groundWidth * 2f, 0);
        }
    }
}