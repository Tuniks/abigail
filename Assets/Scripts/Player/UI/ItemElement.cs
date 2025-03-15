using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemElement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler{
    private PlayerUIManager ui;
    private CanvasGroup cg;

    private RectTransform rect;
    private Transform previousParent;

    public Vector3 tileScale = new Vector3(1, 1, 1); 

    void Start(){
        ui = GetComponentInParent<PlayerUIManager>();
        cg = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData){
        previousParent = transform.parent;
        cg.blocksRaycasts = false;
        transform.SetParent(ui.GetHeldItemParent(), false);
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

        UpdateSpriteOrder(10);
    }

    public void OnDrag(PointerEventData eventData){
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10.0f; 
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
    }

    public void OnPointerUp(PointerEventData eventData){
        cg.blocksRaycasts = true;
        ui.DropItemElement(this);
        UpdateSpriteOrder(-10);
    }

    public void ReturnToPreviousParent(){
        transform.SetParent(previousParent, false);
    }

    public void SetNewParent(Transform dad){
        transform.SetParent(dad,false);
    }

    public Transform GetPreviousParent(){
        return previousParent;
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

    private void UpdateSpriteOrder(int order){
        SpriteRenderer[] sprs = GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer spr in sprs){
            spr.sortingOrder += order;
        }
    }
}
