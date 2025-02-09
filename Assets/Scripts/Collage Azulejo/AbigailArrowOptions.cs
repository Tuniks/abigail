using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbigailArrowOptions : MonoBehaviour
{
    // Public GameObjects
    public GameObject arrow; // The arrow GameObject
    public GameObject Claw;
    public GameObject Garden;
    public GameObject ClawManager;
    public GameObject GardenManager;

    // Variables to keep track of the current selection
    private int selection = 0; // 0 for Claw, 1 for Garden
    private GameObject currentSelection;

    // Movement speed for the arrow
    public float arrowSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Initially, disable both managers
        ClawManager.SetActive(false);
        GardenManager.SetActive(false);
        arrow.SetActive(true);

        // Set initial selection to Claw
        currentSelection = Claw;
        SetActiveManager(); // Set the appropriate manager active
        UpdateArrowPosition(); // Set arrow to initial selection position
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement and selection with WASD/Arrow keys and Enter
        HandleArrowMovement();

        // When the player presses Enter or E, confirm selection
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            SelectCurrentOption();
        }
    }

    // Handle the arrow's movement with WASD or Arrow keys
    void HandleArrowMovement()
    {
        // Cycle between Claw and Garden with Up/Down or W/S keys
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selection = 0; // Select Claw
            UpdateArrowPosition(); // Update arrow to Claw position
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selection = 1; // Select Garden
            UpdateArrowPosition(); // Update arrow to Garden position
        }
    }

    // Update the arrow's position based on current selection
    void UpdateArrowPosition()
    {
        if (selection == 0) // Claw selected
        {
            arrow.transform.position = Claw.transform.position; // Align the arrow with Claw
        }
        else if (selection == 1) // Garden selected
        {
            arrow.transform.position = Garden.transform.position; // Align the arrow with Garden
        }
    }

    // Set the active manager based on the current selection
    void SetActiveManager()
    {
        if (selection == 0) // Claw selected
        {
            ClawManager.SetActive(true);
            GardenManager.SetActive(false);
        }
        else if (selection == 1) // Garden selected
        {
            ClawManager.SetActive(false);
            GardenManager.SetActive(true);
        }
    }

    // Confirm the current selection
    void SelectCurrentOption()
    {
        Debug.Log("Selected: " + currentSelection.name);

        if (selection == 0) // Claw selected
        {
            currentSelection = Claw;
        }
        else if (selection == 1) // Garden selected
        {
            currentSelection = Garden;
        }

        // Set the appropriate manager active
        SetActiveManager();
        arrow.SetActive(false);
        Garden.SetActive(false);
        Claw.SetActive(false);
    }
}
