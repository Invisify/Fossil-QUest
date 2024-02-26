// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Masked" {
    Properties {
        _MainTex ("Main", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Color ("Tint", Color) = (0, 0, 0, 1)
    }

    SubShader {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _MaskTex;
            uniform float4 _MainTex_ST;
            uniform float4 _MaskTex_ST;

            fixed4 _Color;

            struct app2vert
            {
                float4 position: POSITION;
                float2 texcoord: TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct vert2frag
            {
                float4 position: POSITION;
                float2 texcoord: TEXCOORD0;
                fixed4 color : COLOR;
            };

            vert2frag vert(app2vert input)
            {
                vert2frag output;
                output.position = UnityObjectToClipPos(input.position);
                output.texcoord = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.color = input.color;

                return output;
            }

            fixed4 frag(vert2frag input) : COLOR
            {
                fixed4 main_color = tex2D(_MainTex, input.texcoord);
                fixed4 mask_color = tex2D(_MaskTex, input.texcoord);
                main_color *= _Color;
                return fixed4(main_color.r, main_color.g, main_color.b, main_color.a * (1.0f - mask_color.a));
            }
            ENDCG
        }
    }
}