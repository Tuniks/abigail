using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PowerItemElement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler{
    private PowerInventory ui;
    private CanvasGroup cg;

    private Vector3 previousPos;
    private Dictionary<SpriteRenderer, int> originalSpriteOrder = null;

    public Vector3 tileScale = new Vector3(1, 1, 1);

    // Tile Scaling for Azulejo Convo
    private float startingScale = 1f;

    void Start(){
        ui = GetComponentInParent<PowerInventory>();
        cg = GetComponent<CanvasGroup>();
        startingScale = transform.localScale.x;
    }

    public void OnPointerDown(PointerEventData eventData){
        previousPos = transform.position;
        cg.blocksRaycasts = false;
        transform.SetParent(ui.GetHeldItemParent(), false);
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

        UpdateSpriteOrder(1000);
        UpdateMaskBehaviour(false);
    }

    public void OnDrag(PointerEventData eventData){
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        
        Vector3 slotPos = ui.GetSlotPosition();
        float targetScale = ui.GetSlotScale();
        if(targetScale == 0) return;

        float mult = Mathf.Max(0, 1 - (Vector3.Distance(transform.position, slotPos)/Vector3.Distance(previousPos, slotPos)));
        float newScale = startingScale + (targetScale-startingScale)*mult;
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void OnPointerUp(PointerEventData eventData){
        transform.localScale = new Vector3(startingScale, startingScale, startingScale);
        cg.blocksRaycasts = true;
        UpdateMaskBehaviour(true);
        ui.DropItemElement(this);
    }

    public void ReturnToPreviousPosition(){
        transform.position = previousPos;
    }

    public void SetNewParent(Transform dad){
        transform.SetParent(dad,false);
    } 

    public void SetTile(GameObject tile){
        if(tile == null) return;
        
        GameObject tileCopy = Instantiate(tile, Vector3.zero, Quaternion.identity);
        tileCopy.transform.SetParent(transform, false);
        tileCopy.transform.localScale = tileScale;

        tileCopy.SetActive(true);
    }

    public Tile GetTile(){
        return GetComponentInChildren<Tile>();
    }

    public void UpdateSpriteOrder(int order){
        if(originalSpriteOrder == null) InitializeOGOrder();

        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            spr.sortingOrder = originalSpriteOrder[spr] + order;
        }
    }

    private void InitializeOGOrder(){
        originalSpriteOrder = new Dictionary<SpriteRenderer, int>();

        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            originalSpriteOrder[spr] = spr.sortingOrder;
        }
    }

    private void UpdateMaskBehaviour(bool isMaskable){
        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            if(!isMaskable){
                spr.maskInteraction = SpriteMaskInteraction.None;
            } else spr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }
}
