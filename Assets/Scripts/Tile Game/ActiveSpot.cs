using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ActiveSpot : MonoBehaviour{
    public Tile activeTile;
    public PlayerHand playerHand;
    public float size = 0.5f;
        public void ActivateTile(Tile _tile){
            if(activeTile != null)
                Destroy(activeTile.gameObject);

            activeTile = _tile;
            activeTile.transform.parent = transform;
            activeTile.transform.localPosition = Vector3.zero;
            // Scale the tile to half its size.
            activeTile.transform.localScale = Vector3.one * size;
            // Rotate the tile a bit randomly (between -10 and 10 degrees on the Z axis).
            activeTile.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));

            Collider[] colliders = activeTile.GetComponentsInChildren<Collider>();
            foreach(Collider col in colliders){
                col.enabled = false;
            }
        }

        void OnMouseDown(){
            if(playerHand != null && playerHand.isClay){
                playerHand.OnActiveSpotSelected(gameObject);
            }
        }
    }

