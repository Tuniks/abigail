// IntroManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Reveal Bounce Settings")]
    [SerializeField] private float revealBounceUp = 1.1f;
    [SerializeField] private float revealBounceDown = 0.9f;
    [SerializeField] private float revealBounceDuration = 0.1f;

    private int currentIndex = 0;
    private string originalText;
    private bool isScrambling = false;
    private float scrambleTimer = 0f;
    private float currentIntensity = 0f;

    void Start()
    {
        if (dialogueText != null)
            originalText = dialogueText.text;
    }

    void Update()
    {
        if (!isScrambling) return;

        // ramp how often we scramble
        float freq = Mathf.Lerp(minScrambleFrequency, maxScrambleFrequency, currentIntensity);
        float interval = 1f / freq;

        scrambleTimer += Time.deltaTime;
        if (scrambleTimer >= interval)
        {
            scrambleTimer = 0f;
            dialogueText.text = ScrambleText(originalText, currentIntensity * maxScrambleAmount);
        }
    }

    // called each drag‐frame with 0–1 proximity
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

    // call on snap—wave reveal will run over totalRevealTime seconds
    public void RevealText(float totalRevealTime)
    {
        if (textSequence == null || textSequence.Count == 0) return;

        StopAllCoroutines();
        isScrambling = false;
        scrambleTimer = 0f;

        string full = textSequence[currentIndex];
        StartCoroutine(WaveReveal(full, totalRevealTime));

        if (currentIndex < textSequence.Count - 1)
            currentIndex++;
    }

    private IEnumerator WaveReveal(string fullText, float totalTime)
    {
        int len = fullText.Length;
        float delay = totalTime / Mathf.Max(1, len);

        for (int i = 0; i <= len; i++)
        {
            char[] chars = new char[len];
            for (int j = 0; j < len; j++)
            {
                if (j < i)
                {
                    chars[j] = fullText[j];
                }
                else if (char.IsWhiteSpace(fullText[j]))
                {
                    chars[j] = ' ';
                }
                else
                {
                    // match case of original
                    char orig = fullText[j];
                    if (char.IsUpper(orig))
                        chars[j] = (char)('A' + Random.Range(0, 26));
                    else
                        chars[j] = (char)('a' + Random.Range(0, 26));
                }
            }

            dialogueText.text = new string(chars);
            yield return new WaitForSeconds(delay);
        }

        // lock in correct text
        dialogueText.text = fullText;
        originalText = fullText;

        // little bounce to show it's locked
        yield return StartCoroutine(TextBounce());
    }

    private IEnumerator TextBounce()
    {
        var rt = dialogueText.transform;
        Vector3 orig = rt.localScale;
        Vector3 up = orig * revealBounceUp;
        Vector3 down = orig * revealBounceDown;

        // up
        for (float t = 0; t < revealBounceDuration; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(orig, up, t / revealBounceDuration);
            yield return null;
        }
        rt.localScale = up;

        // down
        for (float t = 0; t < revealBounceDuration; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(up, down, t / revealBounceDuration);
            yield return null;
        }
        rt.localScale = down;

        // back
        for (float t = 0; t < revealBounceDuration; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(down, orig, t / revealBounceDuration);
            yield return null;
        }
        rt.localScale = orig;
    }

    // scramble but preserve letter case
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
