Shader "Custom/URP/SpriteShadowOutline"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5

        _Outline("Outline Enable", Float) = 1
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineSize("Outline Size", Range(0,5)) = 1
        
        // 스프라이트 아틀라스 UV 범위 (Inspector에서 자동 설정됨)
        [HideInInspector] _MainTex_TexelSize("Texel Size", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        // ============================================================
        // 1. Forward Pass (Sprite + Outline)
        // ============================================================
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            float4 _Color;
            float _Cutoff;

            float _Outline;
            float4 _OutlineColor;
            float _OutlineSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 uvBounds : TEXCOORD1; // UV 경계 정보
            };

            Varyings Vert(Attributes IN)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                o.uv = IN.uv;
                o.color = IN.color * _Color;
                
                // UV 범위 계산 (현재 스프라이트의 실제 UV 범위)
                // 약간의 패딩을 안쪽으로 추가하여 경계 침범 방지
                float2 padding = _MainTex_TexelSize.xy * 0.5;
                o.uvBounds.xy = IN.uv - padding; // min UV
                o.uvBounds.zw = IN.uv + padding; // max UV
                
                return o;
            }

            // UV를 스프라이트 영역 내로 클램핑하는 함수
            float2 ClampUV(float2 uv, float4 bounds)
            {
                return clamp(uv, bounds.xy, bounds.zw);
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half alpha = c.a;

                // 본체 픽셀
                if (alpha > _Cutoff)
                {
                    half4 col = c * IN.color;
                    col.rgb *= col.a;
                    return col;
                }

                // 외곽선 비활성
                if (_Outline <= 0)
                    discard;

                // 8방향 외곽선 검사 (UV 클램핑 적용)
                half outlineAlpha = 0;
                float2 texelSize = _MainTex_TexelSize.xy * _OutlineSize;
                
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0) continue;
                        
                        float2 offset = float2(x, y) * texelSize;
                        float2 sampleUV = IN.uv + offset;
                        
                        // 현재 스프라이트의 UV 영역 내로 클램핑
                        sampleUV = ClampUV(sampleUV, IN.uvBounds);
                        
                        outlineAlpha += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV).a;
                    }
                }

                if (outlineAlpha > 0)
                {
                    half4 o = _OutlineColor;
                    o.rgb *= o.a;
                    return o;
                }

                discard;
                return half4(0,0,0,0);
            }

            ENDHLSL
        }

        // ============================================================
        // 2. ShadowCaster Pass (Outline 제외, 본체만)
        // ============================================================
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Cutoff;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings ShadowVert(Attributes IN)
            {
                Varyings o;
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                o.positionCS = TransformWorldToHClip(posWS);
                o.uv = IN.uv;
                return o;
            }

            float4 ShadowFrag(Varyings IN) : SV_Target
            {
                float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                clip(alpha - _Cutoff);
                return 0;
            }

            ENDHLSL
        }
    }

    Fallback Off
}