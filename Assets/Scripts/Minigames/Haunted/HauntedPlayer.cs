using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HauntedPlayer : MonoBehaviour{
    [Header("Object References")]
    public Camera cam;
    public Transform pivot;
    public Transform backdrop;
    public GameObject rock;
    public Slider slider;

    [Header("Movement")]
    public Vector2 movementAngleRange = Vector2.zero;
    public float movementSpeed = 5f;

    private float rot = 0;

    [Header("Rock")]
    public float maxForce = 5f;
    public float minForce = 3f;
    public float chargePeriod = 0.5f;
    public LayerMask layer;

    private float currentCharge = 0;
    private bool canLaunch = true;

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

        // Rotating Backdrop for Aim
        backdrop.rotation = Quaternion.Euler(0, rot, 0);

        // Shooting
        if(Input.GetMouseButton(0) && canLaunch){
            slider.gameObject.SetActive(true);

            currentCharge = 0.5f * (1 + Mathf.Sin(2 * Mathf.PI * 1/chargePeriod * Time.time));
            slider.value = currentCharge;
        } else {
            slider.gameObject.SetActive(false);
        }

        if(Input.GetMouseButtonUp(0) && canLaunch){
            Launch();
        }
    }

    private void Launch(){
        canLaunch = false;
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPos = Vector3.zero;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer)){
            targetPos = hit.point;
        }

        GameObject newRock = Instantiate(rock, transform.position, Quaternion.identity);
        Rigidbody rb = newRock.GetComponent<Rigidbody>();
        
        Vector3 direction = targetPos - transform.position;
        rb.AddForce(direction.normalized * (minForce + (maxForce * currentCharge)), ForceMode.Impulse);

        StartCoroutine(DestroyRock(newRock));
        StartCoroutine(ResetLaunch());
    }

    private IEnumerator DestroyRock(GameObject _rock){
        yield return new WaitForSeconds(10);
        Destroy(_rock);
    }

    private IEnumerator ResetLaunch(){
        yield return new WaitForSeconds(0.5f);
        canLaunch = true;
    }
}
