#ifndef STEEL_TOON_SKY
#define STEEL_TOON_SKY
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

TEXTURE2D(_CloudMap); SAMPLER(sampler_CloudMap);
CBUFFER_START(UnityPerMaterial)
    half4 _Zenith, _Horizon, _Ground, _SunColor, _CloudColor, _CloudShade;
    float4 _Drift;
    half _Cover;
CBUFFER_END

struct SkyVaryings { float4 positionCS : SV_POSITION; float3 ray : TEXCOORD0; };

SkyVaryings SkyVertex(float4 positionOS : POSITION)
{
    SkyVaryings output;
    output.positionCS = TransformObjectToHClip(positionOS.xyz);
    output.ray = positionOS.xyz;
    return output;
}

// How much cloud the mask holds at this point of the flat cloud deck.
half Cloud(float2 deck) { return 1 - SAMPLE_TEXTURE2D(_CloudMap, sampler_CloudMap, deck).r; }

half4 SkyFragment(SkyVaryings input) : SV_Target
{
    float3 ray = normalize(input.ray);
    half up = saturate(ray.y);
    half3 sky = lerp(_Horizon.rgb, _Zenith.rgb, smoothstep(0, .55, up));
    sky = lerp(sky, _SunColor.rgb, smoothstep(.9975, .998, dot(ray, _MainLightPosition.xyz)));
    // Clouds lie on a flat deck overhead; dividing by height stretches them towards the horizon.
    float2 deck = ray.xz / (ray.y + .12) * .11 + _Time.y * _Drift.xy;
    half body = smoothstep(_Cover, _Cover + .02, Cloud(deck));
    half lit = smoothstep(_Cover + .1, _Cover + .12, Cloud(deck + float2(.012, .012)));
    half3 cloud = lerp(_CloudShade.rgb, _CloudColor.rgb, lit);
    sky = lerp(sky, cloud, body * smoothstep(.03, .14, ray.y));
    return half4(lerp(_Ground.rgb, sky, smoothstep(-.02, 0, ray.y)), 1);
}
#endif
