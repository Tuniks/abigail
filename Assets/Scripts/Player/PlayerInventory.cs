using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : Inventory{
    public static PlayerInventory Instance;    
    
    void Awake(){        
        DontDestroyOnLoad(gameObject);
        if(Instance == null){
            Instance = this;
        } else Destroy(gameObject);
    }
    
    void Update(){
        if(Input.GetKeyDown("h")){
            SceneManager.LoadScene("TileGame");
        }
    }
}
