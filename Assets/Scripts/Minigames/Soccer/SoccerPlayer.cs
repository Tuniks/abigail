using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoccerPlayer : MonoBehaviour{
    public Camera cam;
    public Transform pointer;
    public Slider slider;
    public TextMeshProUGUI score;

    public Vector3 initialPos = Vector3.zero;
    public float maxPointerLength = 2f;
    public float maxForce = 5f;
    public float minForce = 3f;
    public float chargePeriod = 0.5f;

    private Rigidbody2D rb;
    
    private float currentCharge = 0;
    private bool hasLaunched = false;

    private int myScore = 0;
    private int opponentScore = 0;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        ResetGame();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            ResetTile();
        }

        UpdatePointer();

        if(Input.GetMouseButton(0) && !hasLaunched){
            slider.gameObject.SetActive(true);
            pointer.gameObject.SetActive(true);

            currentCharge = 0.5f * (1 + Mathf.Sin(2 * Mathf.PI * 1/chargePeriod * Time.time));
            slider.value = currentCharge;
        } else {
            slider.gameObject.SetActive(false);
            pointer.gameObject.SetActive(false);
        }

        if(Input.GetMouseButtonUp(0) && !hasLaunched){
            Launch();
        }
    }

    private void UpdatePointer(){
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = cam.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        Vector3 distVector = newPos-transform.position;
        if(distVector.magnitude > maxPointerLength){
            newPos = (distVector.normalized * maxPointerLength)+transform.position;
        }

        pointer.position = newPos;
        pointer.rotation = Quaternion.Euler(0, 0, Vector3.SignedAngle(Vector3.up, distVector, Vector3.forward));
    }

    private void Launch(){
        hasLaunched = true;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 newPos = cam.ScreenToWorldPoint(mousePos);
        newPos.z = 0;

        Vector3 distVector = newPos-transform.position;

        rb.AddForce(distVector.normalized * ((currentCharge * maxForce)+minForce), ForceMode2D.Impulse);
    }

    private void ResetTile(bool _score = false){
        transform.localPosition = initialPos;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;

        if(_score){
            myScore++;
        } else if (hasLaunched) opponentScore++;

        score.text = myScore.ToString() + " - " + opponentScore.ToString();

        hasLaunched = false;
    }

    private void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.CompareTag("Target")){
            ResetTile(true);
        }
    }

    public void ResetGame(){
        rb = GetComponent<Rigidbody2D>();
        myScore = 0;
        opponentScore = 0;
        score.text = myScore.ToString() + " - " + opponentScore.ToString();
        transform.localPosition = initialPos;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }
}
