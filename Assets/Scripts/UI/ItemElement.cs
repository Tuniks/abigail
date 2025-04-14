using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemElement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler{
    private PlayerUIManager ui;
    private CanvasGroup cg;

    private Vector3 previousPos;
    private Dictionary<SpriteRenderer, int> originalSpriteOrder = null;

    public Vector3 tileScale = new Vector3(1, 1, 1); 

    void Start(){
        ui = GetComponentInParent<PlayerUIManager>();
        cg = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData){
        previousPos = transform.position;
        cg.blocksRaycasts = false;
        transform.SetParent(ui.GetHeldItemParent(), false);
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

        UpdateSpriteOrder(1000);

        ui.ShowTileDetails(GetTile());
    }

    public void OnDrag(PointerEventData eventData){
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
    }

    public void OnPointerUp(PointerEventData eventData){
        cg.blocksRaycasts = true;
        ui.DropItemElement(this);
    }

    public void ReturnToPreviousPosition(){
        transform.position = previousPos;
    }

    public void SetNewParent(Transform dad){
        transform.SetParent(dad,false);
    } 

    public void SetTile(GameObject tile){
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
}
