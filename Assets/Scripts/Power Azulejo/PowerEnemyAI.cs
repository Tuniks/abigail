using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEnemyAI : MonoBehaviour{
    // Game State
    private PowerSumoGame sumoGame;
    private PowerTile[] playerTiles;
    private PowerTile[] enemyTiles;

    // Juice
    [Header("Juice")]
    public float timeBetweenActions;

    // Enemy Skill Data
    [Header("Enemy Skill Data")]
    public float targetTileError = 0.1f; // How often they will pick the less optimal target
    public float aimError = 0.025f; // X and Y error after target tile was picked
    public float powerError = 0.05f; // Power error after target position was picked

    // Enemy Current State
    private int currentStamina = 0;
    private List<GameObject> currentMoved;

    public void InitializeEnemyAI(){
        sumoGame = GetComponent<PowerSumoGame>();
        playerTiles = PowerManager.Instance.GetPlayerTiles();
        enemyTiles = PowerManager.Instance.GetEnemyTiles();
    }

    public void StartEnemyTurn(int stam, List<GameObject> movedlist){
        currentStamina = stam;
        currentMoved = movedlist;
        EnemyAIStep();
    }

    private void EnemyAIStep(){
        if(currentStamina <= 0 || !sumoGame.IsGameOn()){
            EndTurn();
            return;
        }

        // Picking a tile to move and a target
        PowerTile currentTile = null;
        PowerTile currentTarget = null;
        float currentMinDist = Mathf.Infinity;

        foreach(PowerTile enemyTile in enemyTiles){
            // If tile is dead, skip
            if(enemyTile == null) continue;
            
            // If tile has already moved, skip
            if(!sumoGame.CanMultiMove() && currentMoved.Contains(enemyTile.gameObject)) continue;

            // If tile hasn't move, check for the nearest player tile
            PowerTile innerTarget = null;
            float innerMinDist = Mathf.Infinity;
            foreach(PowerTile playerTile in playerTiles){
                // If player tile is dead, skip
                if(playerTile == null || playerTile.gameObject == null) continue;
            
                float dist = Vector2.Distance(enemyTile.transform.position, playerTile.transform.position);
                // If distance between enemy and player is smaller than before
                if(dist < innerMinDist){
                    // If there is currently no target, this one goes by default
                    // If there is a target already, there's a chance the enemy won't pick this better option as an error
                    if(innerTarget == null){
                        innerTarget = playerTile;
                        innerMinDist = dist;
                    } else if(Random.value > targetTileError){
                        innerTarget = playerTile;
                        innerMinDist = dist;
                    }
                }
            }

            // If the current checked enemy tile has a better target than previous, set it as the current tile to be moved
            if(innerMinDist <= currentMinDist){
                currentMinDist = innerMinDist;
                currentTile = enemyTile;
                currentTarget = innerTarget;
            }
        }

        // If there are no tiles to move, end turn
        if(currentTile == null || currentTarget == null){
            EndTurn();
            return;
        }

        // If there are, execute move
        StartCoroutine(ExecuteMove(currentTile, currentTarget));
    }

    private IEnumerator ExecuteMove(PowerTile currentTile, PowerTile targetTile){
        // Change camera to follow current tile
        PowerCinemachine.Instance.SetTarget(currentTile.transform);

        yield return new WaitForSeconds(timeBetweenActions);

        // Calculate direction taking error into account
        Vector3 dir = targetTile.transform.position - currentTile.transform.position;
        dir = dir.normalized;
        dir += new Vector3(Random.Range(-aimError, aimError), Random.Range(-aimError, aimError), 0);

        // Calculate power (TO DO ACTUALLY DO SMTH INTERESTING)
        float pct = 1 - Random.Range(-powerError/2, powerError);

        // Mark tile and decrease stamina
        currentMoved.Add(currentTile.gameObject);
        currentStamina--;
        currentTile.SetCanMove(false);
        
        // Launch
        currentTile.LaunchTile(dir, pct);

        StartCoroutine(CallNextStep());
    }

    private IEnumerator CallNextStep(){
        yield return new WaitForSeconds(timeBetweenActions);
        EnemyAIStep();
    }

    private void EndTurn(){
        sumoGame.EndEnemyTurn();
    }

}
