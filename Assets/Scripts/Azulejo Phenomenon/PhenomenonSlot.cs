using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PhenomenonSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public void OnPointerEnter(PointerEventData eventData){
        GetComponentInParent<PlayerUIManager>().SetCurrentPhenomenonSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        GetComponentInParent<PlayerUIManager>().SetCurrentPhenomenonSlot(null);
    }
}
