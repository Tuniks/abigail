using UnityEngine;

public class ActiveTileCollisionHandler : MonoBehaviour {
    [Tooltip("Extra force applied to the first collided tile.")]
    public float collisionTransferForce = 100f; // ✅ ADD THIS!

    private bool hasCollided = false;

    void OnCollisionEnter2D(Collision2D collision) {
        if (hasCollided) return;

        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        if (otherTile != null) {
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (otherRb != null && rb != null && rb.velocity.sqrMagnitude > 0.001f) {
                Vector2 velocityDir = rb.velocity.normalized;
                Vector2 forceToApply = velocityDir * collisionTransferForce;

                otherRb.AddForce(forceToApply);
                rb.AddForce(-0.5f * forceToApply);
            }
        }

        hasCollided = true;
    }
}