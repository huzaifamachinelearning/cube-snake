using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public GameObject hudPanel;

    [Header("Game Over UI")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Game State")]
    public bool isGameActive = false;

    private int score = 0;
    private int highScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ShowStartScreen();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void ShowStartScreen()
    {
        isGameActive = false;
        Time.timeScale = 0f;

        if (startPanel != null)
            startPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(false);
    }

    public void StartGame()
    {
        isGameActive = true;
        score = 0;
        Time.timeScale = 1f;

        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (hudPanel != null)
            hudPanel.SetActive(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0f;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = "Score: " + score;

            if (highScoreText != null)
                highScoreText.text = "High Score: " + highScore;
        }

        if (hudPanel != null)
            hudPanel.SetActive(false);
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public int GetScore()
    {
        return score;
    }
}
