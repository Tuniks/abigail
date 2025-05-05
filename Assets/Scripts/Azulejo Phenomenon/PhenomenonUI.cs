using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PhenomenonUI : MonoBehaviour{
    public GameObject bg;
    public GameObject tileHolder;

    public Animator tileAnimator;

    public float tileScale = 1f;

    public float bagWait = 1f;
    public float triggerWait = 1f;
    public float animationWait = 1.75f;
    
    public void PlaceTile(AzulejoPhenomenon phenomenon, Tile tile){
        ItemElement item = tile.GetComponentInParent<ItemElement>();

        item.transform.SetParent(tileHolder.transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = new Vector3(tileScale, tileScale, tileScale);

        PlayerUIManager.instance.HideInventory();
        PlayerInventory.Instance.RemoveTileFromCollection(tile);

        StartCoroutine(TriggerPhenomenon(phenomenon));
    }

    private IEnumerator TriggerPhenomenon(AzulejoPhenomenon phenomenon){
        yield return new WaitForSeconds(bagWait);
        tileAnimator.SetTrigger("Activate");
        
        yield return new WaitForSeconds(triggerWait);
        phenomenon.TriggerPhenomenon();

        yield return new WaitForSeconds(animationWait - triggerWait);
        Destroy(tileHolder.GetComponentInChildren<ItemElement>().gameObject);
        tileHolder.SetActive(false);
        gameObject.SetActive(false);

        // Resets phenomenon to check if bag should still shake
        PlayerUIManager.instance.SetPhenomenon(phenomenon);
    }

    public void ShowUI(){
        gameObject.SetActive(true);
        bg.SetActive(true);
        tileHolder.SetActive(true);
    }

    public void HideUI(){
        if(tileHolder.GetComponentInChildren<Tile>() == null){
            gameObject.SetActive(false);
            tileHolder.SetActive(false);
        }

        bg.SetActive(false);
    }

    public Vector3 GetSlotPosition(){
        return tileHolder.transform.position;
    }

    public float GetSlotScale(){
        return tileScale;
    }

    public bool IsOn(){
        return bg.activeSelf;
    }
}
