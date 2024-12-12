using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{
    [Header("Prefabs for Generation")]
    public GameObject facePrefab;
    public GameObject bgPrefab;
    public GameObject matPrefab;
    public GameObject glzPrefab;

    [Header("Generated Components")]
    public TileComponent face;
    public TileComponent background;
    public TileComponent material;
    public TileComponent glaze; 

    private PlayerHand playerHand;

    private Dictionary<Attributes, float> multipliers = new Dictionary<Attributes, float>();

    // ==== BUILDING THE COMPONENT ====
    public void Initialize(GameObject _facePrefab, GameObject _bgPrefab, GameObject _matPrefab, GameObject _glzPrefab){
        facePrefab = _facePrefab;
        bgPrefab = _bgPrefab;
        matPrefab = _matPrefab;
        glzPrefab = _glzPrefab;

        RebuildTile();
    }

    private void RebuildTile(){
        // Removing old components
        if(face) DestroyImmediate(face.gameObject);
        if(background) DestroyImmediate(background.gameObject);
        if(material) DestroyImmediate(material.gameObject);
        if(glaze) DestroyImmediate(glaze.gameObject);

        // Adding new components from prefab
        GameObject faceObj = AddTileComponent(facePrefab);
        GameObject bgObj = AddTileComponent(bgPrefab);
        GameObject matObj = AddTileComponent(matPrefab);
        GameObject glzObj = AddTileComponent(glzPrefab);

        // Getting component's component
        face = faceObj.GetComponent<TileComponent>();
        background = bgObj.GetComponent<TileComponent>();
        material = matObj.GetComponent<TileComponent>();
        glaze = glzObj.GetComponent<TileComponent>();
    }

    private GameObject AddTileComponent(GameObject prefab){
        GameObject component = Instantiate(prefab);
        component.transform.parent = transform;
        component.transform.localPosition = prefab.transform.localPosition;
        component.transform.localRotation = prefab.transform.localRotation;
        component.transform.localScale = prefab.transform.localScale;
        return component;
    }

    public void OnGeneratePressed(){
        if(facePrefab == null || bgPrefab == null || matPrefab == null || glzPrefab == null) return;
        RebuildTile();
    }

    public void SetHand(PlayerHand hand){
        playerHand = hand;
    }

    void OnMouseDown(){
        if(playerHand == null) return;

        playerHand.Activate(this);
    }

    public void AddMultiplier(Attributes att, float value){
        if(multipliers.ContainsKey(att)){
            multipliers[att] = multipliers[att] * value;
        } else multipliers[att] = value;
    }

    public string GetName(){
        return face.title;
    }

    public float GetBeauty(){
        float mult = multipliers.ContainsKey(Attributes.Beauty) ? multipliers[Attributes.Beauty] : 1f;
        return mult * (face.beauty + background.beauty + material.beauty + glaze.beauty);
    }

    public float GetStrength(){
        float mult = multipliers.ContainsKey(Attributes.Strength) ? multipliers[Attributes.Strength] : 1f;
        return mult * (face.strength + background.strength + material.strength + glaze.strength);
    }

    public float GetStamina(){
        float mult = multipliers.ContainsKey(Attributes.Stamina) ? multipliers[Attributes.Stamina] : 1f;
        return mult * (face.stamina + background.stamina + material.stamina + glaze.stamina);
    }

    public float GetMagic(){
        float mult = multipliers.ContainsKey(Attributes.Magic) ? multipliers[Attributes.Magic] : 1f;
        return mult * (face.magic + background.magic + material.magic + glaze.magic);
    }

    public float GetSpeed(){
        float mult = multipliers.ContainsKey(Attributes.Speed) ? multipliers[Attributes.Speed] : 1f;
        return mult * (face.speed + background.speed + material.speed + glaze.speed);
    }    
}
