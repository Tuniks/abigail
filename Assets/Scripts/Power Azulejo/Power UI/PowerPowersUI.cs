using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerPowersUI : MonoBehaviour, IPointerExitHandler{
    public void OnPointerExit(PointerEventData eventData){
        GetComponentInParent<PowerPowers>().ExitUIArea();
    }
}
