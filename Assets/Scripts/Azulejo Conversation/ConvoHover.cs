using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConvoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    private AzulejoConvoUI convoUI;
    
    void Start(){
        convoUI = GetComponentInParent<AzulejoConvoUI>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        convoUI.SetHoverHover(true);
    }

    public void OnPointerExit(PointerEventData eventData){
        convoUI.SetHoverHover(false);
    }
}
