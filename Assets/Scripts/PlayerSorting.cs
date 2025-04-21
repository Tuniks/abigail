using UnityEngine;

[ExecuteAlways]
public class PlayerSorting : MonoBehaviour {
    public float sortingModifier = 100f;
    public float yOffset = 0f;

    private SpriteRenderer spr;

    void Awake() {
        spr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        float yValue = transform.position.y + yOffset;
        spr.sortingOrder = -Mathf.RoundToInt(yValue * sortingModifier);
    }
}