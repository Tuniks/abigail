// MagicTileBehavior.cs
using System.Collections;
using UnityEngine;

public class MagicTileBehavior : MonoBehaviour
{
    [Header("Drag Settings")]
    private Vector3 offset;
    private float zDistance;
    private bool dragging = false;

    [Header("Follow Settings")]
    [SerializeField] private float followSmoothTime = 0.1f;
    private Vector3 followVelocity = Vector3.zero;

    [Header("Shake + Magnet")]
    [SerializeField] private Transform shakeTarget;
    [SerializeField] private float effectRadius = 2f;
    [SerializeField] private float snapDistance = 0.5f;

    [Header("Jitter Settings")]
    [SerializeField] private float maxJitter = 0.2f;
    [SerializeField] private float maxRotationAngle = 10f;
    [SerializeField] private float minJitterFrequency = 10f;
    [SerializeField] private float maxJitterFrequency = 60f;

    [Header("Mass Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.1f;
    private float rotationVelocity = 0f;
    private float currentMassAngle = 0f;
    private Vector3 lastPosition;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceScaleUp = 1.2f;
    [SerializeField] private float bounceScaleDown = 0.8f;
    [SerializeField] private float bounceDuration = 0.1f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Sprite Cycle Settings")]
    [SerializeField] private Sprite primarySprite;
    [SerializeField] private Sprite[] cycleSprites;
    [SerializeField] private float minCycleFrequency = 1f;
    [SerializeField] private float maxCycleFrequency = 10f;

    private Vector2 currentJitter = Vector2.zero;
    private float currentRotZ = 0f;
    private float jitterTimer = 0f;
    private float spriteTimer = 0f;
    private int spriteIndex = 0;

    private SpriteRenderer spriteRenderer;
    private IntroManager introMgr;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (primarySprite == null && spriteRenderer != null)
            primarySprite = spriteRenderer.sprite;
    }

    void OnMouseDown()
    {
        dragging = true;
        jitterTimer = spriteTimer = 0f;
        currentJitter = Vector2.zero;
        currentRotZ = currentMassAngle = 0f;
        followVelocity = Vector3.zero;
        rotationVelocity = 0f;
        spriteIndex = 0;

        if (spriteRenderer != null && primarySprite != null)
            spriteRenderer.sprite = primarySprite;

        if (shakeTarget != null)
            introMgr = shakeTarget.GetComponent<IntroManager>();

        lastPosition = transform.position;
        zDistance = transform.position.z - Camera.main.transform.position.z;
        Vector3 mp = Input.mousePosition; mp.z = zDistance;
        offset = transform.position - Camera.main.ScreenToWorldPoint(mp);
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

        Vector3 mp = Input.mousePosition; mp.z = zDistance;
        Vector3 desiredBasePos = Camera.main.ScreenToWorldPoint(mp) + offset;

        if (shakeTarget != null)
        {
            float dist = Vector3.Distance(desiredBasePos, shakeTarget.position);
            float intensity = 1f - Mathf.Clamp01(dist / effectRadius);

            introMgr?.SetScrambleIntensity(intensity);

            // cycle sprites
            if (cycleSprites != null && cycleSprites.Length > 0)
            {
                float cycleFreq = Mathf.Lerp(minCycleFrequency, maxCycleFrequency, intensity);
                float interval = 1f / cycleFreq;
                spriteTimer += Time.deltaTime;
                if (spriteTimer >= interval)
                {
                    spriteTimer = 0f;
                    spriteIndex = (spriteIndex + 1) % cycleSprites.Length;
                    spriteRenderer.sprite = cycleSprites[spriteIndex];
                }
            }

            // snap!
            if (dist <= snapDistance)
            {
                transform.position = shakeTarget.position;
                dragging = false;
                ResetRotationState();

                // reset sprite
                if (spriteRenderer != null && primarySprite != null)
                    spriteRenderer.sprite = primarySprite;

                // start wave reveal immediately, finishing by fadeDuration
                introMgr?.RevealText(fadeDuration);

                StartCoroutine(DoBounce());
                return;
            }

            // jitter timing
            float freq = Mathf.Lerp(minJitterFrequency, maxJitterFrequency, intensity);
            float intervalJ = 1f / freq;
            jitterTimer += Time.deltaTime;
            if (jitterTimer >= intervalJ)
            {
                jitterTimer = 0f;
                currentJitter = Random.insideUnitCircle * maxJitter * intensity;
                currentRotZ   = Random.Range(-maxRotationAngle, maxRotationAngle) * intensity;
            }

            // smooth follow + jitter
            Vector3 targetPos = desiredBasePos + (Vector3)currentJitter;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref followVelocity, followSmoothTime);

            // mass rotation
            Vector3 delta = transform.position - lastPosition;
            if (delta.sqrMagnitude > 0f)
            {
                Vector2 vel2D = new Vector2(delta.x, delta.y) / Time.deltaTime;
                if (vel2D.sqrMagnitude > 0f)
                {
                    float targetAngle = Vector2.SignedAngle(Vector2.up, vel2D.normalized);
                    currentMassAngle = Mathf.SmoothDampAngle(currentMassAngle, targetAngle, ref rotationVelocity, rotationSmoothTime);
                }
            }
            lastPosition = transform.position;

            float finalZ = currentMassAngle + currentRotZ;
            transform.rotation = Quaternion.Euler(0f, 0f, finalZ);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredBasePos, ref followVelocity, followSmoothTime);
        }
    }

    void OnMouseUp()
    {
        dragging = false;
        ResetRotationState();

        if (spriteRenderer != null && primarySprite != null)
            spriteRenderer.sprite = primarySprite;

        introMgr?.SetScrambleIntensity(0f);
    }

    private void ResetRotationState()
    {
        currentJitter = Vector2.zero;
        currentRotZ = currentMassAngle = 0f;
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator DoBounce()
    {
        Vector3 origScale = transform.localScale;
        Vector3 upScale   = origScale * bounceScaleUp;
        Vector3 downScale = origScale * bounceScaleDown;

        // up
        for (float t = 0; t < bounceDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(origScale, upScale, t / bounceDuration);
            yield return null;
        }
        transform.localScale = upScale;

        // down
        for (float t = 0; t < bounceDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(upScale, downScale, t / bounceDuration);
            yield return null;
        }
        transform.localScale = downScale;

        // back
        for (float t = 0; t < bounceDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(downScale, origScale, t / bounceDuration);
            yield return null;
        }
        transform.localScale = origScale;

        // after bounce, fade out and destroy
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        if (spriteRenderer != null)
        {
            Color col = spriteRenderer.color;
            float startA = col.a;

            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                col.a = Mathf.Lerp(startA, 0f, t / fadeDuration);
                spriteRenderer.color = col;
                yield return null;
            }
            col.a = 0f;
            spriteRenderer.color = col;
        }

        Destroy(gameObject);
    }
}
