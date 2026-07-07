using TMPro;
using UnityEngine;

public class reactions : MonoBehaviour
{
    public string[] reaction1 = new string[3];
    public string[] reaction2 = new string[3];
    public string[] reaction3 = new string[3];
    public TextMeshProUGUI display;

    private void Start()
    {
        if (PlayerPrefs.GetInt("encounterNum") == 0)
        {
            if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality <= 30)
            {
                display.text = reaction1[0];
            }
            else if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality >= 80)
            {
                display.text = reaction1[2];
            }
            else
            {
                display.text = reaction1[1];
            }
        }
        if (PlayerPrefs.GetInt("encounterNum") == 1)
        {
            if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality <= 30)
            {
                display.text = reaction2[0];
            }
            else if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality >= 80)
            {
                display.text = reaction2[2];
            }
            else
            {
                display.text = reaction2[1];
            }
        }
        if (PlayerPrefs.GetInt("encounterNum") == 2)
        {
            if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality <= 30)
            {
                display.text = reaction3[0];
            }
            else if (GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality >= 80)
            {
                display.text = reaction3[2];
            }
            else
            {
                display.text = reaction3[1];
            }
        }
    }
}
