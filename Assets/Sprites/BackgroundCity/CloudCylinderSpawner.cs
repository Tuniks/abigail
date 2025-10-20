using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CloudCylinderSpawner : MonoBehaviour
{
    [Header("Cloud Source (Spritesheet)")]
    [Tooltip("Assign the individual sprites from your spritesheet (Texture Type: Sprite, Mode: Multiple).")]
    public Sprite[] cloudSprites;

    [Header("Spawn Settings")]
    [Min(0)] public int cloudCount = 150;
    [Tooltip("Horizontal radius (XZ) of the cylinder.")]
    public float radius = 50f;
    [Tooltip("Total height (Y) of the cylinder.")]
    public float height = 30f;

    [Header("Appearance")]
    [Tooltip("Random uniform scale (local) chosen per cloud: [min, max].")]
    public Vector2 scaleRange = new Vector2(0.75f, 2.5f);
    [Tooltip("Optional material for the SpriteRenderer. Leave empty for default.")]
    public Material spriteMaterial;
    [Tooltip("Optional color tint and alpha for all clouds. Leave white for unmodified.")]
    public Color globalTint = Color.white;
    [Tooltip("Sorting layer name (optional).")]
    public string sortingLayerName = "";
    [Tooltip("Sorting order (optional).")]
    public int sortingOrder = 0;

    [Header("Motion")]
    [Tooltip("Base wind direction for all clouds (XZ recommended).")]
    public Vector3 windDirection = new Vector3(1f, 0f, 0.4f);
    [Tooltip("Per-cloud speed range (multiplies direction magnitude).")]
    public Vector2 speedRange = new Vector2(0.2f, 1.2f);
    [Tooltip("Small random angle variance (degrees) applied to each cloud's direction.")]
    public float directionJitterDeg = 12f;
    [Tooltip("Vertical bobbing (amplitude in meters).")]
    public Vector2 bobAmplitudeRange = new Vector2(0.0f, 0.8f);
    [Tooltip("Vertical bobbing frequency (Hz).")]
    public Vector2 bobFrequencyRange = new Vector2(0.05f, 0.25f);
    [Tooltip("If true, clouds that leave the cylinder wrap to the opposite side.")]
    public bool wrapInsideCylinder = true;

    [Header("Billboarding")]
    [Tooltip("Camera to face. Left empty uses Camera.main.")]
    public Camera targetCamera;
    [Tooltip("Keep billboards upright (ignore camera pitch/roll).")]
    public bool lockUpright = true;

    [Header("Randomness")]
    [Tooltip("Use a fixed seed for reproducible layouts. Set to 0 to use a time-based seed.")]
    public int randomSeed = 12345;

    [Header("Runtime Controls")]
    [Tooltip("Regenerate clouds whenever you change parameters in Play mode.")]
    public bool autoRegenerateOnValidate = true;


    private readonly List<Cloud> _clouds = new List<Cloud>();
    private Transform _poolRoot;

    private struct Cloud
    {
        public Transform tf;
        public SpriteRenderer sr;
        public float speed;
        public Vector3 dirXZ;   
        public float bobAmp;
        public float bobFreq;
        public float bobPhase;
        public float baseY;     
    }

    void OnEnable()
    {
        EnsurePoolRoot();
        if (Application.isPlaying)
        {
            
            if (_clouds.Count == 0)
                RegenerateClouds();
        }
        else if (autoRegenerateOnValidate)
        {
            RegenerateClouds();
        }
    }

    void OnValidate()
    {
        radius = Mathf.Max(0.01f, radius);
        height = Mathf.Max(0.01f, height);
        scaleRange.x = Mathf.Max(0.01f, Mathf.Min(scaleRange.x, scaleRange.y));
        scaleRange.y = Mathf.Max(scaleRange.x, scaleRange.y);
        speedRange.x = Mathf.Max(0f, Mathf.Min(speedRange.x, speedRange.y));
        speedRange.y = Mathf.Max(speedRange.x, speedRange.y);
        bobAmplitudeRange.x = Mathf.Max(0f, Mathf.Min(bobAmplitudeRange.x, bobAmplitudeRange.y));
        bobAmplitudeRange.y = Mathf.Max(bobAmplitudeRange.x, bobAmplitudeRange.y);
        bobFrequencyRange.x = Mathf.Max(0f, Mathf.Min(bobFrequencyRange.x, bobFrequencyRange.y));
        bobFrequencyRange.y = Mathf.Max(bobFrequencyRange.x, bobFrequencyRange.y);

        if (!Application.isPlaying && autoRegenerateOnValidate)
            RegenerateClouds();
    }

    void Update()
    {
        if (_clouds.Count == 0) return;

 
        Camera cam = targetCamera ? targetCamera : Camera.main;

        Vector3 center = transform.position;
        float halfH = height * 0.5f;
        float time = Application.isPlaying ? Time.time : Time.realtimeSinceStartup;

        for (int i = 0; i < _clouds.Count; i++)
        {
            var c = _clouds[i];
            if (!c.tf) continue;

     
            Vector3 pos = c.tf.position;
            Vector3 vel = c.dirXZ * c.speed;
            pos += vel * Time.deltaTime;

            float y = c.baseY + Mathf.Sin((time + c.bobPhase) * (Mathf.PI * 2f) * c.bobFreq) * c.bobAmp;
            y = Mathf.Clamp(y, center.y - halfH, center.y + halfH);
            pos.y = y;

           
            if (wrapInsideCylinder)
            {
                Vector2 xz = new Vector2(pos.x - center.x, pos.z - center.z);
                float d = xz.magnitude;
                if (d > radius)
                {
                    
                    Vector2 dir = new Vector2(c.dirXZ.x, c.dirXZ.z);
                    if (dir.sqrMagnitude < 0.0001f)
                    {
                      
                        xz = -xz.normalized * (radius - 0.1f);
                    }
                    else
                    {
                        
                        xz -= dir.normalized * (2f * radius + 1f);
                    }
                    pos.x = center.x + xz.x;
                    pos.z = center.z + xz.y;
                }
            }

            c.tf.position = pos;

            
            if (cam)
            {
                if (lockUpright)
                {
                    
                    Vector3 toCam = cam.transform.position - c.tf.position;
                    toCam.y = 0f;
                    if (toCam.sqrMagnitude > 0.0001f)
                        c.tf.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
                }
                else
                {
                    c.tf.forward = (c.tf.position - cam.transform.position).normalized;
                }
            }

            _clouds[i] = c;
        }
    }

    [ContextMenu("Regenerate Clouds")]
    public void RegenerateClouds()
    {
        ClearClouds();
        EnsurePoolRoot();

        if (cloudSprites == null || cloudSprites.Length == 0)
        {
            Debug.LogWarning("[CloudCylinderSpawner] No sprites assigned. Please add sliced sprites from your spritesheet.");
            return;
        }

        System.Random rng = (randomSeed == 0) ? new System.Random() : new System.Random(randomSeed);

        Vector3 baseWind = windDirection;
        baseWind.y = 0f;
        if (baseWind.sqrMagnitude < 0.0001f)
        {
            
            baseWind = new Vector3(1f, 0f, 0f);
        }
        Vector3 baseDirXZ = baseWind.normalized;

        float halfH = height * 0.5f;

        for (int i = 0; i < cloudCount; i++)
        {
            
            GameObject go = new GameObject($"Cloud_{i:D3}");
            go.transform.SetParent(_poolRoot, worldPositionStays: false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = cloudSprites[rng.Next(0, cloudSprites.Length)];
            if (spriteMaterial) sr.sharedMaterial = spriteMaterial;
            sr.color = globalTint;
            if (!string.IsNullOrEmpty(sortingLayerName)) sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;

            
            Vector2 r = RandomInsideUnitCircle(rng) * radius;
            float y = (float)Lerp(rng, -halfH, halfH);
            Vector3 worldPos = transform.position + new Vector3(r.x, y, r.y);
            go.transform.position = worldPos;

            
            float s = (float)Lerp(rng, scaleRange.x, scaleRange.y);
            go.transform.localScale = Vector3.one * s;

           
            float speed = (float)Lerp(rng, speedRange.x, speedRange.y);

            
            float jitterRad = Mathf.Deg2Rad * (float)Lerp(rng, -directionJitterDeg, directionJitterDeg);
            Vector3 dirXZ = Quaternion.AngleAxis(jitterRad * Mathf.Rad2Deg, Vector3.up) * baseDirXZ;
            dirXZ.Normalize();

            
            float bobAmp = (float)Lerp(rng, bobAmplitudeRange.x, bobAmplitudeRange.y);
            float bobFreq = (float)Lerp(rng, bobFrequencyRange.x, bobFrequencyRange.y);
            float bobPhase = (float)Lerp(rng, 0f, 10f);

         
            float baseY = worldPos.y;

            
            var bb = go.AddComponent<BillboardHelper>();
            bb.targetCamera = targetCamera;
            bb.lockUpright = lockUpright;

            _clouds.Add(new Cloud
            {
                tf = go.transform,
                sr = sr,
                speed = speed,
                dirXZ = dirXZ,
                bobAmp = bobAmp,
                bobFreq = bobFreq,
                bobPhase = bobPhase,
                baseY = baseY
            });
        }
    }

    [ContextMenu("Clear Clouds")]
    public void ClearClouds()
    {
        _clouds.Clear();
        if (_poolRoot != null)
        {
           
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                for (int i = _poolRoot.childCount - 1; i >= 0; i--)
                    DestroyImmediate(_poolRoot.GetChild(i).gameObject);
            }
            else
#endif
            {
                for (int i = _poolRoot.childCount - 1; i >= 0; i--)
                    Destroy(_poolRoot.GetChild(i).gameObject);
            }
        }
    }

    private void EnsurePoolRoot()
    {
        if (_poolRoot == null)
        {
            var existing = transform.Find("__Clouds");
            _poolRoot = existing ? existing : new GameObject("__Clouds").transform;
            _poolRoot.SetParent(transform, worldPositionStays: false);
            _poolRoot.localPosition = Vector3.zero;
        }
    }

  

    private static Vector2 RandomInsideUnitCircle(System.Random rng)
    {
      
        for (; ; )
        {
            float x = (float)(rng.NextDouble() * 2.0 - 1.0);
            float y = (float)(rng.NextDouble() * 2.0 - 1.0);
            float s = x * x + y * y;
            if (s <= 1f) return new Vector2(x, y);
        }
    }

    private static double Lerp(System.Random rng, double a, double b)
    {
        return a + (b - a) * rng.NextDouble();
    }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.25f);
        DrawWireCylinder(transform.position, radius, height, 48);
    }

    private static void DrawWireCylinder(Vector3 center, float r, float h, int segments)
    {
        float half = h * 0.5f;
        Vector3 up = Vector3.up * half;

        Vector3 prevTop = center + new Vector3(r, half, 0);
        Vector3 prevBot = center + new Vector3(r, -half, 0);

        for (int i = 1; i <= segments; i++)
        {
            float a = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = new Vector3(Mathf.Cos(a) * r, 0, Mathf.Sin(a) * r);
            Vector3 nextTop = center + next + up;
            Vector3 nextBot = center + next - up;

            Gizmos.DrawLine(prevTop, nextTop);
            Gizmos.DrawLine(prevBot, nextBot);
            if (i % (segments / 8) == 0) Gizmos.DrawLine(nextTop, nextBot);

            prevTop = nextTop;
            prevBot = nextBot;
        }
    }

   
    private class BillboardHelper : MonoBehaviour
    {
        public Camera targetCamera;
        public bool lockUpright = true;

        void LateUpdate()
        {
            Camera cam = targetCamera ? targetCamera : Camera.main;
            if (!cam) return;

            if (lockUpright)
            {
                Vector3 toCam = cam.transform.position - transform.position;
                toCam.y = 0f;
                if (toCam.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(-toCam.normalized, Vector3.up);
            }
            else
            {
                transform.forward = (transform.position - cam.transform.position).normalized;
            }
        }
    }
}
