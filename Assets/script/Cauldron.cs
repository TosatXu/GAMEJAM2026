using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Cauldron : MonoBehaviour
{
    public PotionMechanicsManager potionMechanicsManager;
    public bool destroyPieceAfterDrop = true;

    void Awake()
    {
        BoxCollider2D cauldronCollider = GetComponent<BoxCollider2D>();
        cauldronCollider.isTrigger = true;
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
        if (potionMechanicsManager != null)
        {
            potionMechanicsManager.ReceiveIngredient(piece);
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
    }
}