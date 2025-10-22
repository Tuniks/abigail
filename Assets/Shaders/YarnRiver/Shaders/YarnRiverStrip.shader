Shader "Custom/YarnRiver_SimpleFlow_Foam"
{
    Properties{
        // ---------------- Core Yarn (Mid layer) ----------------
        _MainTex("Yarn (single texture) [MID]", 2D) = "white" {}
        _Tiling("UV Tiling (U,V) [MID]", Vector) = (1,1,0,0)
        _Speed("Downstream Speed (+U) [MID]", Range(-3,3)) = 0.2

        // keep strands aligned without changing flow
        _FiberAngleDeg("Fiber Angle (deg)", Range(0,360)) = 0
        _FlipV("Flip V (0/1)", Float) = 0

        // tiny optional warp so it doesn’t feel static
        _WarpAmp("Warp Amplitude (UV)", Range(0,0.3)) = 0.05
        _WarpFreq("Warp Frequency", Range(0,8)) = 1.5
        _WarpScale("Warp Spatial Scale", Range(0.1,10)) = 2.0

        _Tint("Tint [MID]", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0,1)) = 0.95

        // ---------------- Bottom layer ----------------
        _BottomTex("Yarn (bottom layer)", 2D) = "white" {}
        _BottomTiling("UV Tiling (U,V) [BOTTOM]", Vector) = (0.9,1.05,0,0)
        _BottomSpeed("Downstream Speed (+U) [BOTTOM]", Range(-3,3)) = 0.08
        _BottomTint("Tint [BOTTOM]", Color) = (0.8,0.9,1,1)
        _BottomStrength("Bottom Visibility", Range(0,1)) = 0.65
        _BottomShallowPow("Bottom Bank Reveal (pow)", Range(0.2,8)) = 2.0
        _ParallaxAmp("Bottom Parallax (UV)", Range(0,0.15)) = 0.02

        // ---------------- Foam (top/whitewater) ----------------
        _FoamTex("Foam Noise", 2D) = "gray" {}
        _FoamColor("Foam Color", Color) = (0.95,0.97,1,1)
        _FoamTiling("Foam Tiling (U,V)", Vector) = (1.5,1.0,0,0)
        _FoamSpeed("Foam Scroll (+U)", Float) = 0.15

        // shoreline band width (in V) and falloff
        _ShoreWidth("Shoreline Width (V-space)", Range(0,0.5)) = 0.12
        _ShoreSoftness("Shoreline Softness", Range(0.1,10)) = 3.0
        _FoamIntensity("Foam Intensity", Range(0,2)) = 0.9

        // optional contact foam via depth (rocks/banks). requires URP Depth Texture.
        _UseDepthFoam("Use Depth Foam (0/1)", Float) = 1
        _DepthRange("Depth Fade (meters)", Range(0.01,1.0)) = 0.2
        _DepthSharp("Depth Edge Sharpness", Range(0.5,8.0)) = 2.0
    }
    SubShader{
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass{
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            // Textures
            TEXTURE2D(_MainTex);   SAMPLER(sampler_MainTex);     // MID
            TEXTURE2D(_BottomTex); SAMPLER(sampler_BottomTex);   // BOTTOM
            TEXTURE2D(_FoamTex);   SAMPLER(sampler_FoamTex);     // TOP (foam)

            CBUFFER_START(UnityPerMaterial)
                // Mid
                float4 _MainTex_ST;
                float4 _Tiling;
                float _Speed;
                float _FiberAngleDeg;
                float _FlipV;
                float _WarpAmp, _WarpFreq, _WarpScale;
                float4 _Tint;
                float _Alpha;

                // Bottom
                float4 _BottomTex_ST;
                float4 _BottomTiling;
                float _BottomSpeed;
                float4 _BottomTint;
                float _BottomStrength;
                float _BottomShallowPow;
                float _ParallaxAmp;

                // Foam
                float4 _FoamTex_ST;
                float4 _FoamTiling;
                float4 _FoamColor;
                float _FoamSpeed;
                float _ShoreWidth;
                float _ShoreSoftness;
                float _FoamIntensity;

                // Depth foam
                float _UseDepthFoam;
                float _DepthRange;
                float _DepthSharp;
            CBUFFER_END

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;  // YarnRiverStrip: U=downstream, V=across
            };
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uvMesh : TEXCOORD0;
                float2 csFiber : TEXCOORD1; // cos/sin(angle)
                float2 csPerp  : TEXCOORD2; // cos/sin(angle+90)
                float4 screenPos : TEXCOORD3;
            };

            float2 rotate_by(float2 v, float2 cs){
                return float2(cs.x*v.x - cs.y*v.y, cs.y*v.x + cs.x*v.y);
            }

            v2f vert(appdata v){
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);

                float2 uv = v.uv * _Tiling.xy; // base uses _Tiling.xy
                if (_FlipV > 0.5) uv.y = -uv.y;
                o.uvMesh = uv;

                float ang = radians(_FiberAngleDeg);
                o.csFiber = float2(cos(ang), sin(ang));
                float angP = ang + 1.57079632679; // +90°
                o.csPerp  = float2(cos(angP), sin(angP));

                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            // Helper for flowing yarn UV with shared warp
            float2 flow_uv(float2 uvMesh, float2 csFiber, float2 csPerp, float speed, float t, float warpAmp, float warpFreq, float warpScale)
            {
                float2 uvBase = rotate_by(uvMesh, csFiber);                 // orient strands
                float2 uv     = uvBase + rotate_by(float2(t * speed, 0), csFiber); // scroll downstream
                if (warpAmp > 0.0001){
                    float phase = t * warpFreq + (uvBase.x + uvBase.y) * warpScale;
                    uv += csPerp * (sin(phase) * warpAmp);                  // small perpendicular wobble
                }
                return uv;
            }

            half4 frag(v2f i) : SV_Target{
                float t = _Time.y;

                // ---------------- Mid layer (original yarn) ----------------
                float2 uvMid = flow_uv(i.uvMesh, i.csFiber, i.csPerp, _Speed, t, _WarpAmp, _WarpFreq, _WarpScale);
                half3 midCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvMid).rgb * _Tint.rgb;
                half alpha   = _Alpha * _Tint.a;

                // ---------------- Bottom layer ----------------
                // Separate tiling/speed, a tiny cross-river parallax to fake depth.
                // Parallax is strongest where "depth" is high; we use center-vs-banks as a cheap depth proxy.
                float vStrip = abs(frac(i.uvMesh.y));           // 0..1 across
                float dEdge  = min(vStrip, 1.0 - vStrip);       // 0 at banks, 0.5 center
                float depth01 = 1.0 - saturate(dEdge / 0.5);    // 0 banks -> 1 center (deeper in middle)

                // We'll reveal BOTTOM more toward the banks (shallows); shape with a power curve.
                float shallowness = pow(1.0 - depth01, _BottomShallowPow); // 0 center -> 1 banks

                // Bottom flow UV
                float2 uvMeshBottom = i.uvMesh * _BottomTiling.xy; // independent tiling
                float2 uvBottom = flow_uv(uvMeshBottom, i.csFiber, i.csPerp, _BottomSpeed, t, _WarpAmp, _WarpFreq, _WarpScale);

                // Add tiny parallax across width (looks like refraction/height layering)
                uvBottom += i.csPerp * (_ParallaxAmp * depth01); // more offset in center

                half3 bottomCol = SAMPLE_TEXTURE2D(_BottomTex, sampler_BottomTex, uvBottom).rgb * _BottomTint.rgb;

                // Blend bottom into mid near banks
                float bottomMask = saturate(shallowness * _BottomStrength);
                half3 baseCol = lerp(midCol, bottomCol, bottomMask);

                // ---------------- Foam (shoreline + optional depth contact) ----------------
                // shoreline foam: band at V=0 and V=1
                float shoreMask = saturate(1.0 - pow(saturate(dEdge / max(_ShoreWidth, 1e-4)), _ShoreSoftness));

                // foam UV scrolls along U
                float2 foamUV = (i.uvMesh * _FoamTiling.xy);
                foamUV.x += t * _FoamSpeed;

                half foamNoise = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, foamUV).r;
                half foamEdge  = shoreMask * foamNoise;

                // optional contact foam using scene depth
                float foamDepthMask = 0.0;
                if (_UseDepthFoam > 0.5){
                    float2 uvScreen = i.screenPos.xy / i.screenPos.w;
                    float raw = SampleSceneDepth(uvScreen);
                    float sceneEye = LinearEyeDepth(raw, _ZBufferParams);
                    float surfEye  = LinearEyeDepth(i.screenPos.z / i.screenPos.w, _ZBufferParams);
                    float diff = max(sceneEye - surfEye, 0.0);
                    float contact = pow(1.0 - saturate(diff / max(_DepthRange, 1e-3)), _DepthSharp);
                    foamDepthMask = contact;
                }

                float foamMask = saturate(foamEdge * _FoamIntensity + foamDepthMask * 0.6);

                // Composite foam on top
                half3 col = lerp(baseCol, _FoamColor.rgb, foamMask);
                half  a   = max(alpha, foamMask); // foam can lift alpha a bit

                return half4(col, a);
            }
            ENDHLSL
        }
    }
}
