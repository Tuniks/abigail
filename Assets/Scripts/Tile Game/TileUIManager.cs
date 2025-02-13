using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileUIManager : MonoBehaviour{
    [Header("UI Elements")]
    public GameObject tutorialScreen;
    public GameObject challengeScreen;
    public GameObject challengeList;
    public GameObject argumentScreen;
    public GameObject roundWonDialog;
    public GameObject gameWon;

    [Header("Text Fields")]
    public TextMeshProUGUI challengeScreenText;
    public TextMeshProUGUI challengeListText;
    public TextMeshProUGUI roundWonDialogText;
    public TextMeshProUGUI gameWonText;

    [Header("Argument Phase")]
    public GameObject argumentList;
    public TextMeshProUGUI[] argumentTexts = new TextMeshProUGUI[5];
    public TextMeshProUGUI judgeArgumentText;
    public TextMeshProUGUI playerArgumentText;
    public TextMeshProUGUI enemyArgumentText;
    private int argumentationStep = 0;
    private string judgeResp2 = "";

    [Header("Challenges")]
    public Challenges challengesManager;
    
    // Other
    private State currentState;

    void Update(){
        if(Input.GetKeyDown("e")){
            Advance();
        }
    }

    public void Advance(){
        if(currentState != State.Argument) return;
        AdvanceArgumentationDialogue();
    }

    public void SetUIState(State _state){
        currentState = _state;
        
        // Reset
        tutorialScreen.SetActive(false);
        challengeScreen.SetActive(false);
        challengeList.SetActive(false);
        argumentScreen.SetActive(false);
        roundWonDialog.SetActive(false);
        gameWon.SetActive(false);
        
        // Set
        switch(_state){
            case State.Tutorial:
                tutorialScreen.SetActive(true);
                break;
            
            case State.Setup:
                challengeScreen.SetActive(true);
                break;

            case State.Pick:
                challengeList.SetActive(true);
                break;
            
            case State.Argument:
                judgeArgumentText.transform.parent.gameObject.SetActive(true);
                argumentScreen.SetActive(true);
                break;
            
            case State.Decision:
                challengeList.SetActive(true);
                roundWonDialog.SetActive(true);
                break;

            case State.Result:
                gameWon.SetActive(true);
                break;
        }
    }

    // Challenge List for Set Up
    public void UpdateChallengesList(int[] challenges){
        if(!challengesManager) return;

        string challengesString =
            challengesManager.GetChallengeDescription(challenges[0]) + "\n" +
            challengesManager.GetChallengeDescription(challenges[1]) + "\n" +
            challengesManager.GetChallengeDescription(challenges[2]);
        
        challengeScreenText.text = challengesString;
    }

    // Challenge Bubble for Pick 
    public void UpdateChallengeBubble(int challenge){
        string challengesString = "Ok, so the challenge is <b>" +
            challengesManager.GetChallengeDescription(challenge).ToLower() + "</b>";
        
        challengeListText.text = challengesString;
    }

    // Argument Phase
    public void UpdatePreArgumentRoundWinner(bool playerWon){
        if(playerWon){
            judgeArgumentText.text = "Hmmm... I'm currently leaning <b>Abigail</b>, any arguments?";
        } else judgeArgumentText.text = "I think I might go with <b>Oz</b>, what do you say?";
    }

    public void UpdateArgumentList(List<string> arguments){
        for(int i = 0; i < argumentTexts.Length; i++){
            if(i < arguments.Count){
                argumentTexts[i].gameObject.SetActive(true);
                argumentTexts[i].text = arguments[i];
            } else {
                argumentTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateArgumentConversation(string pArg, string jResp1, string eArg, string jResp2){
        playerArgumentText.text = pArg;
        judgeArgumentText.text = jResp1;
        enemyArgumentText.text = eArg;
        judgeResp2 = jResp2;
        AdvanceArgumentationDialogue(true);
    }

    private void AdvanceArgumentationDialogue(bool fromPickedArgument = false){
        // VERY DUMB WAY OF DOING IT, DO IT BETTER LATER (list of strings w names, depending on the name choose where to place text..)
        switch(argumentationStep){
            case 0:
                argumentList.SetActive(true);
                judgeArgumentText.transform.parent.gameObject.SetActive(false);
                argumentationStep++;
                break;
            case 1:
                if(!fromPickedArgument) return;
                argumentList.SetActive(false);
                playerArgumentText.transform.parent.gameObject.SetActive(true);
                argumentationStep++; 
                break;
            case 2:
                playerArgumentText.transform.parent.gameObject.SetActive(false);
                judgeArgumentText.transform.parent.gameObject.SetActive(true);
                argumentationStep++;
                break;
            case 3:
                judgeArgumentText.transform.parent.gameObject.SetActive(false);
                enemyArgumentText.transform.parent.gameObject.SetActive(true);
                argumentationStep++;
                break;
            case 4:
                enemyArgumentText.transform.parent.gameObject.SetActive(false);
                judgeArgumentText.text = judgeResp2; 
                judgeArgumentText.transform.parent.gameObject.SetActive(true);
                argumentationStep++;
                break;
            case 5:
                judgeArgumentText.transform.parent.gameObject.SetActive(false);
                argumentationStep = 0;
                TileGameManager.instance.EndArgument();
                break;
        }
    }

    // Winner Phase
    public void UpdateRoundWinner(bool playerWon, string justification){
        if(playerWon){
            roundWonDialogText.text = "Ok, I gotta give this one to my girl <b>Abigail</b>" + justification;
        } else roundWonDialogText.text = "Yeah, this round <b>Oz</b> takes it!" + justification;
    }

    // Result Phase
    public void UpdateGameWinner(bool playerWon){
        if(playerWon){
            gameWonText.text = "<b>Abigail</b> wins!";
        } else gameWonText.text = "<b>Abigail</b> loses!";
    }  
}
