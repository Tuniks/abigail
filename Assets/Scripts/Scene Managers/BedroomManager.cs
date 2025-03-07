using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class BedroomManager : AreaManager{
    public DialogueRunner dialogueRunner;

    [Header("State 0")]
    public string startNode = "";

    public override void UpdateSceneState(int state){        
        switch(state){
            case 0:
                dialogueRunner.startAutomatically = true;
                dialogueRunner.startNode = startNode;
                break;
            case 1:
                break;
            default:
                break;
        }
    }

    [YarnCommand]
    public void OpeningDone(){
        WorldState.Instance.UpdateSceneState(Areas.Bedroom, 1);
    }
}
