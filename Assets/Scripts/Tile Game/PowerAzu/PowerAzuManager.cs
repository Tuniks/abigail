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
    public float chargeForce = 500f;

    [Header("Indicator Settings")]
    public GameObject indicatorSpritePrefab;
    public float minRange = 1f;         // Minimum distance indicator can be from tile
    public float maxRange = 3f;           // Maximum world distance for maxStatValue
    public float maxStatValue = 15f;      // The stat value that corresponds to maxRange

    private GameObject currentIndicator;
    private Vector3 indicatorTargetPos;
    private bool hasCharged = false;

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
                // Check if tile has stopped moving to allow charging again
                if (hasCharged && rb.velocity.magnitude < 0.05f) {
                    hasCharged = false;
                    Debug.Log("Tile has stopped. Ready to charge again.");
                }

                // Apply force only once when pressing E, if not already charged
                if (!hasCharged && Input.GetKeyDown(KeyCode.E)) {
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    worldMousePos.z = 0;

                    Vector3 direction = (worldMousePos - activeTile.transform.position).normalized;
                    float force = CalculateDirectionalForce(activeTile, direction) * chargeForce;

                    rb.AddForce(direction * force);
                    hasCharged = true;

                    Debug.Log($"Charging tile '{activeTile.name}' with direction: {direction}, force: {force}");
                }

                // Smoothly update indicator position based on stat and clamped distance
                if (currentIndicator != null) {
                    Vector3 mousePos = Input.mousePosition;
                    Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    worldMousePos.z = 0;

                    Vector3 dir = (worldMousePos - activeTile.transform.position).normalized;
                    float statValue = CalculateDirectionalForce(activeTile, dir);
                    float allowedRange = Mathf.Lerp(minRange, maxRange, Mathf.Clamp01(statValue / maxStatValue));

                    indicatorTargetPos = activeTile.transform.position + dir * allowedRange;
                    currentIndicator.transform.position = Vector3.Lerp(currentIndicator.transform.position, indicatorTargetPos, 10f * Time.deltaTime);
                }
            }
        }
    }

    public void OnAssignButtonPressed() {
        if (currentState != GameState.PlayerTurn)
            return;

        playerHand.AssignSelectedTiles();

        if (assignButton != null) {
            assignButton.interactable = false;
            assignButton.gameObject.SetActive(false);
        }

        foreach (Tile tile in playerHand.hand) {
            tile.gameObject.SetActive(false);
        }

        currentState = GameState.Play;
        StartPlay();
    }

    void StartPlay() {
        Debug.Log("Entering Play state");
    }

    public void SetActiveTile(Tile tile) {
        // Destroy any previous indicator
        if (currentIndicator != null) {
            Destroy(currentIndicator);
            currentIndicator = null;
        }

        activeTile = tile;

        Rigidbody2D rb = activeTile.GetComponent<Rigidbody2D>();
        if (rb == null) {
            rb = activeTile.gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.mass = 1;
            rb.drag = 2f;
            Debug.Log("Rigidbody2D added to tile: " + tile.name);
        } else {
            rb.drag = 2f;
        }

        BoxCollider2D collider = activeTile.GetComponent<BoxCollider2D>();
        if (collider == null) {
            collider = activeTile.gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("BoxCollider2D added to tile: " + tile.name);
        }

        collider.isTrigger = false;
        hasCharged = false;

        // Create indicator at cursor
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

        Vector2 up = Vector2.up;
        Vector2 down = Vector2.down;
        Vector2 left = Vector2.left;
        Vector2 right = Vector2.right;

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
