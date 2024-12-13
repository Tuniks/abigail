using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class FeltManager : MonoBehaviour{
    [YarnCommand]
    public void StartTileGame(){
        SceneManager.LoadScene("TileGame");
    }

    [YarnCommand]
    public void GoToClay(){
        SceneManager.LoadScene("ClayArea");
    }
}
