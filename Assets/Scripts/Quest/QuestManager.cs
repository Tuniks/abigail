using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Quick Typedef so i dont have to keep writing this long ass bs
using ConditionPair = System.Collections.Generic.KeyValuePair<QuestCondition, Quest>;

public class QuestManager : MonoBehaviour{
    public static QuestManager Instance;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void OnTileAcquired(string tileName){
        List<ConditionPair> conditions = QuestPlayerState.Instance.GetHasTileConditions();
        List<ConditionPair> conditionsToRemove = new List<ConditionPair>();

        // Check each condition to see if any should be complete when acquiring this tile
        foreach(ConditionPair cond in conditions){
            if(cond.Key.GetTargetName() == tileName){
                bool conditionMet = cond.Value.CompleteCondition(cond.Key);
                if(conditionMet) conditionsToRemove.Add(cond);
            }
        }

        // Remove met conditions from condition list
        foreach(ConditionPair cond in conditionsToRemove){
            conditions.RemoveAll(item => (item.Key == cond.Key) && (item.Value.GetTitle() == cond.Value.GetTitle()));
        }
    }

    public void OnTileShownTo(string tileName, string npcName){
        List<ConditionPair> conditions = QuestPlayerState.Instance.GetShowTileToConditions();
        List<ConditionPair> conditionsToRemove = new List<ConditionPair>();

        // Check each condition to see if any should be complete when showing this tile to npc
        foreach(ConditionPair cond in conditions){
            if(cond.Key.GetTargetName() == tileName && cond.Key.GetSecondaryTargetName() == npcName){
                bool conditionMet = cond.Value.CompleteCondition(cond.Key);
                if(conditionMet) conditionsToRemove.Add(cond);
            }
        }

        // Remove met conditions from condition list
        foreach(ConditionPair cond in conditionsToRemove){
            conditions.RemoveAll(item => (item.Key == cond.Key) && (item.Value.GetTitle() == cond.Value.GetTitle()));
        }
    }

    public void OnTalkToNPC(string npcName){
        List<ConditionPair> conditions = QuestPlayerState.Instance.GetTalkToConditions();
        List<ConditionPair> conditionsToRemove = new List<ConditionPair>();

        // Check each condition to see if any should be complete when talkin to npc
        foreach(ConditionPair cond in conditions){
            if(cond.Key.GetTargetName() == npcName){
                bool conditionMet = cond.Value.CompleteCondition(cond.Key);
                if(conditionMet) conditionsToRemove.Add(cond);
            }
        }

        // Remove met conditions from condition list
        foreach(ConditionPair cond in conditionsToRemove){
            conditions.RemoveAll(item => (item.Key == cond.Key) && (item.Value.GetTitle() == cond.Value.GetTitle()));
        }
    }

    public void OnTrigger(string triggerName){
        List<ConditionPair> conditions = QuestPlayerState.Instance.GetTriggerConditions();
        List<ConditionPair> conditionsToRemove = new List<ConditionPair>();

        // Check each condition to see if any should be complete when talkin to npc
        foreach(ConditionPair cond in conditions){
            if(cond.Key.GetTargetName() == triggerName){
                bool conditionMet = cond.Value.CompleteCondition(cond.Key);
                if(conditionMet) conditionsToRemove.Add(cond);
            }
        }

        // Remove met conditions from condition list
        foreach(ConditionPair cond in conditionsToRemove){
            conditions.RemoveAll(item => (item.Key == cond.Key) && (item.Value.GetTitle() == cond.Value.GetTitle()));
        }
    }
}
