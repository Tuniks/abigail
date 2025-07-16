Shader "Custom/URP/StarrySkyTwinkleRepel_NoTile_Bounce_SpikesDist"
{
    Properties
    {
        _StarCount       ("Stars (X,Y)",            Vector) = (30, 20, 0, 0)
        _BaseStarSize    ("Base Size",              Range(0.001,0.1)) = 0.02
        _SizeVariance    ("Size Variance (%)",      Range(0,1))        = 1.0
        _SpikeAmount     ("Spike Amount (%)",       Range(0,1))        = 0.3

        _StarColorA      ("Star Color A",           Color)             = (1,1,1,1)
        _StarColorB      ("Star Color B",           Color)             = (1,0.8,0.6,1)

        _FlickerSpeed    ("Flicker Speed",          Range(0,10))       = 1.0
        _FlickerVar      ("Flicker Variety (%)",    Range(0,1))        = 0.8

        _RepelRadius     ("Repel Radius (cells)",   Float)             = 5
        _RepelStrength   ("Repel Strength (cells)", Range(0,1))        = 0.5
        _EdgeBoost       ("Edge Boost (0-1)",       Range(0,1))        = 0.5

        _BounceChance    ("Bounce Chance (%)",      Range(0,1))        = 0.3
        _BounceSpeed     ("Bounce Speed",           Range(0.1,10))     = 2.0
        _SpawnSpeed      ("Spawn Speed",            Range(0.1,10))     = 1.0

        _SkyColor        ("Sky Color",              Color)             = (0,0,0.1,1)
        _MouseUV         ("Mouse Position (UV)",    Vector)            = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Background" "RenderType"="Opaque" }
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _StarCount;
                float  _BaseStarSize;
                float  _SizeVariance;
                float  _SpikeAmount;
                float4 _StarColorA;
                float4 _StarColorB;
                float  _FlickerSpeed;
                float  _FlickerVar;
                float  _RepelRadius;
                float  _RepelStrength;
                float  _EdgeBoost;
                float  _BounceChance;
                float  _BounceSpeed;
                float  _SpawnSpeed;
                float4 _SkyColor;
                float4 _MouseUV;
            CBUFFER_END

            struct Attributes { float4 posOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 posCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.posCS = TransformObjectToHClip(IN.posOS);
                OUT.uv    = IN.uv;
                return OUT;
            }

            float rand(float2 co)   { return frac(sin(dot(co, float2(12.9898,78.233))) * 43758.5453); }
            float2 rand2(float2 co) { return float2(rand(co), rand(co + 0.5678)); }
            float2 rotateVec(float2 v, float a)
            {
                float ca = cos(a), sa = sin(a);
                return float2(v.x*ca - v.y*sa, v.x*sa + v.y*ca);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv      = IN.uv;
                float2 gridUV  = uv * _StarCount.xy;
                float2 cell    = floor(gridUV);
                float2 cellUV  = frac(gridUV);
                float2 fragGrid= gridUV;
                float4 col     = _SkyColor;

                // iterate neighborhood cells
                [unroll]
                for (int ox = -1; ox <= 1; ox++)
                for (int oy = -1; oy <= 1; oy++)
                {
                    float2 nCell   = cell + float2(ox, oy);
                    float2 seed    = rand2(nCell);
                    float2 starPos = seed;

                    // triangular distribution centered at ~0.5 for spikes
                    float tri     = (seed.x + seed.y) * 0.5;
                    float spikesF = floor(tri * 5.0 + 0.5) + 2;

                    // base size + variance
                    float sv       = pow(seed.y, 2.0);
                    float scale    = lerp(1 - _SizeVariance, 1 + _SizeVariance, sv);
                    float baseSize = _BaseStarSize * scale;

                    // repulsion
                    float2 starGrid   = nCell + starPos;
                    float2 mouseGrid  = _MouseUV.xy * _StarCount.xy;
                    float2 dG         = starGrid - mouseGrid;
                    float  distG      = length(dG) + 1e-5;
                    float  repel      = saturate((_RepelRadius - distG) / _RepelRadius);
                    float2 dirG       = dG / distG;
                    float2 disp       = dirG * repel * _RepelStrength;
                    float  edgeF      = 1 - repel;

                    // spawn growth envelope
                    float spawnT      = _Time.y * _SpawnSpeed + seed.x;
                    float spawnE      = saturate(sin(spawnT * 3.14159));
                    
                    // effective size
                    float starSize   = baseSize * (1 + edgeF * _EdgeBoost) * spawnE;

                    // flicker
                    float speedVar   = _FlickerSpeed * lerp(0.5,1.5, seed.x * _FlickerVar);
                    float rawF       = lerp(
                        sin(_Time.y * speedVar + seed.x * 6.2831),
                        sin(_Time.y * speedVar * 1.37 + seed.y * 6.2831),
                        lerp(0.3,0.7, seed.x)
                    );
                    float flick      = saturate(rawF * 0.5 + 0.5);
                    float brightness = flick * (1 + edgeF * _EdgeBoost);

                    // position + rotation
                    float2 localPos  = fragGrid - starGrid + disp;
                    float rotSpeed   = (seed.x * 2 - 1) * 0.3;
                    localPos         = rotateVec(localPos, _Time.y * rotSpeed);

                    // spiky shape
                    float ang        = atan2(localPos.y, localPos.x);
                    float rad        = length(localPos);
                    float shapeR     = starSize * (1 + sin(ang * spikesF) * _SpikeAmount);
                    float edge       = smoothstep(shapeR, shapeR * 0.6, rad);

                    // bounce effect
                    float doBounce   = step(1 - _BounceChance, seed.y);
                    float bounceT    = sin(_Time.y * _BounceSpeed + seed.x * 6.2831) * 0.5 + 0.5;
                    float bounceE    = lerp(1.0, bounceT, doBounce);

                    // color
                    float3 starCol   = lerp(_StarColorA.rgb, _StarColorB.rgb, seed.x);
                    col.rgb         += starCol * edge * brightness * bounceE;
                }

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Unlit"
}
