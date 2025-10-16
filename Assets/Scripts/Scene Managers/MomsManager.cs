using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class MomsManager : AreaManager{    
    public DialogueRunner dialogueRunner;

    [Header("NPCs")]
    public GameObject mom;
    public GameObject dani;
    public GameObject chest;

    [Header("Portals")]
    public GameObject doorBlocked;
    public GameObject doorUnblocked;

    [Header("Misc")]
    public Tile[] tilesInChest;
    public Tile toucanTile;
    public GameObject SunflowerInteraction;
    public GameObject EmptySunflower;
    public GameObject Tuca;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                chest.SetActive(true);
                break;
            case 2:
                chest.SetActive(false);
                doorBlocked.SetActive(false);
                doorUnblocked.SetActive(true);
                break;
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void MomIntroDone(){
        WorldState.Instance.UpdateSceneState(Areas.Moms, 1, true);
    }

    [YarnCommand]
    public void ChestIntroDone(){
        PlayerInventory.Instance.AddTilesToCollection(tilesInChest);
    }

    [YarnCommand]
    public void PostAzulejoDone(){
        //PlayerInventory.Instance.AddTilesToCollection(new Tile[]{toucanTile});
        WorldState.Instance.UpdateSceneState(Areas.Moms, 2, true);
    }

    [YarnCommand]
    public void UpdateMomDaniNode(string node){
        UpdateDialogueNode(mom, node);
        //UpdateDialogueNode(dani, node);
    }
    
    [YarnCommand]
    public void TurnOnSunflower(){
        SunflowerInteraction.SetActive(true);
        EmptySunflower.SetActive(false);
        Tuca.SetActive(true);
    }
    
}
