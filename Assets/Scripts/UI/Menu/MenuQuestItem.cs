using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuestItem : MonoBehaviour{
    public string title = "";

    public void Activate(){
        gameObject.SetActive(true);
    }

    public void OnClick(){
        MenuQuestManager menuQuestManager = GetComponentInParent<MenuQuestManager>();
        menuQuestManager.OnClickListElement(title);
    }
}
