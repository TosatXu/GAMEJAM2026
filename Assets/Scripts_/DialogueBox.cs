using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public int dialogueNum;
    public int encounterCounter;
    public string[] intro1 = new string[4];
    public string[] exit1 = new string[2];
    public string[] intro2 = new string[4];
    public string[] exit2 = new string[2];
    public string[] intro3 = new string[4];
    public string[] exit3 = new string[2];
    public TextMeshProUGUI display;

    private void Start()
    {
        if (PlayerPrefs.GetInt("encounterNum") != 0 && PlayerPrefs.GetInt("encounterNum") != 1 && PlayerPrefs.GetInt("encounterNum") != 2 && PlayerPrefs.GetInt("encounterNum") != 3 && PlayerPrefs.GetInt("encounterNum") != 4 && PlayerPrefs.GetInt("encounterNum") != 5)
        {
            PlayerPrefs.SetInt("encounterNum", -1);
        }
    }

    void OnEnable()
    {
        PlayerPrefs.SetInt("encounterNum", PlayerPrefs.GetInt("encounterNum") + 1);
        encounterCounter = PlayerPrefs.GetInt("encounterNum");


    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log(encounterCounter);

            if (encounterCounter == 0)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro1[dialogueNum];
            }
            if (encounterCounter == 1)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 1);
                display.text = exit1[dialogueNum];
            }
            if (encounterCounter == 2)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro2[dialogueNum];
            }
            if (encounterCounter == 3)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 1);
                display.text = exit2[dialogueNum];
            }
            if (encounterCounter == 4)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro3[dialogueNum];
            }
            if (encounterCounter == 5)
            {
                dialogueNum++;
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 1);
                display.text = exit3[dialogueNum];
            }
        }
    }
}
