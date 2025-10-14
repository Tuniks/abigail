using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStep{
    public int id;

    // Conditions
    private QuestCondition visibleCondition;
    private QuestCondition completeCondition;

    private QuestStatus status;

    // === Constructor ===
    public QuestStep(int _id, QuestCondition _vcon, QuestCondition _ccon){
        this.id = _id;
        this.visibleCondition = _vcon;
        this.completeCondition = _ccon;
        this.status = QuestStatus.Hidden;
    }

}
