// Steel/Toon plus a drawn outline, for characters, vehicles and props with smooth normals.
Shader "Steel/ToonOutlined"
{
    Properties
    {
        _BaseColor ("Colour", Color) = (1, 1, 1, 1)
        _BaseMap ("Decal (optional)", 2D) = "white" {}
        _Shade ("Shadow tone", Range(0, 1)) = .78
        [HDR] _EmissionColor ("Glow", Color) = (0, 0, 0, 1)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        _OutlineColor ("Line colour", Color) = (.1, .11, .14, 1)
        _OutlineWidth ("Line width (m)", Range(0, .05)) = .012
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        Pass
        {
            Name "ForwardLit" Tags { "LightMode" = "UniversalForward" }
            Cull [_Cull]
            HLSLPROGRAM
            #pragma vertex ToonVertex
            #pragma fragment ToonFragment
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            #include "ToonForward.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster" Tags { "LightMode" = "ShadowCaster" }
            ColorMask 0
            HLSLPROGRAM
            #pragma vertex ShadowVertex
            #pragma fragment EmptyFragment
            #include "ToonDepth.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly" Tags { "LightMode" = "DepthOnly" }
            ColorMask 0
            HLSLPROGRAM
            #pragma vertex DepthVertex
            #pragma fragment EmptyFragment
            #include "ToonDepth.hlsl"
            ENDHLSL
        }
        Pass
        {
            Name "Outline" Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            HLSLPROGRAM
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment
            #pragma multi_compile_fog
            #include "ToonOutline.hlsl"
            ENDHLSL
        }
    }
}
