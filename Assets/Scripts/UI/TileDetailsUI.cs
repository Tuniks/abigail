using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileDetailsUI : MonoBehaviour{
    public Transform tileParent;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void SetTile(Tile tile){
        Transform oldTile = tileParent.GetChild(0);

        GameObject newTile = Instantiate(tile.gameObject, tileParent);
        newTile.transform.position = oldTile.position;
        newTile.transform.rotation = oldTile.rotation;
        newTile.transform.localScale = oldTile.localScale;
        Destroy(oldTile.gameObject);

        title.text = tile.GetName();
        description.text = tile.GetDescription();
    }
}
