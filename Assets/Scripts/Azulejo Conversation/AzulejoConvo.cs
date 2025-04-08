using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public struct FaceDialoguePair{
    public GameObject facePrefab;
    public string dialogueNode;
}

public class AzulejoConvo : MonoBehaviour{
    public AzulejoConvoUI convoUI;

    public List<FaceDialoguePair> faceDialoguePairs;
    public string defaultNode;
    
    public void OnTileSelected(){
        string selectedFace = convoUI.GetSelectedFace();

        foreach(FaceDialoguePair pair in faceDialoguePairs){
            TileComponent face = pair.facePrefab.GetComponent<TileComponent>();
            if(face.title == selectedFace){
                StartDialogue(pair.dialogueNode);
                return;
            }
        }

        StartDialogue(defaultNode);
    }

    private void StartDialogue(string node){
        convoUI.Hide();
    }

    [YarnCommand]
    public void StartAzulejoConversation(){
        convoUI.Show();
    }
}
