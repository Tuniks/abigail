using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PowerCinemachine : MonoBehaviour{
    private CinemachineVirtualCamera cvc;
    private CinemachineFramingTransposer comp;
    private CinemachineBasicMultiChannelPerlin noise;
    public static PowerCinemachine Instance;

    [Header("Camera")]
    public Camera cam;
    public bool enableCamControl = true;

    [Header("Zoom")]
    public float camZoomSpeed = 1f;
    public Vector2 zoomRange = new Vector2(5, 9f);
    public float zoomStep = 0.01f;
    private float targetZoom = 5;

    [Header("Movement")]
    public Transform cameraTarget;
    public float camMoveSpeed = 4f;
    public Vector2 cameraLimitX = new Vector2(0, 0);
    public Vector2 cameraLimitY = new Vector2(0, 0);

    [Header("Drag Movement Variables")]
    public float deadZoneWidthDrag = 0;
    public float deadZoneHeightDrag = 0;
    public float dampingXDrag = 0.25f;
    public float dampingYDrag = 0.25f;

    [Header("Auto Movement Variables")]
    public float deadZoneWidthAuto = .35f;
    public float deadZoneHeightAuto = .35f;
    public float dampingXAuto = 1f;
    public float dampingYAuto = 1f;

    [Header("Hit Noise Data")]
    public float hitNoiseDuration = 1f;
    public float hitNoiseAmp = 1f;
    public float hitNoiseFreq = 3f;

    [Header("Death Noise Data")]
    public float deathNoiseDuration = 0.2f;
    public float deathNoiseAmp = 3f;
    public float deathNoiseFreq = 9f;

    [Header("Point Noise Data")]
    public float pointNoiseAmp = 0.1f;
    public float pointNoiseFreq = 1f;

    // Noise 
    private int lastShakeId = 0;

    void Awake(){
        Instance = this;
    }

    void Start(){
        cvc = GetComponent<CinemachineVirtualCamera>();
        comp = cvc.GetCinemachineComponent<CinemachineFramingTransposer>();
        noise = cvc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        targetZoom = cvc.m_Lens.OrthographicSize;
        SetTarget(cameraTarget);
    }

    void Update(){
        if(!enableCamControl) return;

        // Zooming
        float deltazoom = -Input.mouseScrollDelta.y*Time.deltaTime*camZoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom+deltazoom, zoomRange.x, zoomRange.y);

        // Move
        // if(IsMouseOutOfBounds()){
        //     cameraTarget.position = cam.transform.position;
        //     isDragging = true;
        //     dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        //     SetTarget(cameraTarget);
        // } else{
        //     isDragging = false;
        // }

            
    }

    void LateUpdate(){
        if(!enableCamControl) return;

        // Zoomin
        float currentZoom = cvc.m_Lens.OrthographicSize;
        if(currentZoom != targetZoom){
            cvc.m_Lens.OrthographicSize = Mathf.MoveTowards(currentZoom, targetZoom, zoomStep);
        }

        // Moovin
        // if(!isDragging) return;
        // Vector2 moveDiff = dragOrigin - (cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position);
        // cameraTarget.position = new Vector3 (moveDiff.x, moveDiff.y, cam.transform.position.z);
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newCamTargetPos = new Vector3(
            Mathf.Clamp(mousePos.x, cameraLimitX.x, cameraLimitX.y),
            Mathf.Clamp(mousePos.y, cameraLimitY.x, cameraLimitY.y),
            mousePos.z
        );
        cameraTarget.position = newCamTargetPos;
    }

    // ========== CAM MOVEMENT ===========
    public void SetTarget(Transform _t){
        if(_t.gameObject == cameraTarget.gameObject){
            SetComposerVariables(deadZoneWidthDrag, deadZoneHeightDrag, dampingXDrag, dampingYDrag);
        } else SetComposerVariables(deadZoneWidthAuto, deadZoneHeightAuto, dampingXAuto, dampingYAuto);
        
        cvc.Follow = _t;
        cvc.LookAt = _t;
    }

    private void SetComposerVariables(float width, float height, float x, float y){
        comp.m_DeadZoneWidth = width;
        comp.m_DeadZoneHeight = height;
        comp.m_XDamping = x;
        comp.m_YDamping = y;
    }

    // private bool IsMouseOutOfBounds(){
        
    // }

    // ========== CAM SHAKE ============
    public void HitCamShake(){
        lastShakeId++;
        StartCoroutine(Shake(hitNoiseDuration, hitNoiseAmp, hitNoiseFreq, lastShakeId));
    }

    public void DeathCamShake(){
        lastShakeId++;
        StartCoroutine(Shake(deathNoiseDuration, deathNoiseAmp, deathNoiseFreq, lastShakeId));
    }

    public void StartPointingCamShake(){
        noise.m_AmplitudeGain = pointNoiseAmp;
        noise.m_FrequencyGain = pointNoiseFreq;
    }

    public void StopPointingCamShake(){
        ResetNoise();
    }

    private IEnumerator Shake(float dur, float amp, float freq, int id){
        noise.m_AmplitudeGain = amp;
        noise.m_FrequencyGain = freq;
        yield return new WaitForSeconds(dur);
        if(id == lastShakeId){
            ResetNoise();
        }
    }

    private void ResetNoise(){
        lastShakeId = 0;
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }

}
