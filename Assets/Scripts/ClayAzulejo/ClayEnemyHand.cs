using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayEnemyHand : MonoBehaviour {
    public List<Tile> hand;
    public int maxHandSize = 5;
    public List<GameObject> enemyActiveSpots;  // Now a list of GameObjects

    public float tileSize = 2f;
    public float tileSpacing = .5f;

    void Start(){
        BuildHand();
        DrawHand();
    }

    public void BuildHand(){
        hand = new List<Tile>();
        for (int i = 0; i < maxHandSize; i++){
            Tile tile = TileMaker.instance.GetRandomTile();
            hand.Add(tile);
        }
        SetHand();
    }

    private void SetHand(){
        foreach(Tile tile in hand){
            tile.transform.SetParent(transform);
            tile.gameObject.SetActive(true);
        }
    }

    void DrawHand(){
        float total = tileSize * hand.Count + (tileSpacing * (hand.Count - 1));
        float startx = tileSize / 2f - (total / 2f);
        for (int i = 0; i < hand.Count; i++){
            hand[i].transform.localPosition = new Vector3(startx + (i * (tileSpacing + tileSize)), 0, 0);
        }
    }

    // Plays the next enemy tile into the corresponding enemy active spot.
    public void PlayNextTile(){
        if(hand.Count == 0) return;
        Tile tile = hand[0];
        hand.RemoveAt(0);
        DrawHand();
        int index = maxHandSize - hand.Count - 1;
        if(index < enemyActiveSpots.Count){
            ClayEnemyActiveSpot spot = enemyActiveSpots[index].GetComponent<ClayEnemyActiveSpot>();
            if(spot != null){
                spot.ActivateTile(tile);
            }
        }
    }
}