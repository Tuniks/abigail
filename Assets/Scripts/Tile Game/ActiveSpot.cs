using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSpot : MonoBehaviour{
    public Tile activeTile;

    public void ActivateTile(Tile _tile){
        if(activeTile!=null) Destroy(activeTile.gameObject);

        activeTile = _tile;
        activeTile.transform.parent = transform;
        activeTile.transform.localPosition = Vector3.zero;
    }
}
