using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class FeltManager : MonoBehaviour{
    public string azulejoScene = "";
    public string clayScene = "";
    public string collageScene = "";
    
    [YarnCommand]
    public void StartTileGame(){
        SceneManager.LoadScene(azulejoScene);
    }

    [YarnCommand]
    public void GoToClay(){
        SceneManager.LoadScene(clayScene);
    }

    [YarnCommand]
    public void GoToCollage(){
        SceneManager.LoadScene(collageScene);
    }
}
