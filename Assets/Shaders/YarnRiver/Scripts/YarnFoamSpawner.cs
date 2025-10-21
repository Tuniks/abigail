using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class YarnFoamSpawner : MonoBehaviour
{
    public YarnRiverStrip river;
    public Material foamMaterial;
    public bool leftBank = true;
    public bool rightBank = true;

    [Header("Distribution")]
    [Range(0.25f, 5f)] public float spacing = 1.0f;
    [Range(0.1f, 5f)] public float emitterWidth = 0.25f;
    [Range(0f, 2f)] public float bankInset = 0.05f;
    public int maxParticlesPerEmitter = 64;

    [Header("Motion & Look")]
    public float downstreamSpeed = 1.0f;
    public float lifetime = 1.5f;
    public Vector2 sizeRange = new Vector2(0.08f, 0.18f);

    [Header("Updates")]
    public bool rebuildEachFrame = true;

    readonly List<ParticleSystem> emitters = new List<ParticleSystem>();

    void OnEnable() { Build(); }
    void OnDisable() { Clear(); }
    void Update(){ if (rebuildEachFrame) Build(); }

    public void Build()
    {
        if (!river)
        {
            river = GetComponent<YarnRiverStrip>();
            if (!river) return;
        }
        MeshFilter mf = GetComponent<MeshFilter>();
        if (!mf || !mf.sharedMesh) return;
        Mesh mesh = mf.sharedMesh;

        var verts = mesh.vertices;
        if (verts == null || verts.Length < 4) return;

        int samples = verts.Length / 2;
        var lefts = new Vector3[samples];
        var rights = new Vector3[samples];

        for (int s = 0; s < samples; s++)
        {
            var left = transform.TransformPoint(verts[s*2 + 0]);
            var right = transform.TransformPoint(verts[s*2 + 1]);

            Vector3 center = (left + right) * 0.5f;
            Vector3 leftDir = (center - left).normalized;
            Vector3 rightDir = (center - right).normalized;
            left += leftDir * bankInset;
            right += rightDir * bankInset;

            lefts[s] = left;
            rights[s] = right;
        }

        Clear();

        float accum = 0f;
        Vector3 prev = lefts[0];

        for (int s = 1; s < samples; s++)
        {
            if (leftBank) PlaceBetween(ref accum, ref prev, lefts[s-1], lefts[s]);
            if (rightBank) PlaceBetween(ref accum, ref prev, rights[s-1], rights[s]);
        }
    }

    void PlaceBetween(ref float accum, ref Vector3 prev, Vector3 a, Vector3 b)
    {
        if (accum == 0f) prev = a;
        float segLen = Vector3.Distance(a, b);
        Vector3 dir = (b - a).normalized;
        float d = 0f;
        while (d + accum < segLen)
        {
            float t = (d + accum) / segLen;
            Vector3 p = Vector3.Lerp(a, b, t);
            CreateEmitter(p, dir);
            d += spacing;
        }
        accum = (d + accum) - segLen;
        prev = b;
    }

    void CreateEmitter(Vector3 pos, Vector3 alongDir)
    {
        var go = new GameObject("FoamEmitter");
        go.transform.SetParent(transform, worldPositionStays: true);
        go.transform.position = pos;
        go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

        var ps = go.AddComponent<ParticleSystem>();
        var r = ps.GetComponent<ParticleSystemRenderer>();
        if (foamMaterial) r.sharedMaterial = foamMaterial;
        r.renderMode = ParticleSystemRenderMode.Billboard;
        r.sortingFudge = 2f;

        var main = ps.main;
        main.loop = true;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = lifetime;
        main.startSpeed = 0f;
        main.startSize = new ParticleSystem.MinMaxCurve(sizeRange.x, sizeRange.y);
        main.startColor = Color.white;
        main.maxParticles = maxParticlesPerEmitter;
        main.playOnAwake = true;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = Mathf.Max(4f, maxParticlesPerEmitter / lifetime * 0.5f);

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(emitterWidth, 0.02f, 0.02f);
        shape.position = Vector3.zero;

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        Vector3 v3 = alongDir.normalized * downstreamSpeed;
        vel.x = v3.x; vel.y = v3.y; vel.z = v3.z;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.98f, 0.99f, 1f), 0f),
                new GradientColorKey(new Color(0.95f, 0.97f, 1f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, 0f),
                new GradientAlphaKey(1.0f, 0.1f),
                new GradientAlphaKey(0.9f, 0.8f),
                new GradientAlphaKey(0.0f, 1f)
            }
        );
        col.color = g;
    }

    void Clear()
    {
        foreach (var ps in emitters)
        {
            if (ps) {
                if (Application.isPlaying) Destroy(ps.gameObject);
                else DestroyImmediate(ps.gameObject);
            }
        }
        emitters.Clear();
    }
}