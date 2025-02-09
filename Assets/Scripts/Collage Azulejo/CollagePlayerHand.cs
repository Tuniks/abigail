using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollagePlayerHand : MonoBehaviour
{
    public List<Tile> hand;
    public int maxHandSize;

    public float tileSize = 2f;
    public float tileSpacing = .5f;
    

    void Start()
    {
        BuildHand();
        DrawHand();
    }

    public void BuildHand()
    {
        PlayerInventory inv = PlayerInventory.Instance;
        if (inv != null)
            hand = inv.GetHandOfTiles(maxHandSize);
        if (hand.Count < maxHandSize)
            GenerateRandomHand(maxHandSize - hand.Count);
        SetHand();
    }

    void GenerateRandomHand(int handCount)
    {
        hand = new List<Tile>();
        for (int i = 0; i < handCount; i++)
        {
            Tile tile = TileMaker.instance.GetRandomTile();
            hand.Add(tile);
        }
    }

    private void SetHand()
    {
        foreach (Tile tile in hand)
        {
            //tile.SetHand(this);
            tile.transform.SetParent(transform);
            tile.gameObject.SetActive(true);
        }
    }

    void DrawHand()
    {
        // Calculate the total width and height for the grid.
        float totalWidth = tileSize * 3 + tileSpacing * 2;  // 3 tiles per row + spacing between tiles
        float totalHeight = tileSize * 3 + tileSpacing * 2; // 3 rows of tiles + spacing between rows

        // Calculate the starting x and y positions to center the grid.
        float startX = -11.05f;
        float startY = -0.28f;

        // Loop through the hand and arrange tiles in a 3x3 grid.
        for (int i = 0; i < hand.Count; i++)
        {
            int row = i / 3;  // Calculate which row the tile belongs to.
            int col = i % 3;  // Calculate which column the tile belongs to.

            // Set the tile's position based on the row and column.
            float posX = startX + col * (tileSize + tileSpacing);
            float posY = startY - row * (tileSize + tileSpacing);

            hand[i].transform.localPosition = new Vector3(posX, posY, 0);
        }
    }

}
