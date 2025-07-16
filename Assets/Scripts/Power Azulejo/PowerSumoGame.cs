using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSumoGame : MonoBehaviour{
    [Header("Game Variables")]
    public int targetScore = 3;
    public int playerMaxStamina = 3;
    public int enemyMaxStamina = 3;
    public bool canMultiMove = false;
    private PowerEnemyAI enemyAI;

    [Header("Timing")]
    public float timeBetweenActions = 1f;
    public float timeBetweenTurns = 1f;

    // State
    private bool isGameOn = false;
    private bool isPlayerTurn = false;
    private List<GameObject> deadTiles = new List<GameObject>();

    // Player Data
    private int playerCurrentScore = 0;
    private int playerCurrentStamina = 0;
    private List<GameObject> playerMovedList = new List<GameObject>();

    // Enemy Data
    private int enemyCurrentScore = 0;
    private int enemyCurrentStamina = 0;
    private List<GameObject> enemyMovedList = new List<GameObject>();

    public void StartGame(){
        isGameOn = true;
        enemyAI = GetComponent<PowerEnemyAI>();
        enemyAI.InitializeEnemyAI();
        StartPlayerTurn();
    }

    private void StartPlayerTurn(){
        isPlayerTurn = true;
        playerCurrentStamina = playerMaxStamina;
        enemyCurrentStamina = enemyMaxStamina;
        playerMovedList = new List<GameObject>();
        enemyMovedList = new List<GameObject>();

        StartCoroutine(PlayerTurnTimer(true, timeBetweenTurns));
    }

    private void EndPlayerTurn(){
        isPlayerTurn = false;
        
        StartCoroutine(StartEnemyTurn());
    }
    
    private IEnumerator StartEnemyTurn(){
        yield return new WaitForSeconds(timeBetweenTurns);
        enemyAI.StartEnemyTurn(enemyCurrentStamina, enemyMovedList);
    }

    public void EndEnemyTurn(){
        CheckForEndGame();
        StartPlayerTurn();
    }

    private IEnumerator PlayerTurnTimer(bool state, float timer){
        yield return new WaitForSeconds(timer);
        isPlayerTurn = state;
    }

    public bool CanPlayerMove(GameObject tile, int cost = 1){
        if(!isGameOn || !isPlayerTurn) return false;

        if(playerCurrentStamina - cost < 0) return false;
        if(!canMultiMove && playerMovedList.Contains(tile)) return false;

        return true;
    }

    public void MovePlayer(GameObject tile, int cost = 1){
        if(!CanPlayerMove(tile, cost)) return;

        // Temporarily stops player from acting
        isPlayerTurn = false;

        // Subtract stamina, mark tile as moved
        playerCurrentStamina -= cost;
        playerMovedList.Add(tile);

        // Check if turn has ended, if not allow player to move after timer
        if(CheckForEndTurn()){
            EndPlayerTurn();
        } else StartCoroutine(PlayerTurnTimer(true, timeBetweenActions));
    }

    private bool CheckForEndTurn(){
        if(playerCurrentStamina <= 0) return true;
        return false;
    }

    public void RegisterDeath(bool isPlayerTile, GameObject obj){
        if(deadTiles.Contains(obj)) return;

        deadTiles.Add(obj);
        
        if(isPlayerTile){
            enemyCurrentScore++;
        } else playerCurrentScore++;

        CheckForEndGame();
    }

    private void CheckForEndGame(){
        if(playerCurrentScore >= targetScore){
            isGameOn = false;
            PowerManager.Instance.TriggerEnding(true);
        } else if(enemyCurrentScore >= targetScore){
            isGameOn = false;
            PowerManager.Instance.TriggerEnding(false);
        } 
    }

    // ======= GETTERS AND SETTERS ========
    public bool IsGameOn(){
        return isGameOn;
    }
    
    public bool CanMultiMove(){
        return canMultiMove;
    }

}
