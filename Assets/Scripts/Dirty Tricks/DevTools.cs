using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevTools : MonoBehaviour{
    public Tile[] tile1;
    public Tile[] tile2;
    public Tile[] tile3;

    void Update(){
        if(Input.GetKeyDown("1")){
            PlayerInventory.Instance.AddTilesToCollection(tile1);
        }

        if(Input.GetKeyDown("2")){
            PlayerInventory.Instance.AddTilesToCollection(tile2);
        }

        if(Input.GetKeyDown("3")){
            PlayerInventory.Instance.AddTilesToCollection(tile3);
        }
    }
}
