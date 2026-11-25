#ifndef STEEL_TOON_FORWARD
#define STEEL_TOON_FORWARD
// Two tones blended across a soft terminator, a faint roundness and a rim on the lit side: one light, no maps.
#include "ToonInput.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

static const half3 ShadeTint = half3(.92, .96, 1.08);   // shadows lean towards the sky
static const half Roundness = .93, RimLift = .35;

struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; float2 uv : TEXCOORD0; half4 color : COLOR; };
struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    half3 normalWS : TEXCOORD1;
    float3 positionWS : TEXCOORD2;
    half fog : TEXCOORD3;
    half3 tint : COLOR;
};

Varyings ToonVertex(Attributes input)
{
    Varyings output;
    output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
    output.positionCS = TransformWorldToHClip(output.positionWS);
    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.fog = ComputeFogFactor(output.positionCS.z);
    // Authored models carry their palette per vertex, stored as sRGB; meshes without colours read white.
#ifdef UNITY_COLORSPACE_GAMMA
    output.tint = input.color.rgb;
#else
    output.tint = SRGBToLinear(input.color.rgb);
#endif
    return output;
}

half4 ToonFragment(Varyings input) : SV_Target
{
    half4 base = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
    base.rgb *= input.tint;
    Light sun = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
    half facing = dot(normalize(input.normalWS), sun.direction);
    // A soft terminator, as on the design sheets: a hard split cuts faces in half at phone sizes.
    half lit = smoothstep(.3, .75, facing * .5 + .5) * smoothstep(.15, .85, sun.shadowAttenuation);
    // The sun only chooses between the two tones; its brightness never scales the palette.
    half3 warmth = sun.color / max(max(sun.color.r, max(sun.color.g, sun.color.b)), .001);
    half3 tone = lerp(base.rgb * _Shade * ShadeTint, base.rgb * lerp(half3(1, 1, 1), warmth, .3), lit);
    // Keeps round shapes round inside each tone, and lifts the edge that faces away from the camera.
    half3 normal = normalize(input.normalWS);
    half rim = 1 - saturate(dot(normal, normalize(GetWorldSpaceViewDir(input.positionWS))));
    tone *= lerp(Roundness, 1 + RimLift * lit * rim * rim * rim, facing * .5 + .5);
    half3 colour = MixFog(tone + _EmissionColor.rgb, input.fog);
    return half4(colour, 1);
}
#endif
