Shader "Unlit/Effect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _BlendMode ("Blend Mode", Float) = 0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off            
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 _Color;
            float _BlendMode;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // 블렌드 모드를 선택
                if (_BlendMode == 1) {
                    texColor.rgb = texColor.rgb * texColor.a + (1.0 - texColor.a);
                }
                else if (_BlendMode == 2) {
                    texColor.rgb = texColor.rgb * texColor.a + 1.0;
                }
                else if (_BlendMode == 3) {
                    texColor.rgb = texColor.rgb * texColor.a;
                }
                else {
                    // 기본 블렌드 모드
                    texColor = texColor;
                }

                return texColor;
            }
            ENDCG
        }

    }
}
