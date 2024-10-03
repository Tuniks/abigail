using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour{    
    public string[] dialogueNode;

    private int currentNode = 0;

    public string GetCurrentNode(){
        string node = dialogueNode[currentNode];
        if(dialogueNode.Length > currentNode+1) currentNode++;
    
        return node;
    }
}