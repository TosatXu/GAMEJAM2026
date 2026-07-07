using UnityEngine;

public class variableStore : MonoBehaviour
{
    public int encounterNum;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
