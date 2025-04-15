using UnityEngine;

public class ActiveTileCollisionHandler : MonoBehaviour {
    [Tooltip("Extra force applied to the first collided tile.")]
    public float collisionTransferForce = 500f; // Exposed variable.
    
    private bool hasCollided = false;
    
    void OnCollisionEnter2D(Collision2D collision) {
        if (hasCollided)
            return;

        Debug.Log($"[CollisionHandler] Collision detected between {gameObject.name} and {collision.gameObject.name}.");
        
        // Try to get the Tile component from the collided object.
        Tile otherTile = collision.gameObject.GetComponent<Tile>();
        if (otherTile != null) {
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (otherRb != null && rb != null && rb.velocity.sqrMagnitude > 0.001f) {
                // Determine the force direction based on the active tile's velocity.
                Vector2 velocityDir = rb.velocity.normalized;
                
                // If the tile is almost traveling straight up, enforce global up.
                if (Vector2.Dot(velocityDir, Vector2.up) > 0.95f)
                    velocityDir = Vector2.up;
                
                // Calculate the force to apply.
                Vector2 forceToApply = velocityDir * collisionTransferForce;
                
                // Apply the full force to the collided tile.
                otherRb.AddForce(forceToApply);
                
                // Apply half of the force in the opposite direction to the active tile for a bounce effect.
                rb.AddForce(-0.5f * forceToApply);
                
                Debug.Log($"[CollisionHandler] Applied force {forceToApply} to {collision.gameObject.name} and bounce force {-0.5f * forceToApply} to {gameObject.name}.");
            }
        }
        
        hasCollided = true;
    }
}