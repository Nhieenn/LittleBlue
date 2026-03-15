using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private ParticleSystem crashParticles;

    private Rigidbody2D rb;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private CinemachineImpulseSource impulseSource;
    private bool wasGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Start()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    // (Giữ nguyên các biến ở trên) ...

    void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.Menu) return;
        if (GameManager.Instance.State == GameManager.GameState.GameOver) return;

        if (GameManager.Instance.State == GameManager.GameState.Playing && rb.bodyType == RigidbodyType2D.Static)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.5f);
            if (dustParticles != null) dustParticles.Play();

            // Phát tiếng nhảy lót đầu tiên
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpClip);
        }

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (!wasGrounded && dustParticles != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                dustParticles.Play();
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        wasGrounded = isGrounded;

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

            if (dustParticles != null) dustParticles.Play();

            // Phát tiếng nhảy
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpClip);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") && GameManager.Instance.State == GameManager.GameState.Playing)
        {
            if (impulseSource != null) impulseSource.GenerateImpulse();
            if (crashParticles != null) crashParticles.Play();

            // Phát tiếng va chạm
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.crashClip);

            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            GetComponent<SpriteRenderer>().enabled = false;

            GameManager.Instance.GameOver();
        }
    }
}
