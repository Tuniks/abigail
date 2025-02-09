using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaniCollageGameManager : MonoBehaviour
{
    // Enum to define the game states
    private enum GameState
    {
        PlayerTileChoice1,
        PlayerSlotChoice1,
        DaniComment1,
        DaniTurn1,
        Judgement,
        SwitchOut,
        FinalJudgment,
        Leave
        
    }

    private List<GameObject> selectedItems = new List<GameObject>();  // Track selected items
    
    // Reference to the CollagePlayerHand script
    public GameObject PromptImages;
    public GameObject Question;
    public GameObject EmptySlots;
    public GameObject LeaveObject;
    
    public CollagePlayerHand playerHand;
    
    private int judgementSelection = 0;

    // Reference to the arrow sprite
    public GameObject arrowSprite;

    public GameObject judgementarrow;

    public GameObject DaniBackupTile;

    public GameObject Stars;
    // Reference to the arrow for slot selection
    public GameObject slotArrowSprite;

    // Current index of the selected tile
    private int currentTileIndex = 0;

    // Current index of the selected slot
    private int currentSlotIndex = 0;

    // List of empty slots to move the arrow to
    public List<GameObject> emptySlots;

    // Dani's hand 
    public List<GameObject> DaniHand;

    // Offset to raise the arrow sprite higher on the tile
    public float arrowOffsetY = 0.5f;

    // Offset for the slot arrow, which can be different from the tile arrow's offset
    public float slotArrowOffsetY = 0.5f;

    // To keep track of the current game state
    private GameState currentState;

    // The selected tile and slot
    private Tile selectedTile;
    private GameObject selectedSlot;

    public GameObject DaniBubble;

    // Speed of the tile movement when it is placed in the slot
    public float tileMoveSpeed = 5f;

    private GameObject tileToMove;  // The tile that's being moved
    private Vector3 startPos;       // Starting position of the tile
    private Vector3 targetPos;      // Target position of the tile
    private float moveTime = 1f;    // Time in seconds for the tile to move
    private float startTime;        // Time when the move started
    private bool isMoving = false;  // Whether the tile is still moving

    // List to store played tiles
    public List<Tile> PlayedTiles = new List<Tile>();

    public GameObject yes;
    public GameObject no;
    void Start()
    {
        PromptImages.SetActive(true);
        EmptySlots.SetActive(true);
        // Set the initial state
        SwitchState(GameState.PlayerTileChoice1);
    }

    void Update()
    {
        // Handle state updates
        switch (currentState)
        {
            case GameState.PlayerTileChoice1:
                HandleTileChoice();
                break;
            case GameState.PlayerSlotChoice1:
                HandleSlotChoice();
                break;
            case GameState.DaniComment1:
                HandleDaniComment();
                break;
            case GameState.DaniTurn1:
                HandleDaniTurn();
                break;
            case GameState.Judgement:
                HandleJudgement();
                break;
            case GameState.SwitchOut:
                HandleSwitchOut();
                break;
            case GameState.FinalJudgment:
                HandleFinalJudgment();
                break;
            case GameState.Leave:
                HandleLeave();
                break;
        }

        // Smoothly move the tile if it is in the process of moving
        if (isMoving)
        {
            // Calculate the distance the tile has moved using Time.time and startTime
            float journeyLength = Vector3.Distance(startPos, targetPos);
            float distanceCovered = (Time.time - startTime) * tileMoveSpeed; // Adjust the speed of the move
            float fractionOfJourney = distanceCovered / journeyLength;

            // Smoothly move the tile to the target position
            selectedTile.transform.position = Vector3.Lerp(startPos, targetPos, fractionOfJourney);

            // If the tile has reached the target position, stop moving
            if (fractionOfJourney >= 1)
            {
                // Ensure the tile is exactly at the target position
                selectedTile.transform.position = targetPos;
                isMoving = false;  // Movement is complete

                // Add the tile to the PlayedTiles list
                PlayedTiles.Add(selectedTile);
            }
        }
    }

    // Switch to the specified state
    private void SwitchState(GameState newState)
    {
        currentState = newState;

        // Handle the transition to the new state
        switch (newState)
        {
            case GameState.PlayerTileChoice1:
                // Ensure only the tile arrow is active
                arrowSprite.SetActive(true);
                slotArrowSprite.SetActive(false);
                break;

            case GameState.PlayerSlotChoice1:
                // Ensure only the slot arrow is active
                arrowSprite.SetActive(false);
                slotArrowSprite.SetActive(true);
                break;
        }
    }

    // Handle player input in the PlayerTileChoice1 state
    private void HandleTileChoice()
    {
        // Handle input for W, S, or arrow keys to move the arrow
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveArrowUp();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveArrowDown();
        }

        // Update the arrow's position based on the current tile index
        UpdateArrowPosition();

        // Handle input for E to select the tile under the arrow
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectTile();
        }
    }

    // Handle player input in the PlayerSlotChoice1 state
    private void HandleSlotChoice()
    {
        // Handle input for W, S, or arrow keys to move the slot selection arrow
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSlotArrowUp();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSlotArrowDown();
        }

        // Update the slot arrow's position based on the current slot index
        UpdateSlotArrowPosition();

        // Handle input for E to select the slot under the arrow
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectSlot();
        }
    }

    // Move the arrow up in the PlayerTileChoice1 state
    private void MoveArrowUp()
    {
        if (currentTileIndex > 0)
        {
            currentTileIndex--;
        }
    }

    // Move the arrow down in the PlayerTileChoice1 state
    private void MoveArrowDown()
    {
        if (currentTileIndex < playerHand.hand.Count - 1)
        {
            currentTileIndex++;
        }
    }

    // Update the arrow's position to be over the selected tile
    private void UpdateArrowPosition()
    {
        if (playerHand.hand.Count > 0)
        {
            // Get the current tile's position
            Vector3 tilePosition = playerHand.hand[currentTileIndex].transform.position;

            // Move the arrow sprite to the current tile's position with the added offset
            arrowSprite.transform.position = new Vector3(tilePosition.x, tilePosition.y + arrowOffsetY, tilePosition.z);
        }
    }

    // Select the tile when the player presses E
    private void SelectTile()
    {
        // Store the selected tile
        selectedTile = playerHand.hand[currentTileIndex];

        // Transition to the next state: PlayerSlotChoice1
        SwitchState(GameState.PlayerSlotChoice1);
    }

    // Move the slot arrow up in the PlayerSlotChoice1 state
    private void MoveSlotArrowUp()
    {
        if (currentSlotIndex > 0)
        {
            currentSlotIndex--;
        }
    }

    // Move the slot arrow down in the PlayerSlotChoice1 state
    private void MoveSlotArrowDown()
    {
        if (currentSlotIndex < emptySlots.Count - 1)
        {
            currentSlotIndex++;
        }
    }

    // Update the slot arrow's position to be over the selected slot, with its own offset
    private void UpdateSlotArrowPosition()
    {
        if (emptySlots.Count > 0)
        {
            // Get the current slot's position
            Vector3 slotPosition = emptySlots[currentSlotIndex].transform.position;

            // Move the slot arrow sprite to the current slot's position with the added offset
            slotArrowSprite.transform.position = new Vector3(slotPosition.x, slotPosition.y + slotArrowOffsetY, slotPosition.z);
        }
    }

    // Select the slot when the player presses E
    private void SelectSlot()
    {
        // Store the selected slot
        selectedSlot = emptySlots[currentSlotIndex];

        // (Optional) Perform some action with the selected tile and slot, such as placing the tile in the slot
        Debug.Log($"Selected Tile: {selectedTile.name}, Selected Slot: {selectedSlot.name}");

        // Set up the smooth movement of the tile
        startPos = selectedTile.transform.position;
        targetPos = selectedSlot.transform.position;

        // Mark that the tile is moving
        isMoving = true;

        // Record the start time for the movement
        startTime = Time.time;

        // Remove the selected tile from the player's hand list
        playerHand.hand.Remove(selectedTile);

        // Remove the selected slot from the emptySlots list
        emptySlots.Remove(selectedSlot);

        // Transition back to PlayerTileChoice1 or any other appropriate action
        SwitchState(GameState.DaniComment1);
    }

    private void HandleDaniComment()
    {
        // Ensure DaniBubble is active during DaniComment1 state
        DaniBubble.SetActive(true);

        // Deactivate the tile and slot arrows
        arrowSprite.SetActive(false);
        slotArrowSprite.SetActive(false);

        // Wait for the player to press E to move to DaniTurn1
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Transition to DaniTurn1 state
            SwitchState(GameState.DaniTurn1);
        }
    }

    private void HandleDaniTurn()
    {
        // Ensure DaniBubble is deactivated during DaniTurn1 state
        DaniBubble.SetActive(false);

        // Check if there are any empty slots left
        if (emptySlots.Count > 0 && DaniHand.Count > 0)
        {
            // Find the first unselected item in DaniHand
            GameObject selectedItem = null;
            for (int i = 0; i < DaniHand.Count; i++)
            {
                if (!selectedItems.Contains(DaniHand[i]))  // Check if this item has not been selected yet
                {
                    selectedItem = DaniHand[i];
                    selectedItems.Add(selectedItem);  // Mark this item as selected
                    break;  // Exit the loop once an unselected item is found
                }
            }

            if (selectedItem != null)
            {
                // Select the first available slot
                GameObject selectedSlot = emptySlots[0];

                // Place the selected item in the selected slot
                selectedItem.SetActive(true);  // Make the DaniHand item active
                selectedItem.transform.position = selectedSlot.transform.position;

                // Remove the selected slot from emptySlots
                emptySlots.RemoveAt(0);  // Remove the slot from emptySlots

                // Transition to the next state or any follow-up actions
                SwitchState(GameState.Judgement); // Or other appropriate transition
            }
        }
    }

    private void HandleJudgement()
    {
        if (emptySlots.Count == 0)
        {
            yes.SetActive(true);
            no.SetActive(true);
            Question.SetActive(true);
            judgementarrow.SetActive(true);

            // Handle arrow key input to cycle between "Yes" and "No"
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                // Cycle between Yes and No
                judgementSelection = 1 - judgementSelection; // Toggle between 0 and 1
                UpdateYesNoArrowPosition(); // Update arrow position (you'll need to define this method)
            }

            // Confirm the choice with the Enter/E key
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            {
                if (judgementSelection == 0) // Yes was selected
                {
                    SwitchState(GameState.SwitchOut);
                }
                else if (judgementSelection == 1) // No was selected
                {
                    SwitchState(GameState.FinalJudgment);
                }
            }
        }
        else
        {
            SwitchState(GameState.PlayerTileChoice1);
        }
    }
    
    private void UpdateYesNoArrowPosition()
    {
        // Move the arrow to the selected option based on judgementSelection
        if (judgementSelection == 0) // Yes is selected
        {
            judgementarrow.transform.position = yes.transform.position; // Align the arrow with the "Yes" button
        }
        else if (judgementSelection == 1) // No is selected
        {
            judgementarrow.transform.position = no.transform.position; // Align the arrow with the "No" button
        }
    }

private void HandleSwitchOut()
{
    // Ensure only the slot arrow is active during SwitchOut state
    slotArrowSprite.SetActive(true);
    arrowSprite.SetActive(false);
    judgementarrow.SetActive(false);
    yes.SetActive(false);
    no.SetActive(false);
    Question.SetActive(false);
    

    // Handle player input for moving the slot selection arrow with WASD or arrow keys
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
    {
        MoveSlotDaniArrowUp();
    }
    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
    {
        MoveSlotDaniArrowDown();
    }

    // Update the slot arrow's position based on the current slot index
    UpdateSlotDaniArrowPosition();

    // Handle input for E to select the tile under the arrow
    if (Input.GetKeyDown(KeyCode.E))
    {
        // Ensure DaniHand has tiles to switch out
        if (DaniHand.Count > 0)
        {
            // Select the tile from DaniHand based on currentSlotIndex
            GameObject selectedTile = DaniHand[currentSlotIndex];

            // Remove the selected tile from DaniHand
            DaniBackupTile.SetActive(true);
            DaniHand.Remove(selectedTile);
            Destroy(selectedTile);
            slotArrowSprite.SetActive(false);

            // Add the DaniBackUpTile to DaniHand
            DaniHand.Add(DaniBackupTile);

            // Optionally, log or update UI to show the replacement
            Debug.Log($"Tile {selectedTile.name} replaced by {DaniBackupTile.name}");

            // Optionally update the tile's position if needed
            DaniBackupTile.transform.position = selectedTile.transform.position;

            // After selection, go back to another state or update the game flow
            SwitchState(GameState.FinalJudgment); // For example, move to FinalJudgment state
        }
    }
}

private void MoveSlotDaniArrowUp()
{
    if (currentSlotIndex > 0)
    {
        currentSlotIndex--;
    }
}

private void MoveSlotDaniArrowDown()
{
    if (currentSlotIndex < DaniHand.Count - 1) // Adjusted to check DaniHand count
    {
        currentSlotIndex++;
    }
}

private void UpdateSlotDaniArrowPosition()
{
    if (DaniHand.Count > 0)
    {
        // Get the current slot's position in DaniHand
        Vector3 slotPosition = DaniHand[currentSlotIndex].transform.position;

        // Move the slot arrow sprite to the selected tile's position with the added offset
        slotArrowSprite.transform.position = new Vector3(slotPosition.x, slotPosition.y + slotArrowOffsetY, slotPosition.z);
    }
}


    private void HandleFinalJudgment()
    {
        yes.SetActive(false);
        no.SetActive(false);
        Question.SetActive(false);
        Stars.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchState(GameState.Leave);
        }
        
    }

    private void HandleLeave()
    {
        foreach (GameObject item in DaniHand)
        {
            item.SetActive(false);
        }

        // Deactivate all tiles in PlayedTiles
        foreach (Tile playedTile in PlayedTiles)
        {
            playedTile.gameObject.SetActive(false); // Assuming Tile is a GameObject
        }
        PromptImages.SetActive(false);
        EmptySlots.SetActive(false);
        Stars.SetActive(false);
        LeaveObject.SetActive(true);
    }
    
}
