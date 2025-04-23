using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzulejoConvoUI : MonoBehaviour{ 
    [Header("UI References")]
    public GameObject convoScreen;
    public GameObject tileSlot;
    public GameObject open;
    public GameObject close;
    
    [Header("Tile Placement")]
    public Vector3 tileOffset = Vector3.zero;
    public Vector3 tileRotation = Vector3.zero;
    public float tileScale = 1f;

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
        open.SetActive(true);
        close.SetActive(false);
    }

    public void SetTile(Tile tile){
        ItemElement item = tile.GetComponentInParent<ItemElement>();

        open.SetActive(false);
        close.SetActive(true);

        item.transform.SetParent(tileSlot.transform);
        item.transform.localPosition = tileOffset;
        item.transform.localRotation = Quaternion.Euler(tileRotation);
        item.transform.localScale = new Vector3(tileScale, tileScale, tileScale);
    }

    public Vector3 GetSlotPosition(){
        return tileSlot.transform.position;
    }

    public float GetSlotScale(){
        return tileScale;
    }
}
