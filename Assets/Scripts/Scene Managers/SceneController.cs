using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum Areas{
    None,
    Default,
    Moms,
    Felt,
    Collage,
    Clay,
    Bedroom,
    Party
}

public class SceneController : MonoBehaviour{
    [System.Serializable]
    public struct AreaPortals{
        public Areas destination;
        public Transform position;
    }
    
    public static SceneController Instance;
    public Transform player;

    [Header("Scene Transition")]
    public SceneFader fader;
    public float fadeDuration = 1f;

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

        if(fader){
            StartCoroutine(fader.FadeOut(fadeDuration)); 
        }
        
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
        ChangeScene(nextScene);
    }

    public void Roundtrip(string scene){
        WorldState.Instance.SetLastArea(currentArea, player.position);
        ChangeScene(scene);
    }

    public void ChangeScene(string scene){
        if(fader==null){
            SceneManager.LoadScene(scene);
            return;
        }

        StartCoroutine(SceneRoutine(scene));
    }

    private IEnumerator SceneRoutine(string scene){
        yield return fader.FadeIn(fadeDuration);
        SceneManager.LoadScene(scene);
    }

    public void SameSceneFadeInAndOut(float fadeDuration, float pauseDuration){
        if(fader==null){
            Debug.Log("No Fader Attached");
            return;
        }

        StartCoroutine(fader.FadeInAndOut(fadeDuration, pauseDuration));
    }
}
