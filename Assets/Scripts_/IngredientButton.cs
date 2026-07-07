using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    static readonly string[] BodyPartNames = { "brain", "liver", "heart", "eyeball", "eye", "tongue" };
    static readonly string[] BottleNames = { "water", "aquaregia", "oilofvitriol", "bottle", "saltedtear", "saltedtears", "tear", "tears" };
    static readonly string[] PlantNames = { "baneberry", "nettle", "dreamlily", "ghostorchid", "citrine", "balsam", "fang", "root", "flower", "herb", "plant" };

    public GameObject Ingredient;
    public GameObject IngredientSpawner;
    public bool matchButtonSize = true;
    public float spawnedSizeMultiplier = 1f;
    public float defaultIngredientPercent = 30f;
    public IngredientType ingredientType = IngredientType.Unknown;

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

        SetupIngredientData(newIngredient, buttonImage);
        SetupPhysicsForCauldron(newIngredient);
    }

    void SetupIngredientData(GameObject newIngredient, Image buttonImage)
    {
        IngredientPiece ingredientPiece = newIngredient.GetComponent<IngredientPiece>();
        if (ingredientPiece == null)
        {
            ingredientPiece = newIngredient.AddComponent<IngredientPiece>();
        }

        string ingredientName = GetIngredientName(buttonImage);
        IngredientType detectedType = GetIngredientType(ingredientName);
        float ingredientPercent = GetIngredientPercent(ingredientName, detectedType);

        ingredientPiece.SetIngredientData(ingredientName, detectedType, ingredientPercent);
    }

    string GetIngredientName(Image buttonImage)
    {
        TextMeshProUGUI tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (tmpText != null)
        {
            string textName = NormalizeIngredientName(tmpText.text);
            if (!IsIgnoredTextName(textName))
            {
                return textName;
            }

            string objectName = NormalizeIngredientName(tmpText.name);
            if (!IsIgnoredTextName(objectName))
            {
                return objectName;
            }
        }

        if (buttonImage != null && buttonImage.sprite != null)
        {
            string spriteName = buttonImage.sprite.name;
            int underscoreIndex = spriteName.IndexOf('_');

            if (underscoreIndex > 0)
            {
                spriteName = spriteName.Substring(0, underscoreIndex);
            }

            return NormalizeIngredientName(spriteName);
        }

        string fallbackName = name.Replace("IngredientButton", "").Replace("Button", "");
        return NormalizeIngredientName(fallbackName);
    }

    bool IsIgnoredTextName(string textName)
    {
        return string.IsNullOrEmpty(textName) ||
            textName == "text" ||
            textName == "texttmp" ||
            textName == "newtext";
    }

    IngredientType GetIngredientType(string ingredientName)
    {
        if (ingredientType != IngredientType.Unknown)
        {
            return ingredientType;
        }

        if (NameIsInList(ingredientName, BodyPartNames))
        {
            return IngredientType.BodyPart;
        }

        if (NameIsInList(ingredientName, BottleNames))
        {
            return IngredientType.Bottle;
        }

        if (NameIsInList(ingredientName, PlantNames))
        {
            return IngredientType.Plant;
        }

        Debug.LogWarning("Cannot detect ingredient type for " + ingredientName + ". Set ingredientType on the IngredientButton if needed.", this);
        return IngredientType.Unknown;
    }

    float GetIngredientPercent(string ingredientName, IngredientType detectedType)
    {
        return defaultIngredientPercent;
    }

    bool NameIsInList(string ingredientName, string[] nameList)
    {
        for (int i = 0; i < nameList.Length; i++)
        {
            if (ingredientName == nameList[i] ||
                ingredientName.Contains(nameList[i]) ||
                nameList[i].Contains(ingredientName))
            {
                return true;
            }
        }

        return false;
    }

    string NormalizeIngredientName(string rawName)
    {
        if (string.IsNullOrWhiteSpace(rawName))
        {
            return "";
        }

        string firstPart = rawName.Trim();
        int lineBreakIndex = firstPart.IndexOf('\n');
        if (lineBreakIndex > 0)
        {
            firstPart = firstPart.Substring(0, lineBreakIndex);
        }

        int dashIndex = firstPart.IndexOf('-');
        if (dashIndex > 0)
        {
            firstPart = firstPart.Substring(0, dashIndex);
        }

        int colonIndex = firstPart.IndexOf(':');
        if (colonIndex > 0)
        {
            firstPart = firstPart.Substring(0, colonIndex);
        }

        string result = "";
        string lowerName = firstPart.ToLowerInvariant();

        for (int i = 0; i < lowerName.Length; i++)
        {
            char currentChar = lowerName[i];
            if (char.IsLetterOrDigit(currentChar))
            {
                result += currentChar;
            }
        }

        return result;
    }

    void SetupPhysicsForCauldron(GameObject newIngredient)
    {
        Collider2D ingredientCollider = newIngredient.GetComponent<Collider2D>();
        if (ingredientCollider != null)
        {
            ingredientCollider.isTrigger = true;
        }

        Rigidbody2D rigidbody2D = newIngredient.GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            rigidbody2D = newIngredient.AddComponent<Rigidbody2D>();
        }

        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.linearVelocity = Vector2.zero;
        rigidbody2D.angularVelocity = 0f;
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



