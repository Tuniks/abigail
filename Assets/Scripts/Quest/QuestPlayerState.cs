using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPlayerState : MonoBehaviour{
    public static QuestPlayerState Instance = null;
    
    private List<Quest> activeQuests;
    private List<Quest> inactiveQuests;
    private List<Quest> completedQuests;

    private List<QuestCondition> currentConditions;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    
        Initialize();
    }

    private void Initialize(){
        if(activeQuests != null || inactiveQuests != null || completedQuests != null) return;

        activeQuests = new List<Quest>();
        inactiveQuests = new List<Quest>();
        completedQuests = new List<Quest>();

        QuestList questList = new QuestList();

        foreach(Quest q in questList.quests){
            activeQuests.Add(q);
        }
    }

    // ======== GETTERS AND SETTERS ===============

    public List<Quest> GetActiveQuests(){
        return activeQuests;
    }

    public List<Quest> GetInactiveQuests(){
        return inactiveQuests;
    }

    public List<Quest> GetCompletedQuests(){
        return completedQuests;
    }
}
