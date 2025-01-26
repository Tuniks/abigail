using UnityEngine;

public class PlayerTile : MonoBehaviour
{
    private CollageAzulejoStates stateManager;
    public float alignmentThreshold = 2f; // How close the tile needs to be to the slot to be considered "aligned"
    private bool isTileAligned = false; // Flag to track if the tile is aligned and should be frozen
    public GameObject successimage;
    public GameObject instruction;

    private void Start()
    {
        stateManager = FindObjectOfType<CollageAzulejoStates>();
    }

    private void OnTriggerEnter2D(Collider2D other) // Use Collider2D for 2D physics
    {
        if (stateManager.currentState == CollageAzulejoStates.GameState.PLAYERTURN && stateManager.emptySlots.Contains(other.gameObject))
        {
            // Snap the tile to the empty slot position
            SnapTileToSlot(other.gameObject);

            // Check if the tile is aligned with the slot (perfectly positioned)
            if (IsTileAlignedWithSlot(other.gameObject))
            {
                // Remove the slot from emptySlots since it's now occupied
                stateManager.emptySlots.Remove(other.gameObject);

                // Log placement and transition state
                Debug.Log("Player tile placed and aligned with empty slot. Moving to NPC2TURN.");
                stateManager.SetState(CollageAzulejoStates.GameState.NPC2TURN);
            }
        }
        else if (stateManager.currentState == CollageAzulejoStates.GameState.REPLACE && !isTileAligned)
        {
            // Snap the tile to the empty slot position
            SnapTileToSlot(other.gameObject);

            // Check if the tile is aligned with the slot (perfectly positioned)
            if (IsTileAlignedWithSlot(other.gameObject))
            {
                // Remove the slot from emptySlots since it's now occupied
                stateManager.emptySlots.Remove(other.gameObject);

                // Freeze the tile's position by disabling the Draggable component
                GetComponent<Draggable>().enabled = false;

                // Set the flag to indicate the tile is aligned
                isTileAligned = true;
                instruction.SetActive(false);
                successimage.SetActive(true);

                // Log placement (no state change here for REPLACE)
                Debug.Log("Player tile placed and aligned with empty slot. Tile position frozen.");
            }
        }
    }

    private void SnapTileToSlot(GameObject slot)
    {
        // Position the tile directly on top of the empty slot
        transform.position = slot.transform.position;
    }

    private bool IsTileAlignedWithSlot(GameObject slot)
    {
        // Compare the position of the tile and the slot
        Vector2 tilePos = transform.position;
        Vector2 slotPos = slot.transform.position;

        // Check if the tile's position is within the alignment threshold of the slot's position
        if (Vector2.Distance(tilePos, slotPos) <= alignmentThreshold)
        {
            return true; // The tile is aligned
        }
        return false; // The tile is not aligned
    }
}
