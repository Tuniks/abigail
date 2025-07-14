using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTileBuilder : MonoBehaviour{
    public Vector3 tileScale = Vector3.one;
    
    public void BuildTile(Tile origin, PowerTile power, bool isPlayer){
        if(origin == null || power == null) return;

        // Creating visuals for tile
        GameObject tileCopy = Instantiate(origin.gameObject, Vector3.zero, Quaternion.identity);
        tileCopy.transform.SetParent(power.transform, false);
        tileCopy.transform.localScale = tileScale;
        tileCopy.GetComponent<Collider2D>().enabled = false;
        tileCopy.SetActive(true);

        // Setting power tile gameplay data
        power.SetIsPlayer(isPlayer);
        power.GenerateTileData();
    }

}
