using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset; // Offset between the mouse and object position
    private Camera mainCamera;
    private bool isDragging;

    void Start()
    {
        // Cache the main camera reference
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // Calculate offset when the object is clicked
        Vector3 mousePosition = GetMouseWorldPosition();
        offset = transform.position - mousePosition;

        // Enable dragging
        isDragging = true;
    }

    void OnMouseUp()
    {
        // Disable dragging
        isDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the object's position while dragging
            Vector3 mousePosition = GetMouseWorldPosition();
            transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in world coordinates
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = -mainCamera.transform.position.z; // Distance from the camera
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}

