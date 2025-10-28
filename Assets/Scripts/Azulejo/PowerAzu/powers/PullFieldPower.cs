using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PullFieldPower : MonoBehaviour, ITilePower {
    public float pullStrength = 5f;
    public float duration = 0.5f;
    public AudioClip pullSound;
    public GameObject pulseEffectPrefab;
    public float bounceScale = 1.3f;
    public float scaleDuration = 0.2f;
    public Sprite icon;
    public Sprite Icon => icon;


    private bool pulling = false;
    private float timer = 0f;
    private Tile sourceTile;

    public void Activate(Tile tile) {
        sourceTile = tile;
        pulling = true;
        timer = duration;

        // Play sound
        if (pullSound)
            AudioSource.PlayClipAtPoint(pullSound, tile.transform.position);

        // Visual bounce
        tile.StartCoroutine(BouncePulse(tile.transform));

        // Visual fade sprite
        if (pulseEffectPrefab != null) {
            GameObject pulse = Instantiate(pulseEffectPrefab, tile.transform.position, Quaternion.identity, tile.transform);
            SpriteRenderer sr = pulse.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sortingOrder = 999;
                sr.sortingLayerName = "UI";
                tile.StartCoroutine(FadeOut(sr, 0.5f));
            }
        }
    }

    void Update() {
        if (!pulling || sourceTile == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) {
            pulling = false;
            return;
        }

        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile t in allTiles) {
            if (t == sourceTile) continue;

            Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
            if (rb != null) {
                Vector2 direction = (sourceTile.transform.position - t.transform.position).normalized;
                rb.AddForce(direction * pullStrength, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator BouncePulse(Transform target) {
        float t = 0;
        Vector3 original = target.localScale;
        Vector3 peak = original * bounceScale;

        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            target.localScale = Vector3.LerpUnclamped(original, peak, t);
            yield return null;
        }

        t = 0;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            target.localScale = Vector3.LerpUnclamped(peak, original, t);
            yield return null;
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sr, float time) {
        float t = 0;
        Color start = sr.color;
        while (t < time) {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / time);
            sr.color = new Color(start.r, start.g, start.b, a);
            yield return null;
        }
        Destroy(sr.gameObject);
    }
}