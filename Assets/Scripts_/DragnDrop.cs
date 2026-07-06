using Unity.VisualScripting;
using UnityEngine;

public class DragnDrop : MonoBehaviour
{
    private void OnMouseDrag()
    {
        this.transform.position = Input.mousePosition;
    }
}
