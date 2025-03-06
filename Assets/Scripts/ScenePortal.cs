using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePortal : MonoBehaviour{
    public Areas origin;
    public Areas destination;

    public bool isBlocked = false;
    public string blockedDialogueNode = "blockerDoor";

    public string AttemptTravel(){
        if(isBlocked){
            return blockedDialogueNode;
        }

        SceneController.Instance.Travel(origin, destination);
        return null;
    }

}
