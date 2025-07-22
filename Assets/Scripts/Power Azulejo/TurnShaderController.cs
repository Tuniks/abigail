using System.Collections;
using UnityEngine;

public class TurnShaderController : MonoBehaviour
{
    [Header("Game State")]
    [Tooltip("Drag your PowerSumoGame here (or leave blank to auto-find)")]
    [SerializeField] private PowerSumoGame sumoGame;

    [Header("Shader Material")]
    [Tooltip("Material using your halftone shader")]
    [SerializeField] private Material backgroundMaterial;

    [Header("Fade Settings")]
    [Tooltip("Seconds to cross-fade between turns")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Launch Grace")]
    [Tooltip("Ignore brief turn-flips shorter than this")]
    [SerializeField] private float launchGracePeriod = 1f;


    private float currentBlend;
    private bool lastEffectivePlayerTurn;
    private float rawFalseTime = -Mathf.Infinity;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (sumoGame == null)
            sumoGame = FindObjectOfType<PowerSumoGame>();

        // determine initial “effective” turn state
        bool raw = sumoGame.IsPlayerTurn();
        lastEffectivePlayerTurn = raw || (sumoGame.GetPlayerStamina() > 0);
        currentBlend = lastEffectivePlayerTurn ? 1f : 0f;

        // only set the blend float—leave your material colors untouched
        backgroundMaterial.SetFloat("_IsPlayerTurn", currentBlend);
    }

    private void Update()
    {
        bool raw = sumoGame.IsPlayerTurn();


        if (!raw && rawFalseTime < 0f)
            rawFalseTime = Time.time;
        else if (raw)
            rawFalseTime = -Mathf.Infinity;


        bool effective = raw || (Time.time - rawFalseTime < launchGracePeriod);

        if (effective != lastEffectivePlayerTurn)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeTo(effective ? 1f : 0f));
            lastEffectivePlayerTurn = effective;
        }
    }

    private IEnumerator FadeTo(float target)
    {
        float start = currentBlend;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            currentBlend = Mathf.Lerp(start, target, elapsed / fadeDuration);
            backgroundMaterial.SetFloat("_IsPlayerTurn", currentBlend);
            yield return null;
        }

        currentBlend = target;
        backgroundMaterial.SetFloat("_IsPlayerTurn", currentBlend);
    }
}
