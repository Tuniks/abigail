using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    // Assign in the Inspector the GameObjects that contain the ActiveSpot components for the player side.
    public List<GameObject> playerActiveSpotObjects; // e.g., 3 slots for player active spots

    // Assign in the Inspector the GameObjects that contain the ClayEnemyActiveSpot components for the enemy side.
    public List<GameObject> enemyActiveSpotObjects;  // e.g., 3 slots for enemy active spots

    // The object that will rotate based on the evaluation.
    public Transform rotationTarget;

    // Lerp speed for rotation.
    public float rotationSpeed = 5f;

    // Boolean checkboxes to select which attributes to evaluate.
    public bool useBeauty = false;
    public bool useVigor = false;
    public bool useMagic = false;
    public bool useHeart = false;
    public bool useIntellect = false;
    public bool useTerror = false;

    void Update()
    {
        UpdateRotation();
    }

    void UpdateRotation()
    {
        float playerTotal = 0f;
        float enemyTotal = 0f;

        // Sum selected attribute values from each player active spot.
        foreach (GameObject go in playerActiveSpotObjects)
        {
            ActiveSpot spot = go.GetComponent<ActiveSpot>();
            if (spot != null && spot.activeTile != null)
            {
                Tile tile = spot.activeTile;
                if (useBeauty)     playerTotal += tile.GetBeauty();
                if (useVigor)      playerTotal += tile.GetVigor();
                if (useMagic)      playerTotal += tile.GetMagic();
                if (useHeart)      playerTotal += tile.GetHeart();
                if (useIntellect)  playerTotal += tile.GetIntellect();
                if (useTerror)     playerTotal += tile.GetTerror();
            }
        }

        // Sum selected attribute values from each enemy active spot.
        foreach (GameObject go in enemyActiveSpotObjects)
        {
            ClayEnemyActiveSpot spot = go.GetComponent<ClayEnemyActiveSpot>();
            if (spot != null && spot.activeTile != null)
            {
                Tile tile = spot.activeTile;
                if (useBeauty)     enemyTotal += tile.GetBeauty();
                if (useVigor)      enemyTotal += tile.GetVigor();
                if (useMagic)      enemyTotal += tile.GetMagic();
                if (useHeart)      enemyTotal += tile.GetHeart();
                if (useIntellect)  enemyTotal += tile.GetIntellect();
                if (useTerror)     enemyTotal += tile.GetTerror();
            }
        }

        // Determine target rotation based on the totals.
        Quaternion targetRotation;
        if (playerTotal > enemyTotal)
        {
            // Player quality is higher → rotate to 180° (down).
            targetRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (enemyTotal > playerTotal)
        {
            // Enemy quality is higher → rotate to 0° (up).
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Totals are equal → rotate to -90° (left).
            targetRotation = Quaternion.Euler(0, 0, -90);
        }

        // Smoothly interpolate the rotationTarget's rotation toward the target rotation.
        if (rotationTarget != null)
            rotationTarget.localRotation = Quaternion.Lerp(rotationTarget.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
