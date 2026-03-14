using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }
    public GameState State { get; private set; }

    [SerializeField] private UIDocument uiDocument;

    public float gameSpeed { get; private set; } = 0f;
    [SerializeField] private float speedIncreaseRate = 0.025f;
    [SerializeField] private float maxSpeed = 3f;

    // Biến tĩnh để ghi nhớ người chơi đang mở game lần đầu hay đang chơi lại
    public static bool isRestarting = false;

    private Label scoreLabel;
    private Label highScoreLabel;
    private Label newRecordLabel;
    private Label gameOverHighScoreLabel;
    private VisualElement gameOverPanel;
    private VisualElement mainMenuPanel;
    private Button restartButton;

    private float score;
    private int highScore;

    private bool recordBurstCompleted;
    private bool isBlinking;
    private float burstTimer;
    private float blinkSubTimer;
    private float blinkInterval = 0.15f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        scoreLabel = root.Q<Label>("ScoreLabel");
        highScoreLabel = root.Q<Label>("HighScoreLabel");
        newRecordLabel = root.Q<Label>("NewRecordLabel");
        gameOverHighScoreLabel = root.Q<Label>("GameOverHighScoreLabel");
        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        mainMenuPanel = root.Q<VisualElement>("MainMenuPanel");
        restartButton = root.Q<Button>("RestartButton");

        restartButton.clicked += RestartScene;
    }

    private void OnDisable()
    {
        restartButton.clicked -= RestartScene;
    }

    void Start()
    {
        score = 0;
        Time.timeScale = 1f;
        recordBurstCompleted = false;
        isBlinking = false;
        burstTimer = 0f;
        blinkSubTimer = 0f;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreLabel.text = "High Score: " + highScore;

        gameOverPanel.style.display = DisplayStyle.None;
        newRecordLabel.style.display = DisplayStyle.None;
        newRecordLabel.RemoveFromClassList("new-record-blink-active");

        // Kiểm tra xem đây là mở game lần đầu hay là chơi lại
        if (isRestarting)
        {
            State = GameState.Playing;
            mainMenuPanel.style.display = DisplayStyle.None;
            gameSpeed = 1f;
            isRestarting = false; // Reset biến tĩnh
        }
        else
        {
            State = GameState.Menu;
            gameSpeed = 0f;
            mainMenuPanel.style.display = DisplayStyle.Flex;
        }
    }

    void Update()
    {
        if (State == GameState.Menu)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartGame();
            }
            return;
        }

        if (State == GameState.Playing)
        {
            if (gameSpeed < maxSpeed)
            {
                gameSpeed += speedIncreaseRate * Time.deltaTime;
            }

            score += Time.deltaTime * 10f * gameSpeed;
            int currentScore = Mathf.FloorToInt(score);
            scoreLabel.text = "Score: " + currentScore;

            if (currentScore > highScore && currentScore > 0 && !recordBurstCompleted)
            {
                recordBurstCompleted = true;
                isBlinking = true;
                newRecordLabel.style.display = DisplayStyle.Flex;
                burstTimer = 0f;
                blinkSubTimer = 0f;
            }

            if (isBlinking)
            {
                burstTimer += Time.deltaTime;
                blinkSubTimer += Time.deltaTime;

                if (burstTimer >= 2.0f)
                {
                    isBlinking = false;
                    newRecordLabel.RemoveFromClassList("new-record-blink-active");
                }
                else if (blinkSubTimer >= blinkInterval)
                {
                    newRecordLabel.ToggleInClassList("new-record-blink-active");
                    blinkSubTimer = 0f;
                }
            }
        }

        // Cho phép bấm Space để chơi lại nhanh khi đang Game Over
        if (State == GameState.GameOver)
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                RestartScene();
            }
        }
    }

    private void StartGame()
    {
        State = GameState.Playing;
        mainMenuPanel.style.display = DisplayStyle.None;
        gameSpeed = 1f;
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        gameSpeed = 0f;

        if (isBlinking)
        {
            isBlinking = false;
            newRecordLabel.RemoveFromClassList("new-record-blink-active");
        }

        int finalScore = Mathf.FloorToInt(score);
        if (finalScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", finalScore);
            PlayerPrefs.Save();
            gameOverHighScoreLabel.text = "New Best: " + finalScore;
        }
        else
        {
            gameOverHighScoreLabel.text = "Best: " + highScore;
        }

        gameOverPanel.style.display = DisplayStyle.Flex;
    }

    private void RestartScene()
    {
        isRestarting = true; // Đánh dấu là đang chơi lại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

// --- VFX & Post-Processing Setup (Implemented via Unity Editor) ---
// Bloom, Color Grading, and Vignette configured in Global Volume Profile.
// Sprite colors updated to HDR for maximum brightness.

// Planned:
// [ ] Step 2: Implement Dust Burst & Crash Explosion Particle Systems.
// [ ] Step 3: Add Trail Renderer to Player.
// [ ] Step 4: Add Environmental Glow.
// [ ] Step 5: Adjust BGM/SFX with new VFX.