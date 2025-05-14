using UnityEngine;
using TMPro;
using System.Collections;

public class FadeTMP : MonoBehaviour
{
    // Reference to the TextMeshPro component.
    public TextMeshProUGUI tmpText;

    // Duration for each fade (in seconds).
    public float fadeDuration = 1.0f;

    // Time to wait once a fade completes.
    public float waitDuration = 0.5f;

    private void Start()
    {
        // If the TMP reference is not set, try getting the component from the same GameObject.
        if (tmpText == null)
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        // Start the fade in/out loop.
        StartCoroutine(FadeLoop());
    }

    // Coroutine that loops indefinitely for fading in and out.
    private IEnumerator FadeLoop()
    {
        while (true)
        {
            // Fade in: from 0 (transparent) to 1 (opaque)
            yield return StartCoroutine(Fade(0f, 1f));
            yield return new WaitForSeconds(waitDuration);

            // Fade out: from 1 (opaque) to 0 (transparent)
            yield return StartCoroutine(Fade(1f, 0f));
            yield return new WaitForSeconds(waitDuration);
        }
    }

    // Coroutine to fade between specified alpha values.
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        // Capture the original color of the text.
        Color originalColor = tmpText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            // Interpolate the alpha value.
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);

            // Update the TMP text color with the new alpha.
            tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

            yield return null;
        }

        // Ensure the final alpha value is set.
        tmpText.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }
}
