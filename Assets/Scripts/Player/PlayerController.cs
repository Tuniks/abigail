using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    private Rigidbody rb;
    
    public float movementSpeed = 5f;

    private Vector3 dir = Vector3.zero;

    void Start(){
        rb = GetComponent<Rigidbody>();
    }

    void Update(){
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0 , Input.GetAxisRaw("Vertical"));
        // rb.velocity = dir.normalized * movementSpeed * Time.deltaTime;
    }

    void FixedUpdate(){
        // rb.velocity = dir.normalized * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + (dir.normalized * movementSpeed * Time.fixedDeltaTime));
    }
}
