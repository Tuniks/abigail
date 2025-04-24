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
    public Tile[] salthairTile;
    public Tile[] handholdTile;
    public Tile[] tvTile;
    public Tile[] slimeTile;
    public Tile[] coffeeTile;
    
    [Header("Game Objects for Interactions")]
    public GameObject LandfillEntrance;
    public GameObject FootAnimation;
    public GameObject Warehouse;

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
                dialogueRunner.StartDialogue(state2Dialogue);
                UpdateDialogueNode(dani, state2Dialogue);
                break;
            case 3:
                UpdateDialogueNode(dani, state2Dialogue);
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
    
    [YarnCommand]
    public void GoToSteamLinkSceneFromCollage()
    {
        SceneManager.LoadScene("STEAMLINK");
    }
    
    [YarnCommand]
    public void Givetv()
    {
        PlayerInventory.Instance.AddTilesToCollection(tvTile);
    }
    
    [YarnCommand]
    public void Giveslime()
    {
        PlayerInventory.Instance.AddTilesToCollection(slimeTile);
    }
    
    [YarnCommand]
    public void Givecoffee()
    {
        PlayerInventory.Instance.AddTilesToCollection(coffeeTile);
    }
    
    [YarnCommand]
    public void OpenLandfill()
    {
        LandfillEntrance.SetActive(true);
    }
    
    [YarnCommand]
    public void PlayFootAnimation()
    {
        FootAnimation.SetActive(true);
        Warehouse.SetActive(false);
    }
    
}
