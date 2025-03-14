using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    private Rigidbody2D rb;
    
    public float movementSpeed = 5f;

    private Vector3 dir = Vector3.zero;
    private bool isBusy = false;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical") , 0);
        if(isBusy) dir = Vector3.zero;
    }

    void FixedUpdate(){
        rb.velocity = Vector3.zero;
        rb.MovePosition(transform.position + (dir.normalized * movementSpeed * Time.fixedDeltaTime));
    }

    public Vector2 GetDirection(){
        return new Vector2(dir.x, dir.y);
    }

    public void SetIsBusy(bool _isBusy){
        isBusy = _isBusy;
    }
}
