Shader "MazeEscape/DetectionCone"
{
    

    Properties
    {
        _AlertColor  ("Alert Color (set at runtime by AI script)", Color) = (1, 0.85, 0.1, 1) // default: yellow patrol
        _MaxAlpha    ("Max Alpha (near zombie)", Range(0, 1))              = 0.45
        _MinAlpha    ("Min Alpha (far edge)",    Range(0, 1))              = 0.0
        [Toggle] _InvertFade ("Invert Fade Direction", Float)              = 0
        _MaxLength ("Max Length (object space)", Range(0.01, 3))           = 1.838597
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha  
        ZWrite Off                        
                                          
                                           
        Cull Off                          
                                           

        Pass
        {
            Name "Unlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _AlertColor;
                float  _MaxAlpha;
                float  _MinAlpha;
                float  _InvertFade;
                float  _MaxLength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float  distFromApex: TEXCOORD1;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv          = IN.uv;
                OUT.distFromApex = IN.positionOS.y;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                clip(_MaxLength - IN.distFromApex);

                float t = _InvertFade > 0.5 ? (1.0 - IN.uv.y) : IN.uv.y;

                float alpha = lerp(_MaxAlpha, _MinAlpha, t);

                return half4(_AlertColor.rgb, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
