using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public GameObject dialogue;

    public void sceneTransition ()
    {
        if (dialogue.GetComponent<DialogueBox>().dialogueNum >= 19) {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
