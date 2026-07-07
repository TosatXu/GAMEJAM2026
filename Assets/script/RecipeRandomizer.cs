using UnityEngine;

public class RecipeRandomizer : MonoBehaviour
{
    public RecipeData[] possibleRecipes;
    public PotionMechanicsManager potionMechanicsManager;

    void Start()
    {
        PickRandomRecipe();
    }

    public void PickRandomRecipe()
    {
        if (possibleRecipes == null || possibleRecipes.Length == 0)
        {
            Debug.LogWarning("No recipes in RecipeRandomizer.");
            return;
        }

        int randomIndex = Random.Range(0, possibleRecipes.Length);
        RecipeData selectedRecipe = possibleRecipes[randomIndex];

        RecipeRuntimeData runtimeData = RecipeRuntimeData.Instance;

        if (runtimeData != null)
        {
            runtimeData.SetRecipe(selectedRecipe);
        }

        if (potionMechanicsManager != null)
        {
            potionMechanicsManager.SetRecipe(selectedRecipe);
        }

        Debug.Log("Selected recipe: " + selectedRecipe.name);
    }
}