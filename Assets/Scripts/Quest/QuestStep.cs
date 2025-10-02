using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStep{
    public string id;

    // Conditions
    private QuestCondition visibleCondition;
    private QuestCondition completeCondition;

    // Text
    private string hintText;
    private string completedText;

    private bool isVisible;
    private bool isCompleted;

    // === Constructor ===
    public QuestStep(string _id, QuestCondition _vcon, QuestCondition _ccon, string _hint, string _completed){
        this.id = _id;
        this.visibleCondition = _vcon;
        this.completeCondition = _ccon;
        this.hintText = _hint;
        this.completedText = _completed;
        this.isVisible = false;
        this.isCompleted = false;
    }

}
