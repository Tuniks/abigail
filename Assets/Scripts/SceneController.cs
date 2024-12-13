using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class SceneController : MonoBehaviour{
    void Update(){
        if(Input.GetKeyDown("p")){
            SceneManager.LoadScene("Proto-Scene-B");
        }
    }
    
    // [YarnCommand]
    // public void StartTileGame(){
    //     SceneManager.LoadScene("TileGame");
    // }
}
