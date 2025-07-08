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

    [Header("Game Settings")]
    public bool showTutorial = true;

    [Header("Tutorial References")]
    public GameObject tutorialUI;

    private PowerStage currentStage = PowerStage.None;

    void Awake(){
        Instance = this;
    }

    void Start(){
        if(showTutorial){
            ChangeState(PowerStage.Tutorial);
        } else ChangeState(PowerStage.Selection);
    }

    // Managing States 
    private void ChangeState(PowerStage newStage){
        if(newStage == currentStage) return;

        // Exiting current state
        switch(currentStage){
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

        // Entering new state
        switch(newStage){
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

    }

    private void ExitGameStage(){

    }

    // End
    private void EnterEndStage(){

    }

    private void ExitEndStage(){

    }

}
