using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPlayerState : MonoBehaviour{
    public static QuestPlayerState Instance = null;
    
    private List<Quest> quests = null;

    // Quest And Steps Conditions
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
        if(quests != null) return;

        quests = new List<Quest>();

        QuestList questList = new QuestList();

        foreach(Quest q in questList.quests){
            quests.Add(q);
        }
    }

    // ======== GETTERS AND SETTERS =============

    public List<Quest> GetAllQuests(){
        return quests;
    }

    public List<Quest> GetActiveQuests(){
        List<Quest> activeQuests = new List<Quest>();

        foreach(Quest q in quests){
            if(q.IsActive()) activeQuests.Add(q);
        }
        
        return activeQuests;
    }

    public List<Quest> GetCompletedQuests(){
        List<Quest> completedQuests = new List<Quest>();

        foreach(Quest q in quests){
            if(q.IsCompleted()) completedQuests.Add(q);
        }
        
        return completedQuests;
    }
}
