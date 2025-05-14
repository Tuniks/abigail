using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ClayManager : AreaManager{
    public DialogueRunner dialogueRunner;
    
    [Header("NPCs")]
    //public GameObject chase;
    public GameObject kate;
    public GameObject mrmiller;
    public GameObject tourist;

    [Header("Dialogue Nodes")]
    public string state1Dialogue = "KateAz0";
    public string state2Dialogue = "Kate3";
    
    [Header("Mr Miller Power Azulejo")]
    public InteriorExteriorSwitching antiqueShopLogic;
    public string powerAzuMrMillerScene = "POWER_azulejo_MrMiller";
    public string powerMillerVictoryNode = "AntiqueShopVictory";
    public string powerMillerLossNode = "AntiqueShopLoss";

    [Header("Tourist Power Azulejo")]
    public string powerAzuTouristScene = "POWER_azulejo_Tourist";
    public string powerTouristVictoryNode = "Tourist1Win";
    public string powerTouristLossNode = "Tourist1Loss";

    [Header("Tiles")]
    public GameObject antiqueShop;

    public GameObject staticantiqueshop;
    public Tile[] salthairTile;
    public Tile[] handholdTile;
    public Tile[] clockTile;
    public Tile[] flyingTile;
    public Tile[] bellTile;
    
    [Header("GameObjects")]
    public GameObject AntiqueShopEntrance;

    private string lastPowerAzulejo = "";

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                UpdateDialogueNode(kate, state1Dialogue);
                break;
            case 2:
                UpdateDialogueNode(kate, state2Dialogue);
                break;
        }

        string lastPowerAzulejo = WorldState.Instance.GetLastOpponent();
        if(lastPowerAzulejo != ""){
            if(lastPowerAzulejo == "miller"){
                OpenAntiqueShop();
                MrMillerPostAzulejo();
            } else if (lastPowerAzulejo == "tourist"){
                TouristPostAzulejo();
            }
            
            WorldState.Instance.SetLastOpponent("");
        }
    }

    // NON YARN COMMANDS
    private void MrMillerPostAzulejo(){
        antiqueShopLogic.EnterArea();
        if(WorldState.Instance.GetWonLastMatch()){
            UpdateDialogueNode(mrmiller, powerMillerVictoryNode);
            dialogueRunner.StartDialogue(powerMillerVictoryNode);
        } else{
            UpdateDialogueNode(mrmiller, powerMillerLossNode);
            dialogueRunner.StartDialogue(powerMillerLossNode); 
        } 
    }

    private void TouristPostAzulejo(){
        if(WorldState.Instance.GetWonLastMatch()){
            UpdateDialogueNode(tourist, powerTouristVictoryNode);
            dialogueRunner.StartDialogue(powerTouristVictoryNode);
        } else{
            UpdateDialogueNode(tourist, powerTouristLossNode);
            dialogueRunner.StartDialogue(powerTouristLossNode);
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void VisitAntiqueShop(){
        WorldState.Instance.UpdateSceneState(Areas.Clay, 1, true);
    }

    [YarnCommand]
    public void PostClayAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Clay, 2);
        WorldState.Instance.UpdateNPCDialogueNode("Chase", "Chase4");
        WorldState.Instance.UpdateSceneState(Areas.Felt, 2);
    }
    
    [YarnCommand]
    public void OpenAntiqueShop(){
        AntiqueShopEntrance.SetActive(true);
        staticantiqueshop.SetActive(false); 
    }
    
    [YarnCommand]
    public void StartMrMillerPowerAzulejo(){
        WorldState.Instance.SetLastOpponent("miller");
        SceneController.Instance.Roundtrip(powerAzuMrMillerScene);
    }

    [YarnCommand]
    public void StartTouristPowerAzulejo(){
        WorldState.Instance.SetLastOpponent("tourist");
        SceneController.Instance.Roundtrip(powerAzuTouristScene);
    }
    
    [YarnCommand]
    public void GiveClock(){
        PlayerInventory.Instance.AddTilesToCollection(clockTile);
        
    }
    
    [YarnCommand]
    public void GiveFlyingPill(){
        PlayerInventory.Instance.AddTilesToCollection(flyingTile);
        
    }
    
    [YarnCommand]
    public void GiveBellTile(){
        PlayerInventory.Instance.AddTilesToCollection(bellTile);
        
    }
    
}
