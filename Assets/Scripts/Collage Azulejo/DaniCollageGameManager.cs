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
    private Transform previouslySelectedTile; // Tracks the last highlighted tile
    private Vector3 originalScale; // Stores original tile scale
    private Vector3 originalScaleSlot;  // Store the original scale of the slot
    private Transform previouslySelectedSlot;  // Keep track of the previously selected slot
    private int previousSlotIndex = -1;
    private Transform previousSelectedOption = null;  // Store the previously selected option
    private Vector3 originalYesScale;
    private Vector3 originalNoScale;
    public GameObject DaniComment1;  // Public GameObject for Dani Comment 1
    public GameObject DaniComment2;  // Public GameObject for Dani Comment 2
    private int currentRound = 1;    // Track the current round (1 or 2)
    
    
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
                selectedTile.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                isMoving = false;  // Movement is complete

                // Add the tile to the PlayedTiles list
                PlayedTiles.Add(selectedTile);
            }
        }
    }

    // Switch to the specified state
// Modify SwitchState to reset arrow positions when switching to a new state
    private void SwitchState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.PlayerTileChoice1:
                slotArrowSprite.SetActive(false);
                if (playerHand.hand.Count > 0)
                {
                    currentTileIndex = 0; // Move to the first tile
                    HighlightSelection();
                    UpdateArrowPosition(); // Update arrow position
                    arrowSprite.SetActive(true);
                }
                break;

            case GameState.PlayerSlotChoice1:
                arrowSprite.SetActive(false);
                if (emptySlots.Count > 0)
                {
                    currentSlotIndex = 0; // Move to the first slot
                    UpdateSlotArrowPosition(); // Update slot arrow position
                    slotArrowSprite.SetActive(true);
                }
                break;

            case GameState.Judgement:
                if (judgementarrow != null) // Check if judgement arrow exists
                {
                    judgementarrow.SetActive(true);
                    // If judgement arrow has a list, reset its position similarly
                }
                break;
        }
    }


// Handle player input in the PlayerTileChoice1 state
    private void HandleTileChoice()
    {
        // Ensure player hand has items before processing input
        if (playerHand.hand.Count == 0) return;

        // Handle input for W, S, or arrow keys to move the arrow
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveArrowUp();
            HighlightSelection();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveArrowDown();
            HighlightSelection();
        }

        // Update the arrow's position based on the current tile index
        UpdateArrowPosition();

        // Handle input for E to select the tile under the arrow
        if (Input.GetKeyDown(KeyCode.E))
        {
            SelectTile();
        }
    }


// Increase the size of the selected tile by 10% and reset the previous one
    private void HighlightSelection()
    {
        if (playerHand.hand.Count == 0) return;

        Transform newSelection = playerHand.hand[currentTileIndex].transform;

        // Ensure we don't scale an already highlighted object again
        if (previouslySelectedTile != null && previouslySelectedTile != newSelection)
        {
            previouslySelectedTile.localScale = originalScale; // Reset previous tile
        }

        // Store original scale if selecting a new tile
        if (newSelection != previouslySelectedTile)
        {
            originalScale = newSelection.localScale;
        }

        // Apply the scale increase relative to original scale
        newSelection.localScale = originalScale * 1.3f;

        // Update the reference
        previouslySelectedTile = newSelection;
    }

// Move the arrow up in the PlayerTileChoice1 state
    private void MoveArrowUp()
    {
        if (currentTileIndex > 0)
        {
            currentTileIndex--;
        }
    }

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
        if (playerHand.hand.Count > 0 && currentTileIndex < playerHand.hand.Count)
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


private void HandleSlotChoice()
{
    // Handle input for W, S, or arrow keys to move the slot selection arrow
    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
    {
        MoveSlotArrowUp();
        //HighlightSlot();
    }
    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
    {
        MoveSlotArrowDown();
        //HighlightSlot();
    }

    // Update the slot arrow's position based on the current slot index
    UpdateSlotArrowPosition();

    // Handle input for E to select the slot under the arrow
    if (Input.GetKeyDown(KeyCode.E))
    {
        SelectSlot();
    }
}



// Move the slot arrow up in the PlayerSlotChoice1 state
private void MoveSlotArrowUp()
{
    if (emptySlots.Count > 0 && currentSlotIndex > 0)
    {
        currentSlotIndex--;
    }
}

// Move the slot arrow down in the PlayerSlotChoice1 state
private void MoveSlotArrowDown()
{
    if (emptySlots.Count > 0 && currentSlotIndex < emptySlots.Count - 1)
    {
        currentSlotIndex++;
    }
}

// Update the slot arrow's position to be over the selected slot, with its own offset
private void UpdateSlotArrowPosition()
{
    if (emptySlots.Count > 0 && currentSlotIndex >= 0 && currentSlotIndex < emptySlots.Count)
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

        // Display the correct Dani comment based on the round
        if (currentRound == 1)
        {
            DaniComment1.SetActive(true);  // Show Dani Comment 1 for the first round
            DaniComment2.SetActive(false); // Ensure Dani Comment 2 is hidden
        }
        else if (currentRound == 2)
        {
            DaniComment1.SetActive(false); // Hide Dani Comment 1
            DaniComment2.SetActive(true);  // Show Dani Comment 2 for the second round
        }

        // Wait for the player to press E to move to the next state
        if (Input.GetKeyDown(KeyCode.E))
        {
            DaniComment1.SetActive(false);
            DaniComment2.SetActive(false);
            // Transition to DaniTurn1 state
            SwitchState(GameState.DaniTurn1);

            // Move to the next round after the player presses E
            currentRound++;  // Increment the round (1 -> 2 or beyond)
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

        // Store the original scales when first activated
        if (previousSelectedOption == null)
        {
            originalYesScale = yes.transform.localScale;
            originalNoScale = no.transform.localScale;
        }

        // Handle arrow key input to cycle between "Yes" and "No"
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Cycle between Yes and No
            judgementSelection = 1 - judgementSelection; // Toggle between 0 and 1
            UpdateYesNoArrowPosition(); // Update arrow position
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
    // If the arrow is over the "Yes" option
    if (judgementSelection == 0)
    {
        judgementarrow.transform.position = yes.transform.position; // Align the arrow with the "Yes" button
        ScaleOption(yes);  // Scale "Yes" option to 110%

        if (previousSelectedOption != yes.transform) // Compare with the transform, not the GameObject
        {
            ResetScale(no); // Reset the scale of "No"
            previousSelectedOption = yes.transform; // Update the previously selected option to "Yes"
        }
    }
    // If the arrow is over the "No" option
    else if (judgementSelection == 1)
    {
        judgementarrow.transform.position = no.transform.position; // Align the arrow with the "No" button
        ScaleOption(no);  // Scale "No" option to 110%

        if (previousSelectedOption != no.transform) // Compare with the transform, not the GameObject
        {
            ResetScale(yes); // Reset the scale of "Yes"
            previousSelectedOption = no.transform; // Update the previously selected option to "No"
        }
    }
}

private void ScaleOption(GameObject option)
{
    option.transform.localScale = option.transform.localScale * 1.3f;  // Scale up by 10%
}

private void ResetScale(GameObject option)
{
    if (option != null) // Check if the option exists
    {
        option.transform.localScale = originalYesScale; // Reset scale to the original for "Yes"
        option.transform.localScale = originalNoScale;  // Reset scale to the original for "No"
    }
}

// Method to scale an option down by 10% when selected
    private void ScaleOption(GameObject option, Vector3 originalScale)
    {
        option.transform.localScale = originalScale * 1.3f;  // Shrink by 10%
    }

// Method to reset the scale of an option to its original size
    private void ResetScale(GameObject option, Vector3 originalScale)
    {
        option.transform.localScale = originalScale;  // Reset to original scale
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
        arrowSprite.SetActive(true);
    }
    
}
