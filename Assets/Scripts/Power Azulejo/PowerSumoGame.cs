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

    [Header("Tile Data")]
    public float tileSide = 0;

    [Header("Power Data")]
    public GameObject wallPrefab;

    // State
    private bool isGameOn = false;
    private bool isPlayerTurn = false;
    private List<GameObject> deadTiles = new List<GameObject>();

    // Player Data
    private int playerCurrentScore = 0;
    private int playerCurrentStamina = 0;
    private PowerTile[] playerTiles;
    private List<GameObject> playerMovedList = new List<GameObject>();

    // Enemy Data
    private int enemyCurrentScore = 0;
    private int enemyCurrentStamina = 0;
    private PowerTile[] enemyTiles;
    private List<GameObject> enemyMovedList = new List<GameObject>();

    public void StartGame(){
        isGameOn = true;
        
        playerTiles = PowerManager.Instance.GetPlayerTiles();
        enemyTiles = PowerManager.Instance.GetEnemyTiles();
        
        enemyAI = GetComponent<PowerEnemyAI>();
        enemyAI.InitializeEnemyAI();
        StartPlayerTurn();
    }

    // ======== GAME STATE ==========
    private void StartPlayerTurn(){
        isPlayerTurn = true;
        playerCurrentStamina = playerMaxStamina;
        enemyCurrentStamina = enemyMaxStamina;
        playerMovedList = new List<GameObject>();
        enemyMovedList = new List<GameObject>();

        foreach(PowerTile tile in playerTiles){
            if(tile == null) continue;
            tile.SetCanMove(true);
        }

        StartCoroutine(PlayerTurnTimer(true, timeBetweenTurns));
    }

    private void EndPlayerTurn(){
        isPlayerTurn = false;

        foreach(PowerTile tile in playerTiles){
            if(tile == null) continue;
            tile.SetCanMove(false);
        }
        
        StartCoroutine(StartEnemyTurn());
    }
    
    private IEnumerator StartEnemyTurn(){
        foreach(PowerTile tile in enemyTiles){
            if(tile == null) continue;
            tile.SetCanMove(true);
        }
        
        yield return new WaitForSeconds(timeBetweenTurns);

        enemyAI.StartEnemyTurn(enemyCurrentStamina, enemyMovedList);
    }

    public void EndEnemyTurn(){
        CheckForEndGame();

        foreach(PowerTile tile in enemyTiles){
            if(tile == null) continue;
            tile.SetCanMove(false);
        }

        StartPlayerTurn();
    }

    private IEnumerator PlayerTurnTimer(bool state, float timer){
        yield return new WaitForSeconds(timer);
        isPlayerTurn = state;
    }

    private bool CheckForEndTurn(){
        if(playerCurrentStamina <= 0) return true;

        // Check if there are any alive tiles that can move
        bool movesLeft = false;
        foreach(PowerTile player in playerTiles){
            // check if tile is dead
            if(player == null) continue;

            // check if tile can move
            if(!playerMovedList.Contains(player.gameObject)) movesLeft = true; 
        }

        return !movesLeft;
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
            PowerManager.Instance.TriggerGameEnd(true);
        } else if(enemyCurrentScore >= targetScore){
            isGameOn = false;
            PowerManager.Instance.TriggerGameEnd(false);
        } 
    }

    // ========== MOVEMENT ===========
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

        UseStamina(tile, cost);
    }

    private void UseStamina(GameObject tile, int cost){
        // Subtract stamina, mark tile as moved
        playerCurrentStamina -= cost;
        playerMovedList.Add(tile);
        tile.GetComponent<PowerTile>().SetCanMove(false);

        // Check if turn has ended, if not allow player to move after timer
        if(CheckForEndTurn()){
            EndPlayerTurn();
        } else StartCoroutine(PlayerTurnTimer(true, timeBetweenActions));
    }

    // ======= POWERS ========
    public bool CanPlayerExecutePower(GameObject tile, int cost){
        if(!isGameOn || !isPlayerTurn) return false;

        if(playerCurrentStamina - cost < 0) return false;
        if(!canMultiMove && playerMovedList.Contains(tile)) return false;

        return true;
    }

    public void ExecutePull(GameObject tile, int cost, float radius, float forcePct){
        if(!CanPlayerExecutePower(tile, cost)) return;

        // Iterate through enemy tiles to see who's affected by the pull
        foreach(PowerTile player in playerTiles){
            if(player == null) continue;
            if(player.gameObject == tile) continue;

            float dist = Vector3.Distance(tile.transform.position, player.gameObject.transform.position);
            
            // If tile is inside of radius (plus a lil bit)
            if(dist < (radius + tileSide/2)){
                Vector3 dir = tile.transform.position - player.gameObject.transform.position;
                player.LaunchTile(dir, forcePct);
            }
        }

        UseStamina(tile, cost);
    }

    public void ExecutePush(GameObject tile, int cost, float radius, float forcePct){
        if(!CanPlayerExecutePower(tile, cost)) return;

        // Iterate through enemy tiles to see who's affected by the pull
        foreach(PowerTile enemy in enemyTiles){
            if(enemy == null) continue;

            float dist = Vector3.Distance(tile.transform.position, enemy.gameObject.transform.position);
            
            // If tile is inside of radius (plus a lil bit)
            if(dist < (radius + tileSide/2)){
                Vector3 dir = tile.transform.position - enemy.gameObject.transform.position;
                enemy.LaunchTile(dir, forcePct);
            }
        }

        UseStamina(tile, cost);
    }

    public void ExecuteTeleport(GameObject tile, int cost){
        if(!CanPlayerExecutePower(tile, cost)) return;
        
        tile.GetComponent<PowerTilePhysics>().StartPowerTargeting(Teleport);
    }

    public void Teleport(GameObject tile, Vector3 pos){
        tile.transform.position = pos;
        UseStamina(tile, 2);
    }

    public void ExecuteWall(GameObject tile, int cost){
        if(!CanPlayerExecutePower(tile, cost)) return;
        
        tile.GetComponent<PowerTilePhysics>().StartPowerTargeting(PlaceWall);
    }

    public void PlaceWall(GameObject tile, Vector3 pos){
        Instantiate(wallPrefab, pos, Quaternion.identity);

        UseStamina(tile, 2);
    }

    // ======= HUD =======
    public void PassTurn(){
        if(!isGameOn || !isPlayerTurn) return;
    
        EndPlayerTurn();
    }

    // ======= GETTERS AND SETTERS ========
    public bool IsGameOn(){
        return isGameOn;
    }
    
    public bool CanMultiMove(){
        return canMultiMove;
    }

    public bool IsPlayerTurn(){
        return isPlayerTurn;
    }

    public int GetPlayerStamina(){
        return playerCurrentStamina;
    }
}
