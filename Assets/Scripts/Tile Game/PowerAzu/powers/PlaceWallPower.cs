using UnityEngine;
using System.Collections;
using System.Linq;

public class PlaceWallPower : MonoBehaviour, ITilePower {
    public GameObject ghostWallPrefab;
    public GameObject wallToPlacePrefab;
    public AudioClip placeSound;
    public float growDuration = 0.2f;
    public float overshootScale = 1.2f;
    public float activationDelay = 0.2f;
    public Sprite icon;
    public Sprite Icon => icon;

    private GameObject ghostInstance;
    private bool placing = false;
    private float activationTime;
    private Tile owningTile;

    public void Activate(Tile sourceTile) {
        owningTile = sourceTile;
        if (wallToPlacePrefab == null) return;

        if (owningTile.isEnemy) {
            Tile nearestPlayerTile = FindNearestPlayerTile(owningTile.transform.position);
            if (nearestPlayerTile != null) {
                Vector3 placePos = nearestPlayerTile.transform.position;
                GameObject wall = Instantiate(wallToPlacePrefab, placePos, Quaternion.identity);
                if (placeSound != null)
                    AudioSource.PlayClipAtPoint(placeSound, placePos);
                StartCoroutine(ScaleBounce(wall.transform));
            }
        } else {
            if (ghostWallPrefab == null) return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            ghostInstance = Instantiate(ghostWallPrefab, mouseWorld, Quaternion.identity);
            placing = true;
            activationTime = Time.time;
        }
    }

    void Update() {
        if (!placing || owningTile == null || owningTile.isEnemy) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        if (ghostInstance != null)
            ghostInstance.transform.position = mouseWorld;

        if (Time.time < activationTime + activationDelay) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) {
            GameObject wall = Instantiate(wallToPlacePrefab, mouseWorld, Quaternion.identity);
            if (placeSound != null)
                AudioSource.PlayClipAtPoint(placeSound, wall.transform.position);

            StartCoroutine(ScaleBounce(wall.transform));

            if (ghostInstance != null) Destroy(ghostInstance);
            placing = false;
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)) {
            if (ghostInstance != null) Destroy(ghostInstance);
            placing = false;
        }
    }

    private IEnumerator ScaleBounce(Transform obj) {
        float t = 0;
        Vector3 start = Vector3.zero;
        Vector3 overshoot = Vector3.one * overshootScale;
        Vector3 target = Vector3.one;

        obj.localScale = start;

        while (t < 1f) {
            t += Time.deltaTime / growDuration;
            obj.localScale = Vector3.LerpUnclamped(start, overshoot, t);
            yield return null;
        }

        t = 0;
        while (t < 1f) {
            t += Time.deltaTime / growDuration;
            obj.localScale = Vector3.LerpUnclamped(overshoot, target, t);
            yield return null;
        }
    }

    private Tile FindNearestPlayerTile(Vector3 fromPos) {
        Tile[] allTiles = GameObject.FindObjectsOfType<Tile>();
        return allTiles
            .Where(t => t != null && !t.isEnemy)
            .OrderBy(t => Vector3.Distance(fromPos, t.transform.position))
            .FirstOrDefault();
    }
}
