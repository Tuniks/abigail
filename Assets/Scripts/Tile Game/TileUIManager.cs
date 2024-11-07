using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileUIManager : MonoBehaviour{
    [Header("UI Elements")]
    public GameObject challengeScreen;
    public GameObject challengeList;
    public GameObject argumentScreen;
    public GameObject roundWonDialog;
    public GameObject gameWon;

    [Header("Text Fields")]
    public TextMeshProUGUI challengeScreenText;
    public TextMeshProUGUI challengeListText;
    public TextMeshProUGUI roundPreWonDialogText;
    public TextMeshProUGUI[] argumentTexts = new TextMeshProUGUI[3];
    public TextMeshProUGUI roundWonDialogText;
    public TextMeshProUGUI gameWonText;

    public Challenges challengesManager;

    public void SetUIState(State _state){
        // Reset
        challengeScreen.SetActive(false);
        challengeList.SetActive(false);
        argumentScreen.SetActive(false);
        roundWonDialog.SetActive(false);
        gameWon.SetActive(false);
        
        // Set
        switch(_state){
            case State.Tutorial:
                break;
            
            case State.Setup:
                challengeScreen.SetActive(true);
                break;

            case State.Pick:
                challengeList.SetActive(true);
                break;
            
            case State.Argument:
                argumentScreen.SetActive(true);
                break;
            
            case State.Winner:
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
            roundPreWonDialogText.text = "Hmmm... I'm currently leaning <b>Abigail</b>, any arguments?";
        } else roundPreWonDialogText.text = "I think I might go with <b>Oz</b>, what do you say?";
    }

    public void UpdateArgumentBubbles(string[] arguments, string cardName){
        argumentTexts[0].text = arguments[0].Replace("$name$", cardName);
        argumentTexts[1].text = arguments[1].Replace("$name$", cardName);
        argumentTexts[2].text = arguments[2].Replace("$name$", cardName);
    }

    // Winner Phase
    public void UpdateRoundWinner(bool playerWon){
        if(playerWon){
            roundWonDialogText.text = "Ok, I gotta give this one to my girl <b>Abigail</b>";
        } else roundWonDialogText.text = "Yeah, this round <b>Oz</b> takes it!";
    }

    // Result Phase
    public void UpdateGameWinner(bool playerWon){
        if(playerWon){
            gameWonText.text = "<b>Abigail</b> wins!";
        } else gameWonText.text = "<b>Abigail</b> loses!";
    }  
}
