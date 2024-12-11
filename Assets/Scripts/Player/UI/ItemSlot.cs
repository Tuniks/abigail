using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private PlayerUIManager ui;
    
    void Start(){
        ui = GetComponentInParent<PlayerUIManager>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        ui.SetCurrentSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        ui.SetCurrentSlot(null);
    }
}
