using UnityEngine;

public class TileMover : MonoBehaviour
{
    // Public references to the Tile and Slot GameObjects
    public GameObject tile;
    public GameObject slot;

    // Camera's Z position (set to -10 as per your setup)
    private float cameraZ = -10f;

    void Update()
    {
        // Handle input for mouse click (left click)
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            // Convert mouse position to world space
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Adjust Z to match the scene's Z position (0 in this case)
            mouseWorldPos.z = 0; // Set Z to 0 for 2D objects

            // Raycast to check if the mouse is clicking on the tile
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            // If the ray hits the tile
            if (hit.collider != null && hit.collider.gameObject == tile)
            {
                // Move the tile to the slot position
                tile.transform.position = slot.transform.position;

                // Optionally, log the action for debugging
                Debug.Log("Tile has been moved to the slot.");
            }
        }
    }
}