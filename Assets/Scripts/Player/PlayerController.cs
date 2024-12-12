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
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical") , 0);
    }

    void FixedUpdate(){
        rb.MovePosition(transform.position + (dir.normalized * movementSpeed * Time.fixedDeltaTime));
    }

    public Vector2 GetDirection(){
        return new Vector2(dir.x, dir.y);
    }
}
