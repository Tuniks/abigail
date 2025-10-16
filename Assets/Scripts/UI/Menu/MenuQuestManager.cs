using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuQuestManager : MenuTab{
    [System.Serializable]
    public struct MenuQuestElement{
        public MenuQuestItem item;
        public MenuQuestPage page;
    }

    [Header("UI Obj References")]
    public MenuQuestElement[] questElements;
    private Dictionary<string, MenuQuestElement> questElementReferences;

    // Current State
    private string currentPage = "";

    override public void Initialize(){
        questElementReferences = new Dictionary<string, MenuQuestElement>();
        
        foreach(MenuQuestElement quest in questElements){
            questElementReferences.Add(quest.item.title, quest);   
        }
    }
    
    public override void ShowTab(){
        List<Quest> activeQuests = QuestPlayerState.Instance.GetActiveQuests();
        
        // Populate list
        foreach(Quest q in activeQuests){
            string title = q.GetTitle();
            if(questElementReferences.ContainsKey(title)){
                MenuQuestElement element = questElementReferences[title];
                element.item.Activate();
            }            
        }

    }

    public void OnClickListElement(string questName){
        if(questElementReferences.ContainsKey(currentPage)){
            MenuQuestElement prevElement = questElementReferences[currentPage];
            prevElement.page.Hide();
        }

        Debug.Log(questName);

        if(questElementReferences.ContainsKey(questName)){
            Debug.Log("Achou");
            Quest questData = QuestPlayerState.Instance.GetQuestFromName(questName);

            MenuQuestElement newElement = questElementReferences[questName];
            newElement.page.Show(questData);
        }
        
        currentPage = questName;
    }
}
