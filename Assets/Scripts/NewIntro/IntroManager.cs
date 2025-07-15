using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroManager : MonoBehaviour
{
    [Header("Text Setup")]
    [Tooltip("The TextMeshProUGUI component to update.")]
    [SerializeField] private TMP_Text dialogueText;
    [Tooltip("The sequence of strings to display, in order.")]
    [SerializeField] private List<string> textSequence;

    [Header("Scramble Settings")]
    [Tooltip("Max fraction of characters to scramble at full intensity.")]
    [Range(0f, 1f)]
    [SerializeField] private float maxScrambleAmount = 1f;
    [Tooltip("Min scramble updates per second (intensity = 0).")]
    [SerializeField] private float minScrambleFrequency = 1f;
    [Tooltip("Max scramble updates per second (intensity = 1).")]
    [SerializeField] private float maxScrambleFrequency = 20f;

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
        if (!isScrambling) 
            return;

        // Compute how often to scramble this frame
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
    
    public void RevealText()
    {
        if (textSequence == null || textSequence.Count == 0) 
            return;

        // Show the next string exactly
        dialogueText.text = textSequence[currentIndex];
        originalText = dialogueText.text;
        isScrambling = false;
        scrambleTimer = 0f;

        // Advance index (clamped)
        if (currentIndex < textSequence.Count - 1)
            currentIndex++;
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
                chars[idx] = (char)('A' + Random.Range(0, 26));
        }

        return new string(chars);
    }
}
