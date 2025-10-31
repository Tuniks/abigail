using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    private Rigidbody rb;
    private PlayerStatus ps;
    
    public float movementSpeed = 5f;

    private Vector3 dir = Vector3.zero;

    void Start(){
        rb = GetComponent<Rigidbody>();
        ps = PlayerStatus.Instance;
    }

    void Update(){
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical") , 0);
        if(ps.CanMove()) dir = Vector3.zero;
    }

    void FixedUpdate(){
        rb.velocity = Vector3.zero;
        rb.MovePosition(transform.position + (dir.normalized * movementSpeed * Time.fixedDeltaTime));
    }

    public Vector2 GetDirection(){
        return new Vector2(dir.x, dir.y);
    }
}
