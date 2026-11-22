#ifndef STEEL_TOON_DEPTH
#define STEEL_TOON_DEPTH
// Shadow and depth passes: position only.
#include "ToonInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

float3 _LightDirection;

float4 ShadowVertex(float4 positionOS : POSITION, float3 normalOS : NORMAL) : SV_POSITION
{
    float3 positionWS = TransformObjectToWorld(positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(normalOS);
    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
#if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
    positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif
    return positionCS;
}

float4 DepthVertex(float4 positionOS : POSITION) : SV_POSITION
{
    return TransformObjectToHClip(positionOS.xyz);
}

half4 EmptyFragment() : SV_Target { return 0; }
#endif
