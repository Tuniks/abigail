using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollCredits : MonoBehaviour
{
    public RectTransform creditsText;  // Assign the RectTransform of your credits text
    public float scrollSpeed = 30f;    // Speed of scrolling (adjust as needed)

    private bool isScrolling = true;

    void Update()
    {
        if (isScrolling)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }

    // Optional: stop scrolling after a delay or position
    public void StopCredits()
    {
        isScrolling = false;
    }
}
