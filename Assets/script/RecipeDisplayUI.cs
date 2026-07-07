using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RecipeDisplayUI : MonoBehaviour
{
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyPartText;
    public TextMeshProUGUI bottleText;
    public TextMeshProUGUI plantText;
    public TextMeshProUGUI drinkText;
    public bool showDrinkPercent;

    public Vector2 panelSize = new Vector2(270f, 210f);
    public Vector2 panelOffset = new Vector2(-24f, 20f);
    public Color panelColor = new Color(0.11f, 0.10f, 0.09f, 0.88f);
    public Color titleColor = new Color(0.96f, 0.88f, 0.68f, 1f);
    public Color textColor = new Color(0.94f, 0.91f, 0.84f, 1f);

    RecipeData currentRecipe;
    bool hasBodyPart;
    bool hasBottle;
    bool hasPlant;

    public static RecipeDisplayUI GetOrCreate(Canvas canvas)
    {
        RecipeDisplayUI existingDisplay = FindFirstObjectByType<RecipeDisplayUI>(FindObjectsInactive.Include);
        if (existingDisplay != null)
        {
            existingDisplay.BuildIfNeeded();
            return existingDisplay;
        }

        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            canvas = CreateCanvas();
        }

        GameObject displayObject = new GameObject(
            "RecipeDisplayUI",
            typeof(RectTransform),
            typeof(Image),
            typeof(VerticalLayoutGroup)
        );

        displayObject.transform.SetParent(canvas.transform, false);
        RecipeDisplayUI displayUI = displayObject.AddComponent<RecipeDisplayUI>();
        displayUI.ApplyDefaultTransform();
        displayUI.BuildIfNeeded();
        return displayUI;
    }

    static Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(
            "Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    void Awake()
    {
        BuildIfNeeded();
    }

    void OnEnable()
    {
        BuildIfNeeded();
    }

    void OnValidate()
    {
        BuildIfNeeded();
    }

    public void BuildIfNeeded()
    {
        panel = transform as RectTransform;
        if (panel == null)
        {
            return;
        }

        SetupBackground();
        SetupLayoutGroup();

        titleText = FindOrCreateText("Title", "RECIPE", 25f, titleColor, FontStyles.Bold, 34f);
        bodyPartText = FindOrCreateText("BodyPart", "", 19f, textColor, FontStyles.Normal, 30f);
        bottleText = FindOrCreateText("Bottle", "", 19f, textColor, FontStyles.Normal, 30f);
        plantText = FindOrCreateText("Plant", "", 19f, textColor, FontStyles.Normal, 30f);
        drinkText = FindOrCreateText("Drink", "", 17f, textColor, FontStyles.Normal, 28f);
        SetTextVisible(drinkText, showDrinkPercent);

        RefreshText();
    }

    public void SetRecipe(RecipeData recipe)
    {
        currentRecipe = recipe;
        RefreshText();
    }

    public void SetIngredientProgress(bool newHasBodyPart, bool newHasBottle, bool newHasPlant)
    {
        hasBodyPart = newHasBodyPart;
        hasBottle = newHasBottle;
        hasPlant = newHasPlant;
        RefreshText();
    }

    public void ApplyDefaultTransform()
    {
        panel = transform as RectTransform;
        if (panel == null)
        {
            return;
        }

        panel.anchorMin = new Vector2(1f, 0.5f);
        panel.anchorMax = new Vector2(1f, 0.5f);
        panel.pivot = new Vector2(1f, 0.5f);
        panel.sizeDelta = panelSize;
        panel.anchoredPosition = panelOffset;
    }

    void SetupBackground()
    {
        Image background = GetComponent<Image>();
        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
        }

        background.color = panelColor;
        background.raycastTarget = false;
    }

    void SetupLayoutGroup()
    {
        VerticalLayoutGroup layoutGroup = GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.padding = new RectOffset(18, 18, 14, 14);
        layoutGroup.spacing = 9f;
        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
    }

    TextMeshProUGUI FindOrCreateText(
        string objectName,
        string defaultText,
        float fontSize,
        Color color,
        FontStyles fontStyle,
        float minHeight
    )
    {
        Transform textTransform = transform.Find(objectName);
        GameObject textObject;

        if (textTransform == null)
        {
            textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            textObject.transform.SetParent(transform, false);
        }
        else
        {
            textObject = textTransform.gameObject;
            textObject.SetActive(true);
        }

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        if (text == null)
        {
            text = textObject.AddComponent<TextMeshProUGUI>();
        }

        LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = textObject.AddComponent<LayoutElement>();
        }

        if (string.IsNullOrEmpty(text.text))
        {
            text.text = defaultText;
        }

        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = TextAlignmentOptions.Left;
        text.enableWordWrapping = true;
        text.raycastTarget = false;

        layoutElement.minHeight = minHeight;
        layoutElement.flexibleWidth = 1f;

        return text;
    }

    void SetTextVisible(TextMeshProUGUI text, bool isVisible)
    {
        if (text != null)
        {
            text.gameObject.SetActive(isVisible);
        }
    }

    void RefreshText()
    {
        if (titleText == null || bodyPartText == null || bottleText == null || plantText == null)
        {
            return;
        }

        if (currentRecipe == null)
        {
            titleText.text = "RECIPE";
            bodyPartText.text = "[ ] Body: ???";
            bottleText.text = "[ ] Bottle: ???";
            plantText.text = "[ ] Plant: ???";

            if (drinkText != null)
            {
                drinkText.text = "Drink: ???";
                SetTextVisible(drinkText, showDrinkPercent);
            }

            return;
        }

        titleText.text = "RECIPE";
        bodyPartText.text = GetBoxText(hasBodyPart) + " Body: " + FormatName(currentRecipe.requiredBodyPartName);
        bottleText.text = GetBoxText(hasBottle) + " Bottle: " + FormatName(currentRecipe.requiredBottleName);
        plantText.text = GetBoxText(hasPlant) + " Plant: " + FormatName(currentRecipe.requiredPlantName);

        if (drinkText != null)
        {
            drinkText.text = "Drink: " + currentRecipe.requiredDrinkPercent.ToString("0") + "%";
            SetTextVisible(drinkText, showDrinkPercent);
        }
    }

    string GetBoxText(bool isDone)
    {
        return isDone ? "[x]" : "[ ]";
    }

    string FormatName(string rawName)
    {
        string normalized = NormalizeName(rawName);

        if (normalized == "aquaregia") return "Aqua Regia";
        if (normalized == "oilofvitriol") return "Oil of Vitriol";
        if (normalized == "ghostorchid" || normalized == "whiteghostorchid") return "Ghost Orchid";
        if (normalized == "singingnettle") return "Singing Nettle";
        if (normalized == "plaguecitrine") return "Plague Citrine";
        if (normalized == "batfang") return "Bat Fang";
        if (normalized == "dreamlily") return "Dream Lily";
        if (normalized == "baneberry") return "Baneberry";
        if (normalized == "eyeball") return "Eyeball";

        if (string.IsNullOrEmpty(rawName))
        {
            return "???";
        }

        string cleanName = rawName.Trim();
        if (cleanName.Length == 1)
        {
            return cleanName.ToUpperInvariant();
        }

        return char.ToUpperInvariant(cleanName[0]) + cleanName.Substring(1);
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
}
