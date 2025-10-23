using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class AzuPieceJuice : MonoBehaviour
{
    [Header("Select Shake")]
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeMagnitude = 0.06f; // world units
    [SerializeField] private float shakeFrequency = 36f;   // oscillations per second

    [Header("Capture Pop")]
    [SerializeField] private float popUpScale = 1.2f;    // bounce bigger
    [SerializeField] private float popUpTime = 0.12f;
    [SerializeField] private float shrinkScale = 0.6f;    // big shrink
    [SerializeField] private float shrinkTime = 0.14f;
    [SerializeField] private float restoreTime = 0.12f;

    private Coroutine shakeCo;
    private Coroutine popCo;

    void OnDisable()
    {
        // stop coroutines and *do not* teleport — leave current pos/scale as-is
        if (shakeCo != null) StopCoroutine(shakeCo);
        if (popCo != null) StopCoroutine(popCo);
        shakeCo = popCo = null;
    }

    // --- API ---

    public void PlaySelectShake()
    {
        if (!isActiveAndEnabled) return;
        if (shakeCo != null) StopCoroutine(shakeCo);
        shakeCo = StartCoroutine(CoShake());
    }

    public void StopShake()
    {
        if (shakeCo != null) StopCoroutine(shakeCo);
        shakeCo = null;
    }

    public void PlayCapturePop(AudioClip sfx = null, AudioSource src = null, System.Action onMidpoint = null, System.Action onFinish = null)
    {
        if (!isActiveAndEnabled)
        {
            onMidpoint?.Invoke();
            onFinish?.Invoke();
            return;
        }

        if (popCo != null) StopCoroutine(popCo);
        popCo = StartCoroutine(CoCapturePop(sfx, src, onMidpoint, onFinish));
    }

    // --- Coroutines ---

    private IEnumerator CoShake()
    {
        // Always shake around the *current* localPosition (after snap/placement)
        Vector3 basePos = transform.localPosition;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / shakeDuration);
            float atten = 1f - u; // ease-out

            float phase = t * shakeFrequency * Mathf.PI * 2f;
            float dx = Mathf.Sin(phase) * shakeMagnitude * atten;
            float dy = Mathf.Cos(phase * 0.9f) * shakeMagnitude * 0.6f * atten;

            transform.localPosition = basePos + new Vector3(dx, dy, 0f);
            yield return null;
        }

        // restore to where we began the shake (not to (0,0)!)
        transform.localPosition = basePos;
        shakeCo = null;
    }

    private IEnumerator CoCapturePop(AudioClip sfx, AudioSource src, System.Action onMidpoint, System.Action onFinish)
    {
        // SFX
        if (sfx != null)
        {
            if (src != null) src.PlayOneShot(sfx);
            else AudioSource.PlayClipAtPoint(sfx, transform.position);
        }

        Vector3 s0 = transform.localScale;

        // 1) pop up
        yield return ScaleOverTime(s0, s0 * popUpScale, popUpTime, EaseOutCubic);

        // midpoint callback (move to bin, etc.)
        onMidpoint?.Invoke();

        // 2) big shrink
        yield return ScaleOverTime(transform.localScale, s0 * shrinkScale, shrinkTime, EaseInCubic);

        // 3) restore
        yield return ScaleOverTime(transform.localScale, s0, restoreTime, EaseOutCubic);

        popCo = null;
        onFinish?.Invoke();
    }

    private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float dur, System.Func<float, float> ease)
    {
        if (dur <= 0f) { transform.localScale = to; yield break; }
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            transform.localScale = Vector3.LerpUnclamped(from, to, ease(u));
            yield return null;
        }
        transform.localScale = to;
    }

    // Easing
    private float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);
    private float EaseInCubic(float x) => x * x * x;
}
