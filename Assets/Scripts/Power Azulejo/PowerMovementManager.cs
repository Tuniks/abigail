using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerMovementManager : MonoBehaviour{
    public static PowerMovementManager Instance;

    [Header("Camera")]
    public Camera cam;
    public float camMoveSpeed = 4f;
    public float camZoomSpeed = 1f;
    public Vector2 zoomRange = new Vector2(5, 9f);
    
    private bool enableCamControl = true;
    private float currentZoom = 5;

    // [Header("Pointer")]
    // public Transform pointer;

    void Awake(){
        Instance = this;
    }

    void Start(){
        currentZoom = cam.orthographicSize;
    }

    void Update(){
        // Camera controls
        if(enableCamControl){
            // Zoom
            float deltazoom = -Input.mouseScrollDelta.y*Time.deltaTime*camZoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom+deltazoom, zoomRange.x, zoomRange.y);
            cam.orthographicSize = currentZoom;

            // Movement

        }


    }

}
