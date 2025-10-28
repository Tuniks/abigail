using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUI : MonoBehaviour{
    // Parent
    private Tile tile;

    [Header("Tile Element References")]
    public Image faceImage;
    public Image bgImage;
    public Image matImage;
    public Image glzImage;

    public void BuildTileUI(Tile _tile){
        tile = _tile;

        float refScale = tile.matPrefab.transform.localScale.x;

        SetUIImage(faceImage, tile.facePrefab, refScale);
        SetUIImage(bgImage, tile.bgPrefab, refScale);
        SetUIImage(matImage, tile.matPrefab, refScale);
        SetUIImage(glzImage, tile.glzPrefab, refScale);
    }

    private void SetUIImage(Image img, GameObject source, float referenceScale){
        SpriteRenderer spr = source.GetComponent<SpriteRenderer>();
        if(spr == null){
            Debug.Log("ERROR: Sprite Renderer of Tile Component not found");
            return;
        } 

        RectTransform rect = img.GetComponent<RectTransform>();

        float scale = spr.transform.localScale.x/referenceScale;

        img.sprite = spr.sprite;
        rect.localPosition = spr.transform.localPosition;
        rect.localRotation = spr.transform.localRotation;
        rect.localScale = new Vector3(scale, scale, scale);
    }
}
