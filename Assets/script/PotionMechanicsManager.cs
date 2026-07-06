using UnityEngine;

public class PotionMechanicsManager : MonoBehaviour
{
    [Header("Current Recipe")]
    public string requiredIngredientName = "heart";
    public float requiredPercent = 30f;
    public float percentTolerance = 5f;

    [Header("Other Systems")]
    public TimingBar timingBar;

    [Header("State")]
    public bool hasCorrectIngredient;
    public bool hasGoodFireTiming;
    public bool potionReady;

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

            Debug.Log("Correct ingredient. Start fire timing.");

            if (timingBar != null)
            {
                timingBar.StartTiming();
            }
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

    public void ReceiveFireTimingResult(bool success)
    {
        hasGoodFireTiming = success;

        if (hasCorrectIngredient && hasGoodFireTiming)
        {
            potionReady = true;
            Debug.Log("Potion is ready!");
        }
        else
        {
            potionReady = false;
            Debug.Log("Potion failed.");
        }
    }
}
