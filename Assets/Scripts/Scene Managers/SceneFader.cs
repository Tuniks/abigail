using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour{
    public static SceneFader Instance;
    
    [Header("Scene Transition")]
    public Fader fader;
    public float fadeDuration = 1f;
    
    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start(){
        if(fader){
            StartCoroutine(fader.FadeOut(fadeDuration)); 
        }
        
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
}
