using UnityEngine;
using System.Collections;
using System.Linq;

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
        owningTile = tile;

        if (owningTile.isEnemy) {
            Vector3 furthestTargetPos = GetFurthestTargetPosition(tile.transform.position);
            if (furthestTargetPos != Vector3.positiveInfinity) {
                owningTile.StartCoroutine(TeleportSequence(furthestTargetPos));
            }
        } else {
            if (teleportMarkerPrefab == null) return;

            teleportActive = true;
            activationTime = Time.time;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            markerInstance = Instantiate(teleportMarkerPrefab, mouseWorld, Quaternion.identity);
        }
    }

    void Update() {
        if (!teleportActive || owningTile == null || owningTile.isEnemy) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (markerInstance != null)
            markerInstance.transform.position = mouseWorld;

        if (Time.time < activationTime + activationDelay) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) {
            owningTile.StartCoroutine(TeleportSequence(mouseWorld));
            teleportActive = false;
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
            if (markerInstance != null) Destroy(markerInstance);
            teleportActive = false;
        }
    }

    private IEnumerator TeleportSequence(Vector3 targetPosition) {
        if (markerInstance != null) Destroy(markerInstance);

        Transform tileTransform = owningTile.transform;

        float t = 0f;
        Vector3 originalScale = tileTransform.localScale;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            tileTransform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        if (disappearSound != null)
            AudioSource.PlayClipAtPoint(disappearSound, tileTransform.position);

        tileTransform.position = targetPosition;

        t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / scaleDuration;
            tileTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        if (reappearSound != null)
            AudioSource.PlayClipAtPoint(reappearSound, tileTransform.position);
    }

    private Vector3 GetFurthestTargetPosition(Vector3 origin) {
        if (EnemyAzuManager.instance == null || EnemyAzuManager.instance.targets == null || EnemyAzuManager.instance.targets.Count == 0)
            return Vector3.positiveInfinity;

        return EnemyAzuManager.instance.targets
            .OrderByDescending(t => Vector3.Distance(origin, t.position))
            .First()
            .position;
    }
}
