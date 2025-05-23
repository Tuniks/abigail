using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Yarn.Unity;
using Yarn.Unity.Addons.SpeechBubbles;

public class NPC : MonoBehaviour{    
    public string dialogueNode;

    private string characterName = "";

    void Start(){
        if(GetComponent<CharacterBubbleAnchor>() != null) characterName = GetComponent<CharacterBubbleAnchor>().CharacterName;
        InitializeState();
    }

    public string GetCurrentNode(){
        return dialogueNode;
    }

    public void SetNewDialogueNode(string node){
        dialogueNode = node;
        UpdateStorage();
    }

    // ==== STATE CONTROL ====
    private void InitializeState(){
        string currentScene = SceneManager.GetActiveScene().name;
        if(currentScene != "FELT_area" && currentScene != "MOMS_area") return;

        string stateNode = WorldState.Instance.GetCurrentNPCDialogueNode(characterName);
        if(stateNode == "") return;

        SetNewDialogueNode(stateNode);
    }

    private void UpdateStorage(){
        if(characterName == "") return;
        WorldState.Instance.UpdateNPCDialogueNode(characterName, dialogueNode);
    }
}