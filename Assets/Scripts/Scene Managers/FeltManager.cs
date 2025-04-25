using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class FeltManager : AreaManager{
    public DialogueRunner dialogueRunner;
    
    [Header("NPCs")]
    public GameObject chase;
    public GameObject oz;

    [Header("Portals")]
    public GameObject doorColBlocked;
    public GameObject doorColUnblocked;
    public GameObject doorClayBlocked;
    public GameObject doorClayUnblocked;
    public GameObject doorMoms;
    public GameObject doorParty;
    public GameObject doorPartyBlocked;
    
    [Header("Tiles")]
    public Tile[] gianlucasTile;

    [Header("GameObjects")] 
    public Image RyanDrawing;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                doorColBlocked.SetActive(false);
                doorClayBlocked.SetActive(false);
                doorColUnblocked.SetActive(true);
                doorClayUnblocked.SetActive(true);
                break;
            case 2:
                chase.SetActive(true);
                UpdateDialogueNode(chase, "Chase4");
                oz.SetActive(false);
                doorColBlocked.SetActive(true);
                doorClayBlocked.SetActive(true);
                doorColUnblocked.SetActive(false);
                doorClayUnblocked.SetActive(false);
                doorMoms.SetActive(false);
                doorParty.SetActive(false);
                doorPartyBlocked.SetActive(true);
                break;
            case 3:
                chase.SetActive(true);
                UpdateDialogueNode(chase, "Chase5");
                oz.SetActive(false);
                doorColBlocked.SetActive(true);
                doorClayBlocked.SetActive(true);
                doorColUnblocked.SetActive(false);
                doorClayUnblocked.SetActive(false);
                doorMoms.SetActive(false);
                doorParty.SetActive(true);
                doorPartyBlocked.SetActive(false);
                break;
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void UpdateChaseOzNode(string node){
        UpdateDialogueNode(chase, node);
        UpdateDialogueNode(oz, node);
    }

    [YarnCommand]
    public void PostFeltAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Felt, 1, true);
    }

    [YarnCommand]
    public void FinishChaseAzulejo(){
        WorldState.Instance.UpdateSceneState(Areas.Felt, 3, true);
    }

    [YarnCommand]
    public void GiveGianlucaTile(){
        PlayerInventory.Instance.AddTilesToCollection(gianlucasTile);
    }
    
    [YarnCommand]
    public void ShowRyanDrawing(){
        RyanDrawing.enabled = true;
        RyanDrawing.gameObject.SetActive(true);
    }
}
