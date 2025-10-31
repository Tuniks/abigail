using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetZone : MonoBehaviour {
    [Header("Score Settings")]
    [Tooltip("Points awarded each pop event.")]
    public int scoreValue = 3;

    [Header("Tick Timing")]
    [Tooltip("Minimum seconds between pop events.")]
    public float minTickDuration = 1f;
    [Tooltip("Maximum seconds between pop events.")]
    public float maxTickDuration = 5f;

    [Header("Scale Multipliers")]
    [Tooltip("Scale multiplier at start of charge.")]
    public float initialScaleMultiplier = 0.5f;
    [Tooltip("Scale multiplier at peak of charge.")]
    public float chargeScaleMultiplier = 1.2f;

    [Header("Shake Settings")]
    [Tooltip("Seconds of shake before pop.")]
    public float shakeDuration = 0.2f;
    [Tooltip("Magnitude of shake offset.")]
    public float shakeMagnitude = 0.05f;

    [Header("Bounce & Flash")]
    [Tooltip("Multiplier for bounce scale relative to initial.")]
    public float bounceScaleMultiplier = 1.2f;
    [Tooltip("Time for bounce up or down.")]
    public float bounceTime = 0.1f;
    [Tooltip("Time to hold team color before resetting.")]
    public float popHoldDuration = 0.5f;

    [Header("Colors")]
    [Tooltip("Base color of the target.")]
    public Color defaultColor = Color.white;
    [Tooltip("Color to fade to during charge.")]
    public Color chargeFadeColor = Color.magenta;
    [Tooltip("Flash color when player scores.")]
    public Color playerFlashColor = Color.blue;
    [Tooltip("Flash color when enemy scores.")]
    public Color enemyFlashColor = Color.red;

    [Header("Audio")]
    [Tooltip("Source for pop sounds.")]
    public AudioSource audioSource;
    [Tooltip("Sound when player gains points.")]
    public AudioClip popPlayerClip;
    [Tooltip("Sound when enemy gains points.")]
    public AudioClip popEnemyClip;
    [Tooltip("Sound when no one gains points.")]
    public AudioClip popNeutralClip;

    [Header("UI")]
    [Tooltip("Text to display the score value.")]
    public TMP_Text pointsDisplay;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;
    private List<Tile> touchingTiles = new List<Tile>();

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;
        if (pointsDisplay != null)
            pointsDisplay.text = scoreValue.ToString();
    }

    private void Start() {
        StartCoroutine(PopLoop());
    }

    private IEnumerator PopLoop() {
        while (true) {
            float tickDuration = Random.Range(minTickDuration, maxTickDuration);
            float elapsed = 0f;

            // Charge & Shake
            while (elapsed < tickDuration) {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / tickDuration);

                float scaleMul = Mathf.Lerp(initialScaleMultiplier, chargeScaleMultiplier, t);
                transform.localScale = originalScale * scaleMul;
                spriteRenderer.color = Color.Lerp(defaultColor, chargeFadeColor, t);

                if (elapsed > tickDuration - shakeDuration) {
                    Vector3 offset = Random.insideUnitCircle * shakeMagnitude;
                    transform.position = originalPosition + offset;
                } else {
                    transform.position = originalPosition;
                }
                yield return null;
            }

            // Pop preparation
            transform.position = originalPosition;
            transform.localScale = originalScale * initialScaleMultiplier;
            spriteRenderer.color = defaultColor;

            // Count majority
            int playerCount = 0, enemyCount = 0;
            foreach (var tile in touchingTiles) {
                if (tile == null) continue;
                if (tile.isEnemy) enemyCount++; else playerCount++;
            }

            // Determine pop sound and flash color
            Color flashColor = defaultColor;
            AudioClip clipToPlay = popNeutralClip;
            if (playerCount > enemyCount) {
                TargetManager.instance?.AddScoreToPlayer(scoreValue);
                flashColor = playerFlashColor;
                clipToPlay = popPlayerClip;
            } else if (enemyCount > playerCount) {
                TargetManager.instance?.AddScoreToEnemy(scoreValue);
                flashColor = enemyFlashColor;
                clipToPlay = popEnemyClip;
            }

            // Play pop sound
            if (audioSource != null && clipToPlay != null)
                audioSource.PlayOneShot(clipToPlay);

            // Bounce & flash animation
            yield return StartCoroutine(BounceAndFlash(flashColor));

            // Hold flash color
            float hold = 0f;
            while (hold < popHoldDuration) {
                hold += Time.deltaTime;
                yield return null;
            }

            // Reset
            transform.localScale = originalScale * initialScaleMultiplier;
            spriteRenderer.color = defaultColor;
        }
    }

    private IEnumerator BounceAndFlash(Color flashColor) {
        spriteRenderer.color = flashColor;
        float half = bounceTime;
        float t = 0f;

        // Bounce up
        while (t < half) {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / half);
            transform.localScale = originalScale * Mathf.Lerp(initialScaleMultiplier, initialScaleMultiplier * bounceScaleMultiplier, f);
            yield return null;
        }

        // Bounce down
        t = 0f;
        while (t < half) {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / half);
            transform.localScale = originalScale * Mathf.Lerp(initialScaleMultiplier * bounceScaleMultiplier, initialScaleMultiplier, f);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<Tile>(out var tile) && !touchingTiles.Contains(tile))
            touchingTiles.Add(tile);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.TryGetComponent<Tile>(out var tile))
            touchingTiles.Remove(tile);
    }
}
