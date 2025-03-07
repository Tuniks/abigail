using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveCollageAzulejo : MonoBehaviour{
    public string nextScene = "COL_area";
    
    void Start(){
        SceneManager.LoadScene(nextScene);
    }
}
