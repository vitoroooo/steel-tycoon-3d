// A painted sky: three flat bands, a sun disc and two-tone clouds that drift. No HDR picture to sample.
Shader "Steel/ToonSky"
{
    Properties
    {
        _Zenith ("Top of the sky", Color) = (.25, .56, .86, 1)
        _Horizon ("Horizon", Color) = (.78, .9, .96, 1)
        _Ground ("Below the horizon", Color) = (.72, .78, .8, 1)
        _SunColor ("Sun disc", Color) = (1, .95, .75, 1)
        _CloudMap ("Cloud mask (R)", 2D) = "white" {}
        _CloudColor ("Cloud", Color) = (1, 1, 1, 1)
        _CloudShade ("Cloud underside", Color) = (.8, .87, .95, 1)
        _Cover ("Cloud cover", Range(0, 1)) = .45
        _Drift ("Drift (u, v per second)", Vector) = (.004, .0015, 0, 0)
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex SkyVertex
            #pragma fragment SkyFragment
            #include "ToonSky.hlsl"
            ENDHLSL
        }
    }
}
