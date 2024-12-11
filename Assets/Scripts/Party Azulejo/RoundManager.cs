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
    private int NPC3score = 0;
    private int NPC4score = 0;
    private int NPC5score = 0;
    private bool isDecisionComplete = false;

    // Public references
    public TextMeshProUGUI winnerText;
    public GameObject uiContainer;
    public TextMeshProUGUI bubbleText;
    public TextMeshProUGUI buttonOption1Text;
    public TextMeshProUGUI buttonOption2Text;
    public Button option1Button;
    public Button option2Button;
    public TextMeshProUGUI CurrentChallenge;

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
        "What's the challenge for this round?", "Please pick something I can use my foot tile for!",
        "You've always taken sooooooo long to pick the challenges."
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
            "Dude, I'm always thinking about how bonkers craters are.", "They should invent non-slippery banana peels."
        } // Round 3
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
        if (roundCount < challengeTexts.Length)
        {
            bubbleText.text = challengeTexts[roundCount];
            buttonOption1Text.text = buttonTexts[roundCount, 0];
            buttonOption2Text.text = buttonTexts[roundCount, 1];
        }
        else
        {
            bubbleText.text = "The game is over!";
            buttonOption1Text.text = "No more challenges";
            buttonOption2Text.text = "No more challenges";
        }

        option1Button.gameObject.SetActive(true);
        option2Button.gameObject.SetActive(true);
        uiContainer.SetActive(true);
    }

    private void HandleChallengeSelection()
    {
        Debug.Log($"Current State: {currentState}, Waiting for button selection.");
    }

    public void OnOption1Click()
    {
        option1Selected = true;
        Debug.Log("Option 1 clicked!");
        TransitionToChallengeReaction();
    }

    public void OnOption2Click()
    {
        option1Selected = false;
        Debug.Log("Option 2 clicked!");
        TransitionToChallengeReaction();
    }

    private void TransitionToChallengeReaction()
    {
        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
        currentState = GameState.ChallengeReaction;
        Debug.Log("Transitioning to ChallengeReaction state");
    }

    private void HandleChallengeReaction()
    {
        if (roundCount >= maxRounds)
        {
            bubbleText.text = "";
            return;
        }

        bubbleText.text = reactionTexts[roundCount, option1Selected ? 0 : 1];
        Debug.Log($"Reaction Text Displayed: {bubbleText.text}");

        // Wait until the player presses 'E' before activating the round options
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Activate the correct Round/Option GameObject
            if (roundCount == 0)
            {
                if (option1Selected)
                    Round1Option1.SetActive(true);
                else
                    Round1Option2.SetActive(true);
            }
            else if (roundCount == 1)
            {
                if (option1Selected)
                    Round2Option1.SetActive(true);
                else
                    Round2Option2.SetActive(true);
            }
            else if (roundCount == 2)
            {
                if (option1Selected)
                    Round3Option1.SetActive(true);
                else
                    Round3Option2.SetActive(true);
            }

            // Hide the challenge reaction text and transition to the TileSubmission state
            bubbleText.text = "";
            uiContainer.SetActive(false);
            currentState = GameState.TileSubmission;
            Debug.Log($"State changed to: {currentState}");
        }
    }


    private void HandleTileSubmission()
{
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
    if (option1Selected)
    {
        CurrentChallenge.text = buttonOption1Text.text;
    }
    else
    {
        CurrentChallenge.text = buttonOption2Text.text;
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
        Round1Option1.SetActive(false);
        Round2Option1.SetActive(false);
        Round3Option1.SetActive(false);
        Round1Option2.SetActive(false);
        Round2Option2.SetActive(false);
        Round3Option2.SetActive(false);
        uiContainer.SetActive(true);
        CurrentChallenge.text = "";
        bubbleText.text = "We like Abigail's decision";

        if (Input.GetKeyDown(KeyCode.E))
        {
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
            winnerText.text = "It's a tie!";
        }
        else if (NPC1score == maxScore)
        {
            winnerText.text = "Gabe won!";
        }
        else if (NPC2score == maxScore)
        {
            winnerText.text = "Kate won!";
        }
        else if (NPC3score == maxScore)
        {
            winnerText.text = "Dani won!";
        }
        else if (NPC4score == maxScore)
        {
            winnerText.text = "Chase won!";
        }
        else if (NPC5score == maxScore)
        {
            winnerText.text = "Joanna won!";
        }

        Debug.Log("Game Complete!");
    }

    private void IncrementRound()
    {
        roundCount++;
        if (roundCount >= maxRounds)
        {
            Debug.Log("Game Complete!");
        }
    }
}

