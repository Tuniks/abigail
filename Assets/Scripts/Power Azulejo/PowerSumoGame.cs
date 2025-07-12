using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSumoGame : MonoBehaviour{
    [Header("Game Variables")]

    public int playerMaxStamina = 3;
    public int enemyMaxStamina = 3;

    [Header("Timing")]
    public float timeBetweenActions = 1f;
    public float timeBetweenTurns = 1.5f;

    // State
    private bool isGameOn = false;
    private bool isPlayerTurn = false;

    // Player Data
    private int playerCurrentStamina = 0;
    private List<GameObject> playerMovedList = new List<GameObject>();

    // Enemy Data
    private int enemyCurrentStamina = 0;

    public void StartGame(){
        isGameOn = true;
        StartPlayerTurn();
    }

    private void StartPlayerTurn(){
        isPlayerTurn = true;
        playerCurrentStamina = playerMaxStamina;
        playerMovedList = new List<GameObject>();

        StartCoroutine(PlayerTurnTimer(true, timeBetweenTurns));
    }

    private void EndPlayerTurn(){
        isPlayerTurn = false;
        enemyCurrentStamina = enemyMaxStamina;
    }

    private IEnumerator PlayerTurnTimer(bool state, float timer){
        yield return new WaitForSeconds(timer);
        isPlayerTurn = state;
    }

    public bool CanPlayerMove(GameObject tile, int cost = 1){
        if(!isGameOn || !isPlayerTurn) return false;

        if(playerCurrentStamina - cost < 0) return false;
        if(playerMovedList.Contains(tile)) return false;

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


}
