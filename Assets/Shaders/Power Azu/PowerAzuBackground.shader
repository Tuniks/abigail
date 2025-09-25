Shader "Custom/TurnBasedHalftoneAnimatedNoise"
{
    Properties
    {

        _OpponentColor   ("Opponent Turn Color",   Color) = (1,0,0,1)
        _PlayerColor     ("Player Turn Color",     Color) = (1,1,0,1)
        _IsPlayerTurn    ("Is Player Turn",        Range(0,1)) = 0

        // Halftone grid controls
        _Tiling          ("Cells per UV-unit",     Vector) = (20,20,0,0)
        _Radius          ("Base Radius (0-0.5)",   Range(0,0.5)) = 0.25
        _Density         ("Radius Scale",          Range(0.5,2))   = 1.0

        // Noise for size variation
        _NoiseScale      ("Noise Frequency",       Float)  = 0.1
        _NoiseIntensity  ("Noise Intensity (0-1)", Range(0,1))   = 0.5
        _NoiseAnimSpeed  ("Noise Animation Speed", Float)  = 0.2

        // Dot color
        _DotColor        ("Dot Color",             Color)  = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Turn blend
            float4 _OpponentColor;
            float4 _PlayerColor;
            float  _IsPlayerTurn;

            // Grid
            float4 _Tiling;
            float  _Radius;
            float  _Density;

            // Noise
            float  _NoiseScale;
            float  _NoiseIntensity;
            float  _NoiseAnimSpeed;

            // Dot
            float4 _DotColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv  : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            // Value‑noise function
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453123);
            }
            float noiseVal(float2 p)
            {
                float2 i = floor(p), f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));
                float2 u = f*f*(3 - 2*f);
                return lerp( lerp(a,b,u.x), lerp(c,d,u.x), u.y );
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1) Blend between your two turn colors
                fixed4 baseCol = lerp(_OpponentColor, _PlayerColor, _IsPlayerTurn);

                // 2) Compute which cell we’re in and the local UV
                float2 uvGrid = i.uv * _Tiling.xy;
                float2 cell   = floor(uvGrid);
                float2 fuv    = frac(uvGrid);

                // 3) Sample animated noise per-cell for radius variance
                float2 noiseUV = cell * _NoiseScale + _Time.y * _NoiseAnimSpeed;
                float n        = noiseVal(noiseUV);
                float variation= (n * 2 - 1) * _NoiseIntensity;
                float r        = _Radius * _Density * (1 + variation);

                // 4) Draw a smooth circle
                float dist = distance(fuv, float2(0.5,0.5));
                float edge = fwidth(dist) * 0.5;
                float mask = smoothstep(r + edge, r - edge, dist);

                // 5) Composite dot over background
                fixed4 dotCol = _DotColor * mask;
                return baseCol + dotCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
