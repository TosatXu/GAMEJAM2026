using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class RecipeText : MonoBehaviour
{
    public string recipe1;
    public string recipe2;
    public string recipe3;
    public bool showProgress = true;

    TextMeshProUGUI recipeText;
    RecipeData currentRecipe;
    bool hasBodyPart;
    bool hasBottle;
    bool hasPlant;

    void Awake()
    {
        SetupText();
        RefreshText();
    }

    void OnEnable()
    {
        SetupText();
        RefreshText();
    }

    void OnValidate()
    {
        SetupText();
        RefreshText();
    }

    void Start()
    {
        SetupText();

        if (currentRecipe == null && RecipeRuntimeData.Instance != null)
        {
            currentRecipe = RecipeRuntimeData.Instance.currentRecipe;
        }

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

    void SetupText()
    {
        if (recipeText == null)
        {
            recipeText = GetComponent<TextMeshProUGUI>();
        }

        if (recipeText == null)
        {
            return;
        }

        recipeText.richText = true;
        recipeText.raycastTarget = false;
    }

    void RefreshText()
    {
        SetupText();

        if (recipeText == null)
        {
            return;
        }

        if (currentRecipe == null)
        {
            recipeText.text =
                "Recipe\n\n" +
                GetLine(false, "Body", "???") + "\n" +
                GetLine(false, "Bottle", "???") + "\n" +
                GetLine(false, "Plant", "???");
            return;
        }

        recipeText.text =
            "Recipe\n\n" +
            GetLine(hasBodyPart, "Body", currentRecipe.requiredBodyPartName) + "\n" +
            GetLine(hasBottle, "Bottle", currentRecipe.requiredBottleName) + "\n" +
            GetLine(hasPlant, "Plant", currentRecipe.requiredPlantName);
    }

    string GetLine(bool isDone, string label, string ingredientName)
    {
        string boxText = showProgress ? GetBoxText(isDone) + " " : "";
        return boxText + "1 x " + label + ": " + FormatName(ingredientName);
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

        if (string.IsNullOrWhiteSpace(rawName))
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
