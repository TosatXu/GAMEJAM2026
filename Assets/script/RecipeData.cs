using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alchemy/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("Ingredient")]
    public string requiredIngredientName = "heart";
    public float requiredIngredientPercent = 30f;
    public float ingredientTolerance = 5f;

    [Header("Drinking")]
    public float requiredDrinkPercent = 60f;
    public float drinkTolerance = 5f;
}