using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCameraManager : MonoBehaviour{
    public static PowerCameraManager Instance;

    [Header("Camera")]
    public Camera cam;
    public bool enableCamControl = true;

    // Zoom
    public float camZoomSpeed = 1f;
    public Vector2 zoomRange = new Vector2(5, 9f);
    private float currentZoom = 5;

    // Movement
    public float camMoveSpeed = 4f;
    private bool isDragging = false;
    private Vector3 dragOrigin = Vector3.zero;

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
            if(Input.GetMouseButtonDown(1)){
                isDragging = true;
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            }
            
            if(Input.GetMouseButtonUp(1)){
                isDragging = false;
            }
        }
    }

    void LateUpdate(){
        if(!enableCamControl || !isDragging) return;

        Vector2 moveDiff = dragOrigin - (cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position);
        cam.transform.position = new Vector3 (moveDiff.x, moveDiff.y, cam.transform.position.z);
    }

    public void SetCameraState(bool state){
        enableCamControl = state;
    }

}
