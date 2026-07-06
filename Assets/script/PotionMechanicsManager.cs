using UnityEngine;

public class PotionMechanicsManager : MonoBehaviour
{
    [Header("Current Recipe")]
    public string requiredIngredientName = "heart";
    public float requiredPercent = 30f;
    public float percentTolerance = 5f;

    [Header("State")]
    public bool hasCorrectIngredient;

    public void ReceiveIngredient(IngredientPiece piece)
    {
        if (piece == null)
        {
            return;
        }

        bool correctName = piece.ingredientName == requiredIngredientName;
        bool correctPercent = piece.IsCloseToPercent(requiredPercent, percentTolerance);

        if (correctName && correctPercent)
        {
            hasCorrectIngredient = true;

            Debug.Log(
                "Correct ingredient! " +
                piece.ingredientName +
                " " +
                piece.ingredientPercent.ToString("0.0") +
                "%"
            );

            Debug.Log("Next step: start fire timing bar.");
        }
        else
        {
            hasCorrectIngredient = false;

            Debug.Log(
                "Wrong ingredient. Need: " +
                requiredIngredientName +
                " " +
                requiredPercent.ToString("0") +
                "% | Got: " +
                piece.ingredientName +
                " " +
                piece.ingredientPercent.ToString("0.0") +
                "%"
            );
        }
    }
}
