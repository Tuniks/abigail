using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHandPA : MonoBehaviour, IPlayerHand {
    public List<Tile> hand;
    public int maxHandSize = 9;

    // List of active spots to assign selected tiles.
    public List<ActiveSpot> activeSpots;

    public float tileSize = 2f;
    public float tileSpacing = 0.5f;

    // Grid parameters (3 columns and 3 rows for 9 tiles)
    public int gridColumns = 3;
    public int gridRows = 3;

    // Selection constraints: the player must select between these many tiles.
    public int minSelection = 3;
    public int maxSelection = 5;
    public List<Tile> selectedTiles = new List<Tile>();

    void Start() {
        BuildHand();
        DrawHandGrid();
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

    // Draws the hand as a grid using x and y axes for a 2D game.
    void DrawHandGrid() {
        // Calculate overall grid dimensions.
        float totalWidth = gridColumns * tileSize + (gridColumns - 1) * tileSpacing;
        float totalHeight = gridRows * tileSize + (gridRows - 1) * tileSpacing;

        // Starting positions so that the grid is centered.
        float startX = -totalWidth / 2 + tileSize / 2;
        float startY = totalHeight / 2 - tileSize / 2;  // using y for vertical placement

        // Place tiles using nested loops.
        for (int row = 0; row < gridRows; row++) {
            for (int col = 0; col < gridColumns; col++) {
                int index = row * gridColumns + col;
                if (index < hand.Count) {
                    float posX = startX + col * (tileSize + tileSpacing);
                    float posY = startY - row * (tileSize + tileSpacing);
                    Vector3 newPos = new Vector3(posX, posY, 0); // z is 0 for 2D
                    hand[index].transform.localPosition = newPos;
                    Debug.Log("Tile " + index + " positioned at: " + newPos);
                }
            }
        }
    }

    // Called when a tile is clicked.
    // This toggles selection: if the tile is already selected, deselect it;
    // otherwise, select it (up to the maximum allowed).
    public void Activate(Tile tile) {
        if (!hand.Contains(tile))
            return;

        if (selectedTiles.Contains(tile)) {
            // Deselect the tile.
            selectedTiles.Remove(tile);
            SetTileTransparency(tile, 1f);
        } else {
            if (selectedTiles.Count < maxSelection) {
                selectedTiles.Add(tile);
                SetTileTransparency(tile, 0.5f);
            } else {
                Debug.Log("Maximum selection reached");
            }
        }
    }

    // Called by an assignable button in the scene.
    // Checks that between minSelection and maxSelection tiles are selected,
    // shuffles available active spots, and assigns each selected tile.
    public void AssignSelectedTiles() {
        if (selectedTiles.Count < minSelection || selectedTiles.Count > maxSelection) {
            Debug.Log("Please select between " + minSelection + " and " + maxSelection + " tiles.");
            return;
        }

        // Make a copy of active spots and shuffle the copy.
        List<ActiveSpot> spotsCopy = new List<ActiveSpot>(activeSpots);
        Shuffle(spotsCopy);

        int assignCount = Mathf.Min(selectedTiles.Count, spotsCopy.Count);
        for (int i = 0; i < assignCount; i++) {
            Tile tile = selectedTiles[i];
            hand.Remove(tile);
            SetTileTransparency(tile, 1f); // Reset transparency, if needed.
            spotsCopy[i].ActivateTile(tile);
        }
        selectedTiles.Clear();
        DrawHandGrid();
    }

    // Fisher-Yates shuffle for a generic list.
    void Shuffle<T>(List<T> list) {
        for (int i = 0; i < list.Count; i++) {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Utility to adjust the transparency of a tile's materials.
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

    // Helper method to retrieve the selected tiles.
    // This provides encapsulation while returning a copy of the current selection.
    public List<Tile> GetSelectedTiles() {
        return new List<Tile>(selectedTiles);
    }
}
