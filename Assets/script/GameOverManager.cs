using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    public GameObject gameOverPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI restartText;
    public string gameOverTitle = "GAME OVER";
    public string defaultMessage = "it did not work well.";
    public string restartMessage = "Press Space to Restart";
    public string restartSceneName = "Counter";
    public int sortingOrder = 1000;

    bool isShowing;

    void Awake()
    {
        Instance = this;
        SetupPanel();

        if (!isShowing)
        {
            Hide();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Update()
    {
        if (isShowing && Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
    }

    public void Show(string message)
    {
        SetupPanel();
        isShowing = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();
        }

        if (titleText != null)
        {
            titleText.text = gameOverTitle;
        }

        if (messageText != null)
        {
            messageText.text = string.IsNullOrEmpty(message) ? defaultMessage : message;
        }

        if (restartText != null)
        {
            restartText.text = restartMessage;
        }
    }

    public void Hide()
    {
        isShowing = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(restartSceneName);
    }

    void SetupPanel()
    {
        if (gameOverPanel == null)
        {
            gameOverPanel = FindInactiveGameObjectByName("GameOverPanel");
        }

        if (gameOverPanel == null)
        {
            return;
        }

        Canvas panelCanvas = gameOverPanel.GetComponent<Canvas>();
        if (panelCanvas == null)
        {
            panelCanvas = gameOverPanel.AddComponent<Canvas>();
        }

        panelCanvas.overrideSorting = true;
        panelCanvas.sortingOrder = sortingOrder;

        if (gameOverPanel.GetComponent<GraphicRaycaster>() == null)
        {
            gameOverPanel.AddComponent<GraphicRaycaster>();
        }

        if (titleText == null)
        {
            titleText = FindText("Title");
        }

        if (messageText == null)
        {
            messageText = FindText("Message");
        }

        if (restartText == null)
        {
            restartText = FindText("Restart");
        }
    }

    TextMeshProUGUI FindText(string childName)
    {
        if (gameOverPanel == null)
        {
            return null;
        }

        Transform child = gameOverPanel.transform.Find(childName);
        if (child == null)
        {
            return null;
        }

        return child.GetComponent<TextMeshProUGUI>();
    }

    GameObject FindInactiveGameObjectByName(string objectName)
    {
        Transform[] transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < transforms.Length; i++)
        {
            if (transforms[i].name == objectName)
            {
                return transforms[i].gameObject;
            }
        }

        return null;
    }
}