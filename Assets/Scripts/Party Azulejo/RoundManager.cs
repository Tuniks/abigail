using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private bool winnerDisplayed = false;

    private int NPC1score = 0;
    private int NPC2score = 0;
    private int NPC3score = 0;
    private int NPC4score = 0;
    private int NPC5score = 0;
    private bool isDecisionComplete = false;
    private string selectedChallengeText; // Variable to store the selected text

    // Public references
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI GabeWinText;
    public GameObject uiContainer;
    [FormerlySerializedAs("bubbleText")] public TextMeshProUGUI GabeText;
    public TextMeshProUGUI MomTextHolder;
    public TextMeshProUGUI ChaseTextHolder;
    public TextMeshProUGUI buttonOption1Text;
    public TextMeshProUGUI buttonOption2Text;
    public Button option1Button;
    public Button option2Button;
    public TextMeshProUGUI CurrentChallenge;
    public GameObject ChaseBubble;
    public GameObject MomBubble;
    public GameObject GabeBubble;
    public GameObject KateBubble;
    public GameObject DaniBubble;
    public GameObject JoannaBubble;
    public TextMeshProUGUI DaniWinText;
    public TextMeshProUGUI KateWinText;
    public TextMeshProUGUI JoannaWinText;
    public TextMeshProUGUI ChaseWinText;
    public GameObject GabeStar;
    public GameObject KateStar;
    public GameObject DaniStar;
    public GameObject JoannaStar;
    public GameObject ChaseStar;
    public GameObject ChallengeCloud;
    public GameObject OptionsClouds;
    public GameObject WillThisWork;

    // Round Option GameObjects
    public GameObject Round1Option1; // Assigned in Inspector
    public GameObject Round1Option2; // Assigned in Inspector
    public GameObject Round2Option1; // Assigned in Inspector
    public GameObject Round2Option2; // Assigned in Inspector
    public GameObject Round3Option1; // Assigned in Inspector
    public GameObject Round3Option2; // Assigned in Inspector

    private bool option1Selected = false;

    private string[] challengeTexts =
    {
        "Hit us with a challenge Abigail!", "Please pick something I can use my foot tile for!",
        "You always take soooo long to pick the challenges."
    };

    private string[,] buttonTexts =
    {
        { "Who would give a really weak handshake?", "Who has never had a cavity?" }, // Round 1
        { "Who would steal the moon right out of the sky?", "Who would make kombucha from scratch?" }, // Round 2
        {
            "Who would reshape the craters of the earth when they jump?",
            "Most likely to slip on a banana peel?"
        } // Round 3
    };

    private string[] chaseTexts =
    {
        "Here comes Abigail with her big city challenges...",
        "I feel like if I were a huge nerd I'd love this one.",
        "Dude what does that even mean?"
    };

    private string[] momTexts =
    {
        "You know whose never had a cavity? My Abigail!",
        "Oh maybe that can be my next little project!",
        "Mmm now I'm thinking about making banana bread."
    };

    private void Update()
    {
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
            case GameState.DecisionReaction:
                HandleDecisionReaction();
                break;
            default:
                Debug.Log("Unknown state!");
                break;
        }
    }

    private void HandleSetChallengeText()
    {
        ClearText(); // Clear text before displaying new challenge text
        GabeBubble.SetActive(true); // Activate GabeBubble when GabeText is shown

        if (roundCount < challengeTexts.Length)
        {
            GabeText.text = challengeTexts[roundCount];
            OptionsClouds.SetActive(true);
            buttonOption1Text.text = buttonTexts[roundCount, 0];
            buttonOption2Text.text = buttonTexts[roundCount, 1];
        }
        else
        {
            GabeText.text = "The game is over!";
            buttonOption1Text.text = "No more challenges";
            buttonOption2Text.text = "No more challenges";
        }

        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);
        uiContainer.SetActive(true);
    }

    private void HandleChallengeSelection()
    {
            // Store the selected text based on the option
            if (option1Selected)
            {
                selectedChallengeText = buttonOption1Text.text;
            }
            else
            {
                selectedChallengeText = buttonOption2Text.text;
            }

            // Optionally, log the selected text for debugging
            Debug.Log("Selected Challenge Text: " + selectedChallengeText);
    }
    

    public void OnOption1Click()
    {
        option1Selected = true;
        Debug.Log("Option 1 clicked!");

        // Store the selected text immediately here
        selectedChallengeText = buttonOption1Text.text;
    
        // Transition to the next state
        TransitionToChallengeReaction();
    }

    public void OnOption2Click()
    {
        option1Selected = false;
        Debug.Log("Option 2 clicked!");

        // Store the selected text immediately here
        selectedChallengeText = buttonOption2Text.text;

        // Transition to the next state
        TransitionToChallengeReaction();
    }


    private void TransitionToChallengeReaction()
    {
        ClearText(); // Clear text before transitioning

        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
        OptionsClouds.SetActive(false);
        GabeBubble.SetActive(false);
        currentState = GameState.ChallengeReaction;
        Debug.Log("Transitioning to ChallengeReaction state");
    }

  private void HandleChallengeReaction()
{
    ClearText(); // Clear text before displaying new reaction text

    if (roundCount >= maxRounds)
    {
        GabeText.text = "";
        return;
    }

    // Display the appropriate reaction text and activate corresponding bubble
    if (option1Selected)
    {
        ChaseTextHolder.text = chaseTexts[roundCount];
        MomTextHolder.text = ""; // Clear MomTextHolder if ChaseText is displayed
        ChaseBubble.SetActive(true);
        MomBubble.SetActive(false); // Ensure MomBubble is off
        Debug.Log($"Chase Text Displayed: {chaseTexts[roundCount]}");
    }
    else
    {
        MomTextHolder.text = momTexts[roundCount];
        ChaseTextHolder.text = ""; // Clear ChaseTextHolder if MomText is displayed
        MomBubble.SetActive(true);
        ChaseBubble.SetActive(false); // Ensure ChaseBubble is off
        Debug.Log($"Mom Text Displayed: {momTexts[roundCount]}");
    }

    // Wait until the player presses 'E' before activating the round options
    if (Input.GetKeyDown(KeyCode.E))
    {
        // Activate the correct Round/Option GameObject based on the round and selection
        switch (roundCount)
        {
            case 0:
                if (option1Selected)
                    Round1Option1.SetActive(true);
                else
                    Round1Option2.SetActive(true);
                break;
            case 1:
                if (option1Selected)
                    Round2Option1.SetActive(true);
                else
                    Round2Option2.SetActive(true);
                break;
            case 2:
                if (option1Selected)
                    Round3Option1.SetActive(true);
                else
                    Round3Option2.SetActive(true);
                break;
        }

        // Transition to the TileSubmission state and turn off both bubbles
        MomTextHolder.text = "";
        ChaseTextHolder.text = "";
        MomBubble.SetActive(false);
        ChaseBubble.SetActive(false);
        currentState = GameState.TileSubmission;
        Debug.Log($"State changed to: {currentState}");
    }
}



    private void HandleTileSubmission()
    {
        ClearText(); // Clear text before displaying new challenge text

        // Determine which round's option to activate based on the selected option
        GameObject activeRoundOption = null;

        // Assign the correct round/option based on the roundCount and option1Selected
        if (roundCount == 0)
        {
            activeRoundOption = option1Selected ? Round1Option1 : Round1Option2;
        }
        else if (roundCount == 1)
        {
            activeRoundOption = option1Selected ? Round2Option1 : Round2Option2;
        }
        else if (roundCount == 2)
        {
            activeRoundOption = option1Selected ? Round3Option1 : Round3Option2;
        }

        // Activate the selected option GameObject
        activeRoundOption.SetActive(true);

        // Set the current challenge text to match the selected option
        if (selectedChallengeText != null)
        {
            CurrentChallenge.text = selectedChallengeText; // Use the stored challenge text
            ChallengeCloud.SetActive(true);
        }
        else
        {
            // Fallback in case no text has been selected
            CurrentChallenge.text = "No challenge selected";
        }

        // Detect player interaction
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Cast a ray from the camera to the mouse position
            if (Physics.Raycast(ray, out RaycastHit hit)) // Check for a hit
            {
                // Loop through all children of the active round option
                foreach (Transform child in activeRoundOption.transform) // Update here to use activeRoundOption's children
                {
                    if (hit.collider.gameObject == child.gameObject) // Check if the clicked object is a child
                    {
                        Debug.Log($"Tile clicked: {child.gameObject.name}"); // Log the clicked tile's name
                        activeRoundOption.SetActive(false); // Hide the selected round option tiles

                        // Increment the score based on the tile's tag
                        if (child.CompareTag("NPC1"))
                        {
                            NPC1score++;
                        }
                        else if (child.CompareTag("NPC2"))
                        {
                            NPC2score++;
                        }
                        else if (child.CompareTag("NPC3"))
                        {
                            NPC3score++;
                        }
                        else if (child.CompareTag("NPC4"))
                        {
                            NPC4score++;
                        }
                        else if (child.CompareTag("NPC5"))
                        {
                            NPC5score++;
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
        ClearText(); // Clear text before displaying new decision reaction text

        // Deactivate specific round-related options
        Round1Option1.SetActive(false);
        Round2Option1.SetActive(false);
        Round3Option1.SetActive(false);
        Round1Option2.SetActive(false);
        Round2Option2.SetActive(false);
        Round3Option2.SetActive(false);
        ChallengeCloud.SetActive(false);
    
        uiContainer.SetActive(true);
        CurrentChallenge.text = "";

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Deactivate all objects with the tag "bubble"
            GameObject[] bubbles = GameObject.FindGameObjectsWithTag("bubble");
            foreach (GameObject bubble in bubbles)
            {
                bubble.SetActive(false);
            }

            // Clear all TextMeshPro text elements on the canvas
            TextMeshProUGUI[] textElements = FindObjectsOfType<TextMeshProUGUI>();
            foreach (TextMeshProUGUI textElement in textElements)
            {
                textElement.text = "";
            }

            IncrementRound();
            if (roundCount >= maxRounds)
            {
                DisplayWinnerText();
                return;
            }
            currentState = GameState.SetChallengeText;
        }
    }

    private void DisplayWinnerText()
    {
        int maxScore = Mathf.Max(NPC1score, NPC2score, NPC3score, NPC4score, NPC5score);
        int tieCount = 0;
        if (NPC1score == maxScore) tieCount++;
        if (NPC2score == maxScore) tieCount++;
        if (NPC3score == maxScore) tieCount++;
        if (NPC4score == maxScore) tieCount++;
        if (NPC5score == maxScore) tieCount++;

        if (tieCount > 1)
        {
            GabeBubble.SetActive(true);
            GabeWinText.text = "Come on! A tie is like no one won!";
        }
        else if (NPC1score == maxScore)
        {
            GabeBubble.SetActive(true);
            GabeStar.SetActive(true);
            GabeWinText.text = "It's all cause of my lucky foot tile!";
        }
        else if (NPC2score == maxScore)
        {
            KateBubble.SetActive(true);
            KateStar.SetActive(true);
            KateWinText.text = "Great minds think alike Abigail!";
        }
        else if (NPC3score == maxScore)
        {
            DaniBubble.SetActive(true);
            DaniStar.SetActive(true);
            DaniWinText.text = "This win goes out to Gianluca!";
        }
        else if (NPC4score == maxScore)
        {
            ChaseBubble.SetActive(true);
            ChaseStar.SetActive(true);
            ChaseWinText.text = "Good game guys!";
        }
        else if (NPC5score == maxScore)
        {
            JoannaBubble.SetActive(true);
            JoannaStar.SetActive(true);
            JoannaWinText.text = "Yes! I knew I could do it.";
        }

        Debug.Log("Game Complete!");
        winnerDisplayed = true;
        WillThisWork.SetActive(true);
        
       // if (winnerDisplayed && Input.GetKeyDown(KeyCode.E))
        {
          //  SceneManager.LoadScene("Start");
        }
    }

    private void IncrementRound()
    {
        roundCount++;
        if (roundCount >= maxRounds)
        {
            Debug.Log("Game Complete!");
        }
    }

    // Helper method to clear text
    private void ClearText()
    {
        GabeText.text = "";
        MomTextHolder.text = "";
        ChaseTextHolder.text = "";
        buttonOption1Text.text = "";
        buttonOption2Text.text = "";
        CurrentChallenge.text = "";
    }
    
    private void OnOptionSelected(bool isOption1Selected)
    {
        // Set which option was selected
        option1Selected = isOption1Selected;

        // Call the function to store the selected challenge text
        HandleChallengeSelection();

        // Transition to the next state or round if needed
        currentState = GameState.TileSubmission;
    }

}

