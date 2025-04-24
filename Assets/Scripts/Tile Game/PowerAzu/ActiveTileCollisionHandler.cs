using UnityEngine;
using System.Collections.Generic;


public class ActiveTileCollisionHandler : MonoBehaviour {
    private Rigidbody2D rb;
    private HashSet<GameObject> hitThisLaunch = new HashSet<GameObject>();
    private float stopVelocityThreshold = 0.05f;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (rb != null && rb.velocity.magnitude < stopVelocityThreshold) {
            hitThisLaunch.Clear();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (rb == null || rb.velocity.sqrMagnitude < 0.1f)
            return;

        GameObject other = collision.gameObject;
        if (hitThisLaunch.Contains(other))
            return;

        Tile otherTile = other.GetComponent<Tile>();
        if (otherTile != null && otherTile.gameObject != this.gameObject) {
            Rigidbody2D otherRb = otherTile.GetComponent<Rigidbody2D>();
            if (otherRb != null) {
                Vector2 velocityDir = rb.velocity.normalized;
                Vector2 forceToApply = velocityDir * PowerAzuManager.instance.transferForce;

                otherRb.AddForce(forceToApply, ForceMode2D.Impulse);
                rb.AddForce(-0.5f * forceToApply, ForceMode2D.Impulse);

                Debug.Log($"[CollisionHandler] Hit {otherTile.name}, force applied: {forceToApply}");
                hitThisLaunch.Add(other);
            }
        }
    }
}