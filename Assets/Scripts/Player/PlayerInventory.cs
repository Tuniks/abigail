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

    // ============ DELETE POST SHOWCASE ============
    void Update(){
        if(Input.GetKeyDown("0")){
            ResetGame();
        }

        if(Input.GetKeyDown(KeyCode.Backspace)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ResetGame(){
        SceneManager.LoadScene("START");
        Destroy(this.gameObject);
    }
}
