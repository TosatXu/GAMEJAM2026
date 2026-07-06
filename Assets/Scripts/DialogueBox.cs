using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public int dialogueNum;
    public int encounterNum;
    public string[] intro1 = new string[20];
    public string[] exit1 = new string[20];
    public string[] intro2 = new string[20];
    public string[] exit2 = new string[20];
    public string[] intro3 = new string[20];
    public string[] exit3 = new string[20];
    public TextMeshProUGUI display;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) 
        {
            dialogueNum++;
        }

        if (encounterNum == 0)
        {
            display.text = intro1[dialogueNum];
        }
    }

    
}
