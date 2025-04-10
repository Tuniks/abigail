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
        convoUI.ShowHover();
        GetComponentInParent<PlayerUIManager>().SetCurrentSlot(this);
    }

    public void OnPointerExit(PointerEventData eventData){
        convoUI.HideHover();
        GetComponentInParent<PlayerUIManager>().SetCurrentSlot(null);
    }
}
