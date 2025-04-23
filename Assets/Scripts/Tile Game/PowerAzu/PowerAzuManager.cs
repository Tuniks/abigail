using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerAzuManager : MonoBehaviour {
    public static PowerAzuManager instance;

    public enum GameState {
        PlayerTurn,
        Play
    }
    public GameState currentState;

    public PlayerHandPA playerHand;
    public Button assignButton;
    public Tile activeTile;

    [Header("Charging Settings")]
    public float chargeForce = 1000f;
    public float maxChargeTime = 1f;

    [Header("Indicator Settings")]
    public GameObject indicatorSpritePrefab;
    public float minRange = 1f;
    public float maxRange = 3f;
    public float maxStatValue = 15f;
    public float baseIndicatorScale = 1f;

    [Header("Bounce Feedback")]
    public GameObject flashEffectPrefab;
    public GameObject bounceParticlesPrefab;

    [Header("Collision and Launch")]
    public float transferForce = 5f;
    private float launchInputDelay = 0.5f;

    private GameObject currentIndicator;
    private Vector3 indicatorTargetPos;
    private bool hasCharged = false;
    private float chargeTimer = 0f;

    private SpriteRenderer activeTileRenderer;
    private Color originalTileColor;

    private bool isInLaunchMode = false;
    private float launchModeStartTime = 0f;

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        currentState = GameState.PlayerTurn;
        if (assignButton != null)
            assignButton.onClick.AddListener(OnAssignButtonPressed);
    }

    private void Update() {
        if (currentState == GameState.Play && activeTile != null && isInLaunchMode) {
            if (Time.time > launchModeStartTime + launchInputDelay) {
                Rigidbody2D rb = activeTile.GetComponent<Rigidbody2D>();
                if (rb != null) {
                    if (Input.GetKeyDown(KeyCode.Escape)) {
                        CancelActiveTile();
                        return;
                    }

                    if (hasCharged && rb.velocity.magnitude < 0.05f) {
                        hasCharged = false;
                        chargeTimer = 0f;
                    }

                    if (!hasCharged && Input.GetKey(KeyCode.E)) {
                        chargeTimer += Time.deltaTime;
                        chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);
                    }

                    if (!hasCharged && Input.GetKeyUp(KeyCode.E)) {
                        BeginTileLaunch();
                    }

                    if (currentIndicator != null) {
                        Vector3 mousePos = Input.mousePosition;
                        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
                        worldMousePos.z = 0;
                        Vector3 dir = (worldMousePos - activeTile.transform.position).normalized;
                        float statValue = CalculateDirectionalForce(activeTile, dir);
                        float statPercent = Mathf.Clamp01(statValue / maxStatValue);
                        float chargePercent = Mathf.Clamp01(chargeTimer / maxChargeTime);
                        float allowedRange = Mathf.Lerp(minRange, maxRange, statPercent);
                        indicatorTargetPos = activeTile.transform.position + dir * allowedRange;
                        currentIndicator.transform.position = Vector3.Lerp(currentIndicator.transform.position, indicatorTargetPos, 10f * Time.deltaTime);

                        float scaleCurve = 0.3f + 1.7f * chargePercent;
                        float finalScale = baseIndicatorScale * scaleCurve * statPercent;
                        finalScale = Mathf.Clamp(finalScale, 0.2f, 3f);
                        currentIndicator.transform.localScale = new Vector3(finalScale, finalScale, 1f);

                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        currentIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                }
            }
        }
    }

    public void BeginTileLaunch() {
        if (activeTile == null) return;
        Rigidbody2D rb = activeTile.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        worldMousePos.z = 0;
        Vector3 direction = (worldMousePos - activeTile.transform.position).normalized;
        float statValue = CalculateDirectionalForce(activeTile, direction);
        float chargePercent = chargeTimer / maxChargeTime;
        float finalForce = statValue * chargePercent * chargeForce;
        rb.AddForce(direction * finalForce);

        hasCharged = true;
        CancelActiveTile();
        Debug.Log($"Force: {finalForce}, Charge %: {chargePercent}, Stat: {statValue}, Dir: {direction}");
    }

    public void EnterLaunchMode() {
        isInLaunchMode = true;
        chargeTimer = 0f;
        hasCharged = false;
        launchModeStartTime = Time.time;

        if (indicatorSpritePrefab != null && currentIndicator == null && activeTile != null) {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            worldMousePos.z = 0;

            currentIndicator = Instantiate(indicatorSpritePrefab, worldMousePos, Quaternion.identity);

            SpriteRenderer sr = currentIndicator.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sortingOrder = 999;
                sr.sortingLayerName = "UI";
            }
        }
    }

    public void OnAssignButtonPressed() {
        if (currentState != GameState.PlayerTurn) return;

        playerHand.AssignSelectedTiles();

        foreach (Tile tile in playerHand.hand) {
            tile.gameObject.SetActive(false);
        }

        List<Tile> chosenTiles = new List<Tile>();
        foreach (ActiveSpot spot in playerHand.activeSpots) {
            if (spot.activeTile != null) {
                spot.activeTile.gameObject.SetActive(true);
                chosenTiles.Add(spot.activeTile);
            }
        }

        foreach (Tile tile in chosenTiles) {
            AssignPhysicsProperties(tile);
        }

        if (chosenTiles.Count > 0) {
            SetActiveTile(chosenTiles[0]);
        }

        if (assignButton != null) {
            assignButton.interactable = false;
            assignButton.gameObject.SetActive(false);
        }

        currentState = GameState.Play;
    }

    void AssignPhysicsProperties(Tile tile) {
        Rigidbody2D rb = tile.GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = tile.gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.mass = 0.5f;
        rb.drag = 1f;
        rb.angularDrag = 5f;

        BoxCollider2D collider = tile.GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = tile.gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;

        float magicStat = tile.GetAttribute(Attributes.Magic);
        float normalizedMagic = Mathf.Clamp01(magicStat / maxStatValue);
        PhysicsMaterial2D bounceMat = new PhysicsMaterial2D("TileBounceMat");
        bounceMat.bounciness = normalizedMagic;
        bounceMat.friction = 0f;
        collider.sharedMaterial = bounceMat;

        if (!tile.gameObject.GetComponent<TileBounceFeedback>()) {
            TileBounceFeedback feedback = tile.gameObject.AddComponent<TileBounceFeedback>();
            feedback.flashEffect = flashEffectPrefab;
            feedback.bounceParticles = bounceParticlesPrefab;
        }

        if (!tile.gameObject.GetComponent<ActiveTileCollisionHandler>()) {
            tile.gameObject.AddComponent<ActiveTileCollisionHandler>();
        }
    }

    public void SetActiveTile(Tile tile) {
        if (activeTile == tile) return;

        if (activeTileRenderer != null)
            activeTileRenderer.color = originalTileColor;

        if (currentIndicator != null) {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        activeTile = tile;

        activeTileRenderer = tile.GetComponent<SpriteRenderer>();
        if (activeTileRenderer != null) {
            originalTileColor = activeTileRenderer.color;
            activeTileRenderer.color = new Color(1f, 0.7f, 0.7f);
        }

        TileActionMenu.instance.ShowMenu(tile);
        Debug.Log("Active tile set: " + tile.name);
    }

    public void CancelActiveTile() {
        if (activeTileRenderer != null)
            activeTileRenderer.color = originalTileColor;

        if (activeTile != null) {
            Debug.Log("Active tile cancelled: " + activeTile.name);
            activeTile = null;
        }

        activeTileRenderer = null;

        if (currentIndicator != null) {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        isInLaunchMode = false;
        TileActionMenu.instance.HideMenu();
    }

    float CalculateDirectionalForce(Tile tile, Vector3 direction) {
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
