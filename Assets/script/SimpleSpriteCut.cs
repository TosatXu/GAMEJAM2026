using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteCut : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    bool hasBeenCut;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CutInHalf();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        CutInHalf();
    }

    public void CutInHalf()
    {
        if (hasBeenCut || spriteRenderer == null || spriteRenderer.sprite == null)
        {
            return;
        }

        hasBeenCut = true;

        Sprite sprite = spriteRenderer.sprite;
        Texture2D original = sprite.texture;
        if (!original.isReadable)
        {
            Debug.LogError("Enable Read/Write on the sprite texture before cutting: " + original.name, this);
            hasBeenCut = false;
            return;
        }

        int width = original.width;
        int height = original.height;
        int cutX = width / 2;

        Texture2D leftTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Texture2D rightTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] originalPixels = original.GetPixels();
        Color[] leftPixels = new Color[originalPixels.Length];
        Color[] rightPixels = new Color[originalPixels.Length];

        Color clear = new Color(0, 0, 0, 0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;

                if (x < cutX)
                {
                    leftPixels[index] = originalPixels[index];
                    rightPixels[index] = clear;
                }
                else
                {
                    leftPixels[index] = clear;
                    rightPixels[index] = originalPixels[index];
                }
            }
        }

        leftTexture.SetPixels(leftPixels);
        rightTexture.SetPixels(rightPixels);

        leftTexture.Apply();
        rightTexture.Apply();

        CreatePiece(leftTexture, Vector2.left);
        CreatePiece(rightTexture, Vector2.right);

        Destroy(gameObject);
    }

    void CreatePiece(Texture2D texture, Vector2 forceDirection)
    {
        Sprite newSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            spriteRenderer.sprite.pixelsPerUnit
        );

        GameObject piece = new GameObject("Cut Piece");
        piece.transform.position = transform.position;
        piece.transform.rotation = transform.rotation;
        piece.transform.localScale = transform.localScale;

        SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
        sr.sprite = newSprite;

        Rigidbody2D rb = piece.AddComponent<Rigidbody2D>();
        rb.AddForce(forceDirection * 2f, ForceMode2D.Impulse);

        piece.AddComponent<BoxCollider2D>();
    }
}
