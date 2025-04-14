using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float pushForce = 5f;           // How strong the bump is
    public float slowdownRate = 5f;        // How quickly it comes to a stop
    private Rigidbody2D rb;
    private Vector2 currentVelocity;

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
        }
    }

    void FixedUpdate()
    {
        currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, slowdownRate * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }
}


