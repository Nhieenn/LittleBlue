using UnityEngine;

[System.Serializable]
public struct GameTheme
{
    public string themeName; // Tên (vd: Sáng, Chiều, Đêm)
    [ColorUsage(true, true)] public Color backgroundColor; // Bật HDR để nền có thể phát sáng nhẹ
    [ColorUsage(true, true)] public Color playerColor;     // Bật HDR cho Player phát sáng
    [ColorUsage(true, true)] public Color obstacleColor;   // Bật HDR cho Chướng ngại vật
    [ColorUsage(true, true)] public Color groundColor;     // Màu mặt đất
}

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Bảng màu chu kỳ (Ngày -> Chiều -> Đêm)")]
    public GameTheme[] themes;
    public float scorePerTheme = 100f; // Cứ mỗi 100 điểm sẽ chuyển sang màu tiếp theo

    [Header("Tham chiếu vật thể")]
    public Camera mainCamera;
    public SpriteRenderer playerSprite;
    public SpriteRenderer groundSprite;

    // Lưu trữ màu chướng ngại vật hiện tại để các Prefab tự động cập nhật
    public Color CurrentObstacleColor { get; private set; }
    public Color CurrentPlayerColor { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;
        if (themes.Length == 0) return;

        // 1. Tính toán xem chúng ta đang ở Giai đoạn màu nào dựa trên Điểm số
        float currentScore = GameManager.Instance.score;
        int currentIndex = Mathf.FloorToInt(currentScore / scorePerTheme) % themes.Length;
        int nextIndex = (currentIndex + 1) % themes.Length;

        // Tiến trình chuyển đổi (từ 0.0 đến 1.0)
        float transitionProgress = (currentScore % scorePerTheme) / scorePerTheme;

        // 2. Nội suy (Lerp) các màu sắc mượt mà
        GameTheme current = themes[currentIndex];
        GameTheme next = themes[nextIndex];

        mainCamera.backgroundColor = Color.Lerp(current.backgroundColor, next.backgroundColor, transitionProgress);
        playerSprite.color = Color.Lerp(current.playerColor, next.playerColor, transitionProgress);
        groundSprite.color = Color.Lerp(current.groundColor, next.groundColor, transitionProgress);

        CurrentObstacleColor = Color.Lerp(current.obstacleColor, next.obstacleColor, transitionProgress);
        CurrentPlayerColor = playerSprite.color;
    }
}