using UnityEngine;

public class PowerActiveSpot : MonoBehaviour {
    public Tile activeTile;

    public void ActivateTile(Tile tile) {
        activeTile = tile;
        tile.transform.position = transform.position;
        tile.transform.rotation = Quaternion.identity;
        tile.transform.SetParent(this.transform);
        tile.gameObject.SetActive(true);
    }
}