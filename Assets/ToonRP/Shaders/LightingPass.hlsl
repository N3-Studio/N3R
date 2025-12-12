#include "../ShaderLibrary/Common.hlsl"

TEXTURE2D(_GBuffer0); SAMPLER(sampler_GBuffer0);
TEXTURE2D(_GBuffer1); SAMPLER(sampler_GBuffer1);
TEXTURE2D(_GBuffer2); SAMPLER(sampler_GBuffer2);
TEXTURE2D(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);

float4 _MainLightPosition;
float4 _MainLightColor;

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

Varyings Vert(uint vertexID : SV_VertexID)
{
    Varyings output;
    
    return output;
}

float4 Frag(Varyings input) : SV_Target
{
    
}