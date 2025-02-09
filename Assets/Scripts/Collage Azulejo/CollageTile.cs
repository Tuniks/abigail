using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollageTile : MonoBehaviour
{
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

    private CollagePlayerHand playerHand; // ✅ Updated from PlayerHand to CollagePlayerHand

    private Dictionary<Attributes, float> multipliers = new Dictionary<Attributes, float>();

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
        List<GameObject> toDestroy = new List<GameObject>();
        foreach(Transform child in transform){
            if(child.gameObject.GetComponent<TileComponent>() != null){
                toDestroy.Add(child.gameObject);
            }
        }
        foreach(GameObject child in toDestroy){
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

    private GameObject AddTileComponent(GameObject prefab){
        GameObject component = Instantiate(prefab);
        component.transform.parent = transform;
        component.transform.localPosition = prefab.transform.localPosition;
        component.transform.localRotation = prefab.transform.localRotation;
        component.transform.localScale = prefab.transform.localScale;
        return component;
    }

    public void SetHand(CollagePlayerHand hand) { // ✅ Updated from PlayerHand to CollagePlayerHand
        playerHand = hand;
    }
}
