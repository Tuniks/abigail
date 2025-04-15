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
    
    [Header("Collision Transfer")]
    [Tooltip("Extra force that will be applied to the first tile collided with upon launch.")]
    public float collisionTransferForce = 500f;

    private GameObject currentIndicator;
    private Vector3 indicatorTargetPos;
    private bool hasCharged = false;
    private float chargeTimer = 0f;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start() {
        currentState = GameState.PlayerTurn;
        if (assignButton != null)
            assignButton.onClick.AddListener(OnAssignButtonPressed);
    }

    void Update() {
        if (currentState == GameState.Play && activeTile != null) {
            Rigidbody2D rb = activeTile.GetComponent<Rigidbody2D>();
            if (rb != null) {

                // If the player presses Escape, cancel/deselect the active tile.
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    Debug.Log("Escape pressed: Cancelling active tile.");
                    CancelActiveTile();
                    return;
                }

                // If the tile has stopped moving, reset charge state.
                if (hasCharged && rb.velocity.magnitude < 0.05f) {
                    hasCharged = false;
                    chargeTimer = 0f;
                    Debug.Log("Tile has stopped. Ready to charge again.");
                }

                // Use the E key to charge the tile.
                if (!hasCharged && Input.GetKey(KeyCode.E)) {
                    chargeTimer += Time.deltaTime;
                    chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);
                }

                // When E is released, launch the tile using the accumulated charge.
                if (!hasCharged && Input.GetKeyUp(KeyCode.E)) {
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    worldMousePos.z = 0;
                    Vector3 direction = (worldMousePos - activeTile.transform.position).normalized;
                    float statValue = CalculateDirectionalForce(activeTile, direction);
                    float chargePercent = chargeTimer / maxChargeTime;
                    float finalForce = statValue * chargePercent * chargeForce;
                    rb.AddForce(direction * finalForce);
                    hasCharged = true;
                    Debug.Log($"Force: {finalForce}, Charge %: {chargePercent}, Stat: {statValue}, Dir: {direction}");
                }

                // Update the indicator's position and scale.
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
                    float finalScale = baseIndicatorScale * chargePercent * statPercent;
                    finalScale = Mathf.Clamp(finalScale, 0.2f, 2f);
                    currentIndicator.transform.localScale = new Vector3(finalScale, finalScale, 1f);
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    currentIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
    }

    // Modified OnAssignButtonPressed:
    // 1. Call the hand’s AssignSelectedTiles() method (which uses ActiveSpot functionality).
    // 2. Disable any remaining (non-chosen) tiles.
    // 3. Collect the chosen tiles from the ActiveSpot objects (using their activeTile property).
    // 4. Apply physics to the chosen tiles.
    // 5. Set the first chosen tile as the active tile and add the collision handler.
    public void OnAssignButtonPressed() {
        if (currentState != GameState.PlayerTurn)
            return;

        // Let the hand assign selected tiles into active spots.
        playerHand.AssignSelectedTiles();

        // Disable any tiles still remaining in the player's hand (non-chosen ones).
        foreach (Tile tile in playerHand.hand) {
            tile.gameObject.SetActive(false);
        }

        // Collect the tiles from each ActiveSpot (using activeTile).
        List<Tile> chosenTiles = new List<Tile>();
        foreach (ActiveSpot spot in playerHand.activeSpots) {
            if (spot.activeTile != null) {
                // Ensure the chosen tile is enabled.
                spot.activeTile.gameObject.SetActive(true);
                chosenTiles.Add(spot.activeTile);
            }
        }

        // Apply physics properties to all chosen tiles.
        foreach (Tile tile in chosenTiles) {
            AssignPhysicsProperties(tile);
        }

        // Optionally set the first chosen tile as the active tile.
        if (chosenTiles.Count > 0) {
            activeTile = chosenTiles[0];

            // Add the collision transfer component if not already there.
            ActiveTileCollisionHandler handler = activeTile.GetComponent<ActiveTileCollisionHandler>();
            if (handler == null) {
                handler = activeTile.gameObject.AddComponent<ActiveTileCollisionHandler>();
            }
            handler.collisionTransferForce = collisionTransferForce;
        }

        if (assignButton != null) {
            assignButton.interactable = false;
            assignButton.gameObject.SetActive(false);
        }

        currentState = GameState.Play;
        StartPlay();
    }

    // Adds physics (and collision) components to a tile.
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
    }

    void StartPlay() {
        Debug.Log("Entering Play state");
        // Additional play-state initialization can be added here.
    }

    public void SetActiveTile(Tile tile) {
        if (currentIndicator != null) {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
        activeTile = tile;
        if (indicatorSpritePrefab != null) {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            worldMousePos.z = 0;
            currentIndicator = Instantiate(indicatorSpritePrefab, worldMousePos, Quaternion.identity);
            indicatorTargetPos = worldMousePos;
        }
        Debug.Log("Active tile set: " + tile.name);
    }

    public void CancelActiveTile() {
        if (activeTile != null) {
            Debug.Log("Active tile cancelled: " + activeTile.name);
            activeTile = null;
        }
        if (currentIndicator != null) {
            Destroy(currentIndicator);
            currentIndicator = null;
        }
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
