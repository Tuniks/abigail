using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State {
    Tutorial,
    Setup,
    Pick,
    Argument,
    Winner,
    Result,
}


public class TileGameManager : MonoBehaviour{
    public static TileGameManager instance;

    [Header("Screen Components")]
    public GameObject playerHand;
    public GameObject playerActive;
    public GameObject enemyHand;
    public GameObject enemyActive;

    [Header("Players")]
    public ActiveSpot p1Active;
    public ActiveSpot p2Active;
    public EnemyHand p2Hand;

    // If not empty, these will override random assignement of cards and challenges 
    [Header("Presets")]
    public int[] challengePresets;

    private TileUIManager UIManager;
    private State current = State.Setup;

    private Challenges challengesManager;
    private int[] challenges;
    private int challengesCount = 3;
    private int currentChallengeIndex = 0;

    private int score = 0;


    void Awake(){
        instance = this;
        challengesManager = GetComponent<Challenges>();
        UIManager = GetComponent<TileUIManager>();

        challenges = challengesManager.GetRandomChallenges(challengesCount);
        currentChallengeIndex = 0;

        StartSetup();
    }

    private void SetState(State _state){
        current = _state;

        // Reset
        playerHand.SetActive(false);
        playerActive.SetActive(false);
        enemyHand.SetActive(false);
        enemyActive.SetActive(false);


        // Set
        switch(_state){
            case State.Tutorial:
                break;
            
            case State.Setup:
                break;

            case State.Pick:
                playerHand.SetActive(true);
                enemyHand.SetActive(true);
                break;
            
            case State.Argument:
                playerActive.SetActive(true);
                enemyActive.SetActive(true);
                break;
            
            case State.Winner:
                playerActive.SetActive(true);
                enemyActive.SetActive(true);
                break;

            case State.Result:
                break;       
        }

        // Set UI Elements
        UIManager.SetUIState(current);
    }

    public void StartSetup(){
        UIManager.UpdateChallengesList(challenges);
        SetState(State.Setup);
    }

    public void StartRound(){
        if(currentChallengeIndex >2){
            RevealResult();
            return;
        }
        SetState(State.Pick);
    }

    public void RevealWinner(){
        // Select card for opponent
        p2Hand.ActivateRandomCard();

        // Check result
        bool result = CheckResult();

        // Update UI
        UIManager.UpdateRoundWinner(result);
        SetState(State.Winner);

        // Move to next challenge
        currentChallengeIndex++;
    }

    public void RevealResult(){
        UIManager.UpdateGameWinner(score>=0);
        SetState(State.Result);
    }

    private bool CheckResult(){
        int index = challenges[currentChallengeIndex];
        float p1 = challengesManager.EvaluateTile(index, p1Active.activeTile);
        float p2 = challengesManager.EvaluateTile(index, p2Active.activeTile);

        if(p1 >= p2){
            score++;
            return true;
        }
        score--;
        return false;
    }

    public void ResetGame(){
        SceneManager.LoadScene("TileGame");
    }
}
