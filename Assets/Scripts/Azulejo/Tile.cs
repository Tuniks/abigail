using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [HideInInspector]
    public bool isEnemy = false; // ✅ Flag to prevent interaction if this is an enemy tile

    private IPlayerHand playerHand;
    public ITilePower tilePower; // Added to support tile powers

    private Dictionary<Attributes, float> multipliers = new Dictionary<Attributes, float>();

    void Awake() {
        RebuildTile();
    }

    public void Initialize(GameObject _facePrefab, GameObject _bgPrefab, GameObject _matPrefab, GameObject _glzPrefab) {
        facePrefab = _facePrefab;
        bgPrefab = _bgPrefab;
        matPrefab = _matPrefab;
        glzPrefab = _glzPrefab;

        RebuildTile();
    }

    public void RebuildTile() {
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<TileComponent>() != null) {
                toDestroy.Add(child.gameObject);
            }
        }
        foreach (GameObject child in toDestroy) {
            DestroyImmediate(child);
        }

        GameObject faceObj = AddTileComponent(facePrefab);
        GameObject bgObj = AddTileComponent(bgPrefab);
        GameObject matObj = AddTileComponent(matPrefab);
        GameObject glzObj = AddTileComponent(glzPrefab);

        face = faceObj.GetComponent<TileComponent>();
        background = bgObj.GetComponent<TileComponent>();
        material = matObj.GetComponent<TileComponent>();
        glaze = glzObj.GetComponent<TileComponent>();
    }

    private GameObject AddTileComponent(GameObject prefab) {
        GameObject component = Instantiate(prefab);
        component.transform.parent = transform;
        component.transform.localPosition = prefab.transform.localPosition;
        component.transform.localRotation = prefab.transform.localRotation;
        component.transform.localScale = prefab.transform.localScale;
        return component;
    }

    public void OnGeneratePressed() {
        if (facePrefab == null || bgPrefab == null || matPrefab == null || glzPrefab == null) return;
        RebuildTile();
    }

    public static bool operator ==(Tile t1, Tile t2) {
        if (!t1 && !t2) return true;
        if (!t1 || !t2) return false;
        return (t1.facePrefab == t2.facePrefab) &&
               (t1.bgPrefab == t2.bgPrefab) &&
               (t1.matPrefab == t2.matPrefab) &&
               (t1.glzPrefab == t2.glzPrefab);
    }

    public static bool operator !=(Tile t1, Tile t2) {
        return !(t1 == t2);
    }

    public override bool Equals(object obj) {
        if (obj == null) return false;
        if (GetType() != obj.GetType()) return false;
        Tile t2 = obj as Tile;
        return this == t2;
    }

    public override int GetHashCode() {
        return facePrefab.GetHashCode() ^
               bgPrefab.GetHashCode() ^
               matPrefab.GetHashCode() ^
               glzPrefab.GetHashCode();
    }

    public void SetHand(IPlayerHand hand) {
        playerHand = hand;
    }

    void OnMouseDown() {
        if (isEnemy) return; // ✅ Do not allow interaction with enemy tiles

        if (PowerAzuManager.instance != null &&
            PowerAzuManager.instance.currentState == PowerAzuManager.GameState.Play) {
            PowerAzuManager.instance.SetActiveTile(this);
        } else {
            if (playerHand == null) return;
            playerHand.Activate(this);
        }
    }

    void OnMouseOver() {
        if (isEnemy) return;

        if (PowerAzuManager.instance != null &&
            PowerAzuManager.instance.currentState == PowerAzuManager.GameState.Play) {
            if (Input.GetMouseButtonDown(1)) {
                if (PowerAzuManager.instance.activeTile == this) {
                    PowerAzuManager.instance.CancelActiveTile();
                }
            }
        }
    }

    public void AddMultiplier(Attributes att, float value) {
        if (multipliers.ContainsKey(att)) {
            multipliers[att] = multipliers[att] * value;
        } else {
            multipliers[att] = value;
        }
    }

    public string GetName(){
        if(face == null){
             return facePrefab.GetComponent<TileComponent>().title;
        }
        return face.title;
    }

    public string GetDescription() {
        return face.description;
    }

    public bool HasTag(Tag tag) {
        return face.tags != null && ((IList<Tag>)face.tags).Contains(tag); // Safe IList cast
    }

    public float GetAttribute(Attributes att) {
        return att switch {
            Attributes.Beauty => GetBeauty(),
            Attributes.Vigor => GetVigor(),
            Attributes.Magic => GetMagic(),
            Attributes.Heart => GetHeart(),
            Attributes.Intellect => GetIntellect(),
            Attributes.Terror => GetTerror(),
            _ => 0,
        };
    }

    public float GetBeauty() {
        float mult = multipliers.ContainsKey(Attributes.Beauty) ? multipliers[Attributes.Beauty] : 1f;
        return mult * (face.beauty + background.beauty + material.beauty + glaze.beauty);
    }

    public float GetVigor() {
        float mult = multipliers.ContainsKey(Attributes.Vigor) ? multipliers[Attributes.Vigor] : 1f;
        return mult * (face.vigor + background.vigor + material.vigor + glaze.vigor);
    }

    public float GetMagic() {
        float mult = multipliers.ContainsKey(Attributes.Magic) ? multipliers[Attributes.Magic] : 1f;
        return mult * (face.magic + background.magic + material.magic + glaze.magic);
    }

    public float GetHeart() {
        float mult = multipliers.ContainsKey(Attributes.Heart) ? multipliers[Attributes.Heart] : 1f;
        return mult * (face.heart + background.heart + material.heart + glaze.heart);
    }

    public float GetIntellect() {
        float mult = multipliers.ContainsKey(Attributes.Intellect) ? multipliers[Attributes.Intellect] : 1f;
        return mult * (face.intellect + background.intellect + material.intellect + glaze.intellect);
    }

    public float GetTerror() {
        float mult = multipliers.ContainsKey(Attributes.Terror) ? multipliers[Attributes.Terror] : 1f;
        return mult * (face.terror + background.terror + material.terror + glaze.terror);
    }

    public bool HasFace(TileComponent _face) {
        if(_face == null) return false;
        return GetName() == _face.title;
    }
}
