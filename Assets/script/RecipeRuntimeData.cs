using UnityEngine;

public class RecipeRuntimeData : MonoBehaviour
{
    public static RecipeRuntimeData Instance;

    public RecipeData currentRecipe;
    public float lastPotionQuality = 100f;
    public bool lastPotionHadCorrectIngredients;
    public bool lastPotionHadGoodFireTiming;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetRecipe(RecipeData recipe)
    {
        currentRecipe = recipe;
    }

    public void SetPotionResult(float quality, bool hadCorrectIngredients, bool hadGoodFireTiming)
    {
        lastPotionQuality = quality;
        lastPotionHadCorrectIngredients = hadCorrectIngredients;
        lastPotionHadGoodFireTiming = hadGoodFireTiming;
    }
}
