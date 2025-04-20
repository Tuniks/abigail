using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzulejoConvoUI : MonoBehaviour{ 
    [Header("UI References")]
    public GameObject convoScreen;
    public GameObject tileSlot;

    [Header("Prefabs")]
    public GameObject itemPrefab;
    
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
}
