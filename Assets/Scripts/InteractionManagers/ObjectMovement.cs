using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float pushForce = 5f;           
    public float slowdownRate = 5f;        
    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    public AudioClip BallKickedSound;
    public AudioSource audioSource;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found on " + gameObject.name);
            enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            currentVelocity = direction * pushForce;
            audioSource.PlayOneShot(BallKickedSound, 0.7F);
        }
    }

    void FixedUpdate()
    {
        currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, slowdownRate * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}


