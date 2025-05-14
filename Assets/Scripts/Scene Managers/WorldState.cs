using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour{
    [System.Serializable]
    public struct SceneNames {
        public Areas area;
        public string name;
    }

    public static WorldState Instance;

    public List<SceneNames> areaScenes = new List<SceneNames>();
    
    private Dictionary<Areas, int> currentAreaState = new Dictionary<Areas, int>(){
        {Areas.Moms, 0},
        {Areas.Felt, 0},
        {Areas.Clay, 0},
        {Areas.Collage, 0},
        {Areas.Bedroom, 0}
    };

    private Areas lastArea = Areas.Default;
    private Dictionary<Areas, Vector3> lastPositionInArea = new Dictionary<Areas, Vector3>(){
        {Areas.Moms, Vector3.zero},
        {Areas.Felt, Vector3.zero},
        {Areas.Clay, Vector3.zero},
        {Areas.Collage, Vector3.zero},
        {Areas.Bedroom, Vector3.zero}
    };

    private Dictionary<string, string> npcConversationState = new Dictionary<string, string>();

    // POWER AZULEJO STUFF
    private string lastOpponent = "";
    private bool wonLastMatch = false;

    void Awake(){
        DontDestroyOnLoad(gameObject);
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    // ==== AREA MANAGEMENT ====
    public void SetLastArea(Areas origin, Vector3 pos){
        lastArea = origin;
        lastPositionInArea[origin] = pos;
    }

    public void UpdateSceneState(Areas area, int newState, bool forceUpdate = false){
        currentAreaState[area] = newState;
        if(forceUpdate) SceneController.Instance.UpdateSceneState();
    }

    // ==== NPC MANAGENT ====
    public void UpdateNPCDialogueNode(string npc, string node){
        npcConversationState[npc] = node;
    }

    public string GetCurrentNPCDialogueNode(string npc){
        if(npcConversationState.ContainsKey(npc)){
            return npcConversationState[npc];
        }

        return "";
    }

    // ===== GETTERS ====
    public string GetSceneName(Areas destination){
        foreach(SceneNames scene in areaScenes){
            if(scene.area == destination) return scene.name;
        }

        return null;
    }

    public int GetSceneState(Areas area){
        if(currentAreaState.ContainsKey(area)) return currentAreaState[area];

        return 0;
    }

    public Areas GetLastArea(){
        return lastArea;
    }

    public Vector3 GetLastPositionInArea(Areas area){
        if(lastPositionInArea.ContainsKey(area)) return lastPositionInArea[area];

        return Vector3.zero;
    }

    public void SetLastOpponent(string _opp){
        lastOpponent = _opp;
    }

    public string GetLastOpponent(){
        return lastOpponent;
    }

    public void SetWonLastMatch(bool status){
        wonLastMatch = status;
    }

    public bool GetWonLastMatch(){
        return wonLastMatch;
    }

}
