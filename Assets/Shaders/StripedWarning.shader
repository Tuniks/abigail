Shader "Custom/StripedWarning"
{
    Properties
    {
        _Color1    ("Stripe Color A",   Color) = (1,0.8,0,1)
        _Color2    ("Stripe Color B",   Color) = (0,0,0,1)
        _Angle     ("Stripe Angle",     Range(0,360)) = 45
        _Speed     ("Slide Speed",      Float)       = 0.5
        _Scale     ("Stripe Density",   Float)       = 10
        _Thickness ("Stripe Thickness", Range(0,1))  = 0.5

        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4    _MainTex_ST;

            fixed4 _Color1;
            fixed4 _Color2;
            float  _Angle;
            float  _Speed;
            float  _Scale;
            float  _Thickness;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                float4 col  : COLOR;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.col = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // rotate UV around center by _Angle
                float rad = radians(_Angle);
                float s = sin(rad), c = cos(rad);
                float2 uvc = i.uv - 0.5;
                float2 uvr;
                uvr.x = uvc.x * c - uvc.y * s;
                uvr.y = uvc.x * s + uvc.y * c;
                uvr += 0.5;

                // slide stripes
                uvr.x += _Time.y * _Speed;

                // stripe pattern
                float f = frac(uvr.x * _Scale);
                // pick color based on thickness cutoff
                float m = step(f, _Thickness);
                fixed4 stripeCol = lerp(_Color2, _Color1, m);

                // apply sprite vertex tint & alpha
                stripeCol.rgb *= i.col.rgb;
                stripeCol.a   *= i.col.a;

                return stripeCol;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
