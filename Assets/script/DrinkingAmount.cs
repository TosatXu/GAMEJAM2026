using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
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

    [Header("Vertical Drink Bar")]
    public RecipeData previewRecipeData;
    public Transform barBack;
    public Transform targetLine;
    public bool useVerticalBar = true;
    public float barBottomLocalY = -1.5f;
    public float verticalBarHeight = 3f;
    public float verticalBarWidth = 0.35f;
    public float targetLineWidth = 0.8f;
    public float targetLineThickness = 0.06f;
    public Color barBackColor = new Color(0.12f, 0.12f, 0.12f, 1f);
    public Color barFillColor = new Color(0.25f, 0.9f, 0.75f, 1f);
    public Color targetLineColor = new Color(1f, 0.2f, 0.15f, 1f);

    [Header("Result Text")]
    public TextMeshProUGUI resultText;
    public string waitingText = "";
    public string aliveText = "hes alive!";
    public string failedText = "it did not work well.";
    public float minimumPotionQualityToRevive = 100f;

    float currentDrinkPercent;
    bool isDrinking;
    bool hasFinished;

    void OnEnable()
    {
        if (!Application.isPlaying)
        {
            RefreshDrinkBarPreview();
        }
    }

    void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall -= DelayedRefreshDrinkBarPreview;
            EditorApplication.delayCall += DelayedRefreshDrinkBarPreview;
        }
#else
        if (!Application.isPlaying)
        {
            RefreshDrinkBarPreview();
        }
#endif
    }

#if UNITY_EDITOR
    void DelayedRefreshDrinkBarPreview()
    {
        if (this == null || Application.isPlaying)
        {
            return;
        }

        RefreshDrinkBarPreview();
    }
#endif

    void Awake()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SetupResultText();
        SetResultText(waitingText);
    }

    void Start()
    {
        if (!Application.isPlaying)
        {
            RefreshDrinkBarPreview();
            return;
        }

        SetupResultText();
        SetResultText(waitingText);
        SetupNextButton();
        HideNextButton();

        if (RecipeRuntimeData.Instance != null && RecipeRuntimeData.Instance.currentRecipe != null)
        {
            requiredDrinkPercent = RecipeRuntimeData.Instance.currentRecipe.requiredDrinkPercent;
        }
        else
        {
            ApplyPreviewRecipePercent();
        }

        SetupDrinkBar();
        currentDrinkPercent = 0f;
        UpdateBar();
    }

    void Update()
    {
        if (!Application.isPlaying || hasFinished)
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

    void RefreshDrinkBarPreview()
    {
        ApplyPreviewRecipePercent();
        SetupDrinkBar();
        currentDrinkPercent = 0f;
        UpdateBar();
    }

    void ApplyPreviewRecipePercent()
    {
        if (previewRecipeData != null)
        {
            requiredDrinkPercent = previewRecipeData.requiredDrinkPercent;
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

    void SetupDrinkBar()
    {
        if (barFill == null)
        {
            barFill = FindChildByName(transform, "BarFill");
        }

        if (barBack == null)
        {
            barBack = FindChildByName(transform, "BarBack");
        }

        if (!useVerticalBar)
        {
            UpdateTargetLine();
            return;
        }

        SetupBarPart(barBack, verticalBarWidth, verticalBarHeight, barBottomLocalY + verticalBarHeight * 0.5f, barBackColor);
        SetupBarPart(barFill, verticalBarWidth, 0f, barBottomLocalY, barFillColor);
        SetupTargetLine();
    }

    Transform FindChildByName(Transform root, string childName)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    void SetupBarPart(Transform barPart, float width, float height, float localY, Color color)
    {
        if (barPart == null)
        {
            return;
        }

        barPart.localScale = new Vector3(width, height, barPart.localScale.z);
        barPart.localPosition = new Vector3(0f, localY, barPart.localPosition.z);

        SpriteRenderer renderer = barPart.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;

            if (barPart == barFill && barBack != null && barBack.TryGetComponent(out SpriteRenderer backRenderer))
            {
                renderer.sortingLayerID = backRenderer.sortingLayerID;
                renderer.sortingOrder = backRenderer.sortingOrder + 1;
            }
        }
    }

    void SetupTargetLine()
    {
        if (targetLine == null)
        {
            targetLine = FindChildByName(transform, "TargetLine");
        }

        if (targetLine == null)
        {
            targetLine = CreateTargetLine();
        }

        UpdateTargetLine();
    }

    Transform CreateTargetLine()
    {
        GameObject targetLineObject = new GameObject("TargetLine");
        targetLineObject.transform.SetParent(transform, false);

        SpriteRenderer targetRenderer = targetLineObject.AddComponent<SpriteRenderer>();
        SpriteRenderer sourceRenderer = GetSourceBarRenderer();

        if (sourceRenderer != null)
        {
            targetRenderer.sprite = sourceRenderer.sprite;
            targetRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
            targetRenderer.sortingOrder = sourceRenderer.sortingOrder + 2;
        }

        targetRenderer.color = targetLineColor;

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(targetLineObject, "Create Drink Target Line");
        EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif

        return targetLineObject.transform;
    }

    SpriteRenderer GetSourceBarRenderer()
    {
        if (barBack != null && barBack.TryGetComponent(out SpriteRenderer backRenderer))
        {
            return backRenderer;
        }

        if (barFill != null && barFill.TryGetComponent(out SpriteRenderer fillRenderer))
        {
            return fillRenderer;
        }

        return null;
    }

    void UpdateTargetLine()
    {
        if (targetLine == null)
        {
            return;
        }

        float target01 = Mathf.Clamp01(requiredDrinkPercent / 100f);
        float targetY = barBottomLocalY + verticalBarHeight * target01;
        targetLine.localPosition = new Vector3(0f, targetY, targetLine.localPosition.z);
        targetLine.localScale = new Vector3(targetLineWidth, targetLineThickness, targetLine.localScale.z == 0f ? 1f : targetLine.localScale.z);

        SpriteRenderer targetRenderer = targetLine.GetComponent<SpriteRenderer>();
        if (targetRenderer != null)
        {
            targetRenderer.color = targetLineColor;
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

        if (!useVerticalBar)
        {
            barFill.localScale = new Vector3(fill01, barFill.localScale.y, barFill.localScale.z);
            return;
        }

        float fillHeight = verticalBarHeight * fill01;
        barFill.localScale = new Vector3(verticalBarWidth, fillHeight, barFill.localScale.z);
        barFill.localPosition = new Vector3(0f, barBottomLocalY + fillHeight * 0.5f, barFill.localPosition.z);
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