using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }
    public GameState State { get; private set; }

    [SerializeField] private UIDocument uiDocument;

    public float gameSpeed { get; private set; } = 0f;
    [SerializeField] private float speedIncreaseRate = 0.025f;
    [SerializeField] private float maxSpeed = 3f;

    public static bool isRestarting = false;

    private Label scoreLabel;
    private Label highScoreLabel;
    private Label newRecordLabel;
    private Label gameOverHighScoreLabel;
    private VisualElement gameOverPanel;
    private VisualElement mainMenuPanel;
    private Button restartButton;

    public float score { get; private set; }
    private int highScore;
    private int nextScoreMilestone = 100; // Mốc điểm để phát âm thanh

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
        nextScoreMilestone = 100;
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

        if (isRestarting)
        {
            State = GameState.Playing;
            mainMenuPanel.style.display = DisplayStyle.None;
            gameSpeed = 1f;
            isRestarting = false;

            // Phát nhạc nền khi chơi lại
            if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM();
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
            bool isTapInput = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                              (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                              (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

            if (isTapInput)
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

            // Logic phát âm thanh khi đạt mốc điểm (100, 200, 300...)
            if (currentScore >= nextScoreMilestone)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.scoreMilestoneClip);
                nextScoreMilestone += 100;
            }

            if (currentScore > highScore && currentScore > 0 && !recordBurstCompleted)
            {
                recordBurstCompleted = true;
                isBlinking = true;
                newRecordLabel.style.display = DisplayStyle.Flex;
                burstTimer = 0f;
                blinkSubTimer = 0f;

                // Phát âm thanh kỷ lục mới
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.newRecordClip);
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

        if (State == GameState.GameOver)
        {
            bool isTapInput = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
                              (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                              (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);

            if (isTapInput)
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

        // Phát nhạc nền khi bắt đầu từ Menu
        if (AudioManager.Instance != null) AudioManager.Instance.PlayBGM();
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        gameSpeed = 0f;

        // Dừng nhạc nền ngay lập tức để tạo không gian tĩnh lặng cho tiếng nổ
        if (AudioManager.Instance != null) AudioManager.Instance.StopBGM();

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

        // GỌI COROUTINE ĐỂ TRÌ HOÃN UI THAY VÌ HIỂN THỊ NGAY
        StartCoroutine(ShowGameOverScreenDelay());
    }

    private IEnumerator ShowGameOverScreenDelay()
    {
        // Chờ 0.8 giây (Bạn có thể tăng giảm số này tùy ý thích)
        yield return new WaitForSeconds(0.8f);

        // Sau khi chờ xong mới bật màn hình đen và bảng điểm
        gameOverPanel.style.display = DisplayStyle.Flex;
    }

    private void RestartScene()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(AudioManager.Instance.uiClickClip);
        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}