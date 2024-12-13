using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerHand : MonoBehaviour{
    public List<Tile> hand;
    public int maxHandSize;

    public ActiveSpot activeSpot;

    public float tileSize = 2f;
    public float tileSpacing = .5f;

    void Start(){
        BuildHand();
        DrawHand();
    }

    private void BuildHand(){
        PlayerInventory inv = PlayerInventory.Instance;
        if (inv != null) hand = inv.GetHandOfTiles(maxHandSize);
        if(hand.Count < maxHandSize) GenerateRandomHand(maxHandSize - hand.Count);
        SetHand();
    }

    void GenerateRandomHand(int handCount){
        hand = new List<Tile>();

        for (int i = 0; i < handCount; i++){
            Tile tile = TileMaker.instance.GetRandomTile();
            hand.Add(tile);
        }
    }

    private void SetHand(){
        foreach(Tile tile in hand){
            tile.SetHand(this);
            tile.transform.SetParent(transform);
            tile.gameObject.SetActive(true);
        }
    }

    void DrawHand(){
        float total = tileSize * hand.Count + (tileSpacing * (hand.Count - 1));
        float startx = tileSize/2f - (total/2f);

        for(int i = 0; i < hand.Count; i++){
            hand[i].transform.localPosition = new Vector3(startx + (i*(tileSpacing + tileSize)), 0, 0);
        }
    }

    public void Activate(Tile tile){
        if(!hand.Contains(tile)) return;
        
        hand.Remove(tile);
        DrawHand();
        activeSpot.ActivateTile(tile);
        TileGameManager.instance.RevealPreArgumentWinner();
    }
}
