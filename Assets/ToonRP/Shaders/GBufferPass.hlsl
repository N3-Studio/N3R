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
    
    
    return output;
}

GBufferOutput Frag(Varyings input)
{
    GBufferOutput output;
    
    

    return output;
}
