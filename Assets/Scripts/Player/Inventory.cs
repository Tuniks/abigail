using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour{
    public Transform tileCollectionParent;
    public Transform activeTilesParent;

    private List<GameObject> tileCollection = new List<GameObject>();
    private List<GameObject> activeTiles = new List<GameObject>();
    
    private const int activeCount = 8;

    protected virtual void Start(){
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

    public List<GameObject> GetTileCollection(){
        return tileCollection;
    }

    public List<GameObject> GetActiveTiles(){
        return activeTiles;
    }

}
