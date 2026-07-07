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
    public string drinkingSceneName = "DrinkingScene";

    public RecipeDisplayUI recipeDisplayUI;

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
    void OnValidate()
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

        recipeDisplayUI.SetRecipe(recipeData);
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
        if (nextButton == null)
        {
            nextButton = GameObject.Find("NextButton");
        }

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
        }
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

    void UpdateRecipeDisplay()
    {
        SetupRecipeDisplay();

        if (recipeDisplayUI == null)
        {
            return;
        }

        recipeDisplayUI.SetRecipe(recipeData);
        recipeDisplayUI.SetIngredientProgress(hasBodyPart, hasBottle, hasPlant);
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




