using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class PowerTilePhysics : MonoBehaviour{
    // Tile References
    private Rigidbody2D rb;
    private PowerTile powerTile;
    private PowerPowers powerPowers;

    // Game References
    private PowerCameraManager pmm;
    private PowerSumoGame game;

    // Power Targeting
    private GameObject targeter;
    private bool isTargeting;
    private Action<GameObject, Vector3> targetAction;

    [Header("Tile Attributes")]
    public float maxForce = 5f;

    [Header("Pointer")]
    private Transform pointer;
    public float maxPointerLength = 2f;
    public float minPointerLength = .25f;

    [Header("Juice")]
    private float freezeFrameDuration = 0.025f;
    private AudioSource audioSource;
    private AudioClip hitFriendClip;
    private AudioClip hitEnemyClip;

    private bool isAiming = false;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        powerTile = GetComponent<PowerTile>();
        powerPowers = GetComponent<PowerPowers>();
        pmm = PowerCameraManager.Instance;
        game = PowerManager.Instance.GetPowerSumoGame();

        audioSource = GetComponent<AudioSource>();
        hitFriendClip = PowerAudioVisualManager.Instance.GetHitFriendClip();
        hitEnemyClip = PowerAudioVisualManager.Instance.GetHitEnemyClip();

        pointer = PowerManager.Instance.GetPointer().transform;
        targeter = PowerManager.Instance.GetTargeter();
    }

    void Update(){
        if(isTargeting){
            UpdateTarget();

            if(Input.GetMouseButtonDown(0)){
                StopPowerTargeting();
            }
            return;
        }
        
        if(isAiming){
            UpdatePointer();
            
            if(Input.GetMouseButtonUp(0)){
                if(CheckMinLaunchDistance()){
                    LaunchPlayer();
                } else StopPointing();
            }
        }
    }

    // ==== PLAYER MOVEMENT ====
    private void OnMouseDown(){
        if(!powerTile.isPlayerTile) return;
        if(isAiming) return;

        if(!game.CanPlayerMove(this.gameObject)) return;

        StartPointing();
    }

    private void LaunchPlayer(){
        StopPointing();

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        Vector3 distVector = newPos-transform.position;

        rb.AddForce(distVector.normalized * ((distVector.magnitude/maxPointerLength) * maxForce), ForceMode2D.Impulse);

        game.MovePlayer(this.gameObject);
    }

    private bool CheckMinLaunchDistance(){
        // Check if inside power ui area
        if(powerPowers.IsInsideUI()) return false;

        // Check distance from centre to mouse
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;
        float dist = Vector3.Distance(newPos, transform.position);
        
        if(dist <= minPointerLength) return false;

        return true;
    }

    // ===== PROCEDURAL MOVEMENT =====
    public void LaunchInDirection(Vector3 dir, float forcePercentage){
        rb.AddForce(dir.normalized * forcePercentage * maxForce, ForceMode2D.Impulse);
    }

    // ===== POINTER BEHAVIOR =====
    private void StartPointing(){
        pointer.gameObject.SetActive(true);
        PowerCinemachine.Instance.SetTarget(transform);
        PowerCinemachine.Instance.StartPointingCamShake();
        isAiming = true;
    }

    private void StopPointing(){
        pointer.gameObject.SetActive(false);
        PowerCinemachine.Instance.StopPointingCamShake();
        isAiming = false;
    }

    private void UpdatePointer(){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        Vector3 distVector = newPos-transform.position;
        if(distVector.magnitude > maxPointerLength){
            newPos = (distVector.normalized * maxPointerLength)+transform.position;
        }

        pointer.position = newPos;
        pointer.rotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.up, distVector, Vector3.forward));
    }


    // ===== POWER =====
    private void OnMouseUpAsButton(){
        if(!powerTile.isPlayerTile) return;
        StopPointing();
        powerPowers.ToggleUI();
    }

    public void StartPowerTargeting(Action<GameObject, Vector3> power){
        isTargeting = true;
        targeter.SetActive(true);
        targetAction = power;
        powerPowers.ToggleUI();
    }

    private void StopPowerTargeting(){
        isTargeting = false;
        targeter.SetActive(false);

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        targetAction(gameObject, newPos);
        targetAction = null;
    }

    private void UpdateTarget(){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        targeter.transform.position = newPos;
    }

    // ===== DEATH =====
    private void OnTriggerEnter2D(Collider2D col){
        if(col.CompareTag("PowerHole")){
            powerTile.Die();
        }
    }

    // ===== ON COLLISION ======
    private void OnCollisionEnter2D(Collision2D collision){
        PowerTile other = collision.gameObject.GetComponent<PowerTile>();
        if(other == null) return;
        // To avoid doing everything twice, do only on the one w higher ID lol
        if(gameObject.GetInstanceID() < other.gameObject.GetInstanceID()) return;

        bool isFriend = other.isPlayerTile == powerTile.isPlayerTile;

        StartCoroutine(TriggerCollision(isFriend));
    }

    private IEnumerator TriggerCollision(bool isFriend){
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeFrameDuration);
        Time.timeScale = 1;
        AudioClip ac = isFriend ? hitFriendClip : hitEnemyClip;
        audioSource.PlayOneShot(ac);
        PowerCinemachine.Instance.HitCamShake();
    }

}
