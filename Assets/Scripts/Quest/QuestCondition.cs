using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType{
    HasTile,
    ShowTileTo,
    TalkTo,
    Trigger
}

public class QuestCondition{
    private ConditionType type;

    private string target1Name = "";
    private string target2Name = "";

    private bool completed = false;

    public QuestCondition(ConditionType _type, string t1, string t2 = ""){
        this.type = _type;
        this.target1Name = t1;
        this.target2Name = t2;
        this.completed = false;
    }

    // Setters
    public void CompleteCondition(){
        this.completed = true;
    }

    // Getters
    public bool IsCompleted(){
        return completed;
    }

    public ConditionType GetConditionType(){
        return type;
    }

    public string GetTargetName(){
        return target1Name;
    }

    public string GetSecondaryTargetName(){
        return target2Name;
    }

    // Operator Overrides
    public static bool operator == (QuestCondition a, QuestCondition b){
        return (a.type == b.type && a.target1Name == b.target1Name && a.target2Name == b.target2Name);
    }

    public static bool operator != (QuestCondition a, QuestCondition b){
        return !(a == b);
    }

    public override bool Equals(object obj) {
        if (obj == null) return false;
        if (GetType() != obj.GetType()) return false;
        QuestCondition b = obj as QuestCondition;
        return this == b;
    }

    public override int GetHashCode() {
        return type.GetHashCode() ^
               target1Name.GetHashCode() ^
               target2Name.GetHashCode();
    }
}
