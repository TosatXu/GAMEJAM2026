using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public GameObject dialogue;

    public void sceneTransition ()
    {
        if (dialogue.GetComponent<DialogueBox>().dialogueNum >= 3 && dialogue.GetComponent<DialogueBox>().encounterCounter % 2 == 0) {
            SceneManager.LoadScene("SampleScene");
        }
        if (dialogue.GetComponent<DialogueBox>().dialogueNum >= 1 && dialogue.GetComponent<DialogueBox>().encounterCounter % 2 == 1)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
