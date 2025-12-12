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
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "GBufferPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "Lighting"
            Tags { "LightMode" = "Lighting" }
            
            ZWrite Off
            ZTest Always
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "LightingPass.hlsl"
            ENDHLSL
        }
    }
}