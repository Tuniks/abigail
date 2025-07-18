using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("Text Setup")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private List<string> textSequence;

    [Header("Scramble Settings")]
    [Range(0f, 1f)] [SerializeField] private float maxScrambleAmount = 1f;
    [SerializeField] private float minScrambleFrequency = 1f;
    [SerializeField] private float maxScrambleFrequency = 20f;

    [Header("End-Sequence Settings")]
    [Tooltip("How many snaps until we trigger the end sequence.")]
    [SerializeField] private int requiredCollisions = 3;
    [Tooltip("All draggable tiles (will fade these).")]
    [SerializeField] private List<GameObject> validColliders;
    [Tooltip("Full-screen CanvasGroup for scene fade.")]
    [SerializeField] private CanvasGroup screenFader;
    [Tooltip("Name of the scene to load after fade.")]
    [SerializeField] private string nextSceneName;

    [Header("Fade Durations")]
    [SerializeField] private float tileFadeTime = 0.5f;
    [SerializeField] private float targetFadeTime = 0.5f;
    [SerializeField] private float textFadeTime = 0.5f;
    [SerializeField] private float sceneFadeTime = 1f;

    private int collisionCount = 0;
    private int currentIndex = 0;
    private string originalText;
    private bool isScrambling = false;
    private float scrambleTimer = 0f;
    private float currentIntensity = 0f;

    void Start()
    {
        if (dialogueText != null)
            originalText = dialogueText.text;

        if (screenFader != null)
            screenFader.alpha = 0f;
    }

    void Update()
    {
        if (!isScrambling) return;

        float freq = Mathf.Lerp(minScrambleFrequency, maxScrambleFrequency, currentIntensity);
        float interval = 1f / freq;

        scrambleTimer += Time.deltaTime;
        if (scrambleTimer >= interval)
        {
            scrambleTimer = 0f;
            dialogueText.text = ScrambleText(originalText, currentIntensity * maxScrambleAmount);
        }
    }

    public void SetScrambleIntensity(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);

        if (intensity > 0f)
        {
            if (!isScrambling)
            {
                isScrambling = true;
                originalText = dialogueText.text;
                scrambleTimer = 0f;
            }
            currentIntensity = intensity;
        }
        else if (isScrambling)
        {
            isScrambling = false;
            dialogueText.text = originalText;
        }
    }

    public void RevealText(float totalRevealTime)
    {
        if (textSequence != null && textSequence.Count > 0)
        {
            dialogueText.text = textSequence[currentIndex];
            originalText = dialogueText.text;
            currentIndex = Mathf.Min(currentIndex + 1, textSequence.Count - 1);
        }

        collisionCount++;
        if (collisionCount >= requiredCollisions)
            StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        // 1) Fade out all remaining tiles
        List<SpriteRenderer> tileRenderers = new List<SpriteRenderer>();
        foreach (GameObject go in validColliders)
        {
            if (go == null) continue;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
                tileRenderers.Add(sr);
        }

        float t = 0f;
        while (t < tileFadeTime)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / tileFadeTime);
            foreach (var renderer in tileRenderers)
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            yield return null;
        }

        // 2) Fade out this target GameObject
        SpriteRenderer targetSR = GetComponent<SpriteRenderer>();
        if (targetSR != null)
        {
            t = 0f;
            Color start = targetSR.color;
            while (t < targetFadeTime)
            {
                t += Time.deltaTime;
                float a = 1f - (t / targetFadeTime);
                targetSR.color = new Color(start.r, start.g, start.b, a);
                yield return null;
            }
        }

        // 3) Fade out the TMP text
        if (dialogueText != null)
        {
            t = 0f;
            Color startText = dialogueText.color;
            while (t < textFadeTime)
            {
                t += Time.deltaTime;
                float a = 1f - (t / textFadeTime);
                dialogueText.color = new Color(startText.r, startText.g, startText.b, a);
                yield return null;
            }
        }

        // 4) Fade full screen then load next scene
        if (screenFader != null)
        {
            t = 0f;
            while (t < sceneFadeTime)
            {
                t += Time.deltaTime;
                screenFader.alpha = t / sceneFadeTime;
                yield return null;
            }
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private string ScrambleText(string text, float amount)
    {
        char[] chars = text.ToCharArray();
        int len = chars.Length;
        int toScramble = Mathf.RoundToInt(len * Mathf.Clamp01(amount));

        for (int i = 0; i < toScramble; i++)
        {
            int idx = Random.Range(0, len);
            if (!char.IsWhiteSpace(chars[idx]))
            {
                char orig = chars[idx];
                if (char.IsUpper(orig))
                    chars[idx] = (char)('A' + Random.Range(0, 26));
                else
                    chars[idx] = (char)('a' + Random.Range(0, 26));
            }
        }

        return new string(chars);
    }
}
