using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyAzulejoManager : MonoBehaviour
{
    // Public references
    public GameObject buttonOption1;
    public GameObject buttonOption2;
    public TextMeshProUGUI displayText; // Drag your TextMeshPro UI object here

    // Text for each option
    private string option1Text = "Option 1 text";
    private string option2Text = "Option 2 text";

    // Handles button clicks
    public void OnOption1Click()
    {
        HandleButtonClick(option1Text);
    }

    public void OnOption2Click()
    {
        HandleButtonClick(option2Text);
    }

    // Manages the behavior after a button click
    private void HandleButtonClick(string selectedText)
    {
        // Hide the buttons
        buttonOption1.SetActive(false);
        buttonOption2.SetActive(false);

        // Update the display text
        displayText.text = selectedText;
    }
}
