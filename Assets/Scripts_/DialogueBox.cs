using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public int dialogueNum;
    public GameObject Variables;
    public int encounterCounter;
    public string[] intro1 = new string[4];
    public string[] exit1 = new string[2];
    public string[] intro2 = new string[4];
    public string[] exit2 = new string[2];
    public string[] intro3 = new string[4];
    public string[] exit3 = new string[2];
    public TextMeshProUGUI display;

    void Awake()
    {
        Variables.GetComponent<variableStore>().encounterNum++;
        encounterCounter = Variables.GetComponent<variableStore>().encounterNum;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Variables.GetComponent<variableStore>().encounterNum == 0)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro1[dialogueNum];
            }
            if (Variables.GetComponent<variableStore>().encounterNum == 1)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 1);
                display.text = exit1[dialogueNum];
            }
            if (Variables.GetComponent<variableStore>().encounterNum == 2)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro2[dialogueNum];
            }
            if (Variables.GetComponent<variableStore>().encounterNum == 3)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 1);
                display.text = exit2[dialogueNum];
            }
            if (Variables.GetComponent<variableStore>().encounterNum == 4)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro3[dialogueNum];
            }
            if (Variables.GetComponent<variableStore>().encounterNum == 5)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 1);
                display.text = exit3[dialogueNum];
            }
        }
    }
}
