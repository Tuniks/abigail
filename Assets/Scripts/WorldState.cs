using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState : MonoBehaviour{
    public static WorldState Instance;
    
    private int momsCurrentState = 0;
    private int feltCurrentState = 0;
    private int clayCurrentState = 0;
    private int collageCurrentState = 0;

    private Areas lastArea = Areas.None;
    private Vector3 customDestination = Vector3.zero;

    void Awake(){
        DontDestroyOnLoad(gameObject);
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetUpScene(){
        
    }
    
    public void SetLastArea(Areas origin){
        lastArea = origin;
    }


}
