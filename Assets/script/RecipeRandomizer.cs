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
        RecipeData selectedRecipe = GetRecipeForCurrentEncounter();
        if (selectedRecipe == null)
        {
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
            (GetCurrentEncounterRecipeIndex() + 1) +
            ": " +
            selectedRecipe.name
        );
    }

    public RecipeData GetRecipeForCurrentEncounter()
    {
        if (possibleRecipes == null || possibleRecipes.Length == 0)
        {
            Debug.LogWarning("No recipes in RecipeRandomizer.", this);
            return null;
        }

        int recipeIndex = GetCurrentEncounterRecipeIndex();
        RecipeData selectedRecipe = possibleRecipes[recipeIndex];

        if (selectedRecipe == null)
        {
            Debug.LogWarning("Selected recipe slot is empty: " + recipeIndex, this);
            return null;
        }

        return selectedRecipe;
    }

    public int GetCurrentEncounterRecipeIndex()
    {
        if (possibleRecipes == null || possibleRecipes.Length == 0)
        {
            return 0;
        }

        int encounterNumber = PlayerPrefs.GetInt("encounterNum", 0);
        int recipeCount = Mathf.Min(possibleRecipes.Length, 3);
        return GetPositiveModulo(encounterNumber, recipeCount);
    }

    int GetPositiveModulo(int value, int modulo)
    {
        return ((value % modulo) + modulo) % modulo;
    }
}