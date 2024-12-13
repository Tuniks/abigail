using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInventory : Inventory{
    public static PlayerInventory Instance;    
    
    protected override void Awake(){        
        DontDestroyOnLoad(gameObject);
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        base.Awake();
    }

    void Update(){
        if(Input.GetKeyDown("p")){
            SceneManager.LoadScene("Start");
        }
    }
}
