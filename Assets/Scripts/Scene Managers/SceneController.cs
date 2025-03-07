using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Areas{
    None,
    Default,
    Moms,
    Felt,
    Collage,
    Clay,
    Bedroom,
}

public class SceneController : MonoBehaviour{
    [System.Serializable]
    public struct AreaPortals{
        public Areas destination;
        public Transform position;
    }
    
    public static SceneController Instance;
    public Transform player;

    [Header("Area Data")]
    public Areas currentArea;
    public List<AreaPortals> areaPortals;
    private AreaManager currentManager;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        currentManager = GetComponent<AreaManager>();

        UpdateSceneState();
        UpdatePlayerPosition();
    }

    public void UpdateSceneState(){
        int currentState = WorldState.Instance.GetSceneState(currentArea);

        if(currentManager) currentManager.UpdateSceneState(currentState);
    }

    private void UpdatePlayerPosition(){
        Areas lastArea = WorldState.Instance.GetLastArea();
        foreach(AreaPortals area in areaPortals){
            if(area.destination == lastArea){
                player.position = area.position.position;
                return;
            }
        }

        // If no portals from the last area were found, move player to last known position in area
        Vector3 lastPos = WorldState.Instance.GetLastPositionInArea(currentArea);
        player.position = lastPos;
    }

    public void Travel(Areas destination){
        string nextScene = WorldState.Instance.GetSceneName(destination);

        if(nextScene == null) return;

        WorldState.Instance.SetLastArea(currentArea, player.position);
        SceneManager.LoadScene(nextScene);
    }

    public void Roundtrip(string scene){
        WorldState.Instance.SetLastArea(currentArea, player.position);
        SceneManager.LoadScene(scene);
    }
}
