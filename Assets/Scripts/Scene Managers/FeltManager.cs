using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Dialogue Nodes")]
    public string state1Dialogue = "PostFeltAzulejo";
    
    [Header("Scenes")]
    public string azulejoScene = "FELT_azulejo";

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                dialogueRunner.startAutomatically = true;
                dialogueRunner.startNode = state1Dialogue;
                UpdateDialogueNode(chase, state1Dialogue);
                UpdateDialogueNode(oz, state1Dialogue);
                break;
            case 2:
                chase.SetActive(false);
                oz.SetActive(false);
                doorColBlocked.SetActive(false);
                doorClayBlocked.SetActive(false);
                doorColUnblocked.SetActive(true);
                doorClayUnblocked.SetActive(true);
                break;
            case 3:
                chase.SetActive(false);
                oz.SetActive(false);
                doorColBlocked.SetActive(false);
                doorClayBlocked.SetActive(false);
                doorColUnblocked.SetActive(true);
                doorClayUnblocked.SetActive(true);
                // Enable party portal
                break;

        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void PreFeltAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Felt, 1);
        SceneController.Instance.Roundtrip(azulejoScene);
    }

    [YarnCommand]
    public void PostFeltAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Felt, 2, true);
    }
}
