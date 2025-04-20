using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerEnemy : MonoBehaviour{
    public Vector2 yRange = Vector2.zero;
    public Vector2 velRange = Vector2.zero;

    private float currentVel = 1;
    private bool isGoingUp = false;
    private int lapCount = 0;
    
    void Start(){
        ChangeDirection();
    }

    void Update(){
        if(isGoingUp){
            transform.position += Vector3.up * currentVel * Time.deltaTime;
            if(transform.position.y >= yRange.y) ChangeDirection();
        } else {
            transform.position -= Vector3.up * currentVel * Time.deltaTime;
            if(transform.position.y <= yRange.x) ChangeDirection();
        }
    }

    private void ChangeDirection(){
        isGoingUp = !isGoingUp;
        lapCount--;

        if(lapCount <= 0){
            lapCount = Random.Range(1, 5);
            currentVel = Random.Range(velRange.x, velRange.y);
            Debug.Log(currentVel);
        }
    }
}
