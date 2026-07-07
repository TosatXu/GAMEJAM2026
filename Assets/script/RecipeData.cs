using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        EditorApplication.delayCall -= RefreshOpenRecipeDisplays;
        EditorApplication.delayCall += RefreshOpenRecipeDisplays;
    }

    static void RefreshOpenRecipeDisplays()
    {
        if (Application.isPlaying)
        {
            return;
        }

        PotionMechanicsManager[] managers = UnityEngine.Object.FindObjectsByType<PotionMechanicsManager>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i] != null)
            {
                managers[i].RefreshRecipeDisplayPreview();
            }
        }
    }
#endif
}