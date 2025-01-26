using UnityEngine;

public class BreakableTile : MonoBehaviour
{
    public GameObject spriteLeft;  // Left half of the tile
    public GameObject spriteRight; // Right half of the tile
    private bool isBroken = false;
    private bool combineMode = false; // Track whether combine mode is active
    private Vector3 offset;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // Deactivate the halves initially
        spriteLeft.SetActive(false);
        spriteRight.SetActive(false);
    }

    void Update()
    {
        // Toggle combine mode when P is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            combineMode = !combineMode;
            Debug.Log("Combine mode: " + (combineMode ? "ON" : "OFF"));
        }
    }

    void OnMouseDown()
    {
        // Calculate the offset between mouse position and object position
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        // Move the object with the mouse
        if (!isBroken)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Distance from the camera
        return cam.ScreenToWorldPoint(mousePosition);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBroken && collision.gameObject.CompareTag("Tile"))
        {
            isBroken = true;

            // Activate the halves
            spriteLeft.SetActive(true);
            spriteRight.SetActive(true);

            // Match their positions to the original sprite
            spriteLeft.transform.position = transform.position;
            spriteRight.transform.position = transform.position;

            // Enable dragging for the halves
            spriteLeft.AddComponent<Draggable>();
            spriteRight.AddComponent<Draggable>();

            // Destroy the original tile
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Combine halves if in combine mode
        if (combineMode && other.CompareTag("TileHalf"))
        {
            CombineHalves(other.gameObject);
        }
    }

    private void CombineHalves(GameObject otherHalf)
    {
        // Create a new parent object
        GameObject newTile = new GameObject("NewTile");

        // Set the position of the new tile as the midpoint between the two halves
        Vector3 midpoint = (transform.position + otherHalf.transform.position) / 2;
        newTile.transform.position = midpoint;

        // Make both halves children of the new tile
        transform.SetParent(newTile.transform);
        otherHalf.transform.SetParent(newTile.transform);

        // Disable dragging for the combined halves
        Destroy(GetComponent<Draggable>());
        Destroy(otherHalf.GetComponent<Draggable>());

        Debug.Log("Halves combined into NewTile");
    }
}
