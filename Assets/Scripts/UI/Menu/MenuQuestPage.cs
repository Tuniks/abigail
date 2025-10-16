using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuestPage : MonoBehaviour{
    [System.Serializable]
    public struct QuestStepReference {
        public int id;
        public MenuQuestStep stepObject;
    }
    
    public QuestStepReference[] stepsUI;

    public void Hide(){
        gameObject.SetActive(false);
    }

    public void Show(Quest questData){
        QuestStep[] stepsData = questData.GetSteps();

        // Update status of each step
        foreach(QuestStepReference _stp in stepsUI){
            _stp.stepObject.SetStepStatus(_stp.id, stepsData);
        }


        // Finally, show page
        gameObject.SetActive(true);
    }
}
