Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        _Outline("Outline", Float) = 1
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineSize("Outline Size", Range(1,5)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnitySprites.cginc"

            float _Outline;
            fixed4 _OutlineColor;
            float _OutlineSize;
            float4 _MainTex_TexelSize;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 mainColor = SampleSpriteTexture(IN.texcoord) * IN.color;
                mainColor.rgb *= mainColor.a; 
                // 이미 불투명한 픽셀은 원래 색 유지
                if (mainColor.a > 0.0)
                    return mainColor;

                // 외곽선 활성화 확인
                if (_Outline <= 0)
                    return mainColor;

                // 8방향 검사
                float alpha = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        float2 offset = float2(x, y) * _OutlineSize * _MainTex_TexelSize.xy;
                        alpha += tex2D(_MainTex, IN.texcoord + offset).a;
                    }
                }

                // 주변 픽셀 중 하나라도 알파가 있으면 외곽선
                if (alpha > 0)
                    return _OutlineColor;

                return mainColor;
            }
            ENDCG
        }
    }
}
