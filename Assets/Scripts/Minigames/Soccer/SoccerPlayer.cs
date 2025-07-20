using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoccerPlayer : MonoBehaviour{
    [Header("Game Variables")]
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

    [Header("Sounds")]
    public AudioClip resetSound;
    public AudioClip launchBallSound;
    public AudioClip goalSound;
    
    [Header("Tiles")]
    public Tile[] stickyhandTile;

    public bool Tilegiven = false;
    
    [Header("Automatic Reset")]
    private bool isInGoalBox = false;
    private float goalBoxTimer = 0f;
    public float stillVelocityThreshold = 0.5f;
    public float waitTime = 3f;
    

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        ResetGame();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            PlayerInteractor.instance.GetAudioSource().PlayOneShot(resetSound);
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
        
        if(isInGoalBox)
        {
            float currentSpeed = rb.velocity.magnitude;

            if (currentSpeed <= stillVelocityThreshold)
            {
                goalBoxTimer += Time.deltaTime;

                if (goalBoxTimer >= waitTime)
                {
                    ResetTile(false);
                    isInGoalBox = false; 
                }
            }
            else
            {
                goalBoxTimer = 0f;
            }
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
        
        PlayerInteractor.instance.GetAudioSource().PlayOneShot(launchBallSound);
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
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Target"))
        {
            if (myScore == 2 && !Tilegiven)
            {
                PlayerInventory.Instance.AddTilesToCollection(stickyhandTile);
                Tilegiven = true;
            }
            PlayerInteractor.instance.GetAudioSource().PlayOneShot(goalSound);
            ResetTile(true);
        }

        if (col.gameObject.CompareTag("OutofBounds"))
        {
            ResetTile(false);
        }

        if (col.gameObject.CompareTag("goalbox"))
        {
            isInGoalBox = true;
            goalBoxTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("goalbox"))
        {
            isInGoalBox = false;
            goalBoxTimer = 0f;
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
