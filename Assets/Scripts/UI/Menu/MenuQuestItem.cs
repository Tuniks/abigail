using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuestItem : MonoBehaviour{
    public string questName = "";

    public void OnClick(){
        MenuQuestManager menuQuestManager = GetComponentInParent<MenuQuestManager>();
        menuQuestManager.OnClickListElement(questName);
    }
}
