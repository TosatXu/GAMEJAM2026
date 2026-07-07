using UnityEngine;
using UnityEngine.UI;

public class CauldronVisual : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image;
    public Sprite defaultSprite;
    public Sprite vitriolSprite;
    public Sprite failedSprite;
    public bool captureDefaultSpriteOnAwake = true;

    void Awake()
    {
        FindTargets();

        if (captureDefaultSpriteOnAwake && defaultSprite == null)
        {
            defaultSprite = GetCurrentSprite();
        }
    }

    public void ShowDefault()
    {
        SetSprite(defaultSprite);
    }

    public void ShowVitriol()
    {
        if (vitriolSprite == null)
        {
            Debug.LogWarning("No vitriol sprite assigned on CauldronVisual.", this);
            return;
        }

        SetSprite(vitriolSprite);
    }

    public void ShowFailed()
    {
        if (failedSprite == null)
        {
            Debug.LogWarning("No failed sprite assigned on CauldronVisual. Showing default cauldron instead.", this);
            ShowDefault();
            return;
        }

        SetSprite(failedSprite);
    }

    void FindTargets()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

        if (image == null)
        {
            image = GetComponentInChildren<Image>(true);
        }
    }

    Sprite GetCurrentSprite()
    {
        FindTargets();

        if (spriteRenderer != null)
        {
            return spriteRenderer.sprite;
        }

        if (image != null)
        {
            return image.sprite;
        }

        return null;
    }

    void SetSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        FindTargets();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }

        if (image != null)
        {
            image.sprite = sprite;
        }
    }
}