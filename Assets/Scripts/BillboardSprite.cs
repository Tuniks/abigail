using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardSprite : MonoBehaviour{
    public Camera cam = null;

    private void Start(){
        if(cam == null) cam = Camera.main;
    }
    
    void LateUpdate(){
        transform.forward = cam.transform.forward;
    }
}
