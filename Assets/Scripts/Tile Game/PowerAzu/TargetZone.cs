
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetZone : MonoBehaviour {
    [Header("Visual Feedback")]
    public SpriteRenderer spriteRenderer;
    public Color enemyColor = Color.red;
    public Color playerColor = Color.blue;
    public Color defaultColor = Color.white;

    [Header("Disappearance Settings")]
    public float checkInterval = 1f;
    public float dominanceDuration = 10f;
    public float shrinkDuration = 0.5f;
    public float bounceScale = 1.2f;

    private List<Tile> touchingTiles = new List<Tile>();
    private float enemyTime = 0f;
    private float playerTime = 0f;
    private bool isDisappearing = false;

    void Start() {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(CheckDominanceRoutine());
    }

    private IEnumerator CheckDominanceRoutine() {
        while (!isDisappearing) {
            int enemyCount = 0;
            int playerCount = 0;

            foreach (var tile in touchingTiles) {
                if (tile != null) {
                    if (tile.isEnemy) enemyCount++;
                    else playerCount++;
                }
            }

            if (enemyCount > playerCount) {
                playerTime = 0f;
                enemyTime += checkInterval;
                spriteRenderer.color = enemyColor;
                if (enemyTime >= dominanceDuration) {
                    StartCoroutine(Disappear());
                }
            } else if (playerCount > enemyCount) {
                enemyTime = 0f;
                playerTime += checkInterval;
                spriteRenderer.color = playerColor;
                if (playerTime >= dominanceDuration) {
                    StartCoroutine(Disappear());
                }
            } else {
                enemyTime = 0f;
                playerTime = 0f;
                spriteRenderer.color = defaultColor;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator Disappear() {
        isDisappearing = true;
        Vector3 originalScale = transform.localScale;
        Vector3 bounceScaleVec = originalScale * bounceScale;
        float elapsed = 0f;

        // Bounce
        while (elapsed < shrinkDuration / 2f) {
            transform.localScale = Vector3.Lerp(originalScale, bounceScaleVec, elapsed / (shrinkDuration / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < shrinkDuration) {
            transform.localScale = Vector3.Lerp(bounceScaleVec, Vector3.zero, elapsed / shrinkDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Tile tile = other.GetComponent<Tile>();
        if (tile != null && !touchingTiles.Contains(tile)) {
            touchingTiles.Add(tile);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        Tile tile = other.GetComponent<Tile>();
        if (tile != null && touchingTiles.Contains(tile)) {
            touchingTiles.Remove(tile);
        }
    }
}
