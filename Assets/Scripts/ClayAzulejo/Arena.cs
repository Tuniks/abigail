using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    // Assign in the Inspector the GameObjects that hold the player active spots.
    public List<GameObject> playerActiveSpotObjects;  // e.g. 3 slots for player active spots

    // Assign in the Inspector the GameObjects that hold the enemy active spots.
    public List<GameObject> enemyActiveSpotObjects;   // e.g. 3 slots for enemy active spots

    // The object that will rotate based on attribute evaluation.
    public Transform rotationTarget;

    // Lerp speed for rotation.
    public float rotationSpeed = 5f;

    // Booleans to select which attributes to evaluate.
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

        // Sum selected attributes for player active spots.
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

        // Sum selected attributes for enemy active spots.
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

        // Determine target rotation based on which side has a higher total.
        Quaternion targetRotation;
        if (playerTotal > enemyTotal)
        {
            // More player quality → rotate to 180° (down)
            targetRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (enemyTotal > playerTotal)
        {
            // More enemy quality → rotate to 0° (up)
            targetRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Equal totals → point left (-90°)
            targetRotation = Quaternion.Euler(0, 0, -90);
        }

        if (rotationTarget != null)
            rotationTarget.localRotation = Quaternion.Lerp(rotationTarget.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // Returns the winner for this arena:
    //   - Returns -1 if player's total is higher.
    //   - Returns  1 if enemy's total is higher.
    //   - Returns  0 if tied.
    public int GetWinner() {
        float playerTotal = 0f;
        float enemyTotal = 0f;

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

        if(playerTotal > enemyTotal)
            return -1;
        else if(enemyTotal > playerTotal)
            return 1;
        else
            return 0;
    }
}
