using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesButtonClickHandler : MonoBehaviour
{
    public GameObject EvaluationObjects;  // Reference to the EvaluationObjects GameObject
    public GameObject LoseMessage;

    // This method will be called when the "NO" GameObject is clicked.
    void OnMouseDown()
    {
        // Ensure the EvaluationObjects are activated and the game state is changed to REEVALUATION
        if (EvaluationObjects != null)
        {
            EvaluationObjects.SetActive(false);  // Activate the EvaluationObjects
            LoseMessage.SetActive(true);
            Debug.Log("Evaluation Objects are now inactive.");
        }
    }
}