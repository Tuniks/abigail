using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePortal : MonoBehaviour{
    public Areas destination;

    public bool isBlocked = false;
    public string blockedDialogueNode = "blockedDoor";

    public string AttemptTravel(){
        if(isBlocked){
            return blockedDialogueNode;
        }

        SceneController.Instance.Travel(destination);
        return null;
    }

}
