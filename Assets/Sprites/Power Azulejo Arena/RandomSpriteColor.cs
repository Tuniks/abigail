using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSpriteColor : MonoBehaviour
{
    [Header("Optional preset colors (leave empty to use random HSV)")]
    [SerializeField] private Color[] presetColors;

    [Header("HSV Ranges (used only if no presets)")]
    [SerializeField] private Vector2 hueRange        = new Vector2(0f, 1f);
    [SerializeField] private Vector2 saturationRange = new Vector2(0.6f, 1f);
    [SerializeField] private Vector2 valueRange      = new Vector2(0.6f, 1f);

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        var baseAlpha = sr.color.a;

        Color chosen;
        if (presetColors != null && presetColors.Length > 0)
        {
            chosen = presetColors[Random.Range(0, presetColors.Length)];
        }
        else
        {
            float h = Random.Range(hueRange.x, hueRange.y);
            float s = Random.Range(saturationRange.x, saturationRange.y);
            float v = Random.Range(valueRange.x, valueRange.y);
            chosen = Color.HSVToRGB(h, s, v);
        }

        chosen.a = baseAlpha;
        sr.color = chosen;
    }
}