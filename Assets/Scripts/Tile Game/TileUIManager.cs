using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileUIManager : MonoBehaviour{
    [Header("UI Elements")]
    public GameObject challengeScreen;
    public GameObject challengeList;
    public GameObject roundWonDialog;
    public GameObject gameWon;

    [Header("Text Fields")]
    public TextMeshProUGUI challengeScreenText;
    public TextMeshProUGUI challengeListText;
    public TextMeshProUGUI roundWonDialogText;
    public TextMeshProUGUI gameWonText;

    public Challenges challengesManager;

    public void SetUIState(State _state){
        // Reset
        challengeScreen.SetActive(false);
        challengeList.SetActive(false);
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
                challengeList.SetActive(true);
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

    public void UpdateChallengesList(int[] challenges){
        if(!challengesManager) Debug.Log("Nadinha");

        string challengesString =
            challengesManager.GetChallengeDescription(challenges[0]) + "\n" +
            challengesManager.GetChallengeDescription(challenges[1]) + "\n" +
            challengesManager.GetChallengeDescription(challenges[2]);
        
        challengeScreenText.text = challengesString;
        challengeListText.text = challengesString;
    }

    public void UpdateRoundWinner(bool playerWon){
        if(playerWon){
            roundWonDialogText.text = "You win this round!";
        } else roundWonDialogText.text = "Your opponent wins this round!";
    }

    public void UpdateGameWinner(bool playerWon){
        if(playerWon){
            gameWonText.text = "You win!";
        } else gameWonText.text = "You lose!";
    }  
}
