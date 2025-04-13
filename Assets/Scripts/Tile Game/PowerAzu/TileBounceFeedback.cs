using UnityEngine;

public class TileBounceFeedback : MonoBehaviour {
    public GameObject flashEffect;
    public GameObject bounceParticles;

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.relativeVelocity.magnitude > 0.5f) {
            if (flashEffect != null) {
                Instantiate(flashEffect, transform.position, Quaternion.identity);
            }

            if (bounceParticles != null) {
                Instantiate(bounceParticles, transform.position, Quaternion.identity);
            }
        }
    }
}