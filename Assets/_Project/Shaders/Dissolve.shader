Shader "MazeEscape/Dissolve"
{


    Properties
    {
        _BaseMap         ("Base Map", 2D)                         = "white" {}
        _BaseColor       ("Base Colour", Color)                   = (1, 1, 1, 1)

        _NoiseMap        ("Dissolve Noise", 2D)                   = "white" {}
        _DissolveAmount  ("Dissolve Amount", Range(0, 1))          = 0

        _EdgeColor       ("Edge Colour", Color)                   = (1, 0.5, 0.1, 1)
        _EdgeWidth       ("Edge Width", Range(0.001, 0.3))         = 0.06
        _EdgeStrength    ("Edge Emission Strength", Range(0, 10))  = 3
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Geometry"
        }

        Cull Off

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _NoiseMap_ST;
                float  _DissolveAmount;
                float4 _EdgeColor;
                float  _EdgeWidth;
                float  _EdgeStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 baseUV      : TEXCOORD0;
                float2 noiseUV     : TEXCOORD1;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.baseUV      = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.noiseUV     = TRANSFORM_TEX(IN.uv, _NoiseMap);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.baseUV) * _BaseColor;
                half noiseVal = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, IN.noiseUV).r;

                // TECHNIQUE: fragment clipping. Any pixel whose noise value
                // is below the current dissolve threshold is discarded
                // entirely — not faded, genuinely removed from the frame.
                clip(noiseVal - _DissolveAmount);

                // Edge glow: pixels just above the clip threshold light up,
                // giving the appearance of a burning/energised boundary
                // rather than a flat hard cutout.
                half edge = 1.0 - smoothstep(0.0, _EdgeWidth, noiseVal - _DissolveAmount);
                half3 edgeGlow = _EdgeColor.rgb * _EdgeStrength * edge;

                half3 finalColor = albedo.rgb + edgeGlow;

                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
