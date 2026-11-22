#ifndef STEEL_TOON_FORWARD
#define STEEL_TOON_FORWARD
// Two flat tones split by a hard terminator: one main light, no normal maps, no specular.
#include "ToonInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; };
struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    half3 normalWS : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    half fog : TEXCOORD3;
};

Varyings ToonVertex(Attributes input)
{
    Varyings output;
    output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.positionCS = TransformWorldToHClip(output.positionWS);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.fog = ComputeFogFactor(output.positionCS.z);
    return output;
}

half4 ToonFragment(Varyings input) : SV_Target
{
    half4 base = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
    Light sun = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
    half facing = dot(normalize(input.normalWS), sun.direction);
    half lit = smoothstep(-.02, .04, facing) * smoothstep(.35, .55, sun.shadowAttenuation);
    half3 tone = lerp(base.rgb * _Shade, base.rgb, lit) * lerp(half3(1, 1, 1), sun.color, .35);
    half3 colour = MixFog(tone + _EmissionColor.rgb, input.fog);
    return half4(colour, 1);
}
#endif
