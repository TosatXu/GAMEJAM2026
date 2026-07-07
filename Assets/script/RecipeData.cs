using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Alchemy/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("Recipe")]
    [FormerlySerializedAs("requiredIngredientName")]
    public string requiredBodyPartName = "heart";

    public string requiredBottleName = "water";
    public string requiredPlantName = "nettle";

    [Header("Drinking")]
    public float requiredDrinkPercent = 60f;
}
