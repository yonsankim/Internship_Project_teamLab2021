Shader "Unlit/PostEffectTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100


        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _Prev;
            float4 _MainTex_ST;
            float _Ratio;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Ratio + tex2D(_Prev, i.uv) * (1-_Ratio);
                return col;
            }
            ENDCG
        }
    }
}
