using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum State {
    Setup,
    Pick,
    Reveal,
    Modifier,
    Winner,
    Result,
}


public class TileGameManager : MonoBehaviour{
    public static TileGameManager instance;

    [Header("Screen Components")]
    public GameObject challengeScreen;
    public TextMeshProUGUI challengeScreenText;
    public GameObject challengeList;
    public TextMeshProUGUI challengeListText;
    public GameObject playerHand;
    public GameObject playerActive;
    public GameObject enemyHand;
    public GameObject enemyActive;

    // public GameObject modifierDialog;

    public GameObject roundWonDialog;
    public TextMeshProUGUI roundWonDialogText;
    public GameObject gameWon;
    public TextMeshProUGUI gameWonText;

    private State current = State.Setup;

    [Header("Players")]
    public ActiveSpot p1Active;
    public ActiveSpot p2Active;
    public EnemyHand p2Hand;

    private Challenges challengesManager;
    private int[] challenges;
    private int challengesCount = 3;
    private int currentChallengeIndex = 0;

    private int score = 0;


    void Awake(){
        instance = this;
        challengesManager = GetComponent<Challenges>();

        challenges = challengesManager.GetRandomChallenges(challengesCount);
        currentChallengeIndex = 0;

        StartSetup();
    }

    private void SetState(State _state){
        current = _state;

        if(current == State.Setup){
            challengeScreen.SetActive(true);
            challengeList.SetActive(false);
            playerHand.SetActive(false);
            playerActive.SetActive(false);
            enemyHand.SetActive(false);
            enemyActive.SetActive(false);
            // modifierDialog.SetActive(false);
            roundWonDialog.SetActive(false);
            gameWon.SetActive(false);
        } else if(current == State.Pick){
            challengeScreen.SetActive(false);
            challengeList.SetActive(true);
            playerHand.SetActive(true);
            playerActive.SetActive(false);
            enemyHand.SetActive(true);
            enemyActive.SetActive(false);
            // modifierDialog.SetActive(false);
            roundWonDialog.SetActive(false);
            gameWon.SetActive(false);
        // } else if(current == State.Reveal){
        //     challengeScreen.SetActive(false);
        //     challengeList.SetActive(true);
        //     playerHand.SetActive(false);
        //     playerActive.SetActive(true);
        //     enemyHand.SetActive(false);
        //     enemyActive.SetActive(true);
        //     // modifierDialog.SetActive(false);
        //     roundWonDialog.SetActive(false);
        //     gameWon.SetActive(false);
        // } else if(current == State.Modifier){
        //     challengeScreen.SetActive(false);
        //     challengeList.SetActive(true);
        //     playerHand.SetActive(false);
        //     playerActive.SetActive(true);
        //     enemyHand.SetActive(false);
        //     enemyActive.SetActive(true);
        //     // modifierDialog.SetActive(true);
        //     roundWonDialog.SetActive(false);
        //     gameWon.SetActive(false);
        } else if(current == State.Winner){
            challengeScreen.SetActive(false);
            challengeList.SetActive(true);
            playerHand.SetActive(false);
            playerActive.SetActive(true);
            enemyHand.SetActive(false);
            enemyActive.SetActive(true);
            // modifierDialog.SetActive(false);
            roundWonDialog.SetActive(true);
            gameWon.SetActive(false);
        } else if(current == State.Result){
            challengeScreen.SetActive(false);
            challengeList.SetActive(false);
            playerHand.SetActive(false);
            playerActive.SetActive(false);
            enemyHand.SetActive(false);
            enemyActive.SetActive(false);
            // modifierDialog.SetActive(false);
            roundWonDialog.SetActive(false);
            gameWon.SetActive(true);
        }
    }

    public void StartSetup(){
        string challengesString =
            challengesManager.GetChallengeDescription(challenges[0]) + "\n" +
            challengesManager.GetChallengeDescription(challenges[1]) + "\n" +
            challengesManager.GetChallengeDescription(challenges[2]);

        challengeScreenText.text = challengesString;
        challengeListText.text = challengesString;

        SetState(State.Setup);
    }

    public void StartRound(){
        if(currentChallengeIndex >2){
            RevealResult();
            return;
        }
        SetState(State.Pick);
    }

    // public void RevealPicks(){
    //     p2Hand.ActivateRandomCard();
    //     SetState(State.Reveal);
    // }

    public void RevealWinner(){
        p2Hand.ActivateRandomCard();
        int result = CheckResult();
        if(result == 0){
            roundWonDialogText.text = "You win this round!";
        } else roundWonDialogText.text = "Your opponent wins this round!";
        SetState(State.Winner);
        currentChallengeIndex++;
    }

    public void RevealResult(){
        if(score>=0){
            gameWonText.text = "You win!";
        } else gameWonText.text = "You lose!";
        SetState(State.Result);
    }

    private int CheckResult(){
        int index = challenges[currentChallengeIndex];
        float p1 = challengesManager.EvaluateTile(index, p1Active.activeTile);
        float p2 = challengesManager.EvaluateTile(index, p2Active.activeTile);

        if(p1 >= p2){
            score++;
            return 0;
        }
        score--;
        return 1;
    }

    public void ResetGame(){
        SceneManager.LoadScene("TileGame");
    }
}
