using UnityEngine;

public class DrinkingAmount : MonoBehaviour
{
    public Transform barFill;
    public float fillSpeed = 40f;
    public float requiredDrinkPercent = 60f;
    public float drinkTolerance = 5f;

    float currentDrinkPercent;
    bool isDrinking;
    bool hasFinished;

    void Start()
    {
        if (RecipeRuntimeData.Instance != null && RecipeRuntimeData.Instance.currentRecipe != null)
        {
            requiredDrinkPercent = RecipeRuntimeData.Instance.currentRecipe.requiredDrinkPercent;
        }

        currentDrinkPercent = 0f;
        UpdateBar();
    }

    void Update()
    {
        if (hasFinished)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isDrinking = true;
        }

        if (Input.GetMouseButton(0) && isDrinking)
        {
            currentDrinkPercent += fillSpeed * Time.deltaTime;
            currentDrinkPercent = Mathf.Clamp(currentDrinkPercent, 0f, 100f);
            UpdateBar();
        }

        if (Input.GetMouseButtonUp(0) && isDrinking)
        {
            isDrinking = false;
            hasFinished = true;
            CheckDrinkAmount();
        }
    }

    void UpdateBar()
    {
        if (barFill == null)
        {
            return;
        }

        float fill01 = currentDrinkPercent / 100f;
        barFill.localScale = new Vector3(fill01, barFill.localScale.y, barFill.localScale.z);
    }

    void CheckDrinkAmount()
    {
        bool success = Mathf.Abs(currentDrinkPercent - requiredDrinkPercent) <= drinkTolerance;

        Debug.Log(
            "Drink target: " +
            requiredDrinkPercent.ToString("0") +
            "% | Actual: " +
            currentDrinkPercent.ToString("0.0") +
            "% | " +
            (success ? "Good drinking amount" : "Wrong drinking amount")
        );
    }
}
