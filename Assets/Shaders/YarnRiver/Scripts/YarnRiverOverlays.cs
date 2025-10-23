using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class YarnRiverOverlays : MonoBehaviour
{
    [System.Serializable]
    public class Overlay
    {
        [Tooltip("U center along flow (mesh UV X).")]
        public float uCenter = 0f;
        [Tooltip("V center across river (mesh UV Y).")]
        public float vCenter = 0.5f;
        [Tooltip("Length along U (bigger = longer downstream).")]
        public float uLength = 0.2f;
        [Tooltip("Width across V (bigger = wider across river).")]
        public float vWidth = 0.1f;

        [Tooltip("Downstream drift speed (+U).")]
        public float speedU = 0.0f;
        [Tooltip("Rotation in degrees around center.")]
        public float rotationDeg = 0f;
        [Range(0, 1)] public float alpha = 1f;
        public bool enabled = true;

        [Tooltip("Per-overlay tint (RGB) and alpha multiplier (A).")]
        public Color tint = Color.white;
    }

    public Renderer targetRenderer;
    [Tooltip("The material index on the renderer to drive (if multiple).")]
    public int materialIndex = 0;

    [Tooltip("Overlays (max 16). Texture comes from shader's _DecalTex.")]
    public List<Overlay> overlays = new List<Overlay>();

    // optional global controls mirrored in material for convenience
    [Range(0, 10)] public float edgeSoftness = 3f;
    public Color globalTint = Color.white;

    static readonly int ID_UVLen = Shader.PropertyToID("_DecalUVLen");
    static readonly int ID_Misc = Shader.PropertyToID("_DecalMisc");
    static readonly int ID_TintA = Shader.PropertyToID("_DecalTintA");
    static readonly int ID_Count = Shader.PropertyToID("_DecalCount");
    static readonly int ID_Soft = Shader.PropertyToID("_DecalEdgeSoft");
    static readonly int ID_GlobT = Shader.PropertyToID("_DecalGlobalTint");

    const int MAX = 16;

    void LateUpdate()
    {
        if (!targetRenderer) return;

        var mats = targetRenderer.sharedMaterials;
        if (materialIndex < 0 || materialIndex >= mats.Length) return;
        var mat = mats[materialIndex];
        if (!mat) return;

        int n = Mathf.Min(overlays.Count, MAX);

        var uvlen = new Vector4[MAX];
        var misc = new Vector4[MAX];
        var tintA = new Vector4[MAX];

        for (int i = 0; i < n; i++)
        {
            var o = overlays[i];
            uvlen[i] = new Vector4(o.uCenter, o.vCenter, Mathf.Max(1e-4f, o.uLength), Mathf.Max(1e-4f, o.vWidth));
            misc[i] = new Vector4(o.speedU, o.rotationDeg, Mathf.Clamp01(o.alpha), o.enabled ? 1f : 0f);
            var c = o.tint;
            tintA[i] = new Vector4(c.r, c.g, c.b, c.a);
        }

        mat.SetInt(ID_Count, n);
        mat.SetFloat(ID_Soft, edgeSoftness);
        mat.SetColor(ID_GlobT, globalTint);
        mat.SetVectorArray(ID_UVLen, uvlen);
        mat.SetVectorArray(ID_Misc, misc);
        mat.SetVectorArray(ID_TintA, tintA);
    }
}
