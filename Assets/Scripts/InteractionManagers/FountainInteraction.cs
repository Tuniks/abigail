using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class FountainInteraction : MonoBehaviour
{
    public List<GameObject> clockColliders; // List of clocks
    public int collisionCount = 0;           // Count of collisions
    public bool twelveClocksHit = false;    // Boolean for 12 collisions
    public bool counting = false;           // Boolean to control whether collisions are counted

    private int currentClockIndex = 0; // Index to keep track of the next active clock
    
    [Header("Interaction Tiles")] public Tile[] tile = null;

    private void Start()
    {
        // Make sure all clocks are active at the start
        foreach (GameObject clock in clockColliders)
        {
            clock.SetActive(true);
        }
    }
    
    [YarnCommand]
    public void FountainCounting()
    {
        counting = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If 12 clocks have been hit or counting is false, stop further processing
        if (twelveClocksHit || !counting)
            return;

        // Check if the collided object is in the clockColliders list
        if (clockColliders.Contains(other.gameObject))
        {
            // Increment collision count and print it for debugging
            collisionCount++;
            Debug.Log("Collision count: " + collisionCount);

            // Turn off all clocks except the next one in the sequence
            TurnOffClocksExceptNext();

            // Move to the next clock in the list
            currentClockIndex++;

            // If we've reached the end of the list, loop back to the start
            if (currentClockIndex >= clockColliders.Count)
            {
                currentClockIndex = 0;
            }

            // If 12 clocks have been hit, set the boolean to true
            if (collisionCount >= 12)
            {
                twelveClocksHit = true;
                PlayerInventory.Instance.AddTilesToCollection(tile);
                Debug.Log("12 clocks hit! Stopping further counting.");
            }
        }
    }

    private void TurnOffClocksExceptNext()
    {
        // Deactivate all clocks except the next one in the sequence
        for (int i = 0; i < clockColliders.Count; i++)
        {
            // If we're not on the current clock in the sequence, turn it off
            if (i != currentClockIndex)
            {
                clockColliders[i].SetActive(false);
            }
        }

        // Activate the next clock in the sequence
        clockColliders[currentClockIndex].SetActive(true);
    }
}
