using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestStep {
    public int id;

    // Conditions
    private QuestCondition[] visibleCondition;
    private QuestCondition[] completeCondition;

    // How Many Conditions Have to be Met
    private int visibilityTarget = 1;
    private int completeTarget = 1;

    // Step can be completed when inactive (default is true, steps can be completed whenever, but won show up until required)
    private bool completeWhenInactive = true;

    private QuestStatus status;

    // === Constructor ===
    public QuestStep(int _id, QuestCondition[] _vcon, QuestCondition[] _ccon, QuestStatus _status = QuestStatus.Hidden, bool _inactive = true, int _vt = 0, int _ct = 0){
        this.id = _id;
        this.visibleCondition = _vcon;
        this.completeCondition = _ccon;
        this.status = _status;
        this.completeWhenInactive = _inactive;

        if(_vt == 0){
            this.visibilityTarget = visibleCondition.Length;
        } else this.visibilityTarget = _vt;

        if(_ct == 0){
            this.completeTarget = completeCondition.Length;
        } else this.completeTarget = _ct;
    }

    // === Complete Condition ===
    public bool CompleteCondition(QuestCondition _condition){
        if(status == QuestStatus.Complete) return true;

        // First going to check if it should become active if still hidden
        if(status == QuestStatus.Hidden){
            int visibilityCount = 0;
            foreach(QuestCondition _vc in visibleCondition){
                if(_vc == _condition){
                    _vc.CompleteCondition();
                    visibilityCount++;
                } else if(_vc.IsCompleted()) visibilityCount++;
            }
            
            if(visibilityCount == visibilityTarget) SetStepActive();
        }

        // Then checking if step can be completed, completing it only if condition is met and completeIsInactive is true or quest is active
        // If condition was met but it couldnt be completed yet, return false
        int completeCount = 0;
        bool wasConditionSuccesfullyCompleted = true;
        foreach(QuestCondition _cc in completeCondition){
            if(_cc == _condition){
                if(completeWhenInactive || status == QuestStatus.Active){
                    _cc.CompleteCondition();
                    completeCount++;
                } else if(completeWhenInactive == false && status == QuestStatus.Hidden) wasConditionSuccesfullyCompleted = false;
            } else if(_cc.IsCompleted()) completeCount++; 
        }

        if(completeCount == completeTarget) SetStepComplete();

        return wasConditionSuccesfullyCompleted;
    }

    // ========= SETTERS =========
    private void SetStepActive(){
        if(status != QuestStatus.Hidden) return;
        status = QuestStatus.Active;
    }

    private void SetStepComplete(){
        if(status != QuestStatus.Active) return;
        status = QuestStatus.Complete;
    }

    // ========= GETTERS =========
    public QuestCondition[] GetConditions(){
        QuestCondition[] allConditions = visibleCondition.Concat(completeCondition).ToArray();

        return allConditions;
    }

    public bool IsComplete(){
        return status == QuestStatus.Complete;
    }

    public bool IsActive(){
        return status == QuestStatus.Active;
    }

}
