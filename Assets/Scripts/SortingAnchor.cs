using UnityEngine;

[ExecuteAlways]
public class SortingAnchor : MonoBehaviour {
    public Transform sortingPoint;
    public float yOffsetBuffer = 0f;
    public float sortingModifier = 100f;

    private SpriteRenderer spr;

    void Awake() {
        spr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (sortingPoint == null) return;

        float yValue = sortingPoint.position.y + yOffsetBuffer;
        spr.sortingOrder = -Mathf.RoundToInt(yValue * sortingModifier);
    }
}