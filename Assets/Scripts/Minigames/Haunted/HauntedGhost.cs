using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntedGhost : MonoBehaviour{
    public HauntedGhosts collection;
    
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Projectile")){
            collection.GhostHit(gameObject);
        }        
    }
}
