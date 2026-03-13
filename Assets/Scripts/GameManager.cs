using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private UIDocument uiDocument;

    public float gameSpeed { get; private set; } = 1f;
    [SerializeField] private float speedIncreaseRate = 0.025f;
    [SerializeField] private float maxSpeed = 3f;

    private Label scoreLabel;
    private VisualElement gameOverPanel;
    private Button restartButton;

    private float score;
    private bool isGameOver;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        scoreLabel = root.Q<Label>("ScoreLabel");
        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        restartButton = root.Q<Button>("RestartButton");

        restartButton.clicked += RestartGame;
    }

    private void OnDisable()
    {
        restartButton.clicked -= RestartGame;
    }

    void Start()
    {
        isGameOver = false;
        score = 0;
        gameSpeed = 1f;
        gameOverPanel.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!isGameOver)
        {
            if (gameSpeed < maxSpeed)
            {
                gameSpeed += speedIncreaseRate * Time.deltaTime;
            }

            score += Time.deltaTime * 10f * gameSpeed;
            scoreLabel.text = "Score: " + Mathf.FloorToInt(score).ToString();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverPanel.style.display = DisplayStyle.Flex;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}