using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private UIDocument uiDocument;

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
        gameOverPanel.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!isGameOver)
        {
            score += Time.deltaTime * 10f;
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