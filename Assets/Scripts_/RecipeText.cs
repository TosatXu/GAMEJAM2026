using TMPro;
using UnityEngine;

public class RecipeText : MonoBehaviour
{
    public string recipe1;
    public string recipe2;
    public string recipe3;

    void Start()
    {
        if (PlayerPrefs.GetInt("encounterNum") % 3 == 0)
        {
            this.GetComponent<TextMeshProUGUI>().text = recipe1;
        }
        if (PlayerPrefs.GetInt("encounterNum") % 3 == 1)
        {
            this.GetComponent<TextMeshProUGUI>().text = recipe2;
        }
        if (PlayerPrefs.GetInt("encounterNum") % 3 == 2)
        {
            this.GetComponent<TextMeshProUGUI>().text = recipe3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
