using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    private CharacterController controller;
    
    public float movementSpeed = 5f;
    public float rotationSpeed = 40f;

    void Start(){
        controller = GetComponent<CharacterController>();
    }

    void Update(){
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        controller.SimpleMove(transform.forward * y * movementSpeed);
        transform.Rotate(transform.up, rotationSpeed * x * Time.deltaTime);
    }
}
