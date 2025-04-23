using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PushFieldPower : MonoBehaviour, ITilePower {
    public float pushStrength = 5f;
    public float duration = 0.2f;
    public AudioClip pushSound;
    public GameObject pulseUIObject; // UI prefab to fade
    public float bounceScale = 1.3f;
    public float scaleDuration = 0.2f;
    public Sprite icon;
    public Sprite Icon => icon;


    private bool pushing = false;
    private float timer = 0f;
    private Tile sourceTile;
    private HashSet<Tile> alreadyPushed = new HashSet<Tile>();

    public void Activate(Tile tile) {
        sourceTile = tile;
        pushing = true;
        timer = duration;
        alreadyPushed.Clear();

        // Play sound
        if (pushSound)
            AudioSource.PlayClipAtPoint(pushSound, tile.transform.position);

        // Visual bounce
        tile.StartCoroutine(BouncePulse(tile.transform));

        // Visual UI pulse
        if (pulseUIObject != null) {
            GameObject pulse = Instantiate(pulseUIObject, tile.transform.position, Quaternion.identity, tile.transform);
            CanvasGroup canvasGroup = pulse.GetComponent<CanvasGroup>();
            if (canvasGroup != null) {
                tile.StartCoroutine(FadeCanvasGroup(canvasGroup, 0.5f));
            }
        }
    }

    void Update() {
        if (!pushing || sourceTile == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) {
            pushing = false;
            return;
        }

        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (Tile t in allTiles) {
            if (t == sourceTile || alreadyPushed.Contains(t)) continue;

            Rigidbody2D rb = t.GetComponent<Rigidbody2D>();
            if (rb != null) {
                Vector2 direction = (t.transform.position - sourceTile.transform.position).normalized;
                rb.AddForce(direction * pushStrength, ForceMode2D.Impulse);
                alreadyPushed.Add(t);
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

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float duration) {
        float t = 0;
        while (t < duration) {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }
        Destroy(cg.gameObject);
    }
}