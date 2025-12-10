Shader "ToonRP/Unlit"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "ToonRenderPipeline" }
        
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ZTest LEqual
            
            ColorMask 0
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct app_data
            {
                float4 vertex : POSITION;
            };

            struct v2_f
            {
                float4 position_cs : SV_POSITION;
            };

            v2_f vert (app_data v)
            {
                v2_f o;
                o.position_cs = TransformObjectToHClip(v.vertex.xyz);

                return o;
            }

            half4 frag (v2_f i) : SV_Target
            {
                return float4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}