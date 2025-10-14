using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionData : MonoBehaviour{
    public static PlayerInteractionData Instance = null;

    private List<string> acquiredTiles;
    private Dictionary<string, List<string>> tilesShownToNPCs;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void RegisterTileAcquisition(string tileName){
        if(acquiredTiles.Contains(tileName)) return;
        acquiredTiles.Add(tileName);

        CheckQuestConditionUpdate();
    }

    public void RegisterNPCTileInteraction(string tileName, string npcName){
        if(!tilesShownToNPCs.ContainsKey(npcName)){
            tilesShownToNPCs[npcName] = new List<string>();
        }

        if(!tilesShownToNPCs[npcName].Contains(tileName)){
            tilesShownToNPCs[npcName].Add(tileName);
        }

        CheckQuestConditionUpdate();
    }

    private void CheckQuestConditionUpdate(){
        
    }
}
