using UnityEngine;

public class IngredientButton : MonoBehaviour
{
    public GameObject Ingredient;
    public GameObject IngredientSpawner;
    public void spawnIngredient ()
    {
        Instantiate(Ingredient, IngredientSpawner.transform.position, IngredientSpawner.transform.rotation, IngredientSpawner.transform);
    }
    
    public void spawnCuttingBoard ()
    {

    }
}
