#pragma once

#include "../ShaderLibrary/Common.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
};

struct GBufferOutput
{
    float4 gBuffer0 : SV_Target0;
    float4 gBuffer1 : SV_Target1;
    float4 gBuffer2 : SV_Target2;
    float4 gBuffer3 : SV_Target3;
};

Varyings Vert(Attributes input)
{
    Varyings output;

    output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

    return output;
}

GBufferOutput Frag(Varyings input)
{
    GBufferOutput output;
    
    output.gBuffer0 = half4(1, 1, 1, 1);
    output.gBuffer1 = half4(1, 1, 1, 1);
    output.gBuffer2 = half4(1, 1, 1, 1);
    output.gBuffer3 = half4(1, 1, 1, 1);
    
    return output;
}
