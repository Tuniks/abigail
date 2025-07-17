using UnityEngine;

[ExecuteAlways]
public class StarryQuadController : MonoBehaviour
{
    [Tooltip("Drag your URP star material here")]
    public Material starMaterial;
    [Range(0,20f)]  public float repelRadius   = 5f;
    [Range(0f,1f)]  public float repelStrength = 0.5f;

    void Update()
    {
        if (starMaterial == null || Camera.main == null) 
            return;

        Vector2 uv = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        starMaterial.SetVector("_MouseUV",      new Vector4(uv.x, uv.y, 0, 0));
        starMaterial.SetFloat("_RepelRadius",   repelRadius);
        starMaterial.SetFloat("_RepelStrength", repelStrength);
    }
}