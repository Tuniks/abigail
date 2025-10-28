using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileDetailsUI : MonoBehaviour{
    public TileUI tileUI;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void SetTile(Tile tile){
        tileUI.BuildTileUI(tile);

        title.text = tile.GetName();
        description.text = tile.GetDescription();
    }
}
