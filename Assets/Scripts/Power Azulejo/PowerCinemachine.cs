using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using NUnit.Framework;
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
    public Vector2 zoomRange = new Vector2(5, 9f);
    public float zoomInputMultiplier = 4f;
    public float zoomTime = 0.25f;
    private float zoomVel = 0;
    private float targetZoom = 5;

    [Header("Movement")]
    public Transform cameraTarget;
    public float camMoveSpeed = 4f;
    public Vector2 cameraLimitX = new Vector2(0, 0);
    public Vector2 cameraLimitY = new Vector2(0, 0);

    [Header("Drag Movement Variables")]
    // Movement for mouse controls
    public Vector2 deadZoneDrag = new Vector2(.4f, .4f); // Width x Height
    public Vector2 dampingDrag = new Vector2(2, 2);

    [Header("Target Movement Variables")]
    // Movement when targeting tiles automatically
    public Vector2 deadZoneTarget = new Vector2(.2f, .2f); // Width x Height
    public Vector2 dampingTarget = new Vector2(1, 1);
    public float minTargetDuration = 1f;
    public float maxTargetDuration = 5f;
    public float minTargetDistance = .3f;

    private Vector2 mousePosBeforeTarget = new Vector2(0, 0);
    private float timeBeforeTarget = 0;
    private GameObject currentTarget = null; 
    private bool isPointing = false;

    [Header("Target Zoom Variables")]
    public Vector2 zoomWhenTargettingPlayerRange = new Vector2(7, 15);
    public Vector2 zoomWhenTargettingEnemyRange = new Vector2(7, 15);

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
        isPointing = false;
    }

    void Update(){
        if(!enableCamControl) return;

        // Zooming
        targetZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomInputMultiplier;
        targetZoom = Mathf.Clamp(targetZoom, zoomRange.x, zoomRange.y);
        
        // Checking if can regain control from auto target
        if(currentTarget != cameraTarget.gameObject && !isPointing){
            if(Time.time > timeBeforeTarget + minTargetDuration){
                Vector2 currentPos = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
                if(Time.time > timeBeforeTarget + maxTargetDuration || Vector2.Distance(currentPos, mousePosBeforeTarget) > minTargetDistance){
                    SetTarget(cameraTarget);
                }
            }
        }
    }

    void LateUpdate(){
        if(!enableCamControl) return;

        // Zoomin
        float currentZoom = cvc.m_Lens.OrthographicSize;
        if(currentZoom != targetZoom){
            cvc.m_Lens.OrthographicSize = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVel, zoomTime);
        }

        // Moving camera target with mouse position
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newCamTargetPos = new Vector3(
            Mathf.Clamp(mousePos.x, cameraLimitX.x, cameraLimitX.y),
            Mathf.Clamp(mousePos.y, cameraLimitY.x, cameraLimitY.y),
            mousePos.z
        );
        cameraTarget.position = newCamTargetPos;
    }

    // ========== CAM MOVEMENT ===========
    public void TargetTile(Transform _t, bool skipZoom = false){
        if (!skipZoom){
            targetZoom = zoomWhenTargettingPlayerRange.x;
        }
        
        mousePosBeforeTarget = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
        timeBeforeTarget = Time.time;
        
        SetTarget(_t);
    }

    public void StartPointing(Transform _t){
        StartPointingCamShake();
        TargetTile(_t);
        isPointing = true;

    }

    public void UpdatePointer(float pct){
        targetZoom = Mathf.Lerp(zoomWhenTargettingPlayerRange.x, zoomWhenTargettingPlayerRange.y, pct);
    }

    public void StopPointing(){
        StopPointingCamShake();
        isPointing = false;
    }

    public void StartTargetEnemy(Transform _t){
        StartPointingCamShake();

        targetZoom = zoomWhenTargettingEnemyRange.x;
        mousePosBeforeTarget = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
        timeBeforeTarget = Time.time;
        
        SetTarget(_t);
    }

    public void ReleaseTargetEnemy(){
        StopPointingCamShake();
        targetZoom = zoomWhenTargettingEnemyRange.y;
    }

    private void SetTarget(Transform _t){
        currentTarget = _t.gameObject;
        if(currentTarget == cameraTarget.gameObject){
            SetComposerVariables(deadZoneDrag.x, deadZoneDrag.y, dampingDrag.x, dampingDrag.y);
        } else SetComposerVariables(deadZoneTarget.x, deadZoneTarget.y, dampingTarget.x, dampingTarget.y);
        
        cvc.Follow = _t;
        cvc.LookAt = _t;
    }

    private void SetComposerVariables(float width, float height, float x, float y){
        comp.m_DeadZoneWidth = width;
        comp.m_DeadZoneHeight = height;
        comp.m_XDamping = x;
        comp.m_YDamping = y;
    }

    // ========== CAM CONTROLS =========
    public void SetCameraControlState(bool state){
        enableCamControl = state;
    }

    // ========== CAM SHAKE ============
    public void HitCamShake(){
        lastShakeId++;
        StartCoroutine(Shake(hitNoiseDuration, hitNoiseAmp, hitNoiseFreq, lastShakeId));
    }

    public void DeathCamShake(){
        lastShakeId++;
        StartCoroutine(Shake(deathNoiseDuration, deathNoiseAmp, deathNoiseFreq, lastShakeId));
    }

    private void StartPointingCamShake(){
        noise.m_AmplitudeGain = pointNoiseAmp;
        noise.m_FrequencyGain = pointNoiseFreq;
    }

    private void StopPointingCamShake(){
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
