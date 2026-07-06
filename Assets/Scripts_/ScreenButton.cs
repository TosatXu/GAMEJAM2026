using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenButton : MonoBehaviour
{
    public int sceneID;

    public void switchScene ()
    {
        SceneManager.LoadScene(sceneID);
    }
}
