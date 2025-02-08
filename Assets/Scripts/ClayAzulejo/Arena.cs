using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    // Assign in the Inspector the GameObjects that contain the ActiveSpot components for the player side.
    public List<GameObject> playerActiveSpotObjects; // e.g. 3 slots for player active spots

    // Assign in the Inspector the GameObjects that contain the ClayEnemyActiveSpot components for the enemy side.
    public List<GameObject> enemyActiveSpotObjects;  // e.g. 3 slots for enemy active spots

    // The object that will rotate based on which side has more filled slots.
    public Transform rotationTarget;

    // Lerp speed for rotation.
    public float rotationSpeed = 5f;

    void Update()
    {
        UpdateRotation();
    }

    void UpdateRotation()
    {
        int playerFilled = 0;
        int enemyFilled = 0;

        // Count filled player active spots.
        foreach (GameObject go in playerActiveSpotObjects)
        {
            // Assumes each GameObject has an ActiveSpot component.
            ActiveSpot spot = go.GetComponent<ActiveSpot>();
            if (spot != null && spot.activeTile != null)
                playerFilled++;
        }

        // Count filled enemy active spots.
        foreach (GameObject go in enemyActiveSpotObjects)
        {
            // Assumes each GameObject has a ClayEnemyActiveSpot component.
            ClayEnemyActiveSpot spot = go.GetComponent<ClayEnemyActiveSpot>();
            if (spot != null && spot.activeTile != null)
                enemyFilled++;
        }

        // Determine the target rotation based on the filled counts.
        Quaternion targetRotation;
        if (playerFilled > enemyFilled)
        {
            // More player spots filled → rotate 180° (down).
            targetRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (enemyFilled > playerFilled)
        {
            // More enemy spots filled → rotate 0° (up).
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Equal counts → point left (-90°).
            targetRotation = Quaternion.Euler(0, 0, -90);
        }

        // Smoothly interpolate to the target rotation.
        if (rotationTarget != null)
            rotationTarget.localRotation = Quaternion.Lerp(rotationTarget.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
