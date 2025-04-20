using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AreaInteractions : MonoBehaviour{
    public static AreaInteractions Instance;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else Destroy(this);
    }
}
