using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum QuestStatus{
    Hidden,
    Active,
    Complete
}

public class Quest {
    private string title;
    
    private QuestStatus status;

    private QuestCondition[] requirements;
    private QuestStep[] steps;

    private int requirementTarget = 1;

    // === Constructor ===
    public Quest(string _title, QuestCondition[] _req, QuestStep[] _steps, int _rt = 0, QuestStatus _status = QuestStatus.Hidden){
        this.title = _title;
        this.requirements = _req;
        this.steps = _steps;
        this.status = _status;

        if(_rt == 0){
            this.requirementTarget = requirements.Length;
        } else this.requirementTarget = _rt;
    }

    // === Complete Condition ===
    // Returns true if all instances of the condition were successfully completed (there werent any )
    public bool CompleteCondition(QuestCondition _condition){
        if(status == QuestStatus.Complete) return true;
        
        // First checking for quest requirements, if the quest is Hidden, check to see if the amount of completed requirements meets the target
        // If so, complete it
        if(status == QuestStatus.Hidden){
            int requirementsMet = 0;
            foreach(QuestCondition _req in requirements){
                if(_req == _condition){
                    _req.CompleteCondition();
                    requirementsMet++;
                }else if(_req.IsCompleted()) requirementsMet++;
            }

            if(requirementsMet == requirementTarget) SetQuestActive();
        }

        // Then go over each step and check if condition can be met
        int stepsComplete = 0;
        bool wasConditionSuccesfullyCompleted = true;
        foreach(QuestStep _stp in steps){
            bool blockCheck = _stp.CompleteCondition(_condition);
            if(!blockCheck) wasConditionSuccesfullyCompleted = false;

            if(_stp.IsComplete()) stepsComplete++;
        }

        // If all steps are complete, complete quest
        if(stepsComplete == steps.Length){
            SetQuestComplete();
        }
        
        return wasConditionSuccesfullyCompleted;
    }

    // ========= SETTERS =========
    private void SetQuestActive(){
        if(status != QuestStatus.Hidden) return;

        status = QuestStatus.Active;
    }

    private void SetQuestComplete(){
        if(status != QuestStatus.Active) return;

        status = QuestStatus.Complete;
    }

    // ========= GETTERS =========
    public string GetTitle(){
        return title;
    }

    public bool IsCompleted(){
        return status == QuestStatus.Complete;
    }

    public bool IsActive(){
        return status == QuestStatus.Active;
    }

    public List<QuestCondition> GetAllConditions(){
        List<QuestCondition> conditions = new List<QuestCondition>();

        // Adding prereq conditions
        foreach(QuestCondition questCondition in requirements){
            conditions.Add(questCondition);
        }

        // Adding steps conditions
        foreach(QuestStep step in steps){
            QuestCondition[] stepConditions = step.GetConditions();
            foreach(QuestCondition stepCon in stepConditions){
                conditions.Add(stepCon);
            }
        }

        return conditions;
    }

    public QuestStep[] GetSteps(){
        return steps;
    }
}
