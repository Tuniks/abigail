using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour{
    public CanvasGroup cg;
    
    private bool isFadingIn = false;
    private bool isFadingOut = false;

    public IEnumerator FadeIn(float duration){
        cg.alpha = 0f;
        cg.gameObject.SetActive(true);
        yield return Fade(0, 1, duration);
    }

    public IEnumerator FadeOut(float duration){
        cg.alpha = 1.0f;
        cg.gameObject.SetActive(true);
        yield return Fade(1, 0, duration);
        cg.gameObject.SetActive(false);
    }

    public IEnumerator FadeInAndOut(float duration, float pause){
        cg.alpha = 0f;
        cg.gameObject.SetActive(true);
        yield return Fade(0, 1, duration);
        yield return new WaitForSeconds(pause);
        yield return Fade(1, 0, duration);
        cg.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float start, float end, float duration){
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1){
            elapsedPercentage = elapsedTime / duration;
            cg.alpha = Mathf.Lerp(start, end, elapsedPercentage);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }
}
