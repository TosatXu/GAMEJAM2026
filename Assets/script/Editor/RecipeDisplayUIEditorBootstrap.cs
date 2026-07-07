#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class RecipeDisplayUIEditorBootstrap
{
    static RecipeDisplayUIEditorBootstrap()
    {
        EditorApplication.delayCall += EnsureRecipeDisplayExists;
    }

    static void EnsureRecipeDisplayExists()
    {
        if (Application.isPlaying)
        {
            return;
        }

        PotionMechanicsManager manager = UnityEngine.Object.FindFirstObjectByType<PotionMechanicsManager>(FindObjectsInactive.Include);
        if (manager == null)
        {
            return;
        }

        RecipeDisplayUI displayUI = manager.recipeDisplayUI;
        if (displayUI == null)
        {
            displayUI = UnityEngine.Object.FindFirstObjectByType<RecipeDisplayUI>(FindObjectsInactive.Include);
        }

        if (displayUI == null)
        {
            Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            if (canvas == null)
            {
                return;
            }

            GameObject displayObject = new GameObject(
                "RecipeDisplayUI",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup)
            );

            Undo.RegisterCreatedObjectUndo(displayObject, "Create Recipe Display UI");
            displayObject.transform.SetParent(canvas.transform, false);
            displayUI = displayObject.AddComponent<RecipeDisplayUI>();
            displayUI.ApplyDefaultTransform();
            displayUI.BuildIfNeeded();
        }

        manager.recipeDisplayUI = displayUI;
        displayUI.SetRecipe(manager.recipeData);
        displayUI.SetIngredientProgress(manager.hasBodyPart, manager.hasBottle, manager.hasPlant);

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(displayUI);
        EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
    }
}
#endif


