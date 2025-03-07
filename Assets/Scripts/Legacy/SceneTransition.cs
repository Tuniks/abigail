using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour{
    public string sceneName = null;

    private void OnTriggerEnter(Collider other){
        if(sceneName == null) return;
        if(other.gameObject.tag != "Player") return;

        SceneManager.LoadScene(sceneName);
    }
}
