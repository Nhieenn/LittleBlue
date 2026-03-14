using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Nếu Unity báo lỗi dòng này, hãy đổi thành: using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private Rigidbody2D rb;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // Biến lưu trữ nguồn phát rung
    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Lấy component Impulse Source đã gắn trên Player
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer))
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            // Phát ra tín hiệu rung ngay trước khi gọi Game Over
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }

            GameManager.Instance.GameOver();
        }
    }
}