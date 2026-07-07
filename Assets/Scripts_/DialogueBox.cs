using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public int dialogueNum;
    public int encounterNum;
    public string[] intro1 = new string[4];
    public string[] exit1 = new string[2];
    public string[] intro2 = new string[4];
    public string[] exit2 = new string[2];
    public string[] intro3 = new string[4];
    public string[] exit3 = new string[2];
    public TextMeshProUGUI display;

    private void Start()
    {
        
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (encounterNum == 0)
            {
                dialogueNum++;
                if ()
                dialogueNum = Mathf.Clamp(dialogueNum, 0, 3);
                display.text = intro1[dialogueNum];
            }
        }
    }
}
