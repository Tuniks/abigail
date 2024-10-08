using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHand : MonoBehaviour{
    public List<Tile> hand;
    public int maxHandSize;

    public ActiveSpot activeSpot;

    public float tileSize = 2f;
    public float tileSpacing = .5f;

    void Start(){
        GenerateRandomHand();
        DrawHand();
    }

    void GenerateRandomHand(){
        hand = new List<Tile>();

        for (int i = 0; i < maxHandSize; i++){
            Tile tile = TileMaker.instance.GetRandomTile();
            tile.transform.SetParent(transform);
            hand.Add(tile);
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
}
