using System.Collections;
using UnityEngine;

public class MagicTileBehavior : MonoBehaviour
{
    [Header("Disorder Settings")]
    [Tooltip("Max random rotation angle for a disheveled vibe.")]
    [SerializeField] private float maxDisorderAngle = 10f;

    [Header("Drag Settings")]
    private Vector3 offset;
    private float zDistance;
    private bool dragging = false;

    [Header("Follow Settings")]
    [Tooltip("Time (seconds) for the tile to catch up to the target position.")]
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
    [Tooltip("Smooth time for rotation inertia.")]
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
    [Tooltip("Primary sprite to reset to on click/release.")]
    [SerializeField] private Sprite primarySprite;
    [Tooltip("Sprites to cycle through while dragging.")]
    [SerializeField] private Sprite[] cycleSprites;
    [Tooltip("Hz at distance (intensity=0).")]
    [SerializeField] private float minCycleFrequency = 1f;
    [Tooltip("Hz when right on target (intensity=1).")]
    [SerializeField] private float maxCycleFrequency = 10f;

    // Internal state
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

        // Apply an initial random rotation
        float initialAngle = Random.Range(-maxDisorderAngle, maxDisorderAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, initialAngle);
    }

    void OnMouseDown()
    {
        dragging = true;

        // Reset timers & state
        jitterTimer = spriteTimer = 0f;
        currentJitter = Vector2.zero;
        currentRotZ = currentMassAngle = 0f;
        followVelocity = Vector3.zero;
        rotationVelocity = 0f;
        spriteIndex = 0;

        // Reset sprite
        if (spriteRenderer != null && primarySprite != null)
            spriteRenderer.sprite = primarySprite;

        // Orient upright
        transform.rotation = Quaternion.identity;

        // Cache IntroManager
        if (shakeTarget != null)
            introMgr = shakeTarget.GetComponent<IntroManager>();

        // Record position & offset
        lastPosition = transform.position;
        zDistance = transform.position.z - Camera.main.transform.position.z;
        Vector3 mp = Input.mousePosition; mp.z = zDistance;
        offset = transform.position - Camera.main.ScreenToWorldPoint(mp);
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

        // Compute desired base position under cursor
        Vector3 mp = Input.mousePosition; mp.z = zDistance;
        Vector3 desiredBasePos = Camera.main.ScreenToWorldPoint(mp) + offset;

        if (shakeTarget != null)
        {
            // Proximity intensity for scrambling/cycling
            float dist = Vector3.Distance(desiredBasePos, shakeTarget.position);
            float intensity = 1f - Mathf.Clamp01(dist / effectRadius);

            // Update text scramble
            introMgr?.SetScrambleIntensity(intensity);

            // Sprite cycling
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

            // Snap check
            if (dist <= snapDistance)
            {
                transform.position = shakeTarget.position;
                dragging = false;

                // Reset jitter & mass rotation
                currentJitter = Vector2.zero;
                currentRotZ = currentMassAngle = 0f;
                // Orient upright for bounce
                transform.rotation = Quaternion.identity;

                // Reset sprite
                if (spriteRenderer != null && primarySprite != null)
                    spriteRenderer.sprite = primarySprite;

                // Trigger text reveal and bounce
                introMgr?.RevealText(fadeDuration);
                StartCoroutine(DoBounce());
                return;
            }

            // Jitter timing
            float freq = Mathf.Lerp(minJitterFrequency, maxJitterFrequency, intensity);
            float intervalJ = 1f / freq;
            jitterTimer += Time.deltaTime;
            if (jitterTimer >= intervalJ)
            {
                jitterTimer = 0f;
                currentJitter = Random.insideUnitCircle * maxJitter * intensity;
                currentRotZ = Random.Range(-maxRotationAngle, maxRotationAngle) * intensity;
            }

            // Smooth follow with jitter
            Vector3 targetPos = desiredBasePos + (Vector3)currentJitter;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref followVelocity, followSmoothTime);

            // Mass-based rotation inertia
            Vector3 delta = transform.position - lastPosition;
            if (delta.sqrMagnitude > 0f)
            {
                Vector2 vel2D = new Vector2(delta.x, delta.y) / Time.deltaTime;
                if (vel2D.sqrMagnitude > 0f)
                {
                    float targetMassAngle = Vector2.SignedAngle(Vector2.up, vel2D.normalized);
                    currentMassAngle = Mathf.SmoothDampAngle(currentMassAngle, targetMassAngle, ref rotationVelocity, rotationSmoothTime);
                }
            }
            lastPosition = transform.position;

            // Apply combined rotation
            float finalZ = currentMassAngle + currentRotZ;
            transform.rotation = Quaternion.Euler(0f, 0f, finalZ);
        }
        else
        {
            // No target: simple smooth follow
            transform.position = Vector3.SmoothDamp(transform.position, desiredBasePos, ref followVelocity, followSmoothTime);
        }
    }

    void OnMouseUp()
    {
        dragging = false;

        // Reset jitter & mass rotation (but apply disheveled rotation)
        currentJitter = Vector2.zero;
        currentRotZ = currentMassAngle = 0f;

        // Apply a new random rotation for the disheveled vibe
        float angle = Random.Range(-maxDisorderAngle, maxDisorderAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Reset sprite on release
        if (spriteRenderer != null && primarySprite != null)
            spriteRenderer.sprite = primarySprite;

        introMgr?.SetScrambleIntensity(0f);
    }

    private IEnumerator DoBounce()
    {
        Vector3 origScale = transform.localScale;
        Vector3 upScale = origScale * bounceScaleUp;
        Vector3 downScale = origScale * bounceScaleDown;

        // Bounce up
        for (float t = 0; t < bounceDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(origScale, upScale, t / bounceDuration);
            yield return null;
        }
        transform.localScale = upScale;

        // Bounce down
        for (float t = 0; t < bounceDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(upScale, downScale, t / bounceDuration);
            yield return null;
        }
        transform.localScale = downScale;

        // Return to normal
        for (float t = 0; t < bounceDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(downScale, origScale, t / bounceDuration);
            yield return null;
        }
        transform.localScale = origScale;

        // Start fading & destroy
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
