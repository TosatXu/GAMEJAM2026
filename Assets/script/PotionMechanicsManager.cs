using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class PotionMechanicsManager : MonoBehaviour
{
    public RecipeData recipeData;

    public TimingBar timingBar;

    public GameObject nextButton;
    public Button nextButtonStyleSource;
    public string nextButtonStyleSourceName = "BodyScreen";
    public string drinkingSceneName = "DrinkingScene";

    public RecipeDisplayUI recipeDisplayUI;
    public RecipeText recipeText;
    public CauldronVisual cauldronVisual;

    [Header("Received Ingredients")]
    public bool hasBodyPart;
    public bool hasBottle;
    public bool hasPlant;
    public string receivedBodyPartName;
    public string receivedBottleName;
    public string receivedPlantName;

    [Header("Result")]
    public bool hasCorrectIngredient;
    public bool hasCorrectBodyPart;
    public bool hasCorrectBottle;
    public bool hasCorrectPlant;
    public bool hasGoodFireTiming;
    public bool potionReady;
    [Range(0f, 100f)] public float potionQuality = 100f;

    bool hasStartedTiming;
    bool hasFireTimingResult;

#if UNITY_EDITOR
    void OnEnable()
    {
        ScheduleEditorRefresh();
    }

    void OnValidate()
    {
        ScheduleEditorRefresh();
    }

    void ScheduleEditorRefresh()
    {
        if (Application.isPlaying)
        {
            return;
        }

        EditorApplication.delayCall -= EnsureRecipeDisplayExistsInEditor;
        EditorApplication.delayCall += EnsureRecipeDisplayExistsInEditor;
    }

    void EnsureRecipeDisplayExistsInEditor()
    {
        if (this == null || Application.isPlaying)
        {
            return;
        }

        SetupNextButtonEditorPreview();

        RecipeData displayRecipe = GetRecipeForDisplay();

        if (recipeText == null)
        {
            recipeText = FindRecipeText();
        }

        if (recipeText != null)
        {
            recipeText.SetRecipe(displayRecipe);
            recipeText.SetIngredientProgress(hasBodyPart, hasBottle, hasPlant);
            DisableLegacyRecipeDisplay();
            DisableUnusedRecipeTexts();

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(recipeText);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            return;
        }

        if (recipeDisplayUI == null)
        {
            recipeDisplayUI = FindFirstObjectByType<RecipeDisplayUI>(FindObjectsInactive.Include);
        }

        if (recipeDisplayUI == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            if (canvas == null)
            {
                return;
            }

            GameObject displayObject = new GameObject(
                "RecipeDisplayUI",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup)
            );

            Undo.RegisterCreatedObjectUndo(displayObject, "Create Recipe Display UI");
            displayObject.transform.SetParent(canvas.transform, false);
            recipeDisplayUI = displayObject.AddComponent<RecipeDisplayUI>();
            recipeDisplayUI.ApplyDefaultTransform();
            recipeDisplayUI.BuildIfNeeded();
        }

        recipeDisplayUI.SetRecipe(displayRecipe);
        recipeDisplayUI.SetIngredientProgress(hasBodyPart, hasBottle, hasPlant);

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(recipeDisplayUI);
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
#endif

    void Start()
    {
        if (RecipeRuntimeData.Instance != null && RecipeRuntimeData.Instance.currentRecipe != null)
        {
            recipeData = RecipeRuntimeData.Instance.currentRecipe;
        }

        SetupTimingBar();
        SetupCauldronVisual();
        SetupNextButton();
        UpdateRecipeDisplay();

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }
    }

    void SetupTimingBar()
    {
        if (timingBar == null)
        {
            timingBar = FindFirstObjectByType<TimingBar>(FindObjectsInactive.Include);
        }
    }

    void SetupNextButton()
    {
        FindNextButton();

        if (nextButton == null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        RectTransform nextButtonRect = nextButton.GetComponent<RectTransform>();

        if (canvas != null && nextButtonRect != null && nextButton.GetComponentInParent<Canvas>() == null)
        {
            nextButtonRect.SetParent(canvas.transform, false);
            nextButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
            nextButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
            nextButtonRect.anchoredPosition = new Vector2(354f, -171f);
        }

        Button button = nextButton.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(GoToDrinkingScene);
            button.onClick.AddListener(GoToDrinkingScene);
            ApplyNextButtonStyle(button);
        }
    }


#if UNITY_EDITOR
    void SetupNextButtonEditorPreview()
    {
        FindNextButton();
        if (nextButton == null)
        {
            return;
        }

        Button button = nextButton.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        ApplyNextButtonStyle(button);
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(nextButton);
        EditorUtility.SetDirty(button);
    }
#endif

    void FindNextButton()
    {
        if (nextButton != null)
        {
            return;
        }

        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == "NextButton")
            {
                nextButton = buttons[i].gameObject;
                return;
            }
        }
    }

    void ApplyNextButtonStyle(Button targetButton)
    {
        Button sourceButton = GetNextButtonStyleSource();
        if (sourceButton == null || targetButton == null || sourceButton == targetButton)
        {
            return;
        }

        RectTransform sourceRect = sourceButton.transform as RectTransform;
        RectTransform targetRect = targetButton.transform as RectTransform;
        if (sourceRect != null && targetRect != null)
        {
            targetRect.sizeDelta = sourceRect.sizeDelta;
        }

        Image sourceImage = sourceButton.GetComponent<Image>();
        Image targetImage = targetButton.GetComponent<Image>();
        if (sourceImage != null && targetImage != null)
        {
            targetImage.sprite = sourceImage.sprite;
            targetImage.type = sourceImage.type;
            targetImage.preserveAspect = sourceImage.preserveAspect;
            targetImage.fillCenter = sourceImage.fillCenter;
            targetImage.fillMethod = sourceImage.fillMethod;
            targetImage.fillAmount = sourceImage.fillAmount;
            targetImage.fillClockwise = sourceImage.fillClockwise;
            targetImage.fillOrigin = sourceImage.fillOrigin;
            targetImage.useSpriteMesh = sourceImage.useSpriteMesh;
            targetImage.pixelsPerUnitMultiplier = sourceImage.pixelsPerUnitMultiplier;
            targetImage.color = sourceImage.color;
            targetImage.material = sourceImage.material;
            targetImage.raycastTarget = sourceImage.raycastTarget;
            targetImage.maskable = sourceImage.maskable;
            targetButton.targetGraphic = targetImage;
        }

        targetButton.transition = sourceButton.transition;
        targetButton.colors = sourceButton.colors;
        targetButton.spriteState = sourceButton.spriteState;
        targetButton.animationTriggers = sourceButton.animationTriggers;
        targetButton.navigation = sourceButton.navigation;
        targetButton.interactable = sourceButton.interactable;

        TextMeshProUGUI sourceText = sourceButton.GetComponentInChildren<TextMeshProUGUI>(true);
        TextMeshProUGUI targetText = targetButton.GetComponentInChildren<TextMeshProUGUI>(true);
        if (sourceText != null && targetText != null)
        {
            string label = string.IsNullOrEmpty(targetText.text) ? "Next" : targetText.text;
            RectTransform sourceTextRect = sourceText.transform as RectTransform;
            RectTransform targetTextRect = targetText.transform as RectTransform;
            if (sourceTextRect != null && targetTextRect != null)
            {
                targetTextRect.anchorMin = sourceTextRect.anchorMin;
                targetTextRect.anchorMax = sourceTextRect.anchorMax;
                targetTextRect.pivot = sourceTextRect.pivot;
                targetTextRect.anchoredPosition = sourceTextRect.anchoredPosition;
                targetTextRect.sizeDelta = sourceTextRect.sizeDelta;
                targetTextRect.offsetMin = sourceTextRect.offsetMin;
                targetTextRect.offsetMax = sourceTextRect.offsetMax;
            }

            targetText.font = sourceText.font;
            targetText.fontSharedMaterial = sourceText.fontSharedMaterial;
            targetText.fontSize = sourceText.fontSize;
            targetText.fontStyle = sourceText.fontStyle;
            targetText.enableAutoSizing = sourceText.enableAutoSizing;
            targetText.fontSizeMin = sourceText.fontSizeMin;
            targetText.fontSizeMax = sourceText.fontSizeMax;
            targetText.color = sourceText.color;
            targetText.alignment = sourceText.alignment;
            targetText.margin = sourceText.margin;
            targetText.raycastTarget = sourceText.raycastTarget;
            targetText.text = label;
        }
    }

    Button GetNextButtonStyleSource()
    {
        if (nextButtonStyleSource != null)
        {
            return nextButtonStyleSource;
        }

        if (string.IsNullOrEmpty(nextButtonStyleSourceName))
        {
            return null;
        }

        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (string.Equals(buttons[i].name, nextButtonStyleSourceName, System.StringComparison.OrdinalIgnoreCase))
            {
                nextButtonStyleSource = buttons[i];
                return nextButtonStyleSource;
            }
        }

        return null;
    }
    public void SetRecipe(RecipeData newRecipe)
    {
        recipeData = newRecipe;
        ResetMachineProgress();

        if (recipeData != null)
        {
            Debug.Log(
                "Potion machine recipe set to: " +
                recipeData.name +
                " | Body: " + recipeData.requiredBodyPartName +
                " | Bottle: " + recipeData.requiredBottleName +
                " | Plant: " + recipeData.requiredPlantName
            );
        }

        UpdateRecipeDisplay();
    }

    void ResetMachineProgress()
    {
        hasBodyPart = false;
        hasBottle = false;
        hasPlant = false;
        receivedBodyPartName = "";
        receivedBottleName = "";
        receivedPlantName = "";

        hasCorrectIngredient = false;
        hasCorrectBodyPart = false;
        hasCorrectBottle = false;
        hasCorrectPlant = false;
        hasGoodFireTiming = false;
        potionReady = false;
        potionQuality = 100f;
        hasStartedTiming = false;
        hasFireTimingResult = false;
        SetupCauldronVisual();
        if (cauldronVisual != null)
        {
            cauldronVisual.ShowDefault();
        }

        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        UpdateRecipeDisplay();
    }

    public void ReceiveIngredient(IngredientPiece piece)
    {
        if (piece == null || recipeData == null)
        {
            return;
        }

        IngredientType pieceType = GetPieceType(piece);

        if (pieceType == IngredientType.Unknown)
        {
            Debug.LogWarning("Unknown ingredient type: " + piece.ingredientName, this);
            return;
        }

        if (pieceType == IngredientType.BodyPart)
        {
            hasBodyPart = true;
            receivedBodyPartName = piece.ingredientName;
            hasCorrectBodyPart = IsCorrectBodyPart(piece);
        }
        else if (pieceType == IngredientType.Bottle)
        {
            hasBottle = true;
            receivedBottleName = piece.ingredientName;
            hasCorrectBottle = NamesMatch(piece.ingredientName, recipeData.requiredBottleName);
        }
        else if (pieceType == IngredientType.Plant)
        {
            hasPlant = true;
            receivedPlantName = piece.ingredientName;
            hasCorrectPlant = NamesMatch(piece.ingredientName, recipeData.requiredPlantName);
        }

        hasCorrectIngredient = hasCorrectBodyPart && hasCorrectBottle && hasCorrectPlant;
        UpdatePotionQuality();
        UpdateRecipeDisplay();

        Debug.Log(
            "Cauldron received " + pieceType + ": " + piece.ingredientName +
            " | All types ready: " + HasAllIngredientTypes() +
            " | Ingredients correct: " + hasCorrectIngredient +
            " | Quality: " + potionQuality.ToString("0")
        );

        TryStartTiming();
    }

    IngredientType GetPieceType(IngredientPiece piece)
    {
        if (piece.ingredientType != IngredientType.Unknown)
        {
            return piece.ingredientType;
        }

        if (NamesMatch(piece.ingredientName, recipeData.requiredBodyPartName))
        {
            return IngredientType.BodyPart;
        }

        if (NamesMatch(piece.ingredientName, recipeData.requiredBottleName))
        {
            return IngredientType.Bottle;
        }

        if (NamesMatch(piece.ingredientName, recipeData.requiredPlantName))
        {
            return IngredientType.Plant;
        }

        return IngredientType.Unknown;
    }

    bool IsCorrectBodyPart(IngredientPiece piece)
    {
        return NamesMatch(piece.ingredientName, recipeData.requiredBodyPartName);
    }

    bool HasAllIngredientTypes()
    {
        return hasBodyPart && hasBottle && hasPlant;
    }

    void TryStartTiming()
    {
        if (hasStartedTiming || !HasAllIngredientTypes())
        {
            return;
        }

        hasStartedTiming = true;
        SetupTimingBar();

        if (timingBar != null)
        {
            Debug.Log("All three ingredient types are in the cauldron. Start fire timing.");
            timingBar.StartTiming();
        }
        else
        {
            Debug.LogWarning("All ingredients are ready, but no TimingBar was found.", this);
        }
    }

    public void ReceiveFireTimingResult(bool success)
    {
        hasFireTimingResult = true;
        hasGoodFireTiming = success;
        potionReady = HasAllIngredientTypes();
        UpdatePotionQuality();
        UpdateRecipeDisplay();
        SavePotionResultToRuntime();
        UpdateCauldronVisual();

        if (!potionReady)
        {
            Debug.Log("Potion is missing ingredients.");
            return;
        }

        Debug.Log(
            "Potion finished. Fire timing: " +
            (hasGoodFireTiming ? "Good" : "Bad") +
            " | Ingredient match: " + hasCorrectIngredient +
            " | Quality: " + potionQuality.ToString("0")
        );

        SetupNextButton();

        if (nextButton != null)
        {
            nextButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Potion is finished, but no NextButton was found.", this);
        }
    }

    void UpdatePotionQuality()
    {
        float quality = 100f;

        if (hasBodyPart && !hasCorrectBodyPart)
        {
            quality -= 30f;
        }

        if (hasBottle && !hasCorrectBottle)
        {
            quality -= 25f;
        }

        if (hasPlant && !hasCorrectPlant)
        {
            quality -= 25f;
        }

        if (hasFireTimingResult && !hasGoodFireTiming)
        {
            quality -= 20f;
        }

        potionQuality = Mathf.Clamp(quality, 0f, 100f);
    }

    void SetupCauldronVisual()
    {
        if (cauldronVisual == null)
        {
            cauldronVisual = FindFirstObjectByType<CauldronVisual>(FindObjectsInactive.Include);
        }
    }

    void UpdateCauldronVisual()
    {
        SetupCauldronVisual();

        if (cauldronVisual == null)
        {
            return;
        }

        if (hasCorrectIngredient && hasGoodFireTiming)
        {
            cauldronVisual.ShowVitriol();
        }
        else
        {
            cauldronVisual.ShowFailed();
        }
    }

    bool NamesMatch(string firstName, string secondName)
    {
        string first = NormalizeName(firstName);
        string second = NormalizeName(secondName);

        if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(second))
        {
            return false;
        }

        return first == second || first.Contains(second) || second.Contains(first);
    }

    string NormalizeName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return "";
        }

        string result = "";
        string lowerName = rawName.ToLowerInvariant();

        for (int i = 0; i < lowerName.Length; i++)
        {
            char currentChar = lowerName[i];
            if (char.IsLetterOrDigit(currentChar))
            {
                result += currentChar;
            }
        }

        return result;
    }

    void SetupRecipeDisplay()
    {
        if (recipeText == null)
        {
            recipeText = FindRecipeText();
        }

        if (recipeText != null)
        {
            DisableLegacyRecipeDisplay();
            DisableUnusedRecipeTexts();
            return;
        }

        if (recipeDisplayUI == null)
        {
            recipeDisplayUI = FindFirstObjectByType<RecipeDisplayUI>(FindObjectsInactive.Include);
        }

        if (recipeDisplayUI == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            recipeDisplayUI = RecipeDisplayUI.GetOrCreate(canvas);
        }
    }

    public void RefreshRecipeDisplayPreview()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            SetupNextButtonEditorPreview();
        }
#endif

        UpdateRecipeDisplay();

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
            if (recipeText != null)
            {
                EditorUtility.SetDirty(recipeText);
            }

            if (recipeDisplayUI != null)
            {
                EditorUtility.SetDirty(recipeDisplayUI);
            }

            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    void UpdateRecipeDisplay()
    {
        SetupRecipeDisplay();
        RecipeData displayRecipe = GetRecipeForDisplay();

        if (recipeText != null)
        {
            recipeText.SetRecipe(displayRecipe);
            recipeText.SetIngredientProgress(hasBodyPart, hasBottle, hasPlant);
            return;
        }

        if (recipeDisplayUI == null)
        {
            return;
        }

        recipeDisplayUI.SetRecipe(displayRecipe);
        recipeDisplayUI.SetIngredientProgress(hasBodyPart, hasBottle, hasPlant);
    }

    RecipeData GetRecipeForDisplay()
    {
        if (recipeData != null)
        {
            return recipeData;
        }

        if (RecipeRuntimeData.Instance != null && RecipeRuntimeData.Instance.currentRecipe != null)
        {
            return RecipeRuntimeData.Instance.currentRecipe;
        }

        RecipeRandomizer randomizer = FindFirstObjectByType<RecipeRandomizer>(FindObjectsInactive.Include);
        if (randomizer != null)
        {
            return randomizer.GetRecipeForCurrentEncounter();
        }

        return null;
    }

    RecipeText FindRecipeText()
    {
        RecipeText[] recipeTexts = FindObjectsByType<RecipeText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < recipeTexts.Length; i++)
        {
            if (HasParentNamed(recipeTexts[i].transform, "Panel"))
            {
                return recipeTexts[i];
            }
        }

        if (recipeTexts.Length > 0)
        {
            return recipeTexts[0];
        }

        return null;
    }

    bool HasParentNamed(Transform child, string parentName)
    {
        Transform current = child;
        while (current != null)
        {
            if (current.name == parentName)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    void DisableLegacyRecipeDisplay()
    {
        if (recipeDisplayUI == null)
        {
            recipeDisplayUI = FindFirstObjectByType<RecipeDisplayUI>(FindObjectsInactive.Include);
        }

        if (recipeDisplayUI != null)
        {
            recipeDisplayUI.gameObject.SetActive(false);
        }
    }

    void DisableUnusedRecipeTexts()
    {
        if (recipeText == null)
        {
            return;
        }

        RecipeText[] recipeTexts = FindObjectsByType<RecipeText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < recipeTexts.Length; i++)
        {
            if (recipeTexts[i] != recipeText)
            {
                recipeTexts[i].gameObject.SetActive(false);
            }
        }
    }

    void SavePotionResultToRuntime()
    {
        if (RecipeRuntimeData.Instance != null)
        {
            RecipeRuntimeData.Instance.SetPotionResult(
                potionQuality,
                hasCorrectIngredient,
                hasGoodFireTiming
            );
        }
    }

    public void GoToDrinkingScene()
    {
        if (!potionReady)
        {
            Debug.Log("Potion is not ready yet.");
            return;
        }

        SavePotionResultToRuntime();
        SceneManager.LoadScene(drinkingSceneName);
    }
}
