Shader "ToonRP/Lit"
{
    Properties
    {
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
            #pragma vertex Vert
            #pragma fragment Frag

            #include "DepthOnlyPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "GBuffer"
            Tags { "LightMode" = "GBuffer" }
            
            ZWrite Off
            ZTest Equal
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "GBufferPass.hlsl"
            ENDHLSL
        }
    }
}