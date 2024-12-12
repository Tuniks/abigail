using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderer : MonoBehaviour{
    private SpriteRenderer spr;
    public bool isMobile = false;
    
    void Start(){
        spr = GetComponent<SpriteRenderer>();
        spr.sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    void Update(){
        if(!isMobile) return;
        spr.sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }
}
