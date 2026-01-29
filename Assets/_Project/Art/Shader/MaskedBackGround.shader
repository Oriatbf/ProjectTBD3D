Shader "UI/HighlightRectHole"
{
    Properties
    {
        _OverlayColor ("Overlay Color", Color) = (0,0,0,0.75)
        _HoleCenter ("Hole Center", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize ("Hole Size", Vector) = (0.2, 0.2, 0, 0)
        _EdgeSoftness ("Edge Softness", Range(0, 0.05)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 screenUV : TEXCOORD0;
            };

            fixed4 _OverlayColor;
            float2 _HoleCenter;
            float2 _HoleSize;
            float _EdgeSoftness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float4 sp = ComputeScreenPos(o.pos);
                o.screenUV = sp.xy / sp.w;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 delta = abs(i.screenUV - _HoleCenter);
                float2 halfSize = _HoleSize * 0.5;

                float maskX = smoothstep(halfSize.x, halfSize.x - _EdgeSoftness, delta.x);
                float maskY = smoothstep(halfSize.y, halfSize.y - _EdgeSoftness, delta.y);

                float hole = maskX * maskY;

                fixed4 col = _OverlayColor;
                col.a *= (1 - hole);

                return col;
            }
            ENDCG
        }
    }
}
