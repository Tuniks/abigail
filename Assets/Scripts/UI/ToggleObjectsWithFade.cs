using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleObjectsWithFade : MonoBehaviour
{
    public List<GameObject> objectsToDisable;
    public List<GameObject> uiToEnable;
    public float fadeDuration = 1.0f;
    private bool isToggled = false;
    public GameObject PowerAzulejoSceneStarter;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isToggled = !isToggled;
            StartCoroutine(CrossfadeObjects(isToggled));
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PowerAzulejoSceneStarter.SetActive(true);
        }
    }

    private IEnumerator CrossfadeObjects(bool enableUI)
    {
        float elapsedTime = 0f;
        Dictionary<GameObject, float> startAlphas = new Dictionary<GameObject, float>();

        foreach (var obj in objectsToDisable)
        {
            if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                startAlphas[obj] = canvasGroup.alpha;
            else if (obj.TryGetComponent(out Graphic graphic))
                startAlphas[obj] = graphic.color.a;
            else if (obj.TryGetComponent(out Renderer renderer))
                startAlphas[obj] = renderer.material.color.a;
        }

        foreach (var obj in uiToEnable)
        {
            if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                startAlphas[obj] = canvasGroup.alpha;
            else if (obj.TryGetComponent(out Graphic graphic))
                startAlphas[obj] = graphic.color.a;
            else if (obj.TryGetComponent(out Renderer renderer))
                startAlphas[obj] = renderer.material.color.a;
        }

        foreach (var obj in uiToEnable)
        {
            obj.SetActive(true);
            if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                canvasGroup.alpha = 0f;
            else if (obj.TryGetComponent(out Graphic graphic))
            {
                Color color = graphic.color;
                color.a = 0f;
                graphic.color = color;
            }
            else if (obj.TryGetComponent(out Renderer renderer))
            {
                Color color = renderer.material.color;
                color.a = 0f;
                renderer.material.color = color;
            }
            startAlphas[obj] = 0f;
        }

        while (elapsedTime < fadeDuration)
        {
            float fadeRatio = elapsedTime / fadeDuration;

            foreach (var obj in objectsToDisable)
            {
                if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                    canvasGroup.alpha = Mathf.Lerp(startAlphas[obj], 0, fadeRatio);
                else if (obj.TryGetComponent(out Graphic graphic))
                {
                    Color color = graphic.color;
                    color.a = Mathf.Lerp(startAlphas[obj], 0, fadeRatio);
                    graphic.color = color;
                }
                else if (obj.TryGetComponent(out Renderer renderer))
                {
                    Color color = renderer.material.color;
                    color.a = Mathf.Lerp(startAlphas[obj], 0, fadeRatio);
                    renderer.material.color = color;
                }
            }

            foreach (var obj in uiToEnable)
            {
                if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                    canvasGroup.alpha = Mathf.Lerp(startAlphas[obj], 1, fadeRatio);
                else if (obj.TryGetComponent(out Graphic graphic))
                {
                    Color color = graphic.color;
                    color.a = Mathf.Lerp(startAlphas[obj], 1, fadeRatio);
                    graphic.color = color;
                }
                else if (obj.TryGetComponent(out Renderer renderer))
                {
                    Color color = renderer.material.color;
                    color.a = Mathf.Lerp(startAlphas[obj], 1, fadeRatio);
                    renderer.material.color = color;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var obj in objectsToDisable)
        {
            if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                canvasGroup.alpha = 0;
            else if (obj.TryGetComponent(out Graphic graphic))
            {
                Color color = graphic.color;
                color.a = 0;
                graphic.color = color;
            }
            else if (obj.TryGetComponent(out Renderer renderer))
            {
                Color color = renderer.material.color;
                color.a = 0;
                renderer.material.color = color;
            }
            obj.SetActive(!enableUI);
        }

        foreach (var obj in uiToEnable)
        {
            if (obj.TryGetComponent(out CanvasGroup canvasGroup))
                canvasGroup.alpha = 1;
            else if (obj.TryGetComponent(out Graphic graphic))
            {
                Color color = graphic.color;
                color.a = 1;
                graphic.color = color;
            }
            else if (obj.TryGetComponent(out Renderer renderer))
            {
                Color color = renderer.material.color;
                color.a = 1;
                renderer.material.color = color;
            }
        }
    }
}
