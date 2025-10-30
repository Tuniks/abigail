using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour{
    public GameObject shopScreen;
    public string dialogueNode;
    
    public void ShowShop(){
        shopScreen.SetActive(true);
    }

    public void HideShop(){
        shopScreen.SetActive(false);
    }

    public string GetCurrentNode(){
        return dialogueNode;
    }

    public void SetNewDialogueNode(string node){
        dialogueNode = node;
    }
}
