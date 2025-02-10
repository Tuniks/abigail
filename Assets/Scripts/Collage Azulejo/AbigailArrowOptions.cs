using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbigailArrowOptions : MonoBehaviour
{
    // Enum for states
    private enum State { Choose, Leave }
    private State currentState = State.Choose;

    // Public GameObjects
    public GameObject arrow;         // The arrow sprite
    public GameObject option1;       // Option 1 (Claw)
    public GameObject option2;       // Option 2 (Garden)
    public GameObject option1Manager; // Manager for Option 1
    public GameObject option2Manager; // Manager for Option 2

    // Variables to track the current selection
    private int selection = 0; // 0 for Option 1, 1 for Option 2

    void Start()
    {
        // Ensure we start in the "Choose" state
        currentState = State.Choose;

        // Position the arrow at the first option
        arrow.transform.position = option1.transform.position;
    }

    void Update()
    {
        if (currentState == State.Choose)
        {
            HandleArrowMovement();
            if (Input.GetKeyDown(KeyCode.E))
            {
                ConfirmSelection();
            }
        }
    }

    // Handle arrow movement between options
    void HandleArrowMovement()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (selection != 0) // Move to Option 1
            {
                selection = 0;
                arrow.transform.position = option1.transform.position;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selection != 1) // Move to Option 2
            {
                selection = 1;
                arrow.transform.position = option2.transform.position;
            }
        }
    }

    // Confirm the current selection and switch to "Leave" state
    void ConfirmSelection()
    {
        currentState = State.Leave;

        // Disable selection visuals
        arrow.SetActive(false);
        option1.SetActive(false);
        option2.SetActive(false);

        // Activate the selected manager
        if (selection == 0)
        {
            option1Manager.SetActive(true);
        }
        else
        {
            option2Manager.SetActive(true);
        }
    }
}