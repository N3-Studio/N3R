Shader "ToonRP/Blit"
{
    SubShader
    {
        HLSLINCLUDE
        #include "../ShaderLibrary/Common.hlsl"
        ENDHLSL

        Tags { "RenderType" = "Opaque" }
        LOD 100
        
        ZWrite Off
        ZTest Always
        Cull Off
        
        Pass
        {
            Name "Blit"

            HLSLPROGRAM
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BlitTexture);
            SAMPLER(sampler_LinearClamp);
            
            float4 _BlitScaleBias;
            
            #pragma vertex Vert
            #pragma fragment Frag
            
            Varyings Vert(uint vertexID : SV_VertexID)
            {
                Varyings output;
                output.positionCS = GetFullScreenTriangleVertexPosition(vertexID);
                float2 uv = GetFullScreenTriangleTexCoord(vertexID);
        
                output.uv = uv * _BlitScaleBias.xy + _BlitScaleBias.zw;
                
                return output;
            }

            float4 Frag (Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.uv).rgba;
                return color;
            }
            
            ENDHLSL
        }
    }
}