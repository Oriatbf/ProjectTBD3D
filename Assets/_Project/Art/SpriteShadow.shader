Shader "Custom/URP/SpriteShadow_NoShadowHeader"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
            "PreviewType" = "Plane"
        }

        // ============================================================
        // 1. Forward Pass (sprite rendering)
        // ============================================================
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend One OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Color;
            float _Cutoff;

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
            };

            Varyings Vert(Attributes IN)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                o.uv = IN.uv;
                o.color = IN.color * _Color;
                return o;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                clip(c.a - _Cutoff);

                half4 outColor = c * IN.color;
                outColor.rgb *= outColor.a;
                return outColor;
            }

            ENDHLSL
        }

        // ============================================================
        // 2. ShadowCaster Pass (NO Shadow.hlsl needed)
        // ============================================================
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

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

            // (URP 공용 ShadowCaster 버텍스 변환)
            float4 GetShadowPositionHClip(float3 posOS)
            {
                float3 posWS = TransformObjectToWorld(posOS);
                float4 posCS = TransformWorldToHClip(posWS);
                return posCS;
            }

            Varyings ShadowVert(Attributes IN)
            {
                Varyings o;
                o.positionCS = GetShadowPositionHClip(IN.positionOS.xyz);
                o.uv = IN.uv;
                return o;
            }

            float4 ShadowFrag(Varyings IN) : SV_Target
            {
                float alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;

                // alpha cutout – 중요
                clip(alpha - _Cutoff);

                return 0;
            }

            ENDHLSL
        }
    }

    Fallback Off
}
