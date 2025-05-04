using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WordDialoguePair{
    public string word;
    public string dialogueNode;
}

public class ClayAzulejoConvo : BaseConvo{
    public ClayAzulejoConvoUI convoUI;

    [Header("Outcomes and Dialogue Nodes")]
    public WordDialoguePair baseOutcome;
    public WordDialoguePair beautyOutcome;
    public WordDialoguePair vigorOutcome;
    public WordDialoguePair magicOutcome;
    public WordDialoguePair heartOutcome;
    public WordDialoguePair intellectOutcome;
    public WordDialoguePair terrorOutcome;

    [Header("Friend Data")]
    public List<Tile> friendTiles =  new List<Tile>();

    [Header("Juice")]
    public float friendTileDelay = 0.5f;
    public float roundDelay = 1f;
    public float endConvoDelay = 1f;

    // Game Variables
    private int tileCount = 0;
    private float[] attributeTotals = new float[6]{0,0,0,0,0,0};
    private List<Tile> currentFriendHand;

    // State Variables
    private bool currentActive = true;

    protected override void StartConvo(){
        PlayerInteractor.instance.StartAzulejoConvo();
        PlayerUIManager.instance.SetCurrentConvo(this);
        PlayerUIManager.instance.ShowInventory();

        tileCount = 0;
        attributeTotals = new float[6]{0,0,0,0,0,0};
        currentFriendHand = new List<Tile>(friendTiles);
        currentActive = true;

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
        tileCount++;
        currentActive = false;
        
        // Inputing current user tile
        convoUI.SetTile(tile, slot);
        AddAttributes(tile);
        PlayerInteractor.instance.UpdateLastTileUsed(tile);

        // Inputing friend tile
        StartCoroutine(SelectFriendTile());        

    }

    private IEnumerator SelectFriendTile(){
        yield return new WaitForSeconds(friendTileDelay);

        Tile tile = currentFriendHand[Random.Range(0, currentFriendHand.Count)];
        convoUI.SetFriendTile(tile, tileCount);
        AddAttributes(tile);
        currentFriendHand.Remove(tile);

        StartCoroutine(ResolveRound());
    }

    private IEnumerator ResolveRound(){
        // Start animation

        yield return new WaitForSeconds(roundDelay);

        WordDialoguePair current = GetCurrentWord();
        convoUI.UpdateWord(current.word, tileCount);
        currentActive = true;

        if(tileCount >= 3) StartCoroutine(EndConvo(current.dialogueNode));
    }

    private void AddAttributes(Tile tile){
        attributeTotals[0]+=tile.GetBeauty();
        attributeTotals[1]+=tile.GetVigor();
        attributeTotals[2]+=tile.GetMagic();
        attributeTotals[3]+=tile.GetHeart();
        attributeTotals[4]+=tile.GetIntellect();
        attributeTotals[5]+=tile.GetTerror();
    }

    private WordDialoguePair GetCurrentWord(){
        WordDialoguePair[] pairs = new WordDialoguePair[6]{
            beautyOutcome, vigorOutcome, magicOutcome,
            heartOutcome, intellectOutcome, terrorOutcome
        };

        float max = 0;
        float diff = 0;
        WordDialoguePair winner = baseOutcome;

        for(int i = 0; i < 6; i++){
            if(attributeTotals[i] > max){
                diff = attributeTotals[i] - max;
                max = attributeTotals[i];
                winner = pairs[i];
            }
        }

        if(diff <= 3) return baseOutcome;
        return winner;
    }

    // === GETTERS AND SETTERS ===

    public override bool IsActive(){
        return currentActive;
    }

    public override Vector3 GetSlotPosition(){
        return convoUI.GetSlotPosition();
    }

    public override float GetSlotScale(){
        return convoUI.GetSlotScale();
    }
}
