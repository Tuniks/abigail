using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOrderer : MonoBehaviour {
    private SpriteRenderer spr;

    [Header("Sorting Options")]
    public bool isMobile = false;
    public float yOffsetBuffer = 0f;     // Custom buffer to adjust sorting order
    public float sortingModifier = 100f; // Precision scale for sorting

    void Start() {
        spr = GetComponent<SpriteRenderer>();
        UpdateSortingOrder();
    }

    void Update() {
        if (!isMobile) return;
        UpdateSortingOrder();
    }

    void UpdateSortingOrder() {
        // Use the sprite's pivot point (typically center) minus half-height to estimate bottom
        float spriteBottom = transform.position.y - (spr.sprite.bounds.extents.y * transform.lossyScale.y);
        
        // Apply buffer and sorting modifier
        float adjustedY = spriteBottom + yOffsetBuffer;
        spr.sortingOrder = -Mathf.RoundToInt(adjustedY * sortingModifier);
    }
}