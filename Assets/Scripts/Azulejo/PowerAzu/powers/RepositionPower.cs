using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RepositionPower : MonoBehaviour, ITilePower {
    public AudioClip teleportSound;
    public float staggerDelay = 0.1f;
    public float bounceScale = 1.3f;
    public float scaleDuration = 0.2f;
    public Sprite icon;
    public Sprite Icon => icon;


    public void Activate(Tile sourceTile) {
        sourceTile.StartCoroutine(ShuffleTilesWithJuice());
    }

    private IEnumerator ShuffleTilesWithJuice() {
        Tile[] allTiles = FindObjectsOfType<Tile>();
        List<Vector3> originalPositions = new List<Vector3>();

        foreach (Tile t in allTiles)
            originalPositions.Add(t.transform.position);

        for (int i = 0; i < allTiles.Length; i++) {
            allTiles[i].StartCoroutine(ShrinkOut(allTiles[i]));
        }

        yield return new WaitForSeconds(scaleDuration);

        for (int i = originalPositions.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            (originalPositions[i], originalPositions[j]) = (originalPositions[j], originalPositions[i]);
        }

        for (int i = 0; i < allTiles.Length; i++) {
            allTiles[i].transform.position = originalPositions[i];
            allTiles[i].StartCoroutine(BounceIn(allTiles[i]));
            if (teleportSound)
                AudioSource.PlayClipAtPoint(teleportSound, allTiles[i].transform.position);
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    private IEnumerator ShrinkOut(Tile tile) {
        float t = 0;
        Vector3 start = tile.transform.localScale;
        Vector3 target = Vector3.zero;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            tile.transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }

    private IEnumerator BounceIn(Tile tile) {
        float t = 0;
        Vector3 start = Vector3.zero;
        Vector3 overshoot = Vector3.one * bounceScale;
        Vector3 settle = Vector3.one;

        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            Vector3 scale = Vector3.LerpUnclamped(start, overshoot, t);
            tile.transform.localScale = scale;
            yield return null;
        }

        t = 0;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            Vector3 scale = Vector3.LerpUnclamped(overshoot, settle, t);
            tile.transform.localScale = scale;
            yield return null;
        }
    }
}
