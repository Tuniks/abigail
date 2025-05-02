using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAzuManager : MonoBehaviour {
    public static EnemyAzuManager instance;

    [Header("Enemy Setup")]
    public List<GameObject> enemyTilePrefabs;
    public List<PowerActiveSpot> enemyActiveSpots;
    public List<Transform> targets;

    [Header("Physics")]
    public float chargeForce = 1000f;
    public float maxStatValue = 15f;
    public float enemyTileMass = 0.5f;
    public float enemyTileDrag = 1f;
    public float enemyTileAngularDrag = 5f;
    public float enemyLaunchForceMultiplier = 0.25f;
    public float launchDelayMin = 1f;
    public float launchDelayMax = 2f;

    [Header("Bounce Feedback")]
    public GameObject flashEffectPrefab;
    public GameObject bounceParticlesPrefab;

    [Header("Visuals")]
    public GameObject baseSpritePrefab;

    [Header("Collision Transfer")]
    public float collisionTransferForce = 5f;

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
        int tileCount = Mathf.Min(enemyTilePrefabs.Count, enemyActiveSpots.Count);

        for (int i = 0; i < tileCount; i++) {
            GameObject prefab = enemyTilePrefabs[i];
            PowerActiveSpot spot = enemyActiveSpots[i];

            if (prefab != null && spot != null) {
                GameObject enemyTileObj = Instantiate(prefab, spot.transform.position, Quaternion.identity);
                Tile enemyTile = enemyTileObj.GetComponent<Tile>();

                if (enemyTile == null) continue;

                SetupTile(enemyTile);
                spot.ActivateTile(enemyTile);

                Rigidbody2D rb = enemyTile.GetComponent<Rigidbody2D>();
                if (rb != null) {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }

                activeEnemyTiles.Add(enemyTile);
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (!launching) {
            StartCoroutine(LaunchTilesContinuously());
        }
    }

    private void SetupTile(Tile tile) {
        tile.transform.localScale = Vector3.one;
        tile.isEnemy = true;

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

        PhysicsMaterial2D bounceMat = new PhysicsMaterial2D("EnemyTileMat") {
            bounciness = normalizedMagic,
            friction = 0f
        };
        collider.sharedMaterial = bounceMat;

        if (!tile.GetComponent<TileBounceFeedback>()) {
            TileBounceFeedback feedback = tile.gameObject.AddComponent<TileBounceFeedback>();
            feedback.flashEffect = flashEffectPrefab;
            feedback.bounceParticles = bounceParticlesPrefab;
        }

        if (!tile.GetComponent<ActiveTileCollisionHandler>()) {
            ActiveTileCollisionHandler handler = tile.gameObject.AddComponent<ActiveTileCollisionHandler>();
            handler.collisionTransferForce = collisionTransferForce;
        }

        if (!tile.GetComponent<EnemyTileTargetTracker>()) {
            tile.gameObject.AddComponent<EnemyTileTargetTracker>();
        }

        if (baseSpritePrefab != null) {
            GameObject baseSprite = Instantiate(baseSpritePrefab, tile.transform.position, Quaternion.identity);
            baseSprite.transform.SetParent(tile.transform);
            baseSprite.transform.localPosition = Vector3.zero;
            baseSprite.transform.localScale = Vector3.one * 1.29f;
            baseSprite.transform.SetAsFirstSibling();
        }
    }

    private IEnumerator LaunchTilesContinuously() {
        launching = true;

        while (true) {
            if (activeEnemyTiles.Count > 0) {
                Tile tile = GetNextTileToLaunch();
                if (tile != null) {
                    var tracker = tile.GetComponent<EnemyTileTargetTracker>();
                    if (tracker != null && !tracker.IsTouchingTarget()) {
                        LaunchTile(tile);
                        lastLaunchedTile = tile;
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(launchDelayMin, launchDelayMax));
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

        Vector3 targetDirection;

        if (targets.Count > 0 && Random.value < 0.75f) {
            Transform closest = targets.OrderBy(t => Vector3.Distance(tile.transform.position, t.position)).FirstOrDefault();
            targetDirection = (closest.position - tile.transform.position).normalized;
        } else {
            targetDirection = Random.insideUnitCircle.normalized;
        }

        float statValue = CalculateDirectionalForce(tile, targetDirection);
        float finalForce = statValue * chargeForce * enemyLaunchForceMultiplier;

        rb.AddForce(targetDirection * finalForce);
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

        var sorted = alignment.OrderByDescending(pair => pair.Value).ToList();

        if (Mathf.Abs(sorted[0].Value - sorted[1].Value) < 0.2f) {
            float attr1 = tile.GetAttribute((Attributes)System.Enum.Parse(typeof(Attributes), sorted[0].Key));
            float attr2 = tile.GetAttribute((Attributes)System.Enum.Parse(typeof(Attributes), sorted[1].Key));
            return (attr1 + attr2) / 2f;
        }

        return tile.GetAttribute((Attributes)System.Enum.Parse(typeof(Attributes), sorted[0].Key));
    }
}
