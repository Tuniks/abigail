using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerHandSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    
    
    public void OnPointerEnter(PointerEventData eventData){
        GetComponentInParent<PowerInventory>().SetCurrentSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        GetComponentInParent<PowerInventory>().SetCurrentSlot(null);
    }
}
