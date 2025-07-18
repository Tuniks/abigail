using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class PowerTilePhysics : MonoBehaviour{
    // Tile References
    private Rigidbody2D rb;
    private PowerTile powerTile;

    // Game References
    private PowerCameraManager pmm;
    private PowerSumoGame game;

    [Header("Tile Attributes")]
    public float maxForce = 5f;

    [Header("Pointer")]
    public Transform pointer;
    public float maxPointerLength = 2f;
    public float minPointerLength = .25f;

    private bool isAiming = false;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        powerTile = GetComponent<PowerTile>();
        pmm = PowerCameraManager.Instance;
        game = PowerManager.Instance.GetPowerSumoGame();
    }

    void Update(){
        if(isAiming){
            UpdatePointer();
            
            if(Input.GetMouseButtonUp(0)){
                LaunchPlayer();
            }
        }
    }

    private void OnMouseDown(){
        if(!powerTile.isPlayerTile) return;
        if(isAiming) return;

        if(!game.CanPlayerMove(this.gameObject)) return;

        pointer.gameObject.SetActive(true);
        isAiming = true;
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

    private void LaunchPlayer(){
        pointer.gameObject.SetActive(false);
        isAiming = false;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        Vector3 distVector = newPos-transform.position;

        rb.AddForce(distVector.normalized * ((distVector.magnitude/maxPointerLength) * maxForce), ForceMode2D.Impulse);

        game.MovePlayer(this.gameObject);
    }

    public void LaunchInDirection(Vector3 dir, float forcePercentage){
        rb.AddForce(dir.normalized * forcePercentage * maxForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D col){
        if(col.CompareTag("PowerHole")){
            powerTile.Die();
        }
    }

}
