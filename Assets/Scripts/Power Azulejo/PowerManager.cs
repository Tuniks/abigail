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
    public List<Tile> playerInventory;
    public PowerTile[] playerTiles;

    public List<Tile> enemyInventory;
    public PowerTile[] enemyTiles;

    [Header("Tutorial References")]
    public GameObject tutorialUI;

    private PowerStage currentStage = PowerStage.None;

    void Awake(){
        Instance = this;
    }

    void Start(){
        builder = GetComponent<PowerTileBuilder>();

        if(showTutorial){
            ChangeState(PowerStage.Tutorial);
        } else ChangeState(PowerStage.Game);
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

    }

    private void ExitTutorialStage(){

    }

    // Selection
    private void EnterSelectionStage(){

    }

    private void ExitSelectionStage(){

    }

    // Intro
    private void EnterIntroStage(){

    }

    private void ExitIntroStage(){

    }

    // Game
    private void EnterGameStage(){
        BuildTiles();
        game.StartGame();

    }

    private void ExitGameStage(){

    }

    // End
    private void EnterEndStage(){

    }

    private void ExitEndStage(){

    }


    // ========= AUX ========
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

    public void TriggerEnding(bool playerWon){
        
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
