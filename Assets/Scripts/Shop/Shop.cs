using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour{
    public GameObject shopScreen;
    public string[] dialogueNode;

    private int currentNode = 0;
    
    public void ShowShop(){
        shopScreen.SetActive(true);
    }

    public void HideShop(){
        shopScreen.SetActive(false);
    }

    public string GetCurrentNode(){
        string node = dialogueNode[currentNode];
        if(dialogueNode.Length > currentNode+1) currentNode++;
    
        return node;
    }

    public void SetNewDialogueNode(string[] nodes){
        dialogueNode = nodes;
        currentNode = 0;
    }
}
