using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = Convert.ToString(GameObject.Find("RecipeRuntimeData").GetComponent<RecipeRuntimeData>().lastPotionQuality);
    }
}
