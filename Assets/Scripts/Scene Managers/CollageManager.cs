using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class CollageManager : AreaManager{
    public DialogueRunner dialogueRunner;
    
    [Header("NPCs")]
    public GameObject dani;

    [Header("Dialogue Nodes")]
    public string state1Dialogue = "DaniDay1PostAzulejo";
    
    [Header("Scenes")]
    public string azulejoScene = "COL_azulejo";

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                // Initial State
                break;
            case 1:
                dialogueRunner.startAutomatically = true;
                dialogueRunner.startNode = state1Dialogue;
                UpdateDialogueNode(dani, state1Dialogue);
                break;
            case 2:
                UpdateDialogueNode(dani, state1Dialogue);
                dialogueRunner.startAutomatically = false;
                break;
        }
    }

    // YARN COMMANDS
    [YarnCommand]
    public void PreCollageAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 1);
        SceneController.Instance.Roundtrip(azulejoScene);
    }

    [YarnCommand]
    public void PostCollageAzulejoDone(){
        WorldState.Instance.UpdateSceneState(Areas.Collage, 2);
        WorldState.Instance.UpdateSceneState(Areas.Felt, 3);
    }
}
