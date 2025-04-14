using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour {
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

    // Change the type to IPlayerHand so it can accept either PlayerHand or PlayerHandPA.
    private IPlayerHand playerHand;

    private Dictionary<Attributes, float> multipliers = new Dictionary<Attributes, float>();

    // ==== BUILDING THE COMPONENT ====
    void Awake(){
        RebuildTile();
    }

    public void Initialize(GameObject _facePrefab, GameObject _bgPrefab, GameObject _matPrefab, GameObject _glzPrefab){
        facePrefab = _facePrefab;
        bgPrefab = _bgPrefab;
        matPrefab = _matPrefab;
        glzPrefab = _glzPrefab;

        RebuildTile();
    }

    private void RebuildTile(){
        // Removing old components
        List<GameObject> toDestroy = new List<GameObject>();
        foreach(Transform child in transform){
            if(child.gameObject.GetComponent<TileComponent>() != null){
                toDestroy.Add(child.gameObject);
            }
        }
        foreach(GameObject child in toDestroy){
            DestroyImmediate(child);
        }

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

    // === HELPER OVERRIDES ===
    public static bool operator == (Tile t1, Tile t2){
        if(!t1 && !t2) return true;
        if(!t1 || !t2) return false;
        return (t1.facePrefab == t2.facePrefab) && (t1.bgPrefab == t2.bgPrefab) && (t1.matPrefab == t2.matPrefab) && (t1.glzPrefab == t2.glzPrefab);
    }

    public static bool operator != (Tile t1, Tile t2){
        return !(t1 == t2);
    }

    public override bool Equals(object obj){
        if(obj == null) return false;
        if(GetType() != obj.GetType()) return false;

        Tile t2 = obj as Tile;
        return this == t2;
    }

    public override int GetHashCode(){
        return facePrefab.GetHashCode() ^ bgPrefab.GetHashCode() ^ matPrefab.GetHashCode() ^ glzPrefab.GetHashCode();
    }

    // === FUNCTIONS FOR AZULEJO GAME ===

    // Updated to accept IPlayerHand so it works with both classes.
    public void SetHand(IPlayerHand hand){
        playerHand = hand;
    }

    // When the tile is clicked...
    void OnMouseDown(){
        // If the game is in Play state, handle active tile selection.
        if (PowerAzuManager.instance != null && PowerAzuManager.instance.currentState == PowerAzuManager.GameState.Play) {
            PowerAzuManager.instance.SetActiveTile(this);
        }
        // Otherwise, maintain existing behavior.
        else {
            if(playerHand == null) return;
            playerHand.Activate(this);
        }
    }

    // Detect right-click input while the cursor is over this tile.
    void OnMouseOver() {
        if (PowerAzuManager.instance != null && PowerAzuManager.instance.currentState == PowerAzuManager.GameState.Play) {
            if (Input.GetMouseButtonDown(1)) { // Right-click
                if (PowerAzuManager.instance.activeTile == this) {
                    PowerAzuManager.instance.CancelActiveTile();
                }
            }
        }
    }

    public void AddMultiplier(Attributes att, float value){
        if(multipliers.ContainsKey(att)){
            multipliers[att] = multipliers[att] * value;
        } else {
            multipliers[att] = value;
        }
    }

    // === GETTERS ===

    public string GetName(){
        return face.title;
    }

    public string GetDescription(){
        return face.description;
    }

    public bool HasTag(Tag tag){
        if(face.tags == null) return false;
        return face.tags.Contains(tag);
    }

    public float GetAttribute(Attributes att){
        return att switch{
            Attributes.Beauty => GetBeauty(),
            Attributes.Vigor => GetVigor(),
            Attributes.Magic => GetMagic(),
            Attributes.Heart => GetHeart(),
            Attributes.Intellect => GetIntellect(),
            Attributes.Terror => GetTerror(),
            _ => 0,
        };
    }

    public float GetBeauty(){
        float mult = multipliers.ContainsKey(Attributes.Beauty) ? multipliers[Attributes.Beauty] : 1f;
        return mult * (face.beauty + background.beauty + material.beauty + glaze.beauty);
    }

    public float GetVigor(){
        float mult = multipliers.ContainsKey(Attributes.Vigor) ? multipliers[Attributes.Vigor] : 1f;
        return mult * (face.vigor + background.vigor + material.vigor + glaze.vigor);
    }

    public float GetMagic(){
        float mult = multipliers.ContainsKey(Attributes.Magic) ? multipliers[Attributes.Magic] : 1f;
        return mult * (face.magic + background.magic + material.magic + glaze.magic);
    }

    public float GetHeart(){
        float mult = multipliers.ContainsKey(Attributes.Heart) ? multipliers[Attributes.Heart] : 1f;
        return mult * (face.heart + background.heart + material.heart + glaze.heart);
    }

    public float GetIntellect(){
        float mult = multipliers.ContainsKey(Attributes.Intellect) ? multipliers[Attributes.Intellect] : 1f;
        return mult * (face.intellect + background.intellect + material.intellect + glaze.intellect);
    }

    public float GetTerror(){
        float mult = multipliers.ContainsKey(Attributes.Terror) ? multipliers[Attributes.Terror] : 1f;
        return mult * (face.terror + background.terror + material.terror + glaze.terror);
    }
}
