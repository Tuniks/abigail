using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State {
    Tutorial,
    Setup,
    Pick,
    Argument,
    Decision,
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
    public ArgumentCollection playerArguments;
    public ArgumentCollection enemyArguments;
    public ArgumentCollection judgeArguments;
    private List<Argument> currentPlayerArgs;
    private Argument currentEnemyArg;

    // If not empty, these will override random assignement of cards and challenges 
    [Header("Presets")]
    public int[] challengePresets;

    public bool showTutorial = false;

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

        StartGame();
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
                UIManager.UpdateChallengeBubble(challenges[currentChallengeIndex]);
                playerHand.SetActive(true);
                enemyHand.SetActive(true);
                break;
            
            case State.Argument:
                playerActive.SetActive(true);
                enemyActive.SetActive(true);
                break;
            
            case State.Decision:
                playerActive.SetActive(true);
                enemyActive.SetActive(true);
                break;

            case State.Result:
                break;
        }

        // Set UI Elements
        UIManager.SetUIState(current);
    }

    public void StartGame(){
        if(showTutorial){
            StartTutorial();
        } else StartSetup();
    }

    // Tutorial Phase
    public void StartTutorial(){
        SetState(State.Tutorial);
    }


    // Set Up Phase
    public void StartSetup(){
        UIManager.UpdateChallengesList(challenges);
        SetState(State.Setup);
    }

    // Pick Phase
    public void StartRound(){
        if(currentChallengeIndex >2){
            RevealResult();
            return;
        }
        SetState(State.Pick);
    }

    // Argument Phase
    public void RevealPreArgumentWinner(){
        if(current != State.Pick) return;

        // Select card for opponent
        p2Hand.ActivateRandomCard();

        // Check result
        bool result = CheckResult();

        // Get Argument overlap
        List<Argument> playerArgs = judgeArguments.GetOverlap(playerArguments.argumentCollection);
        List<Argument> enemyArgs = judgeArguments.GetOverlap(enemyArguments.argumentCollection);

        // Get narrower set of arguments
        currentPlayerArgs = PickRandomArguments(playerArgs, 5);
        List<string> playerArgsLines= GetArgumentsText(currentPlayerArgs);
        enemyArgs = PickRandomArguments(enemyArgs, 1);
        currentEnemyArg = enemyArgs[0];

        // Update UI
        UIManager.UpdatePreArgumentRoundWinner(result);
        UIManager.UpdateArgumentList(playerArgsLines);
        SetState(State.Argument);
    }

    public void OnArgumentPicked(int arg){
        Argument chosen = currentPlayerArgs[arg];
        // p1Active.activeTile.AddMultiplier(chosen.Item1, chosen.Item3);
        RevealWinner();
    }

    private List<Argument> PickRandomArguments(List<Argument> args, int count){
        List<Argument> rand = new List<Argument>();
        
        // TO DO ALWAYS PICK SILENCE OPTION
        while(args.Count > 0 && count > 0){
            int r = Random.Range(0, args.Count);
            rand.Add(args[r]);
            args.RemoveAt(r);
            count--;
        }

        return rand;
    }

    private List<string> GetArgumentsText(List<Argument> args){
        List<string> lines = new List<string>();
        foreach(Argument arg in args){
            lines.Add(arg.GetArgumentationLine());
        }
        return lines;
    }

    // Winner Phase
    public void RevealWinner(){
        // Select argument for opponent

        // Check result
        bool result = CheckResult();

        // Update UI
        UIManager.UpdateRoundWinner(result);
        SetState(State.Decision);

        // Move to next challenge
        currentChallengeIndex++;
    }

    // Result Phase
    public void RevealResult(){
        UIManager.UpdateGameWinner(score>=0);
        SetState(State.Result);
    }

    // General
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
        SceneManager.LoadScene("FeltAreaPostAzulejo");
    }
}
