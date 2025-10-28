using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHand : MonoBehaviour{
    public Inventory enemyInventory;
    
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
        if (enemyInventory != null) hand = enemyInventory.GetHandOfTiles(maxHandSize);
        if(hand.Count < maxHandSize) GenerateRandomHand(maxHandSize - hand.Count);   
        SetHand();
    }

    void GenerateRandomHand(int handCount){
        hand = new List<Tile>();

        for (int i = 0; i < handCount; i++){
            Tile tile = TileMaker.instance.GetRandomTile();
            tile.transform.SetParent(transform);
            hand.Add(tile);
        }
    }

    private void SetHand(){
        foreach(Tile tile in hand){
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

    public void ActivateRandomCard(){
        Tile current = hand[Random.Range(0,hand.Count)];
        hand.Remove(current);
        DrawHand();
        activeSpot.ActivateTile(current);
    }

    public void ActivateCard(Tile tile){
        Tile current = hand[Random.Range(0,hand.Count)];
        if(hand.Contains(tile)) current = hand[hand.IndexOf(tile)];
        hand.Remove(current);
        DrawHand();
        activeSpot.ActivateTile(current);
    }
}
