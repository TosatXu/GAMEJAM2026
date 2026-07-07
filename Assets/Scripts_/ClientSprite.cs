using TMPro;
using UnityEngine;

public class ClientSprite : MonoBehaviour
{

    public Sprite client1;
    public Sprite client2;
    public Sprite client3;

    void Start()
    {
        if (PlayerPrefs.GetInt("encounterNum") % 3 == 0)
        {
            this.GetComponent<SpriteRenderer>().sprite = client1;
        }
        if (PlayerPrefs.GetInt("encounterNum") % 3 == 1)
        {
            this.GetComponent<SpriteRenderer>().sprite = client2;
        }
        if (PlayerPrefs.GetInt("encounterNum") % 3 == 2)
        {
            this.GetComponent<SpriteRenderer>().sprite = client3;
        }
    }
}
