using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public struct FaceDialoguePair{
    public GameObject facePrefab;
    public string dialogueNode;
}

public class AzulejoConvo : BaseConvo{
    public AzulejoConvoUI convoUI;

    [Header("Conversation Nodes")]
    public List<FaceDialoguePair> faceDialoguePairs;
    public string defaultNode;

    [Header("Juice")]
    public float endConvoDelay = 1f;

    protected override void StartConvo(){
        PlayerInteractor.instance.StartAzulejoConvo();
        PlayerUIManager.instance.SetCurrentConvo(this);
        PlayerUIManager.instance.ShowInventory();
        convoUI.Show();
    }

    private IEnumerator EndConvo(string node){
        yield return new WaitForSeconds(endConvoDelay);

        PlayerInteractor.instance.EndAzulejoConvo();
        PlayerUIManager.instance.SetCurrentConvo(null);
        PlayerUIManager.instance.HideInventory();
        convoUI.Hide();

        PlayerInteractor.instance.StartConversation(node);
    }

    public override void QuitConvo(){
        PlayerInteractor.instance.EndAzulejoConvo();
        PlayerUIManager.instance.SetCurrentConvo(null);
        PlayerUIManager.instance.HideInventory();
        convoUI.Hide();
    }

    public override void OnTileSelected(Tile tile, ConvoSlot slot){
        convoUI.SetTile(tile);
        string selectedFace = tile.GetName();

        PlayerInteractor.instance.UpdateLastTileUsed(tile);

        foreach(FaceDialoguePair pair in faceDialoguePairs){
            TileComponent face = pair.facePrefab.GetComponent<TileComponent>();
            if(face.title == selectedFace){
                StartCoroutine(EndConvo(pair.dialogueNode));
                return;
            }
        }

        StartCoroutine(EndConvo(defaultNode));
    }

    public override bool IsActive(){
        return true;
    }

    public override Vector3 GetSlotPosition(){
        return convoUI.GetSlotPosition();
    }

    public override float GetSlotScale(){
        return convoUI.GetSlotScale();
    }
}
