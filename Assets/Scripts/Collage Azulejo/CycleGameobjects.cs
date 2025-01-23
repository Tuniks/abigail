using UnityEngine;

public class CycleGameobjects : MonoBehaviour
{
    [Header("GameObject Sets")]
    public GameObject[] set1; // First set of GameObjects
    public GameObject[] set2; // Second set of GameObjects
    public GameObject[] set3; // Third set of GameObjects

    private int currentSetIndex = 0; // Tracks the active set

    void Update()
    {
        // Check for the "C" key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            CycleSets();
        }
    }

    private void CycleSets()
    {
        // Turn off the current set
        DeactivateSet(currentSetIndex);

        // Update the current set index to the next one (loop back to 0 after the last set)
        currentSetIndex = (currentSetIndex + 1) % 3;

        // Turn on the next set
        ActivateSet(currentSetIndex);
    }

    private void ActivateSet(int setIndex)
    {
        GameObject[] setToActivate = GetSetByIndex(setIndex);
        foreach (GameObject obj in setToActivate)
        {
            obj.SetActive(true);
        }
    }

    private void DeactivateSet(int setIndex)
    {
        GameObject[] setToDeactivate = GetSetByIndex(setIndex);
        foreach (GameObject obj in setToDeactivate)
        {
            obj.SetActive(false);
        }
    }

    private GameObject[] GetSetByIndex(int setIndex)
    {
        switch (setIndex)
        {
            case 0: return set1;
            case 1: return set2;
            case 2: return set3;
            default: return null;
        }
    }
}

