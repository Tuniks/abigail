using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class FlashBackRoundManager : MonoBehaviour
{
    
        [YarnCommand]
        public void EndReaction()
        {
            DecisionReactionDone = true;
            Debug.Log("EndReaction triggered!");
        }
        
        [YarnCommand]
        public void ReadytoEnd()
        {
            EndDisplayed = true;
            Debug.Log("Readytoendtriggered!");
        }
        [YarnCommand]
        public void TextIsDone()
        {
            TextDone = true;
            Debug.Log("Readytoendtriggered!");
        }
        
        [YarnCommand]
        public void RevealWinner()
        {
            Timetorevealwinner = true;
        }
    
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
    public bool DecisionReactionDone = false;
    public bool EndDisplayed = false;

    private int NPC1score = 0;
    private int NPC2score = 0;
    private int NPC3score = 0;
    private int NPC4score = 0;
    private int NPC5score = 0;
    private bool isDecisionComplete = false;
    private string selectedChallengeText; // Variable to store the selected text
    private bool winnerStarDisplayed = false; // Track if the star has been displayed
    private bool Timetorevealwinner = false;
    private bool TextDone = false;
    
    public GameObject uiContainer;
    [FormerlySerializedAs("bubbleText")] public TextMeshProUGUI GabeText;
    public TextMeshProUGUI buttonOption1Text;
    public TextMeshProUGUI buttonOption2Text;
    public Button option1Button;
    public Button option2Button;
    public TextMeshProUGUI CurrentChallenge;
    public GameObject GabeStar;
    public GameObject KateStar;
    public GameObject DaniStar;
    public GameObject JoannaStar;
    public GameObject ChaseStar;
    public GameObject TieStar;
    public GameObject ChallengeCloud;
    public GameObject OptionsClouds;
    public GameObject WillThisWork;
    private DialogueRunner dialogueRunner;

    // Round Option GameObjects
    public GameObject Round1Option1; // Assigned in Inspector
    public GameObject Round1Option2; // Assigned in Inspector
    public GameObject Round2Option1; // Assigned in Inspector
    public GameObject Round2Option2; // Assigned in Inspector
    public GameObject Round3Option1; // Assigned in Inspector
    public GameObject Round3Option2; // Assigned in Inspector

    private bool option1Selected = false;
    
    // Add an array to track whether each round's dialogue has been triggered
    private bool[] chaseDialogueTriggered = new bool[3];
    private bool[] momDialogueTriggered = new bool[3];
    

    private string[] challengeTexts =
    {
        "Hit us with a challenge Abigail!", "Please pick something I can use my foot tile for!",
        "You always take soooo long to pick the challenges."
    };

    private string[,] buttonTexts =
    {
        { "What will I miss the most about Kiln Bay?", "What's something I could encounter that would send me running home?" }, // Round 1
        { "What's my favorite secret ingredient?", "What does the worst recipe I've ever made taste like?" }, // Round 2
        {
            "What's a creature I'd like to take the form of?",
            "If I could only bring one thing with me to the big city, what would it be?"
        } // Round 3
    };
    
    void Start()
    {
        // Find the DialogueRunner in the scene
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (dialogueRunner == null)
        {
            Debug.LogError("No DialogueRunner found in the scene!");
        }
    }

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
        //GabeBubble.SetActive(true); // Activate GabeBubble when GabeText is shown

        if (roundCount < challengeTexts.Length)
        {
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

    void TriggerChaseDialogue(int round)
    {
        if (!chaseDialogueTriggered[round]) // Check if the dialogue has already been triggered
        {
            switch (round)
            {
                case 0:
                    dialogueRunner.StartDialogue("Round1PromptOption1Reaction");
                    break;
                case 1:
                    dialogueRunner.StartDialogue("Round2PromptOption1Reaction");
                    break;
                case 2:
                    dialogueRunner.StartDialogue("Round3PromptOption1Reaction");
                    break;
                default:
                    Debug.LogWarning("Invalid round number!");
                    break;
            }
            chaseDialogueTriggered[round] = true; // Mark this round's dialogue as triggered
        }
    }

    void TriggerMomDialogue(int round)
    {
        if (!momDialogueTriggered[round]) // Check if the dialogue has already been triggered
        {
            switch (round)
            {
                case 0:
                    dialogueRunner.StartDialogue("Round1PromptOption2Reaction");
                    break;
                case 1:
                    dialogueRunner.StartDialogue("Round2PromptOption2Reaction");
                    break;
                case 2:
                    dialogueRunner.StartDialogue("Round3PromptOption2Reaction");
                    break;
                default:
                    Debug.LogWarning("Invalid round number!");
                    break;
            }
            momDialogueTriggered[round] = true; // Mark this round's dialogue as triggered
        }
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
        TriggerChaseDialogue(roundCount);
    }
    else
    {
        TriggerMomDialogue(roundCount);
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
        currentState = GameState.TileSubmission;
        Debug.Log($"State changed to: {currentState}");
    }
}



    private void HandleTileSubmission()
    {
        ClearText(); 

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
        //this is where I'm gonna be like - if TextDone=true
        if (TextDone == true)
        {
            activeRoundOption.SetActive(true);
        }

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
        if (TextDone == true)
        {

            if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input
                    .mousePosition); // Cast a ray from the camera to the mouse position
                if (Physics.Raycast(ray, out RaycastHit hit)) // Check for a hit
                {
                    // Loop through all children of the active round option
                    foreach (Transform child in
                             activeRoundOption.transform) // Update here to use activeRoundOption's children
                    {
                        if (hit.collider.gameObject == child.gameObject) // Check if the clicked object is a child
                        {
                            Debug.Log($"Tile clicked: {child.gameObject.name}"); // Log the clicked tile's name
                            activeRoundOption.SetActive(false); // Hide the selected round option tiles
                            TextDone = false;

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

        if (DecisionReactionDone == true)//i want to change this to if     public bool DecisionReactionDone = true;
        {
            // Deactivate all objects with the tag "bubble"
            //GameObject[] bubbles = GameObject.FindGameObjectsWithTag("bubble");
            //foreach (GameObject bubble in bubbles)
          //  {
               // bubble.SetActive(false);
           // }

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
                DecisionReactionDone = false;
                return;
            }
            currentState = GameState.SetChallengeText;
            DecisionReactionDone = false;
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

        {
            if (Timetorevealwinner == true)
            {
                if (tieCount > 1)
                {
                    TieStar.SetActive(true);
                    //dialogueRunner.StartDialogue("Tie");
                }
                else if (NPC1score == maxScore)
                {
                    //dialogueRunner.StartDialogue("GabeWin");
                    GabeStar.SetActive(true);
                }
                else if (NPC2score == maxScore)
                {
                    //dialogueRunner.StartDialogue("KateWin");
                    KateStar.SetActive(true);
                }
                else if (NPC3score == maxScore)
                {
                    //dialogueRunner.StartDialogue("DaniWin");
                    DaniStar.SetActive(true);
                }
                else if (NPC4score == maxScore)
                {
                    //dialogueRunner.StartDialogue("ChaseWin");
                    ChaseStar.SetActive(true);
                }
                else if (NPC5score == maxScore)
                {
                    //dialogueRunner.StartDialogue("JoannaWin");
                    JoannaStar.SetActive(true);
                }
            }
        }

        if (EndDisplayed == true)
        {
            winnerDisplayed = true;
            WillThisWork.SetActive(true);
        }
    }


    private void IncrementRound()
    {
        roundCount++;
        if (roundCount >= maxRounds)
        {
            //Debug.Log("Game Complete!");
        }
    }

    // Helper method to clear text
    private void ClearText()
    {
        buttonOption1Text.text = "";
        buttonOption2Text.text = "";
        CurrentChallenge.text = "";
    }
    

}
