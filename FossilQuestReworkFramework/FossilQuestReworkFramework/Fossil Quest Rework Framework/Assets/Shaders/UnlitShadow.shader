Shader"UnlitWithShadows"
{
    Properties
    {
        _MainTex("Texture", 2D) = "" {}
        _TileAmount("Scale Multiplier", float) = 1
        _ShadowBrightness("Shadow Brightness", Range(0, 1)) = 0
    }
        SubShader
        {
            Pass
            {
                Tags {"LightMode" = "ForwardBase"}
                CGPROGRAM
                #pragma vertex VSMain
                #pragma fragment PSMain
                #pragma multi_compile_fwdbase
    #include "AutoLight.cginc"

    sampler2D _MainTex;
    float _TileAmount;
    float _ShadowBrightness;
    struct SHADERDATA
    {
        float4 position : SV_POSITION;
        float2 uv : TEXCOORD0;
        float4 _ShadowCoord : TEXCOORD1;
    };
    float4 ComputeScreenPos(float4 p)
    {
        float4 o = p * 0.5;
        return float4(float2(o.x, o.y * _ProjectionParams.x) + o.w, p.zw);
    }
    SHADERDATA VSMain(float4 vertex : POSITION, float2 uv : TEXCOORD0)
    {
        SHADERDATA vs;
        vs.position = UnityObjectToClipPos(vertex);
        vs.uv = uv;
        vs._ShadowCoord = ComputeScreenPos(vs.position);
        return vs;
    }
    float4 PSMain(SHADERDATA ps) : SV_TARGET
    {
        float4 col = tex2D(_MainTex, ps.uv * _TileAmount);
        return lerp(col * float4(_ShadowBrightness, _ShadowBrightness, _ShadowBrightness, 1), col, step(0.5, SHADOW_ATTENUATION(ps)));
    }

                ENDCG
            }

            Pass
            {
                Tags{ "LightMode" = "ShadowCaster" }
                CGPROGRAM
                #pragma vertex VSMain
                #pragma fragment PSMain
    float4 VSMain(float4 vertex : POSITION) : SV_POSITION
    {
        return UnityObjectToClipPos(vertex);
    }
    float4 PSMain(float4 vertex : SV_POSITION) : SV_TARGET
    {
        return 0;
    }

                ENDCG
            }
        }
}