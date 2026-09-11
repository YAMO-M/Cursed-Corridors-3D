Shader "MazeEscape/GlowingCollectible"
{
    Properties
    {
        _BaseMap          ("Base Map",          2D)             = "white" {}
        _BaseColor        ("Base Colour",       Color)          = (1, 1, 1, 1)
        _EmissionColor    ("Emission Colour",   Color)          = (1, 0.75, 0.35, 1)
        _EmissionStrength ("Emission Strength", Range(0, 8))     = 1.5
        _FresnelPower     ("Fresnel Power",     Range(0.5, 8))   = 4.0
        _BobHeight        ("Bob Height",        Range(0, 1))     = 0.06
        _BobSpeed         ("Bob Speed",         Range(0, 10))    = 1.8
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off   // the map is thin, so render both sides

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _EmissionColor;
                float  _EmissionStrength;
                float  _FresnelPower;
                float  _BobHeight;
                float  _BobSpeed;
            CBUFFER_END

            // ---------------- VERTEX STAGE ----------------
            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 positionOS = IN.positionOS.xyz;

                // TECHNIQUE 3: vertex animation
                positionOS.y += sin(_Time.y * _BobSpeed) * _BobHeight;

                OUT.positionWS  = TransformObjectToWorld(positionOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv          = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            // ---------------- FRAGMENT STAGE ----------------
            half4 frag (Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);

                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // TECHNIQUE 1: Lambert diffuse lighting model
                Light  mainLight = GetMainLight();
                float3 L         = normalize(mainLight.direction);
                float  NdotL     = saturate(dot(N, L));
                float3 diffuse   = albedo.rgb * _BaseColor.rgb * mainLight.color * NdotL;

                // Ambient floor so the map is readable in a dark maze
                float3 ambient   = albedo.rgb * _BaseColor.rgb * 0.25;

                // TECHNIQUE 2: Fresnel rim emission (surface property)
                float3 V        = normalize(GetWorldSpaceViewDir(IN.positionWS));
                float  fresnel  = pow(1.0 - saturate(dot(N, V)), _FresnelPower);
                float3 emission = _EmissionColor.rgb * _EmissionStrength * (0.15 + fresnel);

                return half4(diffuse + ambient + emission, 1);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}