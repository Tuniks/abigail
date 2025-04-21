using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntedPlayer : MonoBehaviour{
    public Transform pivot;
    
    public Vector2 movementAngleRange = Vector2.zero;
    public float movementSpeed = 5f;
    
    private float rot = 0;

    void Start(){
        
    }

    void Update(){
        // Clamped Rotating Around
        float rotDelta = -Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float newRot = Mathf.Clamp(rot + rotDelta, movementAngleRange.x, movementAngleRange.y);
        rotDelta = newRot - rot;
        transform.RotateAround(pivot.position, Vector3.up, rotDelta);
        rot += rotDelta;
        transform.rotation = Quaternion.Euler(0, rot, 0);

    }
}
