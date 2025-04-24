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
    public string state1Dialogue = "Kate1";
    public string state2Dialogue = "Kate2";
    
    [Header("Scenes")]
    public string azulejoScene = "CLAY_azulejo";
    
    [Header("Tiles")]
    public GameObject antiqueShop;
    public Tile[] salthairTile;
    public Tile[] handholdTile;
    public Tile[] clockTile;
    public Tile[] flyingTile;
    public Tile[] bellTile;
    
    [Header("GameObjects")]
    public GameObject AntiqueShopEntrance;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                UpdateDialogueNode(kate, state1Dialogue);
                break;
            case 2:
                dialogueRunner.StartDialogue(state2Dialogue);
                UpdateDialogueNode(kate, state2Dialogue);
                break;
            case 3:
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
        WorldState.Instance.UpdateSceneState(Areas.Clay, 3, true);
        WorldState.Instance.UpdateSceneState(Areas.Felt, 3);
    }
    
    [YarnCommand]
    public void GoToSteamLinkScene()
    {
        SceneManager.LoadScene("STEAMLINK");
    }
    
    [YarnCommand]
    public void OpenAntiqueShop()
    {
        AntiqueShopEntrance.SetActive(true);
        
    }
    
    [YarnCommand]
    public void GiveClock()
    {
        PlayerInventory.Instance.AddTilesToCollection(clockTile);
        
    }
    
    [YarnCommand]
    public void GiveFlyingPill()
    {
        PlayerInventory.Instance.AddTilesToCollection(flyingTile);
        
    }
    
    [YarnCommand]
    public void GiveBellTile()
    {
        PlayerInventory.Instance.AddTilesToCollection(bellTile);
        
    }
    
}
