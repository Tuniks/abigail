using UnityEngine;

public class EnemyTileTargetTracker : MonoBehaviour {
    private bool touchingTarget = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<TargetZone>()) {
            touchingTarget = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<TargetZone>()) {
            touchingTarget = false;
        }
    }

    public bool IsTouchingTarget() {
        return touchingTarget;
    }
}