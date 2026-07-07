using UnityEngine;

public class IngredientPiece : MonoBehaviour
{
    public string ingredientName;
    public IngredientType ingredientType = IngredientType.Unknown;
    public float ingredientPercent;

    public void SetIngredientData(string newName, IngredientType newType, float newPercent)
    {
        ingredientName = newName;
        ingredientType = newType;
        ingredientPercent = newPercent;
    }

    public void SetIngredientData(string newName, float newPercent)
    {
        SetIngredientData(newName, IngredientType.Unknown, newPercent);
    }

    public bool IsCloseToPercent(float targetPercent, float tolerance)
    {
        float difference = Mathf.Abs(ingredientPercent - targetPercent);
        return difference <= tolerance;
    }
}
