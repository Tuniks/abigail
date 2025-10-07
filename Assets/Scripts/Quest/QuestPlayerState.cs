using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPlayerState : MonoBehaviour{
    private List<Quest> activeQuests;
    private List<Quest> inactiveQuests;
    private List<Quest> completedQuests;

    private List<QuestCondition> currentConditions;
}
