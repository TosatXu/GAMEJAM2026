using UnityEngine;
using UnityEngine.SceneManagement;

public class PotionMechanicsManager : MonoBehaviour
{
    public RecipeData recipeData;

    public TimingBar timingBar;

    public GameObject nextButton;
    public string drinkingSceneName = "DrinkingScene";

    public bool hasCorrectIngredient;
    public bool hasGoodFireTiming;
    public bool potionReady;

    void Start()
    {
        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }
    }

    public void ReceiveIngredient(IngredientPiece piece)
    {
        if (piece == null || recipeData == null)
        {
            return;
        }

        bool correctName = piece.ingredientName == recipeData.requiredIngredientName;
        bool correctPercent = piece.IsCloseToPercent(
            recipeData.requiredIngredientPercent,
            recipeData.ingredientTolerance
        );

        if (correctName && correctPercent)
        {
            hasCorrectIngredient = true;
            Debug.Log("Correct ingredient. Start fire timing.");

            if (timingBar != null)
            {
                timingBar.StartTiming();
            }
        }
        else
        {
            hasCorrectIngredient = false;

            Debug.Log(
                "Wrong ingredient. Need: " +
                recipeData.requiredIngredientName +
                " " +
                recipeData.requiredIngredientPercent.ToString("0") +
                "% | Got: " +
                piece.ingredientName +
                " " +
                piece.ingredientPercent.ToString("0.0") +
                "%"
            );
        }
    }

    public void ReceiveFireTimingResult(bool success)
    {
        hasGoodFireTiming = success;

        if (hasCorrectIngredient && hasGoodFireTiming)
        {
            potionReady = true;
            Debug.Log("Potion is ready!");

            if (nextButton != null)
            {
                nextButton.SetActive(true);
            }
        }
        else
        {
            potionReady = false;
            Debug.Log("Potion failed.");
        }
    }

    public void GoToDrinkingScene()
    {
        if (!potionReady)
        {
            Debug.Log("Potion is not ready yet.");
            return;
        }

        SceneManager.LoadScene(drinkingSceneName);
    }
}
