using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest {
    private string title;
    private string description;
    
    private bool isVisible;
    private bool isCompleted;

    private QuestStep[] steps;

    // === Constructor ===
    public Quest(string _title, string _description, QuestStep[] _steps){
        this.title = _title;
        this.description = _description;
        this.steps = _steps;
        this.isVisible = false;
        this.isCompleted = false;
    }

    // Getters and Setters
    public string GetTitle(){
        return title;
    }

    public string GetDescription(){
        return description;
    }


}
