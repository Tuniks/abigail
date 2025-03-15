using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour{
    public Transform tileCollectionParent;
    public Transform activeTilesParent;

    private List<GameObject> tileCollection = new List<GameObject>();
    private List<GameObject> activeTiles = new List<GameObject>();
    
    private const int activeCount = 8;

    protected virtual void Awake(){
        InitializeActiveTiles();
        InitializeTileCollection();
    }

    private void InitializeActiveTiles(){
        int count = 0;
        foreach(Transform child in activeTilesParent){
            if(child.gameObject.CompareTag("Tile")){
                count++;
                if(count > activeCount){
                    child.SetParent(tileCollectionParent);
                } else activeTiles.Add(child.gameObject);
            }
        }
    }

    private void InitializeTileCollection(){
        foreach(Transform child in tileCollectionParent){
            if(child.gameObject.CompareTag("Tile")){
                tileCollection.Add(child.gameObject);
            }
        }
    }

    public void AddTilesToCollection(Tile[] newTiles){
        foreach(Tile tile in newTiles){
            if(tile.gameObject.CompareTag("Tile")){
                tile.transform.SetParent(tileCollectionParent);
                tileCollection.Add(tile.gameObject);
            }
        }
        
        PlayerUIManager.instance.SetInventoryUI();
        PlayerUIManager.instance.ShowNewTileNotification();
    }

    public List<GameObject> GetTileCollection(){
        return tileCollection;
    }

    public List<GameObject> GetActiveTiles(){
        return activeTiles;
    }

    public void MoveTile(Tile tile, bool toActive){
        if(toActive){
            foreach(GameObject child in tileCollection){
                Tile childTile = child.GetComponent<Tile>();
                if(childTile == tile){
                    tileCollection.Remove(child);
                    activeTiles.Add(child);
                    child.transform.SetParent(activeTilesParent);
                    return;
                }
            }
        } else {
            foreach(GameObject child in activeTiles){
                Tile childTile = child.GetComponent<Tile>();
                if(childTile == tile){
                    activeTiles.Remove(child);
                    tileCollection.Add(child);
                    child.transform.SetParent(tileCollectionParent);
                    return;
                }
            }
        }
    }

    public List<Tile> GetHandOfTiles(int count){
        List<Tile> tiles = new List<Tile>();

        // Adding random tiles from active set
        List<GameObject> act = new List<GameObject>(activeTiles);
        while(tiles.Count < count && act.Count > 0){
            GameObject tileObj = act[Random.Range(0, act.Count)];
            GameObject clone = Instantiate(tileObj, Vector3.zero, Quaternion.identity);
            tiles.Add(clone.GetComponent<Tile>());
            act.Remove(tileObj);
        }

        // If active set couldnt fill up a full hand, get from collection set
        List<GameObject> col = new List<GameObject>(tileCollection);
        while(tiles.Count < count && col.Count > 0){
            GameObject tileObj = col[Random.Range(0, col.Count)];
            GameObject clone = Instantiate(tileObj, Vector3.zero, Quaternion.identity);
            tiles.Add(clone.GetComponent<Tile>());
            col.Remove(tileObj);
        }

        return tiles;
    }

}
