Shader "Custom/UnlitParticleFoam"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _SoftFade("Soft Particle Fade (m)", Range(0,2)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Forward"
            Tags{ "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _Color;
                float _SoftFade;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float4 color  : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                half4  color : COLOR;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.color = v.color * _Color;
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                float2 uvScreen = i.screenPos.xy / i.screenPos.w;
                float raw = SampleSceneDepth(uvScreen);
                float sceneEye = LinearEyeDepth(raw, _ZBufferParams);
                float surfEye  = LinearEyeDepth(i.screenPos.z / i.screenPos.w, _ZBufferParams);
                float d = max(sceneEye - surfEye, 0.0);
                float soft = saturate(d / max(_SoftFade, 1e-4));
                half alpha = tex.a * i.color.a * soft;
                return half4(tex.rgb * i.color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}