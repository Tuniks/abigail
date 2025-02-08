using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClayEnemyActiveSpot : MonoBehaviour {
    public Tile activeTile;
    public float size = 1.3f;

    public void ActivateTile(Tile _tile){
        if(activeTile != null)
            Destroy(activeTile.gameObject);

        activeTile = _tile;
        activeTile.transform.parent = transform;
        activeTile.transform.localPosition = Vector3.zero;
        activeTile.transform.localScale = Vector3.one * size;
        activeTile.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

        Collider[] colliders = activeTile.GetComponentsInChildren<Collider>();
        foreach(Collider col in colliders){
            col.enabled = false;
        }
    }
}