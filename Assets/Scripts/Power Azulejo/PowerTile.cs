using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTile : MonoBehaviour{
    public Tile parent;
    
    public bool isPlayerTile = false;

    public void GenerateTileData(){
        
    }

    public void SetParent(Tile _parent){
        parent = _parent;
    }

    public void SetIsPlayer(bool _state){
        isPlayerTile = _state;
    }

}
