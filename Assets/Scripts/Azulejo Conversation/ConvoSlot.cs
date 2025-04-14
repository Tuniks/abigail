using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConvoSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private AzulejoConvoUI convoUI;
    
    void Start(){
        convoUI = GetComponentInParent<AzulejoConvoUI>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        convoUI.SetHoverSlot(true);
        GetComponentInParent<PlayerUIManager>().SetCurrentSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        convoUI.SetHoverSlot(false);
        GetComponentInParent<PlayerUIManager>().SetCurrentSlot(null);
    }
}
