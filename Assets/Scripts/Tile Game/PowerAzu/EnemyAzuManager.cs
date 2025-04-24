using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAzuManager : MonoBehaviour {
    public static EnemyAzuManager instance;

    public ClayEnemyHand enemyHand;
    public List<GameObject> enemyActiveSpots;

    [Header("Physics Setup")]
    public float maxStatValue = 15f;
    public float enemyTileMass = 0.5f;
    public float enemyTileDrag = 1f;
    public float enemyTileAngularDrag = 5f;
    public float chargeForce = 1000f;
    [Range(0f, 1f)] public float enemyLaunchForceMultiplier = 0.25f;

    [Header("Bounce Feedback")]
    public GameObject flashEffectPrefab;
    public GameObject bounceParticlesPrefab;

    [Header("Enemy Visuals")]
    public GameObject baseSpritePrefab;

    private List<Tile> activeEnemyTiles = new List<Tile>();
    private bool launching = false;
    private Tile lastLaunchedTile = null;

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartEnemyTurn() {
        activeEnemyTiles.Clear();
        StartCoroutine(PlaceEnemyTiles());
    }

    private IEnumerator PlaceEnemyTiles() {
        List<Tile> selectedTiles = new List<Tile>();
        List<Tile> shuffled = new List<Tile>(enemyHand.hand);
        shuffled.Shuffle();
        int count = Mathf.Min(3, shuffled.Count);

        for (int i = 0; i < count; i++) {
            selectedTiles.Add(shuffled[i]);
        }

        List<GameObject> availableSpots = new List<GameObject>(enemyActiveSpots);
        availableSpots.Shuffle();

        for (int i = 0; i < selectedTiles.Count; i++) {
            if (i >= availableSpots.Count) break;

            GameObject spotObj = availableSpots[i];
            Tile tile = selectedTiles[i];

            tile.transform.position = spotObj.transform.position;
            tile.transform.localScale = Vector3.one;
            tile.gameObject.SetActive(true);
            tile.isEnemy = true;

            if (baseSpritePrefab != null) {
                GameObject baseSprite = Instantiate(baseSpritePrefab, tile.transform.position, Quaternion.identity);
                baseSprite.transform.SetParent(tile.transform);
                baseSprite.transform.localPosition = Vector3.zero;
                baseSprite.transform.localScale = baseSprite.transform.localScale * 1.29f;
                baseSprite.transform.SetAsFirstSibling();
            }

            ClayEnemyActiveSpot spot = spotObj.GetComponent<ClayEnemyActiveSpot>();
            if (spot != null) {
                spot.ActivateTile(tile);
            }

            AssignPhysicsProperties(tile);
            activeEnemyTiles.Add(tile);
            yield return new WaitForSeconds(0.1f);
        }

        if (!launching) StartCoroutine(LaunchTilesContinuously());
    }

    private IEnumerator LaunchTilesContinuously() {
        launching = true;

        while (true) {
            if (activeEnemyTiles.Count > 0) {
                Tile tile = GetNextTileToLaunch();
                if (tile != null) {
                    LaunchTile(tile);
                    lastLaunchedTile = tile;
                }
            }
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    private Tile GetNextTileToLaunch() {
        if (activeEnemyTiles.Count <= 1) return activeEnemyTiles[0];

        List<Tile> candidates = new List<Tile>(activeEnemyTiles);
        candidates.Remove(lastLaunchedTile);
        return candidates[Random.Range(0, candidates.Count)];
    }

    private void LaunchTile(Tile tile) {
        Rigidbody2D rb = tile.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 dir3D = new Vector3(randomDirection.x, randomDirection.y, 0);
        float statValue = CalculateDirectionalForce(tile, dir3D);
        float finalForce = statValue * chargeForce * enemyLaunchForceMultiplier;

        rb.AddForce(randomDirection * finalForce);
    }

    private float CalculateDirectionalForce(Tile tile, Vector3 direction) {
        Vector2 dir = new Vector2(direction.x, direction.y).normalized;
        Transform t = tile.transform;
        Vector2 up = t.up;
        Vector2 down = -t.up;
        Vector2 left = -t.right;
        Vector2 right = t.right;

        float dotUp = Vector2.Dot(dir, up);
        float dotDown = Vector2.Dot(dir, down);
        float dotLeft = Vector2.Dot(dir, left);
        float dotRight = Vector2.Dot(dir, right);

        Dictionary<string, float> alignment = new Dictionary<string, float> {
            { "Vigor", dotUp },
            { "Terror", dotDown },
            { "Beauty", dotLeft },
            { "Intellect", dotRight }
        };

        var sorted = new List<KeyValuePair<string, float>>(alignment);
        sorted.Sort((a, b) => b.Value.CompareTo(a.Value));

        if (Mathf.Abs(sorted[0].Value - sorted[1].Value) < 0.2f) {
            float attr1 = tile.GetAttribute((Attributes)System.Enum.Parse(typeof(Attributes), sorted[0].Key));
            float attr2 = tile.GetAttribute((Attributes)System.Enum.Parse(typeof(Attributes), sorted[1].Key));
            return (attr1 + attr2) / 2f;
        }

        return tile.GetAttribute((Attributes)System.Enum.Parse(typeof(Attributes), sorted[0].Key));
    }

    private void AssignPhysicsProperties(Tile tile) {
        Rigidbody2D rb = tile.GetComponent<Rigidbody2D>();
        if (rb == null) rb = tile.gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.mass = enemyTileMass;
        rb.drag = enemyTileDrag;
        rb.angularDrag = enemyTileAngularDrag;

        BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();
        if (collider == null) collider = tile.gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;

        float magicStat = tile.GetAttribute(Attributes.Magic);
        float normalizedMagic = Mathf.Clamp01(magicStat / maxStatValue);

        PhysicsMaterial2D bounceMat = new PhysicsMaterial2D("EnemyTileMat");
        bounceMat.bounciness = normalizedMagic;
        bounceMat.friction = 0f;
        collider.sharedMaterial = bounceMat;

        if (!tile.GetComponent<TileBounceFeedback>()) {
            TileBounceFeedback feedback = tile.gameObject.AddComponent<TileBounceFeedback>();
            feedback.flashEffect = flashEffectPrefab;
            feedback.bounceParticles = bounceParticlesPrefab;
        }

        if (!tile.GetComponent<ActiveTileCollisionHandler>()) {
            tile.gameObject.AddComponent<ActiveTileCollisionHandler>();
        }
    }
}

public static class ListExtensions {
    public static void Shuffle<T>(this IList<T> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}