using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quick Typedef so i dont have to keep writing this long ass bs
using ConditionPair = System.Collections.Generic.KeyValuePair<QuestCondition, Quest>;

public class QuestPlayerState : MonoBehaviour{
    public static QuestPlayerState Instance = null;
    
    // List of all quests in the game
    private List<Quest> quests = null;

    // Quest/Steps Conditions Per Condition Type
    // Makes list smaller to diminsh load per check
    private List<ConditionPair> currentHasTileConditions;
    private List<ConditionPair> currentShowTileToConditions;
    private List<ConditionPair> currentTalkToConditions;
    private List<ConditionPair> currentTriggerConditions;

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
        currentHasTileConditions = new List<ConditionPair>();
        currentShowTileToConditions = new List<ConditionPair>();
        currentTalkToConditions = new List<ConditionPair>();
        currentTriggerConditions = new List<ConditionPair>();
        

        QuestList questList = new QuestList();
        foreach(Quest q in questList.quests){
            quests.Add(q);

            // Adding conditions to list
            List<QuestCondition> questConditions = q.GetAllConditions();
            foreach(QuestCondition con in questConditions){
                AddConditionPairToList(q, con);
            }
        }
    }

    // ========= HELPERS =============
    private void AddConditionPairToList(Quest q, QuestCondition con){
        ConditionType conType = con.GetConditionType();
        
        switch (conType){
            case ConditionType.HasTile:
                currentHasTileConditions.Add(new ConditionPair(con, q));
                break;
            case ConditionType.ShowTileTo:
                currentShowTileToConditions.Add(new ConditionPair(con, q));
                break;
            case ConditionType.TalkTo:
                currentTalkToConditions.Add(new ConditionPair(con, q));
                break;
            case ConditionType.Trigger:
                currentTriggerConditions.Add(new ConditionPair(con, q));
                break;    
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

    public Quest GetQuestFromName(string _title){
        foreach(Quest q in quests){
            if(_title == q.GetTitle()) return q;
        }

        return null;
    }

    public List<ConditionPair> GetHasTileConditions(){
        return currentHasTileConditions;
    }

    public List<ConditionPair> GetShowTileToConditions(){
        return currentShowTileToConditions;
    }

    public List<ConditionPair> GetTalkToConditions(){
        return currentTalkToConditions;
    }

    public List<ConditionPair> GetTriggerConditions(){
        return currentTriggerConditions;
    }
}
