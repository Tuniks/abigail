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
        // Tank Controls
        // float x = Input.GetAxis("Horizontal");
        // float y = Input.GetAxis("Vertical");

        // controller.SimpleMove(transform.forward * y * movementSpeed);
        // transform.Rotate(transform.up, rotationSpeed * x * Time.deltaTime);

        // Normal Controls

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 camFor = Camera.main.transform.forward;
        camFor.y = 0;
        camFor.Normalize();
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 dir = camFor * y + camRight * x;

        controller.SimpleMove(dir * movementSpeed);
    }
}
