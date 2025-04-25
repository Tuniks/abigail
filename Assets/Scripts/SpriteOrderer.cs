using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOrderer : MonoBehaviour {
    private SpriteRenderer spr;
    public bool isMobile = false;
    public int offset = 0;
    
    void Start(){
        spr = GetComponent<SpriteRenderer>();
        spr.sortingOrder = -Mathf.RoundToInt(transform.position.y) + offset;
    }

    void Update(){
        if(!isMobile) return;
        spr.sortingOrder = -Mathf.RoundToInt(transform.position.y) + offset;
    }
}