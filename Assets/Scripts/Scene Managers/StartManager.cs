using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour{
    public string startScene = "BED_area";
    public float transitionDelay = 1.5f;

    void Start(){
       StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene(){
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(startScene);
    }

}
