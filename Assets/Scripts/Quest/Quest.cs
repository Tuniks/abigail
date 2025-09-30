using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest {
    private string title;
    private string description;
    
    private bool isVisible;
    private bool isCompleted;

    private QuestCondition visibilityCondition;

    private QuestStep[] steps;

    // Constructor
    Quest(string _title, string _description, QuestCondition _visibilityCondition, QuestStep[] _steps){
        this.title = _title;
        this.description = _description;
        this.visibilityCondition = _visibilityCondition;
        this.steps = _steps;
        isVisible = false;
        isCompleted = false;
    }

    // Getters and Setters
    public string GetTitle(){
        return title;
    }

    public string GetDescription(){
        return description;
    }


}
