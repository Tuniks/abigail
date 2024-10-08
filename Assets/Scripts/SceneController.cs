using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class SceneController : MonoBehaviour{
    [YarnCommand]
    public void StartTileGame(){
        SceneManager.LoadScene("TileGame");
    }
}
