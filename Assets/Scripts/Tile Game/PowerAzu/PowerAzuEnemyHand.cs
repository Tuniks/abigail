using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerAzuEnemyHand : MonoBehaviour {
    public List<Tile> hand = new List<Tile>();
    public int minHandSize = 3;
    public int maxHandSize = 5;
    public GameObject tilePrefab; // ✅ assign a basic Tile prefab here

    private void Start() {
        BuildHand();
    }

    public void BuildHand() {
        hand.Clear();

        int numTiles = Random.Range(minHandSize, maxHandSize + 1);

        for (int i = 0; i < numTiles; i++) {
            GameObject newTileObj = Instantiate(tilePrefab, transform.position, Quaternion.identity);
            Tile newTile = newTileObj.GetComponent<Tile>();

            if (newTile != null) {
                hand.Add(newTile);

                newTile.transform.SetParent(transform);
                newTile.transform.localPosition = Vector3.zero;
                newTile.transform.localScale = Vector3.one;
                newTile.gameObject.SetActive(false); // ✅ stay hidden until assigned
            } else {
                Debug.LogError("Tile prefab missing Tile component!");
            }
        }
    }
}