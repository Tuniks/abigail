using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuestPage : MonoBehaviour{
    [System.Serializable]
    public struct QuestStepReference {
        public string id;
        public GameObject stepObject;
    }
    
    public QuestStepReference[] stepList;
}
