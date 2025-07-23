using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerAzulejoStarter : MonoBehaviour
{
    public string startScene = "PWR_oz";
    public float transitionDelay = 1.5f;
    public Image fadeImage; // assign in inspector
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        // Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Optional delay after fade
        yield return new WaitForSeconds(transitionDelay);

        // Load new scene
        SceneManager.LoadScene(startScene);
    }

    IEnumerator FadeToBlack()
    {
        float t = 0f;
        Color c = fadeImage.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }
}