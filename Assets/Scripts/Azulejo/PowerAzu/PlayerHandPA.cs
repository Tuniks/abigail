using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHandPA : MonoBehaviour, IPlayerHand {
    public List<Tile> hand;
    public int maxHandSize = 9;
    public int GetSelectedTileCount() { return selectedTiles.Count; }
    public UnityAction OnSelectionChanged;

    public List<ActiveSpot> activeSpots;
    public float tileSize = 2f;
    public float tileSpacing = 0.5f;
    public int gridColumns = 3;
    public int gridRows = 3;
    public int minSelection = 3;
    public int maxSelection = 5;
    public List<Tile> selectedTiles = new List<Tile>();

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip selectSound;
    public AudioClip deselectSound;

    [Header("Background FX")]
    public GameObject backgroundPrefab;
    private GameObject backgroundInstance;

    void Start() {
        BuildHand();
        DrawHandGrid();
        SpawnBackground();
    }

    public void BuildHand() {
        PlayerInventory inv = PlayerInventory.Instance;
        if (inv != null)
            hand = inv.GetHandOfTiles(maxHandSize);
        if (hand.Count < maxHandSize)
            GenerateRandomHand(maxHandSize - hand.Count);
        SetHand();
    }

    void GenerateRandomHand(int count) {
        if (hand == null)
            hand = new List<Tile>();
        for (int i = 0; i < count; i++) {
            Tile tile = TileMaker.instance.GetRandomTile();
            hand.Add(tile);
        }
    }

    void SetHand() {
        foreach (Tile tile in hand) {
            tile.SetHand(this);
            tile.transform.SetParent(transform);
            tile.gameObject.SetActive(true);
        }
    }

    void DrawHandGrid() {
        float totalWidth = gridColumns * tileSize + (gridColumns - 1) * tileSpacing;
        float totalHeight = gridRows * tileSize + (gridRows - 1) * tileSpacing;
        float startX = -totalWidth / 2 + tileSize / 2;
        float startY = totalHeight / 2 - tileSize / 2;

        for (int row = 0; row < gridRows; row++) {
            for (int col = 0; col < gridColumns; col++) {
                int index = row * gridColumns + col;
                if (index < hand.Count) {
                    float posX = startX + col * (tileSize + tileSpacing);
                    float posY = startY - row * (tileSize + tileSpacing);
                    hand[index].transform.localPosition = new Vector3(posX, posY, 0);
                }
            }
        }
    }

    public void Activate(Tile tile) {
        if (!hand.Contains(tile)) return;

        if (selectedTiles.Contains(tile)) {
            selectedTiles.Remove(tile);
            SetTileTransparency(tile, 1f);
            PlaySound(deselectSound);
        } else {
            if (selectedTiles.Count < maxSelection) {
                selectedTiles.Add(tile);
                SetTileTransparency(tile, 0.5f);
                PlaySound(selectSound);
            }
        }

        OnSelectionChanged?.Invoke();
    }

    public void AssignSelectedTiles() {
        if (selectedTiles.Count < minSelection || selectedTiles.Count > maxSelection) {
            Debug.Log("Please select between " + minSelection + " and " + maxSelection + " tiles.");
            return;
        }

        List<ActiveSpot> spotsCopy = new List<ActiveSpot>(activeSpots);
        Shuffle(spotsCopy);

        int assignCount = Mathf.Min(selectedTiles.Count, spotsCopy.Count);
        for (int i = 0; i < assignCount; i++) {
            Tile tile = selectedTiles[i];
            hand.Remove(tile);
            SetTileTransparency(tile, 1f);
            spotsCopy[i].ActivateTile(tile);
        }

        selectedTiles.Clear();
        DrawHandGrid();
        AnimateAndDestroyBackground();
    }

    void Shuffle<T>(List<T> list) {
        for (int i = 0; i < list.Count; i++) {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void SetTileTransparency(Tile tile, float alpha) {
        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            foreach (Material mat in renderer.materials) {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }

    public List<Tile> GetSelectedTiles() {
        return new List<Tile>(selectedTiles);
    }

    private void PlaySound(AudioClip clip) {
        if (audioSource != null && clip != null) {
            audioSource.PlayOneShot(clip);
        }
    }

    private void SpawnBackground() {
        if (backgroundPrefab != null) {
            backgroundInstance = Instantiate(backgroundPrefab, transform.position, Quaternion.identity);
            backgroundInstance.transform.SetParent(transform);
            backgroundInstance.transform.localPosition = Vector3.zero;
        }
    }

    private void AnimateAndDestroyBackground() {
        if (backgroundInstance == null) return;
        StartCoroutine(AnimateThenDestroy(backgroundInstance));
    }

    private IEnumerator AnimateThenDestroy(GameObject obj) {
        float bounceTime = 0.15f;
        float shrinkTime = 0.3f;
        Vector3 original = obj.transform.localScale;
        Vector3 larger = original * 1.2f;

        float t = 0;
        while (t < bounceTime) {
            obj.transform.localScale = Vector3.Lerp(original, larger, t / bounceTime);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        while (t < shrinkTime) {
            obj.transform.localScale = Vector3.Lerp(larger, Vector3.zero, t / shrinkTime);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }
}
