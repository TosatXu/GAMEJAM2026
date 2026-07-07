using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenButton : MonoBehaviour
{
    public int sceneID;

    public void switchScene ()
    {
        if (PlayerPrefs.GetInt("encounterNum") >= 2)
        {
            sceneID = 4;
        }
        SceneManager.LoadScene(sceneID);
    }
}
