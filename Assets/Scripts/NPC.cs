using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour{    
    public string dialogueNode;

    public string GetCurrentNode(){
        return dialogueNode;
    }

    public void SetNewDialogueNode(string node){
        dialogueNode = node;
    }
}