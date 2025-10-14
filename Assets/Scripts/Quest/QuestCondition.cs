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

    public bool IsCompleted(){
        return completed;
    }
    

}
