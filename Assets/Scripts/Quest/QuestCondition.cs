using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType{
    HasTile,
    ShowTileTo,
    Trigger
}

public class QuestCondition{
    private ConditionType type;

    private string target1Name = "";
    private string target2Name = "";

    public QuestCondition(ConditionType _type, string t1, string t2 = ""){
        this.type = _type;
        this.target1Name = t1;
        this.target2Name = t2;
    }
    

}
