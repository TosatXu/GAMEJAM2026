using UnityEngine;

public class RecipeRandomizer : MonoBehaviour
{
    public RecipeData[] possibleRecipes;
    public PotionMechanicsManager potionMechanicsManager;

    void Start()
    {
        PickRecipeForCurrentEncounter();
    }

    public void PickRandomRecipe()
    {
        PickRecipeForCurrentEncounter();
    }

    public void PickRecipeForCurrentEncounter()
    {
        if (possibleRecipes == null || possibleRecipes.Length == 0)
        {
            Debug.LogWarning("No recipes in RecipeRandomizer.");
            return;
        }

        int encounterNumber = PlayerPrefs.GetInt("encounterNum", 0);
        int recipeCount = Mathf.Min(possibleRecipes.Length, 3);
        int recipeIndex = GetPositiveModulo(encounterNumber, recipeCount);
        RecipeData selectedRecipe = possibleRecipes[recipeIndex];

        if (selectedRecipe == null)
        {
            Debug.LogWarning("Selected recipe slot is empty: " + recipeIndex, this);
            return;
        }

        RecipeRuntimeData runtimeData = RecipeRuntimeData.Instance;

        if (runtimeData != null)
        {
            runtimeData.SetRecipe(selectedRecipe);
        }

        if (potionMechanicsManager != null)
        {
            potionMechanicsManager.SetRecipe(selectedRecipe);
        }

        Debug.Log(
            "Selected recipe for NPC " +
            (recipeIndex + 1) +
            ": " +
            selectedRecipe.name
        );
    }

    int GetPositiveModulo(int value, int modulo)
    {
        return ((value % modulo) + modulo) % modulo;
    }
}
