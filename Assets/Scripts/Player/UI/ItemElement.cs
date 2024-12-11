using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemElement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler{
    private PlayerUIManager ui;
    private CanvasGroup cg;

    private Transform previousParent;

    void Start(){
        ui = GetComponentInParent<PlayerUIManager>();
        cg = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData){
        previousParent = transform.parent;
        cg.blocksRaycasts = false;
        transform.SetParent(ui.GetHeldItemParent());

    }

    public void OnDrag(PointerEventData eventData){
        Vector3 mousePos = eventData.position;
        transform.position = mousePos;
    }

    public void OnPointerUp(PointerEventData eventData){
        cg.blocksRaycasts = true;
        ui.DropItemElement(this);
    }

    public void ReturnToPreviousParent(){
        transform.SetParent(previousParent);
    }

    public void SetNewParent(Transform dad){
        transform.SetParent(dad);
    }

    public Transform GetPreviousParent(){
        return previousParent;
    }
}
