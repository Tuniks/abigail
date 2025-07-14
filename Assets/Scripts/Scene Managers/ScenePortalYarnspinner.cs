using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;


public class ScenePortalYarnspinner : MonoBehaviour{
    public Areas destination;

    public bool isBlocked = false;
    public string blockedDialogueNode = "blockedDoor";
    public string unblockedDialogueNode = "UnBlockedDoorMom";

    public string AttemptTravel(){
        if(isBlocked){
            return blockedDialogueNode;
        }
        
        SceneController.Instance.Travel(destination);
        return null;
    }
    
    [YarnCommand]
    public void TryToLeave(){
        SceneController.Instance.Travel(destination); 
    }

}
