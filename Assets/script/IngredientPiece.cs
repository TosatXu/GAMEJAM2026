using UnityEngine;

public class IngredientPiece : MonoBehaviour
{
    public string ingredientName;
    public float ingredientPercent;

    public void SetIngredientData(string newName, float newPercent)
    {
        ingredientName = newName;
        ingredientPercent = newPercent;
    }

    public bool IsCloseToPercent(float targetPercent, float tolerance)
    {
        float difference = Mathf.Abs(ingredientPercent - targetPercent);
        return difference <= tolerance;
    }
}