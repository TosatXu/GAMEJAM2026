using UnityEngine;

public class Drag : MonoBehaviour
{
    public bool canDrag = true;

    Vector3 offset;
    float zPosition;

    public bool IsDragging { get; private set; }

    void OnMouseDown()
    {
        if (!canDrag)
        {
            return;
        }

        IsDragging = true;
        zPosition = transform.position.z;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (!canDrag)
        {
            return;
        }

        Vector3 newPosition = GetMouseWorldPosition() + offset;
        newPosition.z = zPosition;
        transform.position = newPosition;
    }

    void OnMouseUp()
    {
        IsDragging = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(cam.transform.position.z - transform.position.z);

        return cam.ScreenToWorldPoint(mousePosition);
    }
}