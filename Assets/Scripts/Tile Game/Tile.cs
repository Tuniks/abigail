using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Tile : MonoBehaviour {
    public GameObject facePrefab;
    public GameObject bgPrefab;
    public GameObject matPrefab;
    public GameObject glzPrefab;

    public TileComponent face;
    public TileComponent background;
    public TileComponent material;
    public TileComponent glaze;

    private IPlayerHand playerHand;
    private Dictionary<Attributes, float> multipliers = new Dictionary<Attributes, float>();
    
    public ITilePower tilePower;



    public bool isEnemy = false; 

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

    private void RebuildTile() {
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
        GameObject component = Instantiate(prefab, transform);
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
        return (t1.facePrefab == t2.facePrefab) && (t1.bgPrefab == t2.bgPrefab) && (t1.matPrefab == t2.matPrefab) && (t1.glzPrefab == t2.glzPrefab);
    }

    public static bool operator !=(Tile t1, Tile t2) {
        return !(t1 == t2);
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) return false;
        Tile t2 = obj as Tile;
        return this == t2;
    }

    public override int GetHashCode() {
        return facePrefab.GetHashCode() ^ bgPrefab.GetHashCode() ^ matPrefab.GetHashCode() ^ glzPrefab.GetHashCode();
    }

    public void SetHand(IPlayerHand hand) {
        playerHand = hand;
    }

    void OnMouseDown() {
        if (isEnemy) return; // ✅ prevent enemy tile selection

        if (PowerAzuManager.instance != null && PowerAzuManager.instance.currentState == PowerAzuManager.GameState.Play) {
            PowerAzuManager.instance.SetActiveTile(this);
        } else {
            if (playerHand == null) return;
            playerHand.Activate(this);
        }
    }

    void OnMouseOver() {
        if (isEnemy) return; // ✅ prevent enemy tile right-click

        if (PowerAzuManager.instance != null && PowerAzuManager.instance.currentState == PowerAzuManager.GameState.Play) {
            if (Input.GetMouseButtonDown(1)) {
                if (PowerAzuManager.instance.activeTile == this) {
                    PowerAzuManager.instance.CancelActiveTile();
                }
            }
        }
    }

    public void AddMultiplier(Attributes att, float value) {
        if (multipliers.ContainsKey(att)) {
            multipliers[att] *= value;
        } else {
            multipliers[att] = value;
        }
    }

    public string GetName() => face.title;
    public string GetDescription() => face.description;
    public bool HasTag(Tag tag) => face.tags != null && face.tags.Contains(tag);

    public float GetAttribute(Attributes att) => att switch {
        Attributes.Beauty => GetBeauty(),
        Attributes.Vigor => GetVigor(),
        Attributes.Magic => GetMagic(),
        Attributes.Heart => GetHeart(),
        Attributes.Intellect => GetIntellect(),
        Attributes.Terror => GetTerror(),
        _ => 0,
    };

    public float GetBeauty() => GetStat(face.beauty, background.beauty, material.beauty, glaze.beauty, Attributes.Beauty);
    public float GetVigor() => GetStat(face.vigor, background.vigor, material.vigor, glaze.vigor, Attributes.Vigor);
    public float GetMagic() => GetStat(face.magic, background.magic, material.magic, glaze.magic, Attributes.Magic);
    public float GetHeart() => GetStat(face.heart, background.heart, material.heart, glaze.heart, Attributes.Heart);
    public float GetIntellect() => GetStat(face.intellect, background.intellect, material.intellect, glaze.intellect, Attributes.Intellect);
    public float GetTerror() => GetStat(face.terror, background.terror, material.terror, glaze.terror, Attributes.Terror);

    private float GetStat(float a, float b, float c, float d, Attributes att) {
        float baseVal = a + b + c + d;
        float mult = multipliers.ContainsKey(att) ? multipliers[att] : 1f;
        return baseVal * mult;
    }
}
