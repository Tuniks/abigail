using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class HoverItem : MonoBehaviour{
    public Image tileFace;
    public TextMeshProUGUI tileName;
    public TextMeshProUGUI tip;

    public void SetTile(FaceDialoguePair tileInfo){
        tileFace.sprite = tileInfo.facePrefab.GetComponent<SpriteRenderer>().sprite;
        tileName.text = tileInfo.facePrefab.GetComponent<TileComponent>().title;
        tip.text = tileInfo.tip;
    }
}
