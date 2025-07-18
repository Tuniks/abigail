using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerStage {
    None,
    Tutorial,
    Selection,
    Intro,
    Game,
    End
}

public class PowerManager : MonoBehaviour{
    public static PowerManager Instance;
    private PowerTileBuilder builder;

    [Header("Game Settings")]
    public PowerSumoGame game;
    public bool showTutorial = true;

    [Header("Game Data")]
    private List<Tile> playerInventory;
    public PowerTile[] playerTiles;

    public List<Tile> enemyInventory;
    public PowerTile[] enemyTiles;

    [Header("Tutorial References")]
    public GameObject tutorialUI;

    [Header("Selection Manager")]
    public GameObject selectionUI;
    public PowerInventory powerInventory;

    [Header("Intro References")]
    public GameObject introUI;

    [Header("Game References")]
    public GameObject playerHUD;

    [Header("Ending References")]
    public GameObject endUI;

    private PowerStage currentStage = PowerStage.None;
    private bool playerWon = false;

    void Awake(){
        Instance = this;
    }

    void Start(){
        builder = GetComponent<PowerTileBuilder>();

        if(showTutorial){
            ChangeState(PowerStage.Tutorial);
        } else ChangeState(PowerStage.Selection);
    }

    // ===== Managing States ======= 
    private void ChangeState(PowerStage newStage){
        if(newStage == currentStage) return;
        
        // Exiting current state
        switch(currentStage){
            case PowerStage.None:
                break;
            case PowerStage.Tutorial:
                ExitTutorialStage();
                break;
            case PowerStage.Selection:
                ExitSelectionStage();
                break;
            case PowerStage.Intro:
                ExitIntroStage();
                break;
            case PowerStage.Game:
                ExitGameStage();
                break;
            case PowerStage.End:
                ExitEndStage();
                break;
        }

        // Entering new state
        switch(newStage){
            case PowerStage.None:
                break;
            case PowerStage.Tutorial:
                EnterTutorialStage();
                break;
            case PowerStage.Selection:
                EnterSelectionStage();
                break;
            case PowerStage.Intro:
                EnterIntroStage();
                break;
            case PowerStage.Game:
                EnterGameStage();
                break;
            case PowerStage.End:
                EnterEndStage();
                break;
        }

        // Updating current state
        currentStage = newStage;
    }

    // Tutorial
    private void EnterTutorialStage(){
        tutorialUI.SetActive(true);
        tutorialUI.GetComponent<PowerSumoTutorial>().Initialize();

    }

    private void ExitTutorialStage(){
        tutorialUI.SetActive(false);
    }

    // Selection
    private void EnterSelectionStage(){
        selectionUI.SetActive(true);
        powerInventory.Initialize();
    }

    private void ExitSelectionStage(){
        selectionUI.SetActive(false);
    }

    // Intro
    private void EnterIntroStage(){
        introUI.SetActive(true);
        introUI.GetComponent<PowerIntro>().Initialize(playerInventory, enemyInventory);
    }

    private void ExitIntroStage(){
        introUI.SetActive(false);
    }

    // Game
    private void EnterGameStage(){
        BuildTiles();
        playerHUD.SetActive(true);
        PowerCameraManager.Instance.SetCameraState(true);
        game.StartGame();

    }

    private void ExitGameStage(){
        playerHUD.SetActive(false);
        PowerCameraManager.Instance.SetCameraState(false);
        
        foreach(PowerTile tile in playerTiles){
            tile.gameObject.SetActive(false);
        }

        foreach(PowerTile tile in enemyTiles){
            tile.gameObject.SetActive(false);
        }
    }

    // End
    private void EnterEndStage(){
        endUI.SetActive(true);
        endUI.GetComponent<PowerEnd>().Initialize(playerWon);
    }

    private void ExitEndStage(){
        endUI.SetActive(false);
    }


    // ========= AUX ========
    public void TriggerTutorialEnd(){
        ChangeState(PowerStage.Selection);
    }

    public void TriggerSelectionEnd(List<Tile> tileset){
        playerInventory = tileset;
        ChangeState(PowerStage.Intro);
    }

    public void TriggerIntroEnd(){
        ChangeState(PowerStage.Game);
    }

    private void BuildTiles(){
        // Building player tiles
        for(int i = 0; i < playerTiles.Length; i++){
            builder.BuildTile(playerInventory[i], playerTiles[i], true);
        }

        // Building enemy tiles
        for(int i = 0; i < enemyTiles.Length; i++){
            builder.BuildTile(enemyInventory[i], enemyTiles[i], false);
        }
    }

    public void TriggerGameEnd(bool won){
        playerWon = won;
        ChangeState(PowerStage.End);    
    }

    public void TriggerEndEnd(){
        // Return to last scene
        Debug.Log("aaaaa");
    }

    // ========= GETTERS AND SETTERS =========
    public PowerSumoGame GetPowerSumoGame(){
        return game;
    }

    public PowerTile[] GetPlayerTiles(){
        return playerTiles;
    }

    public PowerTile[] GetEnemyTiles(){
        return enemyTiles;
    }

}
