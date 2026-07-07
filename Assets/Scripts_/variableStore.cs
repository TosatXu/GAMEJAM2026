using UnityEngine;

public class variableStore : MonoBehaviour
{

    void Awake()
    {

        DontDestroyOnLoad(gameObject);
        PlayerPrefs.DeleteAll();

    }
}
