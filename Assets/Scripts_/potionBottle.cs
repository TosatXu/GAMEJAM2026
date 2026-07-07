using UnityEngine;

public class potionBottle : MonoBehaviour
{

    public Sprite green;
    public Sprite tears;
    public Sprite brown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality <= 30)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = brown;
        }
        else if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality >= 80)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = green;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = tears;
        }
    }

}
