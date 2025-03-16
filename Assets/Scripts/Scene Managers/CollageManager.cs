using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class CollageManager : AreaManager{
    public DialogueRunner dialogueRunner;
    
    [Header("NPCs")]
    public GameObject dani;
    public GameObject gwen;

    [Header("Dialogue Nodes")]
    public string gwenTradeDialogue = "GwenTrade";

    public string state1Dialogue = "DaniDay1PostTrade";
    public string state2Dialogue = "DaniDay1PostAzulejo";
    
    [Header("Scenes")]
    public string azulejoScene = "COL_azulejo";

    [Header("Misc")]
    public GameObject antiqueShop;
    public Tile[] salthairTile;
    public Tile[] handholdTile;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Pre Trade
                break;
            case 1:
                // Trade, Pre azulejo
                UpdateDialogueNode(dani, state1Dialogue);
                break;
            case 2:
                // Post Azulejo
                dialogueRunner.StartDialogue(state1Dialogue);
                UpdateDialogueNode(dani, state1Dialogue);
                break;
            case 3:
                UpdateDialogueNode(dani, state1Dialogue);
                break;
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void UpdateGwenDialogue(){
        UpdateDialogueNode(gwen, gwenTradeDialogue);
    }

    [YarnCommand]
    public void TradeSaltwaterTile(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 1, true);
        PlayerInventory.Instance.AddTilesToCollection(salthairTile);
    }

    [YarnCommand]
    public void TradeHandTile(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 1, true);
        PlayerInventory.Instance.AddTilesToCollection(handholdTile);
    }

    [YarnCommand]
    public void PreCollageAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 2);
        SceneController.Instance.Roundtrip(azulejoScene);
    }

    [YarnCommand]
    public void PostCollageAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 3);
        WorldState.Instance.UpdateSceneState(Areas.Felt, 3);
    }
}
