using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RollCredits : MonoBehaviour
{
    public RectTransform creditsText;   // Assign the RectTransform of your credits text
    public float scrollSpeed = 30f;     // Speed of scrolling
    public GameObject Test;
    public AudioSource backgroundMusic; // Assign the music AudioSource
    public float fadeDuration = 2f;     // How long to fade out the music

    public float endYPosition = 1200f;  // Set this to the Y position at which credits are fully offscreen

    private bool isScrolling = true;
    private bool hasStopped = false;

    void Update()
    {
        if (isScrolling)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (creditsText.anchoredPosition.y >= endYPosition)
            {
                StopCredits();
            }
        }
        else if (!hasStopped)
        {
            hasStopped = true;
            Test.SetActive(true);
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    public void StopCredits()
    {
        isScrolling = false;
    }

    IEnumerator FadeOutAndLoadScene()
    {
        float startVolume = backgroundMusic.volume;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        backgroundMusic.Stop();
        SceneManager.LoadScene("START");
    }
}