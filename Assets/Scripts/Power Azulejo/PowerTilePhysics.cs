using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.WSA;

public class PowerTilePhysics : MonoBehaviour{
    private Rigidbody2D rb;
    private PowerCameraManager pmm;
    private PowerSumoGame game;

    [Header("Tile Attributes")]
    public bool isPlayerTile = true;
    public float maxForce = 5f;

    [Header("Pointer")]
    public Transform pointer;
    public float maxPointerLength = 2f;
    public float minPointerLength = .25f;

    private bool isAiming = false;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        pmm = PowerCameraManager.Instance;
        game = PowerManager.Instance.GetPowerSumoGame();
    }

    void Update(){
        if(isAiming){
            UpdatePointer();
            
            if(Input.GetMouseButtonUp(0)){
                Launch();
            }
        }
    }

    private void OnMouseDown(){
        if(!isPlayerTile) return;
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

    private void Launch(){
        pointer.gameObject.SetActive(false);
        isAiming = false;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = Camera.main.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        Vector3 distVector = newPos-transform.position;

        rb.AddForce(distVector.normalized * ((distVector.magnitude/maxPointerLength) * maxForce), ForceMode2D.Impulse);

        game.MovePlayer(this.gameObject);
    }

}
