using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class ClayManager : AreaManager{
    public DialogueRunner dialogueRunner;
    
    [Header("NPCs")]
    public GameObject chase;
    public GameObject kate;

    [Header("Dialogue Nodes")]
    public string state1Dialogue = "PostAntiqueShop";
    public string state2Dialogue = "PostClayAzulejo";
    
    [Header("Scenes")]
    public string azulejoScene = "CLAY_azulejo";
    
    [Header("Misc")]
    public GameObject antiqueShop;
    public Tile[] salthairTile;
    public Tile[] handholdTile;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                UpdateDialogueNode(chase, state1Dialogue);
                UpdateDialogueNode(kate, state1Dialogue);
                break;
            case 2:
                dialogueRunner.StartDialogue(state2Dialogue);
                UpdateDialogueNode(chase, state2Dialogue);
                UpdateDialogueNode(kate, state2Dialogue);
                break;
            case 3:
                UpdateDialogueNode(chase, state2Dialogue);
                UpdateDialogueNode(kate, state2Dialogue);
                break;
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void FlashTile(){

    }

    [YarnCommand]
    public void GiveSaltwaterTile(){
        PlayerInventory.Instance.AddTilesToCollection(salthairTile);
    }

    [YarnCommand]
    public void GiveHandTile(){
        PlayerInventory.Instance.AddTilesToCollection(handholdTile);
    }

    [YarnCommand]
    public void VisitAntiqueShop(){
        WorldState.Instance.UpdateSceneState(Areas.Clay, 1, true);
    }

    [YarnCommand]
    public void PreClayAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Clay, 2);
        SceneController.Instance.Roundtrip(azulejoScene);
    }

    [YarnCommand]
    public void PostClayAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Clay, 3);
        WorldState.Instance.UpdateSceneState(Areas.Felt, 3);
    }
}
