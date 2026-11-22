#ifndef STEEL_TOON_INPUT
#define STEEL_TOON_INPUT
// Shared by every pass so the SRP batcher can group all toon materials.
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

CBUFFER_START(UnityPerMaterial)
    float4 _BaseMap_ST;
    half4 _BaseColor;
    half4 _EmissionColor;
    half4 _OutlineColor;
    half _Shade;
    half _OutlineWidth;
CBUFFER_END
#endif
