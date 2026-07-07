using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(BoxCollider2D))]
public class Cauldron : MonoBehaviour
{
    public PotionMechanicsManager potionMechanicsManager;
    public bool destroyPieceAfterDrop = true;
    public Sprite standard;
    public Sprite green;
    public Sprite brown;
    public Sprite tears;

    [Header("Drop Sound")]
    public AudioClip dropSound;
    public AudioMixerGroup sfxMixerGroup;
    [Range(0f, 1f)] public float dropSoundVolume = 0.8f;
    public bool playDropSound = true;

    AudioSource audioSource;

    void Awake()
    {
        BoxCollider2D cauldronCollider = GetComponent<BoxCollider2D>();
        cauldronCollider.isTrigger = true;
        SetupAudioSource();

        if (potionMechanicsManager == null)
        {
            potionMechanicsManager = FindFirstObjectByType<PotionMechanicsManager>();
        }
    }

    void OnValidate()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyAudioSourceSettings();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        IngredientPiece piece = other.GetComponent<IngredientPiece>();

        if (piece == null)
        {
            return;
        }

        Drag drag = other.GetComponent<Drag>();

        if (drag != null && drag.IsDragging)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            return;
        }

        PutPieceIntoCauldron(piece);
    }

    void PutPieceIntoCauldron(IngredientPiece piece)
    {
        PlayDropSound();

        if (potionMechanicsManager == null)
        {
            potionMechanicsManager = FindFirstObjectByType<PotionMechanicsManager>();
        }

        if (potionMechanicsManager != null)
        {
            potionMechanicsManager.ReceiveIngredient(piece);
        }
        else
        {
            Debug.LogWarning("No PotionMechanicsManager found for cauldron.", this);
        }

        if (destroyPieceAfterDrop)
        {
            Destroy(piece.gameObject);
        }
        else
        {
            piece.transform.position = transform.position;

            Drag drag = piece.GetComponent<Drag>();
            if (drag != null)
            {
                drag.canDrag = false;
            }

            Collider2D pieceCollider = piece.GetComponent<Collider2D>();
            if (pieceCollider != null)
            {
                pieceCollider.enabled = false;
            }
        }

        changeColour();
    }

    void changeColour()
    {
        if (potionMechanicsManager == null)
        {
            return;
        }

        if (potionMechanicsManager.potionQuality <= 30)
        {
            SetCauldronSprite(brown);
        }
        else if (potionMechanicsManager.potionQuality >= 80)
        {
            SetCauldronSprite(green);
        }
        else
        {
            SetCauldronSprite(tears);
        }
    }

    public void ShowPotionResult(bool ingredientsCorrect, bool fireTimingGood)
    {
        if (ingredientsCorrect && fireTimingGood)
        {
            SetCauldronSprite(green);
        }
        else
        {
            SetCauldronSprite(tears != null ? tears : brown);
        }
    }

    public void ShowDefaultVisual()
    {
        SetCauldronSprite(standard);
    }

    void SetCauldronSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        ApplyAudioSourceSettings();
    }

    void ApplyAudioSourceSettings()
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.outputAudioMixerGroup = sfxMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    void PlayDropSound()
    {
        if (!playDropSound)
        {
            return;
        }

        if (audioSource == null)
        {
            SetupAudioSource();
        }

        if (audioSource == null || dropSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(dropSound, dropSoundVolume);
    }
}
