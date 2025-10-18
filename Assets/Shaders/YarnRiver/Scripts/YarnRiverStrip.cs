// YarnRiverStrip.cs (updated for parallel flow & vertex speed)
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class YarnRiverStrip : MonoBehaviour
{
    public List<Transform> controlPoints = new List<Transform>();
    public float halfWidth = 0.75f;
    [Range(1, 32)] public int subdivisions = 8;
    public float uvLengthTiling = 1f;
    [Range(0f,1f)] public float bankEdgeSlowdown = 0.65f;

    Mesh mesh;

    void OnEnable(){ EnsureMesh(); Rebuild(); }
    void OnValidate(){ EnsureMesh(); Rebuild(); }

    void EnsureMesh(){
        var mf = GetComponent<MeshFilter>();
        if (mesh == null){
            mesh = new Mesh();
            mesh.name = "YarnRiverStripMesh";
            mf.sharedMesh = mesh;
        }
    }

    Vector3 SamplePoint(List<Vector3> pts, float t){
        int count = pts.Count;
        if (count < 2) return Vector3.zero;
        float f = Mathf.Clamp01(t) * (count - 1);
        int i = Mathf.FloorToInt(f);
        float lt = f - i;

        int i0 = Mathf.Clamp(i - 1, 0, count - 1);
        int i1 = Mathf.Clamp(i, 0, count - 1);
        int i2 = Mathf.Clamp(i + 1, 0, count - 1);
        int i3 = Mathf.Clamp(i + 2, 0, count - 1);

        return 0.5f * (
            (2f * pts[i1]) +
            (-pts[i0] + pts[i2]) * lt +
            (2f*pts[i0] - 5f*pts[i1] + 4f*pts[i2] - pts[i3]) * (lt*lt) +
            (-pts[i0] + 3f*pts[i1] - 3f*pts[i2] + pts[i3]) * (lt*lt*lt)
        );
    }

    public void Rebuild(){
        if (controlPoints == null || controlPoints.Count < 2) return;

        var pts = new List<Vector3>(controlPoints.Count);
        foreach (var t in controlPoints) if (t) pts.Add(t.position);
        if (pts.Count < 2) return;

        int segments = (pts.Count - 1) * subdivisions;
        int vertCount = (segments + 1) * 2;

        var vertices = new Vector3[vertCount];
        var uvs = new Vector2[vertCount];
        var colors = new Color[vertCount];
        var tris = new int[segments * 6];

        float totalLength = 0f;
        Vector3 prev = SamplePoint(pts, 0f);
        int samples = segments + 1;
        var centers = new Vector3[samples];
        var tangents = new Vector3[samples];

        for (int s = 0; s < samples; s++){
            float t = (samples == 1) ? 0f : (float)s / (samples - 1);
            Vector3 p = SamplePoint(pts, t);
            centers[s] = p;
            Vector3 forward = (p - prev);
            if (forward.sqrMagnitude < 1e-6f && s < samples - 1){
                Vector3 p2 = SamplePoint(pts, Mathf.Clamp01(t + 1f / (samples - 1)));
                forward = (p2 - p);
            }
            if (forward.sqrMagnitude < 1e-6f) forward = Vector3.forward;
            tangents[s] = forward.normalized;
            if (s > 0) totalLength += (p - prev).magnitude;
            prev = p;
        }

        float accumLen = 0f;
        for (int s = 0; s < samples; s++){
            if (s > 0) accumLen += (centers[s] - centers[s - 1]).magnitude;

            Vector3 tdir = tangents[s];
            Vector2 dirXZ = new Vector2(tdir.x, tdir.z).normalized;
            if (dirXZ.sqrMagnitude < 1e-6f) dirXZ = new Vector2(0, 1);

            Vector3 up = Vector3.up;
            Vector3 right3 = Vector3.Cross(up, tdir).normalized;
            if (right3.sqrMagnitude < 1e-6f) right3 = Vector3.right;

            Vector3 leftPos = centers[s] - right3 * halfWidth;
            Vector3 rightPos = centers[s] + right3 * halfWidth;

            int vi = s * 2;
            vertices[vi + 0] = transform.InverseTransformPoint(leftPos);
            vertices[vi + 1] = transform.InverseTransformPoint(rightPos);

            float u = (accumLen / Mathf.Max(0.0001f, totalLength)) * uvLengthTiling;
            float v0 = 0f; float v1 = 1f;
            uvs[vi + 0] = new Vector2(u, v0);
            uvs[vi + 1] = new Vector2(u, v1);

            float edge0 = Mathf.Abs(v0 - 0.5f) * 2f;
            float edge1 = Mathf.Abs(v1 - 0.5f) * 2f;
            float speedMul0 = Mathf.Lerp(1f, bankEdgeSlowdown, edge0);
            float speedMul1 = Mathf.Lerp(1f, bankEdgeSlowdown, edge1);

            colors[vi + 0] = new Color(dirXZ.x * 0.5f + 0.5f, dirXZ.y * 0.5f + 0.5f, 0f, speedMul0);
            colors[vi + 1] = new Color(dirXZ.x * 0.5f + 0.5f, dirXZ.y * 0.5f + 0.5f, 0f, speedMul1);
        }

        int ti = 0;
        for (int s = 0; s < segments; s++){
            int vi = s * 2;
            tris[ti++] = vi + 0;
            tris[ti++] = vi + 2;
            tris[ti++] = vi + 1;
            tris[ti++] = vi + 1;
            tris[ti++] = vi + 2;
            tris[ti++] = vi + 3;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}