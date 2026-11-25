#ifndef STEEL_TOON_OUTLINE
#define STEEL_TOON_OUTLINE
// The drawn line: the mesh pushed out along its normals and shown from the inside.
#include "ToonInput.hlsl"

struct OutlineVaryings { float4 positionCS : SV_POSITION; half fog : TEXCOORD0; };

OutlineVaryings OutlineVertex(float4 positionOS : POSITION, float3 normalOS : NORMAL)
{
    OutlineVaryings output;
    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(normalOS);
    // Thicken with distance so the line stays readable on a phone, within limits.
    float reach = clamp(distance(positionWS, _WorldSpaceCameraPos) * .12, .6, 2.5);
    output.positionCS = TransformWorldToHClip(positionWS + normalWS * _OutlineWidth * reach);
    output.fog = ComputeFogFactor(output.positionCS.z);
    return output;
}

half4 OutlineFragment(OutlineVaryings input) : SV_Target
{
    return half4(MixFog(_OutlineColor.rgb, input.fog), 1);
}
#endif
