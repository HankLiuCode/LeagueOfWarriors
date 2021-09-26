Shader "Custom/UnlitSimpleDisolve"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MainColor("Main Color", Color) = (1,1,1,1)
        _DissolveTex("Dissolve Texture", 2D) = "white"{}
        _DissolveValue("Dissolve value", Range(0, 1)) = 0

        _DissolveColor("Dissolve Color", Color) = (1,1,1,1)
        _DissolveEdge("Dissolve Edge", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _MainColor;

            sampler2D _DissolveTex;
            float _DissolveValue;
            fixed4 _DissolveColor;
            float _DissolveEdge;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                float4 dissolve = tex2D(_DissolveTex, i.uv);
                float fDelta = dissolve.r - _DissolveValue;
                if (fDelta < 0) {
                    discard;
                }

                float fWeight = 1.0 - clamp(fDelta / _DissolveEdge, 0.0, 1.0);
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                col.rgb += _DissolveColor.rgb * fWeight;
                return col;
            }
            ENDCG
        }
    }
}
