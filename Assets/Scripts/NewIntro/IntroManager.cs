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

    [Header("Reveal Bounce Settings")]
    [SerializeField] private float revealBounceUp = 1.1f;
    [SerializeField] private float revealBounceDown = 0.9f;
    [SerializeField] private float revealBounceDuration = 0.1f;

    [Header("End-Sequence Settings")]
    [Tooltip("How many tiles must snap before the end sequence starts.")]
    [SerializeField] private int requiredCollisions = 3;
    [Tooltip("All draggable tiles (to fade at the end).")]
    [SerializeField] private List<GameObject> validColliders;
    [Tooltip("Full-screen CanvasGroup for final fade.")]
    [SerializeField] private CanvasGroup screenFader;
    [Tooltip("Scene to load after fade.")]
    [SerializeField] private string nextSceneName = "";

    [Header("Fade Durations")]
    [SerializeField] private float tileFadeTime  = 0.5f;
    [SerializeField] private float targetFadeTime = 0.5f;
    [SerializeField] private float textFadeTime   = 0.5f;
    [SerializeField] private float sceneFadeTime  = 1f;

    [Header("Swap Objects")]
    [Tooltip("Active when NO tile is placed.")]
    [SerializeField] private GameObject objectWhenNone;
    [Tooltip("Active when ≥1 tile is placed.")]
    [SerializeField] private GameObject objectWhenPlaced;

    // ---- Internal ----
    private int   collisionCount   = 0;
    private int   currentIndex     = 0;
    private int   placedTileCount  = 0;
    private string originalText;
    private bool  isScrambling     = false;
    private float scrambleTimer    = 0f;
    private float currentIntensity = 0f;

    void Start()
    {
        if (dialogueText != null) originalText = dialogueText.text;
        if (screenFader  != null) screenFader.alpha = 0f;
        SetSwapObjectsActive(noTile: true);
    }

    void Update()
    {
        if (!isScrambling) return;

        float freq     = Mathf.Lerp(minScrambleFrequency, maxScrambleFrequency, currentIntensity);
        float interval = 1f / freq;

        scrambleTimer += Time.deltaTime;
        if (scrambleTimer >= interval)
        {
            scrambleTimer = 0f;
            dialogueText.text = ScrambleText(originalText, currentIntensity * maxScrambleAmount);
        }
    }

    // Called every frame by the tile while dragging
    public void SetScrambleIntensity(float intensity)
    {
        intensity = Mathf.Clamp01(intensity);

        if (intensity > 0f)
        {
            if (!isScrambling)
            {
                isScrambling  = true;
                originalText  = dialogueText.text;
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

    // Called right after snap. totalRevealTime usually == tile's fadeDuration
    public void RevealText(float totalRevealTime)
    {
        if (textSequence == null || textSequence.Count == 0) return;

        StopAllCoroutines();
        isScrambling  = false;
        scrambleTimer = 0f;

        string full = textSequence[currentIndex];
        StartCoroutine(WaveReveal(full, totalRevealTime));

        if (currentIndex < textSequence.Count - 1) currentIndex++;

        collisionCount++;
        if (collisionCount >= requiredCollisions)
            StartCoroutine(EndSequence());
    }

    // Tile bookkeeping
    public void TilePlaced()
    {
        placedTileCount++;
        SetSwapObjectsActive(noTile: false);
    }

    public void TileRemoved()
    {
        placedTileCount = Mathf.Max(0, placedTileCount - 1);
        if (placedTileCount == 0)
            SetSwapObjectsActive(noTile: true);
    }

    private void SetSwapObjectsActive(bool noTile)
    {
        if (objectWhenNone   != null) objectWhenNone.SetActive(noTile);
        if (objectWhenPlaced != null) objectWhenPlaced.SetActive(!noTile);
    }

    // -------- Coroutines --------

    private IEnumerator WaveReveal(string fullText, float totalTime)
    {
        int len = fullText.Length;
        float perCharDelay = totalTime / Mathf.Max(1, len);

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
                    char orig = fullText[j];
                    chars[j] = char.IsUpper(orig)
                        ? (char)('A' + Random.Range(0, 26))
                        : (char)('a' + Random.Range(0, 26));
                }
            }
            dialogueText.text = new string(chars);
            yield return new WaitForSeconds(perCharDelay);
        }

        dialogueText.text = fullText;
        originalText      = fullText;
        yield return StartCoroutine(TextBounce());
    }

    private IEnumerator TextBounce()
    {
        Transform rt = dialogueText.transform;
        Vector3 orig = rt.localScale;
        Vector3 up   = orig * revealBounceUp;
        Vector3 down = orig * revealBounceDown;

        for (float t = 0; t < revealBounceDuration; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(orig, up, t / revealBounceDuration);
            yield return null;
        }
        rt.localScale = up;

        for (float t = 0; t < revealBounceDuration; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(up, down, t / revealBounceDuration);
            yield return null;
        }
        rt.localScale = down;

        for (float t = 0; t < revealBounceDuration; t += Time.deltaTime)
        {
            rt.localScale = Vector3.Lerp(down, orig, t / revealBounceDuration);
            yield return null;
        }
        rt.localScale = orig;
    }

    private IEnumerator EndSequence()
    {
        // 1) Fade out remaining tiles
        List<SpriteRenderer> tileRenderers = new List<SpriteRenderer>();
        foreach (GameObject go in validColliders)
        {
            if (go == null) continue;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) tileRenderers.Add(sr);
        }

        float t = 0f;
        while (t < tileFadeTime)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / tileFadeTime);
            foreach (var r in tileRenderers)
            {
                if (r == null) continue;
                Color c = r.color;
                r.color = new Color(c.r, c.g, c.b, alpha);
            }
            yield return null;
        }

        // 2) Fade out this target sprite (if present)
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

        // 3) Fade out text
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

        // 4) Fade screen & load scene
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

    // -------- Helpers --------
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
                chars[idx] = char.IsUpper(orig)
                    ? (char)('A' + Random.Range(0, 26))
                    : (char)('a' + Random.Range(0, 26));
            }
        }
        return new string(chars);
    }
}
