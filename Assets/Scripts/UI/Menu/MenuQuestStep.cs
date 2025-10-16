using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuestStep : MonoBehaviour{
    public GameObject activeStep;
    public GameObject completeStep;

    public void SetStepStatus(int id,  QuestStep[] stepsData){
        activeStep.SetActive(false);
        completeStep.SetActive(false);
        
        foreach(QuestStep step in stepsData){
            if(step.id == id){
                if(step.IsComplete()){
                    completeStep.SetActive(true);
                } else if(step.IsActive()){
                    activeStep.SetActive(true);
                }
                return;
            }
        }
    }
}
