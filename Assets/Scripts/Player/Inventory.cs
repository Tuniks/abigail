using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour{
    public Transform tileCollectionParent;
    public Transform activeTilesParent;

    private List<GameObject> tileCollection = new List<GameObject>();
    
    protected virtual void Awake(){
        InitializeTileCollection();
    }

    private void InitializeTileCollection(){
        List<GameObject> tileCollection = new List<GameObject>();
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

    public void RemoveTileFromCollection(Tile toRemove){
        if(!toRemove.gameObject.CompareTag("Tile")) return;

        foreach(GameObject tileObj in tileCollectionParent){
            Tile tile = tileObj.GetComponent<Tile>();
            if(tile != null){
                if(tile==toRemove){
                    Destroy(tileObj);
                    InitializeTileCollection();
                    PlayerUIManager.instance.SetInventoryUI();
                    return;
                }
            }
        }
    }

    public List<GameObject> GetTileCollection(){
        return tileCollection;
    }

    public List<Tile> GetHandOfTiles(int count){
        List<Tile> tiles = new List<Tile>();

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
