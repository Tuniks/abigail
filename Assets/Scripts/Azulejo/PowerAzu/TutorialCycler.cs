using UnityEngine;
using UnityEngine.UI;

public class TutorialCycler : MonoBehaviour {
    public GameObject[] tutorialScreens; // Assign UI elements in order
    public GameObject objectToEnableAfterTutorial; // Assign what to enable after last screen

    private int currentScreen = 0;
    private bool tutorialDone = false;

    void Start() {
        // Hide all screens initially
        foreach (GameObject screen in tutorialScreens) {
            screen.SetActive(false);
        }

        // Show the first screen if available
        if (tutorialScreens.Length > 0) {
            tutorialScreens[0].SetActive(true);
        }
    }

    void Update() {
        if (tutorialDone) return;

        if (Input.GetKeyDown(KeyCode.E)) {
            // Hide current screen
            if (currentScreen < tutorialScreens.Length) {
                tutorialScreens[currentScreen].SetActive(false);
            }

            currentScreen++;

            if (currentScreen < tutorialScreens.Length) {
                tutorialScreens[currentScreen].SetActive(true);
            } else {
                // Tutorial finished
                tutorialDone = true;
                if (objectToEnableAfterTutorial != null) {
                    objectToEnableAfterTutorial.SetActive(true);
                }
            }
        }
    }
}