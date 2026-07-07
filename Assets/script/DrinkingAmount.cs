using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DrinkingAmount : MonoBehaviour
{
    public Transform barFill;
    public float fillSpeed = 40f;
    public float requiredDrinkPercent = 60f;
    public float drinkTolerance = 5f;
    public Button nextButton;
    public string counterSceneName = "Counter";
    public Vector2 nextButtonPosition = new Vector2(-150f, 40f);
    public Vector2 nextButtonSize = new Vector2(160f, 30f);

    [Header("Result Text")]
    public TextMeshProUGUI resultText;
    public string waitingText = "";
    public string aliveText = "hes alive!";
    public string failedText = "it did not work well.";
    public float minimumPotionQualityToRevive = 100f;

    float currentDrinkPercent;
    bool isDrinking;
    bool hasFinished;

    void Awake()
    {
        SetupResultText();
        SetResultText(waitingText);
    }

    void Start()
    {
        SetupResultText();
        SetResultText(waitingText);
        SetupNextButton();
        HideNextButton();

        if (RecipeRuntimeData.Instance != null && RecipeRuntimeData.Instance.currentRecipe != null)
        {
            requiredDrinkPercent = RecipeRuntimeData.Instance.currentRecipe.requiredDrinkPercent;
        }

        currentDrinkPercent = 0f;
        UpdateBar();
    }

    void Update()
    {
        if (hasFinished)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDrinking = true;
        }

        if (Input.GetMouseButton(0) && isDrinking)
        {
            currentDrinkPercent += fillSpeed * Time.deltaTime;
            currentDrinkPercent = Mathf.Clamp(currentDrinkPercent, 0f, 100f);
            UpdateBar();
        }

        if (Input.GetMouseButtonUp(0) && isDrinking)
        {
            isDrinking = false;
            hasFinished = true;
            CheckDrinkAmount();
        }
    }

    void SetupResultText()
    {
        if (resultText == null)
        {
            GameObject panelObject = GameObject.Find("Panel");
            if (panelObject != null)
            {
                resultText = panelObject.GetComponentInChildren<TextMeshProUGUI>(true);
                DisableRecipeTextComponents(panelObject.transform);
            }
        }

        if (resultText == null)
        {
            resultText = FindFirstObjectByType<TextMeshProUGUI>(FindObjectsInactive.Include);
        }

        if (resultText == null)
        {
            return;
        }

        resultText.raycastTarget = false;

        RecipeText recipeText = resultText.GetComponent<RecipeText>();
        if (recipeText != null)
        {
            recipeText.enabled = false;
        }
    }

    void DisableRecipeTextComponents(Transform root)
    {
        RecipeText[] recipeTexts = root.GetComponentsInChildren<RecipeText>(true);
        for (int i = 0; i < recipeTexts.Length; i++)
        {
            recipeTexts[i].enabled = false;
        }
    }

    void SetResultText(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
    }

    void SetupNextButton()
    {
        if (nextButton == null)
        {
            nextButton = FindNextButtonInScene();
        }

        if (nextButton == null)
        {
            nextButton = CreateNextButton();
        }

        if (nextButton == null)
        {
            Debug.LogWarning("No Canvas found, so DrinkingAmount could not create a NextButton.", this);
            return;
        }

        nextButton.onClick.RemoveListener(GoToCounterScene);
        nextButton.onClick.AddListener(GoToCounterScene);
    }

    Button FindNextButtonInScene()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == "NextButton")
            {
                return buttons[i];
            }
        }

        return null;
    }

    Button CreateNextButton()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        GameObject buttonObject = new GameObject("NextButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(canvas.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1f, 0f);
        buttonRect.anchorMax = new Vector2(1f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = nextButtonPosition;
        buttonRect.sizeDelta = nextButtonSize;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = Color.white;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;

        GameObject textObject = new GameObject("Text (TMP)", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI buttonText = textObject.GetComponent<TextMeshProUGUI>();
        buttonText.text = "Next";
        buttonText.fontSize = 24f;
        buttonText.color = new Color(0.196f, 0.196f, 0.196f, 1f);
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.raycastTarget = false;

        return button;
    }

    void HideNextButton()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    void ShowNextButton()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    void UpdateBar()
    {
        if (barFill == null)
        {
            return;
        }

        float fill01 = currentDrinkPercent / 100f;
        barFill.localScale = new Vector3(fill01, barFill.localScale.y, barFill.localScale.z);
    }

    void CheckDrinkAmount()
    {
        bool goodDrinkingAmount = Mathf.Abs(currentDrinkPercent - requiredDrinkPercent) <= drinkTolerance;
        bool potionWorked = DidPotionWork(goodDrinkingAmount);

        Debug.Log(
            "Drink target: " +
            requiredDrinkPercent.ToString("0") +
            "% | Actual: " +
            currentDrinkPercent.ToString("0.0") +
            "% | " +
            (goodDrinkingAmount ? "Good drinking amount" : "Wrong drinking amount") +
            " | Revival: " +
            (potionWorked ? "Alive" : "Failed")
        );

        SetResultText(potionWorked ? aliveText : failedText);
        ShowNextButton();
    }

    bool DidPotionWork(bool goodDrinkingAmount)
    {
        if (!goodDrinkingAmount)
        {
            return false;
        }

        if (RecipeRuntimeData.Instance == null || RecipeRuntimeData.Instance.currentRecipe == null)
        {
            return true;
        }

        return RecipeRuntimeData.Instance.lastPotionHadCorrectIngredients &&
               RecipeRuntimeData.Instance.lastPotionHadGoodFireTiming &&
               RecipeRuntimeData.Instance.lastPotionQuality >= minimumPotionQualityToRevive;
    }

    public void GoToCounterScene()
    {
        SceneManager.LoadScene(counterSceneName);
    }
}
