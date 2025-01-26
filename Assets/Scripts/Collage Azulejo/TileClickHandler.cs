using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private GameObject tile;  // To reference the tile
    private CollageAzulejoStates collageAzulejoStates; // Reference to the CollageAzulejoStates script
    private Transform slot;   // The slot the tile is occupying

    void Start()
    {
        // Get reference to the CollageAzulejoStates script
        collageAzulejoStates = FindObjectOfType<CollageAzulejoStates>();
        slot = transform.parent;  // Assuming the tile is a child of the slot, or adjust based on your setup
    }

    // This method is called when the tile is clicked
    void OnMouseDown()
    {
        // Check if the game is in the REEVALUATION state before allowing the tile to be destroyed
        if (collageAzulejoStates.currentState == CollageAzulejoStates.GameState.REEVALUATION)
        {
            // Destroy the clicked tile
            Destroy(gameObject);  // Destroys this tile object
            Debug.Log("Tile destroyed.");

            // Change the state to REPLACE
            collageAzulejoStates.SetState(CollageAzulejoStates.GameState.REPLACE);
        }
    }
}