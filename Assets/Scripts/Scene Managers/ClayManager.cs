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

    [Header("Dialogue Nodes")]
    public string state1Dialogue = "KateAz0";
    public string state2Dialogue = "Kate3";
    
    [Header("Scenes")]
    public string powerAzuMrMillerScene = "CLAY_azulejo";
    public string powerAzuTouristScene = "CLAY_azulejo";
    
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

        if(lastPowerAzulejo != ""){
            if(lastPowerAzulejo == "miller"){

            } else if (lastPowerAzulejo == "tourist"){

            }
            lastPowerAzulejo = "";
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
    public void OpenAntiqueShop()
    {
        AntiqueShopEntrance.SetActive(true);
        staticantiqueshop.SetActive(false); 
    }
    
    [YarnCommand]
    public void StartMrMillerPowerAzulejo(){

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
