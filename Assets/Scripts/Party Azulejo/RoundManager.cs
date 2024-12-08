using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    private enum GameState
    {
        SetChallengeText,
        ChallengeSelection,
        ChallengeReaction,
        TileSubmission,
        DecisionReaction 
    }


    private GameState currentState = GameState.SetChallengeText;

    // Round data
    private int roundCount = 0;
    private const int maxRounds = 3;

    private int NPC1score = 0;
    private int NPC2score = 0;
    private bool isDecisionComplete = false;

    // Public references
    public TextMeshProUGUI winnerText;
    public GameObject uiContainer; // Assign the UI container (panel with text and buttons)
    public TextMeshProUGUI bubbleText; // Assign the TextMeshPro for the bubble text
    public TextMeshProUGUI buttonOption1Text; // Assign Button 1's text object
    public TextMeshProUGUI buttonOption2Text; // Assign Button 2's text object
    public Button option1Button; // Changed from GameObject to Button
    public Button option2Button; // Changed from GameObject to Button
    public GameObject tileOption1; // Assign the Tile GameObject for Option 1
    public GameObject tileOption2; // Assign the Tile GameObject for Option 2
    public TextMeshProUGUI CurrentChallenge; // Assign this in the Inspector


    // Text data for each round
    private string[] challengeTexts =
    {
        "What's the challenge for this round?", "Please please pick something that I can use my foot tile for!",
        "You've always taken sooooooo long to pick the challenges Abigail."
    };

    private string[,] buttonTexts =
    {
        { "Who would give a really weak handshake?", "Who has never had a cavity?" }, // Round 1
        { "Who would steal the moon right out of the sky?", "Who would make kombucha from scratch?" }, // Round 2
        {
            "Who would reshape the craters of the earth when they jump?",
            "Most likely to slip and fall on a banana peel?"
        } // Round 3
    };

    private string[,] reactionTexts =
    {
        {
            "Here comes Abigail with her big city challenges...",
            "Is this your subtle way of bragging that you've never had a cavity?"
        }, // Round 1
        {
            "My tiles are never good for these poetic challenges.",
            "That reminds me I need to bring you some of my homemade hibiscus kombucha."
        }, // Round 2
        {
            "Dude, I'm always thinking about how bonkers craters are.", "They should invent non slippery banana peels."
        } // Round 3
    };

    private bool option1Selected = false; // Tracks which option was chosen

    private void Update()
    {
        //Debug.Log($"Update called. Current State: {currentState}");

        switch (currentState)
        {
            case GameState.SetChallengeText:
                HandleSetChallengeText();
                break;
            case GameState.ChallengeSelection:
                HandleChallengeSelection();
                break;
            case GameState.ChallengeReaction:
                HandleChallengeReaction();
                break;
            case GameState.TileSubmission:
                HandleTileSubmission();
                break;
            case GameState.DecisionReaction: // Add the new state
                HandleDecisionReaction();
                break;
            default:
                Debug.Log("Unknown state!");
                break;
        }

    }

    // State: SetChallengeText
    private void HandleSetChallengeText()
    {
        // Ensure that the roundCount does not exceed the number of available challenge texts
        if (roundCount < challengeTexts.Length)
        {
            bubbleText.text = challengeTexts[roundCount];
            buttonOption1Text.text = buttonTexts[roundCount, 0];
            buttonOption2Text.text = buttonTexts[roundCount, 1];
        }
        else
        {
            // Handle the case where rounds exceed available data (game over or final decision)
            bubbleText.text = "The game is over!";
            buttonOption1Text.text = "No more challenges";
            buttonOption2Text.text = "No more challenges";
        }

        option1Button.gameObject.SetActive(true); // Use gameObject to set active status
        option2Button.gameObject.SetActive(true); // Use gameObject to set active status
        uiContainer.SetActive(true);

        // Do not transition immediately, wait for button click to change the state
    }

    // State: ChallengeSelection
    private void HandleChallengeSelection()
    {
        Debug.Log($"Current State: {currentState}, Waiting for button selection.");

        // Since the state transition is now handled by the button clicks, 
        // we don't need to check for keyboard input anymore.

        // Option 1 and Option 2 button clicks are now handled in their respective methods
        Debug.Log("Waiting for button selection.");
    }

    public void OnOption1Click()
    {
        option1Selected = true; // Option 1 selected
        Debug.Log("Option 1 clicked!"); // Log for debugging
        TransitionToChallengeReaction();
    }

    public void OnOption2Click()
    {
        option1Selected = false; // Option 2 selected
        Debug.Log("Option 2 clicked!"); // Log for debugging
        TransitionToChallengeReaction();
    }

    private void TransitionToChallengeReaction()
    {
        option1Button.gameObject.SetActive(false); // Deactivate the button
        option2Button.gameObject.SetActive(false); // Deactivate the button
        currentState = GameState.ChallengeReaction;
        Debug.Log("Transitioning to ChallengeReaction state");
    }
private void HandleChallengeReaction()
{
    Debug.Log($"Current State: {currentState}, Round: {roundCount}"); // Log current state and round

    // Check if we have exceeded the number of rounds
    if (roundCount >= maxRounds)
    {
        // Game over, no more challenges
        bubbleText.text = ""; // Clear the reaction text
        return;
    }

    // Otherwise, display the reaction text based on the round and selection
    bubbleText.text = reactionTexts[roundCount, option1Selected ? 0 : 1];
    Debug.Log($"Reaction Text Displayed: {bubbleText.text}"); // Log the reaction text being displayed

    // Check if E is pressed
    if (Input.GetKeyDown(KeyCode.E))
    {
        Debug.Log("E key pressed! Transitioning to TileSubmission state."); // Log E key press
        bubbleText.text = ""; // Clear the bubble text (optional)
        uiContainer.SetActive(false); // Hide the UI container
        currentState = GameState.TileSubmission; // Move to TileSubmission state
        Debug.Log($"State changed to: {currentState}"); // Log the state change
    }
    else
    {
        Debug.Log("E key not pressed yet."); // Log if E is not detected
    }
}



private void HandleTileSubmission()
{
    // Determine the active tile set based on the selected option
    GameObject activeTileParent = option1Selected ? tileOption1 : tileOption2;
    activeTileParent.SetActive(true); // Ensure the correct tile set is visible

    // Set the text of the CurrentChallenge text object based on the selected option
    if (option1Selected)
    {
        CurrentChallenge.text = buttonOption1Text.text; // Show the text of the selected option
    }
    else
    {
        CurrentChallenge.text = buttonOption2Text.text; // Show the text of the selected option
    }

    // Detect player interaction
    if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
        if (Physics.Raycast(ray, out RaycastHit hit)) // Check for a hit
        {
            // Loop through all children of the active tile parent
            foreach (Transform child in activeTileParent.transform)
            {
                if (hit.collider.gameObject == child.gameObject) // Check if the clicked object is a child
                {
                    Debug.Log($"Tile clicked: {child.gameObject.name}"); // Log the clicked tile's name
                    activeTileParent.SetActive(false); // Hide all tiles

                    // Increment the score based on the tile's tag
                    if (child.CompareTag("NPC1"))
                    {
                        NPC1score++;
                    }
                    else if (child.CompareTag("NPC2"))
                    {
                        NPC2score++;
                    }

                    currentState = GameState.DecisionReaction; // Move to the new state
                    return; // Exit the loop after processing
                }
            }
        }
    }
}


private void HandleDecisionReaction()
{
    // Turn off the tile option game objects
    tileOption1.SetActive(false);
    tileOption2.SetActive(false);

    // Turn the UI back on
    uiContainer.SetActive(true);

    // Reset the CurrentChallenge text when the state changes to DecisionReaction
    CurrentChallenge.text = ""; // Clear the text

    // Update the bubble text
    bubbleText.text = "We like Abigail's decision";

    // Wait for the player to press the space bar to move to the next state
    if (Input.GetKeyDown(KeyCode.E))
    {
        // Increment the round count
        IncrementRound();

        // If 3 rounds have been played, end the game and show the winner
        if (roundCount >= maxRounds)
        {
            // Call DisplayWinnerText to show the winner and stop the game
            DisplayWinnerText();
            return; // Stop the game here by returning
        }

        // Otherwise, restart from the first state of the cycle
        currentState = GameState.SetChallengeText;
    }
}

    
    private void DisplayWinnerText()
    {
        // Determine the winner based on the scores
        if (NPC1score > NPC2score)
        {
            winnerText.text = "NPC 1 won!"; // Display the winner on WinnerText
        }
        else if (NPC2score > NPC1score)
        {
            winnerText.text = "NPC 2 won!"; // Display the winner on WinnerText
        }
        else
        {
            winnerText.text = "It's a tie!"; // Display a tie message on WinnerText
        }

        // Optionally stop the game here by returning (this will halt further state transitions)
        Debug.Log("Game Complete!");
    }



    private void IncrementRound()
    {
        roundCount++;

        // Ensure that the round count doesn't exceed maxRounds
        if (roundCount >= maxRounds)
        {
            Debug.Log("Game Complete!");
            // Optionally stop the game or display final message here
            // This will ensure the game doesn't loop after round 3
        }
        else
        {
            // Continue to the next round
            currentState = GameState.SetChallengeText; // Restart the cycle with the new round
        }
    }
}