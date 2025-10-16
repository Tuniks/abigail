using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionData : MonoBehaviour{
    public static PlayerInteractionData Instance = null;

    private List<string> acquiredTiles = null;
    private Dictionary<string, List<string>> tilesShownToNPCs = null;
    private List<string> npcsTalkedTo = null;
    private List<string> triggersTriggered = null;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        Initialize();
    }

    private void Initialize(){
        if(acquiredTiles != null) return;

        acquiredTiles = new List<string>();
        tilesShownToNPCs = new Dictionary<string, List<string>>();
        npcsTalkedTo = new List<string>();
        triggersTriggered = new List<string>();
    }

    public void RegisterTileAcquisition(string tileName){
        if(acquiredTiles.Contains(tileName)) return;
        acquiredTiles.Add(tileName);

        QuestManager.Instance.OnTileAcquired(tileName);
    }

    public void RegisterNPCTileInteraction(string tileName, string npcName){
        if(!tilesShownToNPCs.ContainsKey(npcName)){
            tilesShownToNPCs[npcName] = new List<string>();
        }

        if(!tilesShownToNPCs[npcName].Contains(tileName)){
            tilesShownToNPCs[npcName].Add(tileName);
        }

        QuestManager.Instance.OnTileShownTo(tileName, npcName);
    }

    public void RegisterNPCTalk(string npcName){
        if(npcsTalkedTo.Contains(npcName)) return;
        npcsTalkedTo.Add(npcName);

        QuestManager.Instance.OnTalkToNPC(npcName);
    }

    public void RegisterTrigger(string trigger){
        if(triggersTriggered.Contains(trigger)) return;
        triggersTriggered.Add(trigger);

        QuestManager.Instance.OnTrigger(trigger);
    }

    
}
