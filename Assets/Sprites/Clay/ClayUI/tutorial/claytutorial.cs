using System.Collections.Generic;
using UnityEngine;

public class UITutorialSlideShow : MonoBehaviour
{
    // List of UI GameObjects that act as slides. Assign these in the Inspector.
    public List<GameObject> uiSlides;
    
    // An additional GameObject to enable once the final slide is deactivated.
    public GameObject finalGameObject;
    
    // When true, the tutorial UI slides are skipped (all disabled) and finalGameObject starts enabled.
    public bool SkipTutorial;

    // currentIndex points to the currently active slide.
    // If currentIndex equals uiSlides.Count, no slide is active.
    private int currentIndex = 0;

    void Start()
    {
        // If we're skipping the tutorial...
        if (SkipTutorial)
        {
            // Disable all tutorial UI slides.
            foreach (GameObject slide in uiSlides)
            {
                slide.SetActive(false);
            }
            // Enable the final game object.
            if (finalGameObject != null)
            {
                finalGameObject.SetActive(true);
            }
            // Disable this script to prevent further input handling.
            enabled = false;
            return;
        }

        // Normal tutorial initialization:
        // Disable the final game object.
        if (finalGameObject != null)
        {
            finalGameObject.SetActive(false);
        }
        // Activate only the first slide and disable the rest.
        for (int i = 0; i < uiSlides.Count; i++)
        {
            uiSlides[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        // If the tutorial is skipped, do nothing.
        if (SkipTutorial)
        {
            return;
        }

        // Left click or E key goes to the next slide.
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            ShowNextSlide();
        }

        // Right click takes it a step back.
        if (Input.GetMouseButtonDown(1))
        {
            ShowPreviousSlide();
        }
    }

    void ShowNextSlide()
    {
        // If there are more slides ahead...
        if (currentIndex < uiSlides.Count - 1)
        {
            uiSlides[currentIndex].SetActive(false);
            currentIndex++;
            uiSlides[currentIndex].SetActive(true);
        }
        // If on the final slide, disable it and enable the finalGameObject.
        else if (currentIndex == uiSlides.Count - 1)
        {
            uiSlides[currentIndex].SetActive(false);
            if (finalGameObject != null)
            {
                finalGameObject.SetActive(true);
            }
            currentIndex = uiSlides.Count;  // Indicates no UI slide is active.
        }
    }

    void ShowPreviousSlide()
    {
        // If in the "no slide active" state, disable the finalGameObject and re-enable the final slide.
        if (currentIndex == uiSlides.Count)
        {
            if (finalGameObject != null)
            {
                finalGameObject.SetActive(false);
            }
            currentIndex = uiSlides.Count - 1;
            uiSlides[currentIndex].SetActive(true);
        }
        // Otherwise, if we're not at the first slide, step back.
        else if (currentIndex > 0)
        {
            uiSlides[currentIndex].SetActive(false);
            currentIndex--;
            uiSlides[currentIndex].SetActive(true);
        }
    }
}
