using UnityEngine;

public class RecipeRuntimeData : MonoBehaviour
{
    public static RecipeRuntimeData Instance;

    public RecipeData currentRecipe;

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
}