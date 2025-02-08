using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerHand : MonoBehaviour{
    public List<Tile> hand;
    public int maxHandSize;

    public ActiveSpot activeSpot;
    public List<GameObject> activeSpots;

    public float tileSize = 2f;
    public float tileSpacing = .5f;
    
    // When false the hand behaves as originally written.
    // When true, selecting a tile is deferred until an active spot is clicked,
    // and the state machine in ClayGameManager is used.
    public bool isClay = false;
    private Tile pendingTile = null;

    void Start(){
        BuildHand();
        DrawHand();
    }

    public void BuildHand(){
        PlayerInventory inv = PlayerInventory.Instance;
        if (inv != null) 
            hand = inv.GetHandOfTiles(maxHandSize);
        if(hand.Count < maxHandSize) 
            GenerateRandomHand(maxHandSize - hand.Count);
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
        
        if(isClay){
            // In Clay mode, ignore input if the ClayGameManager isn’t in PlayerTurn.
            if(ClayGameManager.instance != null &&
               ClayGameManager.instance.currentState != ClayGameManager.GameState.PlayerTurn)
                return;
            
            // If another tile is pending, restore its opacity.
            if(pendingTile != null && pendingTile != tile){
                SetTileTransparency(pendingTile, 1f);
            }
            pendingTile = tile;
            SetTileTransparency(tile, 0.5f);
        }
        else{
            // Original functionality: immediately remove and assign the tile.
            hand.Remove(tile);
            DrawHand();
            activeSpot.ActivateTile(tile);
            TileGameManager.instance.RevealPreArgumentWinner();
        }
    }

    // Called (by an ActiveSpot) when an active spot is clicked.
    public void OnActiveSpotSelected(GameObject activeSpotObject){
        if(!isClay) return;
        if(ClayGameManager.instance != null &&
           ClayGameManager.instance.currentState != ClayGameManager.GameState.PlayerTurn)
            return;
        if(pendingTile == null) return;
        
        ActiveSpot spot = activeSpotObject.GetComponent<ActiveSpot>();
        if(spot == null) return;
        
        SetTileTransparency(pendingTile, 1f);
        hand.Remove(pendingTile);
        DrawHand();
        spot.ActivateTile(pendingTile);
        if(ClayGameManager.instance != null)
            ClayGameManager.instance.OnTilePlaced();
        pendingTile = null;
    }

    private void SetTileTransparency(Tile tile, float alpha){
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers){
            foreach(Material mat in renderer.materials){
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }
}
