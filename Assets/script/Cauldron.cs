using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Cauldron : MonoBehaviour
{
    public PotionMechanicsManager potionMechanicsManager;
    public bool destroyPieceAfterDrop = true;
    public Sprite standard;
    public Sprite green;
    public Sprite brown;
    public Sprite tears;

    void Awake()
    {
        BoxCollider2D cauldronCollider = GetComponent<BoxCollider2D>();
        cauldronCollider.isTrigger = true;

        if (potionMechanicsManager == null)
        {
            potionMechanicsManager = FindFirstObjectByType<PotionMechanicsManager>();
        }
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
        if (potionMechanicsManager.GetComponent<PotionMechanicsManager>().potionQuality <= 30)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = brown;
        }
        else if (potionMechanicsManager.GetComponent<PotionMechanicsManager>().potionQuality >= 80)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = green;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = tears;
        }
    }
}