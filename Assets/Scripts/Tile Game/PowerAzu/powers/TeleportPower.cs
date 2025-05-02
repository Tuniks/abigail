using UnityEngine;
using System.Collections;

public class TeleportPower : MonoBehaviour, ITilePower {
    public GameObject teleportMarkerPrefab;
    public AudioClip disappearSound;
    public AudioClip reappearSound;
    public float scaleDuration = 0.2f;
    public float activationDelay = 0.2f;
    public Sprite icon;
    public Sprite Icon => icon;

    private GameObject markerInstance;
    private Tile owningTile;
    private bool teleportActive = false;
    private float activationTime;

    public void Activate(Tile tile) {
        if (teleportMarkerPrefab == null) return;

        owningTile = tile;
        teleportActive = true;
        activationTime = Time.time;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        markerInstance = Instantiate(teleportMarkerPrefab, mouseWorld, Quaternion.identity);
    }

    void Update() {
        if (!teleportActive) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (markerInstance != null)
            markerInstance.transform.position = mouseWorld;

        // Prevent immediate teleport
        if (Time.time < activationTime + activationDelay) return;

        // Confirm teleport with left click or E
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) {
            if (owningTile != null)
                owningTile.StartCoroutine(TeleportSequence(mouseWorld));
            teleportActive = false;
        }

        // Cancel with right click or Escape
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
            if (markerInstance != null) Destroy(markerInstance);
            teleportActive = false;
        }
    }

    private IEnumerator TeleportSequence(Vector3 targetPosition) {
        if (markerInstance != null) Destroy(markerInstance);

        Transform tileTransform = owningTile.transform;

        // Shrink & disappear
        float t = 0;
        Vector3 originalScale = tileTransform.localScale;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            tileTransform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        if (disappearSound != null)
            AudioSource.PlayClipAtPoint(disappearSound, tileTransform.position);

        // Move while invisible
        tileTransform.position = targetPosition;

        // Reappear & grow
        t = 0;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            tileTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        if (reappearSound != null)
            AudioSource.PlayClipAtPoint(reappearSound, tileTransform.position);
    }
}
