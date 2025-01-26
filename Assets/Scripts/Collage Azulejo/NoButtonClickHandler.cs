using UnityEngine;

public class NoButtonClickHandler : MonoBehaviour
{
    public GameObject EvaluationObjects;  // Reference to the EvaluationObjects GameObject
    public CollageAzulejoStates collageAzulejoStates;
    public GameObject Instrution;

    // This method will be called when the "NO" GameObject is clicked.
    void OnMouseDown()
    {
        // Ensure the EvaluationObjects are activated and the game state is changed to REEVALUATION
        if (EvaluationObjects != null)
        {
            EvaluationObjects.SetActive(false);  // Activate the EvaluationObjects
            Instrution.SetActive(true);
            Debug.Log("Evaluation Objects are now inactive.");
        }

        if (collageAzulejoStates != null)
        {
            collageAzulejoStates.SetState(CollageAzulejoStates.GameState.REEVALUATION);  // Change the game state to REEVALUATION
            Debug.Log("Game state changed to REEVALUATION.");
        }
    }
}