using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public struct FaceDialoguePair{
    public GameObject facePrefab;
    public string dialogueNode;
    public string tip;
}

public class AzulejoConvo : MonoBehaviour{
    public AzulejoConvoUI convoUI;

    [Header("Conversation Nodes")]
    public List<FaceDialoguePair> faceDialoguePairs;
    public string defaultNode;

    [Header("Juice")]
    public float endConvoDelay = 1f;

    private void StartConvo(){
        PlayerUIManager.instance.SetCurrentConvo(this);
        PlayerUIManager.instance.ShowInventory();
        convoUI.Show();
    }

    private IEnumerator EndConvo(string node){
        yield return new WaitForSeconds(endConvoDelay);

        PlayerUIManager.instance.SetCurrentConvo(null);
        PlayerUIManager.instance.HideInventory();
        convoUI.Hide();

        PlayerInteractor.instance.StartConversation(node);
    }

    public void QuitConvo(){
        PlayerUIManager.instance.SetCurrentConvo(null);
        PlayerUIManager.instance.HideInventory();
        convoUI.Hide();
    }

    public void OnTileSelected(Tile tile){
        convoUI.SetTile(tile);
        string selectedFace = tile.GetName();

        foreach(FaceDialoguePair pair in faceDialoguePairs){
            TileComponent face = pair.facePrefab.GetComponent<TileComponent>();
            if(face.title == selectedFace){
                PlayerInteractor.instance.UpdateLastTileUsed(tile);
                StartCoroutine(EndConvo(pair.dialogueNode));
                return;
            }
        }

        StartCoroutine(EndConvo(defaultNode));
    }

    // ==== YARN COMMANDS ====
    [YarnCommand]
    public void StartAzulejoConversation(){
        StartConvo();
    }
}
