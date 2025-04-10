using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConvoSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public void OnPointerEnter(PointerEventData eventData){
        GetComponentInParent<PlayerUIManager>().SetCurrentSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        GetComponentInParent<PlayerUIManager>().SetCurrentSlot(null);
    }
}
