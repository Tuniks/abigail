Shader "Custom/YarnRiver_SimpleFlow_Foam"
{
    Properties{
        // core yarn
        _MainTex("Yarn (single texture)", 2D) = "white" {}
        _Tiling("UV Tiling (U,V)", Vector) = (1,1,0,0)
        _Speed("Downstream Speed (+U)", Range(-3,3)) = 0.2

        // keep strands aligned without changing flow
        _FiberAngleDeg("Fiber Angle (deg)", Range(0,360)) = 0
        _FlipV("Flip V (0/1)", Float) = 0

        // tiny optional warp so it doesn’t feel static
        _WarpAmp("Warp Amplitude (UV)", Range(0,0.3)) = 0.05
        _WarpFreq("Warp Frequency", Range(0,8)) = 1.5
        _WarpScale("Warp Spatial Scale", Range(0.1,10)) = 2.0

        _Tint("Tint", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0,1)) = 0.95

        // ---------- Foam (noise along edges) ----------
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

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_FoamTex); SAMPLER(sampler_FoamTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Tiling;
                float _Speed;
                float _FiberAngleDeg;
                float _FlipV;
                float _WarpAmp, _WarpFreq, _WarpScale;
                float4 _Tint;
                float _Alpha;

                float4 _FoamTex_ST;
                float4 _FoamTiling;
                float4 _FoamColor;
                float _FoamSpeed;
                float _ShoreWidth;
                float _ShoreSoftness;
                float _FoamIntensity;

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

                float2 uv = v.uv * _Tiling.xy;
                if (_FlipV > 0.5) uv.y = -uv.y;
                o.uvMesh = uv;

                float ang = radians(_FiberAngleDeg);
                o.csFiber = float2(cos(ang), sin(ang));
                float angP = ang + 1.57079632679; // +90°
                o.csPerp  = float2(cos(angP), sin(angP));

                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target{
                float t = _Time.y;

                // --- yarn base ---
                float2 uvBase = rotate_by(i.uvMesh, i.csFiber);                       // orient strands
                float2 uv = uvBase + rotate_by(float2(t * _Speed, 0), i.csFiber);     // scroll strictly downstream

                // optional small warp perpendicular to flow
                if (_WarpAmp > 0.0001){
                    float phase = t * _WarpFreq + (uvBase.x + uvBase.y) * _WarpScale;
                    uv += i.csPerp * (sin(phase) * _WarpAmp);
                }

                half3 yarn = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb * _Tint.rgb;
                half alpha = _Alpha * _Tint.a;

                // --- shoreline foam (noise band near V=0 and V=1) ---
                // distance to nearest bank in V space
                float v = abs(frac(i.uvMesh.y)); // assume 0..1 from strip
                float dEdge = min(v, 1.0 - v);   // 0 at banks, 0.5 at center
                float shoreMask = saturate(1.0 - pow(saturate(dEdge / max(_ShoreWidth, 1e-4)), _ShoreSoftness));

                // foam UV scrolls along downstream U
                float2 foamUV = (i.uvMesh * _FoamTiling.xy);
                foamUV.x += t * _FoamSpeed;

                half foamNoise = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, foamUV).r;
                // make noise brighter near edge
                half foamEdge = shoreMask * foamNoise;

                // --- optional depth contact foam (rocks/banks) ---
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

                // combine shoreline + depth masks
                float foamMask = saturate(foamEdge * _FoamIntensity + foamDepthMask * 0.6);

                // composite foam (non-premultiplied alpha blend)
                half3 col = lerp(yarn, _FoamColor.rgb, foamMask);
                half a = max(alpha, foamMask);  // foam can lift alpha a bit

                return half4(col, a);
            }
            ENDHLSL
        }
    }
}
