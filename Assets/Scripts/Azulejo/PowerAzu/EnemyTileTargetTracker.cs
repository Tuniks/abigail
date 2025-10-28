// EnemyTileTargetTracker.cs
using System.Collections.Generic;
using UnityEngine;

public class EnemyTileTargetTracker : MonoBehaviour {
    private HashSet<Transform> currentTargets = new HashSet<Transform>();

    public void RegisterCurrentTarget(Transform target) {
        if (!currentTargets.Contains(target)) {
            currentTargets.Add(target);
        }
    }

    public void RemoveCurrentTarget(Transform target) {
        if (currentTargets.Contains(target)) {
            currentTargets.Remove(target);
        }
    }

    public bool IsTouchingTarget() {
        return currentTargets.Count > 0;
    }
}