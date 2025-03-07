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

    [Header("Dialogue Nodes")]
    public string state2Dialogue = "MomDaniPreAzulejo";
    public string state3Dialogue = "MomDaniPostAzulejo";
    
    [Header("Scenes")]
    public string azulejoScene = "MomsAzulejo";

    [Header("Misc")]
    public Tile[] tilesInChest;

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                chest.SetActive(true);
                break;
            case 2:
                UpdateDialogueNode(mom, state2Dialogue);
                UpdateDialogueNode(dani, state2Dialogue);
                break;
            case 3:
                dialogueRunner.StartDialogue(state3Dialogue);
                UpdateDialogueNode(mom, state3Dialogue);
                UpdateDialogueNode(dani, state3Dialogue);
                break;
            case 4:
                UpdateDialogueNode(mom, state3Dialogue);
                UpdateDialogueNode(dani, state3Dialogue);
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
        WorldState.Instance.UpdateSceneState(Areas.Moms, 2, true);
    }

    [YarnCommand]
    public void StartAzulejo(){
        WorldState.Instance.UpdateSceneState(Areas.Moms, 3);
        SceneController.Instance.Roundtrip(azulejoScene);
    }

    [YarnCommand]
    public void PostAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Moms, 4, true);
    }

}
