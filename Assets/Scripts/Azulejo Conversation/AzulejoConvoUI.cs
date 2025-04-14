using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzulejoConvoUI : MonoBehaviour{ 
    [Header("UI References")]
    public GameObject convoScreen;
    public GameObject tileSlot;
    public GameObject tileList;
    public GameObject tileListContent;

    [Header("Prefabs")]
    public GameObject itemPrefab;

    // === Other ===
    private bool isHoveringSlot = false;
    private bool isHoveringHover = false;

    private void Update(){
        if(isHoveringHover || isHoveringSlot){
            tileList.SetActive(true);
        } else tileList.SetActive(false);
    }
    
    public void Show(){
        convoScreen.SetActive(true);
    }

    public void Hide(){
        foreach(Transform child in tileSlot.transform){
            if(child.GetComponent<ItemElement>()){
                Destroy(child.gameObject);
            }
        }
        convoScreen.SetActive(false);
    }

    public void SetTile(Tile tile){
        ItemElement item = tile.GetComponentInParent<ItemElement>();

        item.transform.SetParent(tileSlot.transform);
        item.transform.localPosition = Vector3.zero;
    }

    public void BuildHover(List<FaceDialoguePair> faces){
        foreach(Transform child in tileListContent.transform){
            Destroy(child.gameObject);
        }

        foreach(FaceDialoguePair face in faces){
            GameObject item = Instantiate(itemPrefab, tileListContent.transform);
            HoverItem hov = item.GetComponent<HoverItem>();
            hov.SetTile(face);
        }
    }

    public void SetHoverSlot(bool _state){
        isHoveringSlot = _state;
    }

    public void SetHoverHover(bool _state){
        isHoveringHover = _state;
    }
}
