using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameObject Ingredient;
    public GameObject IngredientSpawner;
    public bool matchButtonSize = true;
    public float spawnedSizeMultiplier = 1f;

    GameObject spawnedIngredient;
    bool spawnedFromPointer;

    public void spawnIngredient()
    {
        if (spawnedFromPointer)
        {
            spawnedFromPointer = false;
            return;
        }

        SpawnIngredientObject();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        spawnedIngredient = SpawnIngredientObject();
        spawnedFromPointer = true;
        MoveSpawnedIngredient(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveSpawnedIngredient(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MoveSpawnedIngredient(eventData.position);
        spawnedIngredient = null;
    }

    GameObject SpawnIngredientObject()
    {
        if (IngredientSpawner == null)
        {
            IngredientSpawner = GameObject.Find("IngredientSpawner");
        }

        if (Ingredient == null)
        {
            Debug.LogError("Ingredient is not assigned on " + name, this);
            return null;
        }

        if (IngredientSpawner == null)
        {
            Debug.LogError("Cannot find IngredientSpawner in the scene.", this);
            return null;
        }

        GameObject newIngredient = Instantiate(
            Ingredient,
            IngredientSpawner.transform.position,
            Quaternion.identity
        );

        SetupSpawnedIngredient(newIngredient);
        return newIngredient;
    }

    void SetupSpawnedIngredient(GameObject newIngredient)
    {
        Image buttonImage = GetComponent<Image>();
        SpriteRenderer spawnedRenderer = newIngredient.GetComponent<SpriteRenderer>();

        if (buttonImage != null && buttonImage.sprite != null && spawnedRenderer != null)
        {
            spawnedRenderer.sprite = buttonImage.sprite;
            spawnedRenderer.sortingOrder = 10;
        }

        if (matchButtonSize && buttonImage != null && spawnedRenderer != null)
        {
            MatchSpawnedSizeToButton(newIngredient, spawnedRenderer, buttonImage);
        }
    }

    void MatchSpawnedSizeToButton(GameObject newIngredient, SpriteRenderer spawnedRenderer, Image buttonImage)
    {
        Camera cam = Camera.main;
        if (cam == null || spawnedRenderer.sprite == null)
        {
            return;
        }

        RectTransform buttonRect = buttonImage.rectTransform;
        Vector3[] buttonCorners = new Vector3[4];
        buttonRect.GetWorldCorners(buttonCorners);

        Camera uiCamera = GetUICamera();
        Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(uiCamera, buttonCorners[0]);
        Vector2 topRight = RectTransformUtility.WorldToScreenPoint(uiCamera, buttonCorners[2]);

        float buttonPixelWidth = Mathf.Abs(topRight.x - bottomLeft.x);
        float buttonPixelHeight = Mathf.Abs(topRight.y - bottomLeft.y);
        float buttonPixelSize = Mathf.Min(buttonPixelWidth, buttonPixelHeight);

        if (buttonPixelSize <= 0f)
        {
            return;
        }

        float targetZ = IngredientSpawner != null ? IngredientSpawner.transform.position.z : newIngredient.transform.position.z;
        float cameraDistance = Mathf.Abs(cam.transform.position.z - targetZ);

        Vector3 screenA = new Vector3(0f, 0f, cameraDistance);
        Vector3 screenB = new Vector3(buttonPixelSize, 0f, cameraDistance);
        float targetWorldSize = Vector3.Distance(cam.ScreenToWorldPoint(screenA), cam.ScreenToWorldPoint(screenB)) * spawnedSizeMultiplier;

        Bounds bounds = spawnedRenderer.bounds;
        float currentWorldSize = Mathf.Max(bounds.size.x, bounds.size.y);

        if (currentWorldSize <= 0f)
        {
            return;
        }

        float scaleFactor = targetWorldSize / currentWorldSize;
        newIngredient.transform.localScale *= scaleFactor;
    }

    Camera GetUICamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        if (canvas.worldCamera != null)
        {
            return canvas.worldCamera;
        }

        return Camera.main;
    }

    void MoveSpawnedIngredient(Vector2 screenPosition)
    {
        if (spawnedIngredient == null)
        {
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        float targetZ = IngredientSpawner != null ? IngredientSpawner.transform.position.z : 0f;
        Vector3 mousePosition = screenPosition;
        mousePosition.z = Mathf.Abs(cam.transform.position.z - targetZ);

        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);
        worldPosition.z = targetZ;
        spawnedIngredient.transform.position = worldPosition;
    }

    public void spawnCuttingBoard()
    {

    }
}