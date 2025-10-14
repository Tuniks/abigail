using System.Collections;
using System.Collections.Generic;
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

    // === Constructor ===
    public Quest(string _title, QuestCondition[] _req, QuestStep[] _steps){
        this.title = _title;
        this.requirements = _req;
        this.steps = _steps;
        this.status = QuestStatus.Active;
    }

    // Getters and Setters
    public string GetTitle(){
        return title;
    }

    public bool IsCompleted(){
        return status == QuestStatus.Complete;
    }

    public bool IsActive(){
        return status == QuestStatus.Active;
    }

}
