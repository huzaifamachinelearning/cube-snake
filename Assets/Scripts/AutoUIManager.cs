using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AutoUIManager : MonoBehaviour
{
    public static AutoUIManager Instance { get; private set; }

    private Canvas canvas;
    private GameObject startPanel;
    private GameObject gameOverPanel;
    private GameObject hudPanel;

    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI chainCountText;
    private TextMeshProUGUI finalScoreText;
    private TextMeshProUGUI highScoreText;

    private int score = 0;
    private int highScore = 0;
    private bool isGameActive = false;
    private bool isFirstLoad = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (isFirstLoad)
        {
            ShowStartScreen();
            isFirstLoad = false;
        }
    }

    void CreateUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        CreateEventSystem();

        CreateStartPanel();
        CreateGameOverPanel();
        CreateHUDPanel();
    }

    void CreateEventSystem()
    {
        UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    void CreateStartPanel()
    {
        startPanel = CreatePanel(new Color(0.1f, 0.1f, 0.1f, 0.95f));

        RectTransform titleRect = CreateText(startPanel.transform, "CUBE MAZE", 80, new Vector2(0, 150), Color.cyan);
        titleRect.sizeDelta = new Vector2(800, 100);

        RectTransform instructRect = CreateText(startPanel.transform, "Use mouse to steer. Collect smaller cubes, avoid larger ones!", 30, new Vector2(0, 50), Color.white);
        instructRect.sizeDelta = new Vector2(600, 100);

        CreateButton(startPanel.transform, "START GAME", new Vector2(0, -50), new Color(0, 0.7f, 0.3f), StartGame);
        CreateButton(startPanel.transform, "QUIT", new Vector2(0, -130), new Color(0.7f, 0.2f, 0.2f), QuitGame);
    }

    void CreateGameOverPanel()
    {
        gameOverPanel = CreatePanel(new Color(0.1f, 0.1f, 0.1f, 0.95f));
        gameOverPanel.SetActive(false);

        RectTransform titleRect = CreateText(gameOverPanel.transform, "GAME OVER", 80, new Vector2(0, 150), Color.red);
        titleRect.sizeDelta = new Vector2(600, 100);

        finalScoreText = CreateText(gameOverPanel.transform, "Score: 0", 40, new Vector2(0, 50), Color.white).GetComponent<TextMeshProUGUI>();

        highScoreText = CreateText(gameOverPanel.transform, "High Score: 0", 30, new Vector2(0, 0), Color.yellow).GetComponent<TextMeshProUGUI>();

        CreateButton(gameOverPanel.transform, "PLAY AGAIN", new Vector2(0, -80), new Color(0, 0.7f, 0.3f), RestartGame);
        CreateButton(gameOverPanel.transform, "MAIN MENU", new Vector2(0, -160), new Color(0.3f, 0.3f, 0.7f), ShowStartScreen);
    }

    void CreateHUDPanel()
    {
        hudPanel = CreatePanel(new Color(0, 0, 0, 0.5f));
        hudPanel.SetActive(false);

        RectTransform panelRect = hudPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.pivot = new Vector2(0.5f, 1);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(0, 60);

        scoreText = CreateText(hudPanel.transform, "Score: 0", 24, new Vector2(-350, -15), Color.white).GetComponent<TextMeshProUGUI>();
        RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 0.5f);
        scoreRect.anchorMax = new Vector2(0, 0.5f);
        scoreRect.pivot = new Vector2(0, 0.5f);
        scoreRect.anchoredPosition = new Vector2(20, 0);

        chainCountText = CreateText(hudPanel.transform, "Chain: 0", 24, new Vector2(350, -15), Color.white).GetComponent<TextMeshProUGUI>();
        RectTransform chainRect = chainCountText.GetComponent<RectTransform>();
        chainRect.anchorMin = new Vector2(1, 0.5f);
        chainRect.anchorMax = new Vector2(1, 0.5f);
        chainRect.pivot = new Vector2(1, 0.5f);
        chainRect.anchoredPosition = new Vector2(-20, 0);
    }

    GameObject CreatePanel(Color bgColor)
    {
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvas.transform, false);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = bgColor;

        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        return panel;
    }

    RectTransform CreateText(Transform parent, string text, int fontSize, Vector2 position, Color color)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(400, 50);

        return rect;
    }

    void CreateButton(Transform parent, string text, Vector2 position, Color color, UnityEngine.Events.UnityAction action)
    {
        GameObject btnObj = new GameObject(text.Replace(" ", "") + "Button");
        btnObj.transform.SetParent(parent, false);

        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = color;

        Button btn = btnObj.AddComponent<Button>();

        ColorBlock colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = new Color(color.r + 0.2f, color.g + 0.2f, color.b + 0.2f);
        colors.pressedColor = new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f);
        colors.selectedColor = color;
        btn.colors = colors;

        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(250, 60);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        btn.onClick.AddListener(action);

        Debug.Log("Created button: " + text + " with action");
    }

    void ShowStartScreen()
    {
        isGameActive = false;
        Time.timeScale = 0f;

        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(false);
    }

    public void StartGame()
    {
        isGameActive = true;
        score = 0;
        Time.timeScale = 1f;

        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(true);

        ResetGame();
    }

    void ResetGame()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.ResetPlayer();
        }

        CubeSpawner spawner = FindObjectOfType<CubeSpawner>();
        if (spawner != null)
        {
            spawner.RespawnCubes();
        }
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

        gameOverPanel.SetActive(true);
        hudPanel.SetActive(false);

        finalScoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public int GetScore()
    {
        return score;
    }

    void Update()
    {
        if (isGameActive && scoreText != null)
        {
            scoreText.text = "Score: " + score;

            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null && chainCountText != null)
            {
                chainCountText.text = "Chain: " + player.chain.Count;
            }
        }
    }
}
