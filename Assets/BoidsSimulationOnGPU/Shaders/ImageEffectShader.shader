// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius("Radius", Range(0, 10)) = 0
        _NoiseScale("Noise Scale", Range(0, 50)) = 10
        _NoiseAspect("Noise Aspct", Range(0, 10)) = 1
    }
        SubShader
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Pass
            {   
                CGPROGRAM
                #pragma vertex vert_img
                #pragma fragment frag
                #pragma target 3.0
                #include "UnityCG.cginc"
                #include "../Shaders/PhotoshopMath.cginc"
                

                struct v2f {
                    float4 pos : SV_POSITION;
                    half2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _Prev;
                float _Ratio;
                float _BaseNewBaseBlendRatio;
                float _Debug;


                

                v2f vert(appdata_base v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                int _Radius;
                float4 _MainTex_TexelSize;

                //////////////////////////////////////////////////////
                // HASH AND NOISE
                float2 hash(in float2 x)
                {
                    const float2 k = float2(0.3183099, 0.3678794);
                    x = x * k + k.yx;
                    return -1.0 + 2.0 * frac(16.0 * k * frac(x.x * x.y * (x.x + x.y)));
                }

                float3 noised(in float2 p)
                {
                    float2 i = floor(p);
                    float2 f = frac(p);

                    //quintic interpolation
                    float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
                    float2 du = 30.0 * f * f * (f * (f - 2.0) + 1.0);

                    float2 ga = hash(i + float2(0.0, 0.0));
                    float2 gb = hash(i + float2(1.0, 0.0));
                    float2 gc = hash(i + float2(0.0, 1.0));
                    float2 gd = hash(i + float2(1.0, 1.0));


                    float va = dot(ga, f - float2(0.0, 0.0));
                    float vb = dot(gb, f - float2(1.0, 0.0));
                    float vc = dot(gc, f - float2(0.0, 1.0));
                    float vd = dot(gd, f - float2(1.0, 1.0));

                    return float3(va + u.x * (vb - va) + u.y * (vc - va) + u.x * u.y * (va - vb - vc + vd), 
                        ga + u.x * (gb - ga) + u.y * (gc - ga) + u.x * u.y * (ga - gb - gc + gd) + 
                        du * (u.yx * (va - vb - vc + vd) + float2(vb, vc) - va));
                }


                //STARRY BLUR
                //https://www.shadertoy.com/view/tsXBRH
                const float numTap = 15;
                float2 getField(float2 p)
                {
                    float3 nzd = noised(p * 2.0);
                    nzd += noised(p * 8.) * 0.3;
                    return normalize(float2(nzd.z, -nzd.y));
                }

                //////////////////////////////////////////////////////
                
                float random(float2 seeds)
                {
                    return frac(sin(dot(seeds, float2(12.9898, 78.233))) * 43758.5453);
                }

                float2 random2(float2 seeds)
                {
                    seeds = float2(dot(seeds, float2(127.1, 311.7)),
                        dot(seeds, float2(269.5, 183.3)));

                    return frac(sin(seeds) * 43758.5453123);
                }

                float3 random3(float3 seeds)
                {
                    seeds = float3(dot(seeds, float3(127.1, 311.7, 542.3)),
                        dot(seeds, float3(269.5, 183.3, 461.7)),
                        dot(seeds, float3(732.1, 845.3, 231.7)));

                    return frac(sin(seeds) * 43758.5453123);
                }

                float perlinNoise3D(float3 seeds)
                {
                    float3 i = floor(seeds);
                    float3 f = frac(seeds);

                    float3 i000 = i + float3(0, 0, 0);
                    float3 i100 = i + float3(1, 0, 0);
                    float3 i010 = i + float3(0, 1, 0);
                    float3 i110 = i + float3(1, 1, 0);
                    float3 i001 = i + float3(0, 0, 1);
                    float3 i101 = i + float3(1, 0, 1);
                    float3 i011 = i + float3(0, 1, 1);
                    float3 i111 = i + float3(1, 1, 1);

                    float3 f000 = f - float3(0, 0, 0);
                    float3 f100 = f - float3(1, 0, 0);
                    float3 f010 = f - float3(0, 1, 0);
                    float3 f110 = f - float3(1, 1, 0);
                    float3 f001 = f - float3(0, 0, 1);
                    float3 f101 = f - float3(1, 0, 1);
                    float3 f011 = f - float3(0, 1, 1);
                    float3 f111 = f - float3(1, 1, 1);

                    float3 g000 = normalize(-1 + 2 * random3(i000));
                    float3 g100 = normalize(-1 + 2 * random3(i100));
                    float3 g010 = normalize(-1 + 2 * random3(i010));
                    float3 g110 = normalize(-1 + 2 * random3(i110));
                    float3 g001 = normalize(-1 + 2 * random3(i001));
                    float3 g101 = normalize(-1 + 2 * random3(i101));
                    float3 g011 = normalize(-1 + 2 * random3(i011));
                    float3 g111 = normalize(-1 + 2 * random3(i111));

                    float v000 = dot(g000, f000);
                    float v100 = dot(g100, f100);
                    float v010 = dot(g010, f010);
                    float v110 = dot(g110, f110);
                    float v001 = dot(g001, f001);
                    float v101 = dot(g101, f101);
                    float v011 = dot(g011, f011);
                    float v111 = dot(g111, f111);

                    float3 p = smoothstep(0, 1, f);

                    float v000v100 = lerp(v000, v100, p.x);
                    float v010v110 = lerp(v010, v110, p.x);
                    float v001v101 = lerp(v001, v101, p.x);
                    float v011v111 = lerp(v011, v111, p.x);

                    float v000v100v010v110 = lerp(v000v100, v010v110, p.y);
                    float v001v101v011v111 = lerp(v001v101, v011v111, p.y);

                    return lerp(v000v100v010v110,
                        v001v101v011v111, p.z) * 0.5 + 0.5;
                }

                float _NoiseScale;
                float _NoiseAspect; 

                float SpoutX;
                float SpoutY;

                float circle(float2 _uv, float _radius) {
                    float2 dist = _uv - float2(SpoutX, SpoutY);
                    return 1. - smoothstep(_radius - (_radius * 0.01),
                        _radius + (_radius * 0.01),
                        dot(dist, dist) * 4.0);
                }
                

                float4 frag(v2f_img i) : SV_Target
                {
                    float2 uv = i.uv;
                    float4 color = tex2D(_MainTex, uv);
                    float4 prevColor = tex2D(_Prev, uv);
                    

                    // debug Spout 
                    float output = circle(uv, 0.004);
                    float3 debugCircle = float3(output, output, output)*float3(1, 0, 0);

                    
                    
                     //return float4(color.rgb*color.rgb, color.a);

                    float4 baseColor = float4(color.rgb, color.a);
                    float4 prevBaseColor = float4(prevColor.rgb, prevColor.a);
                    float4 blendColor = float4(0.f, 0.f, 0.14f, 0.027f);
                    /*i.uv.x *= _NoiseAspect;

                    return perlinNoise3D(float3(i.uv, _Time.x) * _NoiseScale)*color;*/
                    //baseColor + blendColor + 
                    //return   float4(BlendColor(baseColor.rgb, blendColor), baseColor.a);
                    //return blendColor + baseColor;
                    ////////////////OPTION 1
                    /*float4 blendedCurPrev = baseColor * _Ratio + prevColor * (1 - _Ratio);
                    baseColor = baseColor* _Ratio + blendedCurPrev * (1 - _Ratio);
                    return float4(BlendScreen(baseColor.rgb, blendColor), baseColor.a*0.5);*/

                    ////////////////OPTION 2
                    float4 blendedCurPrev = baseColor * _Ratio + prevColor * (1 - _Ratio);
                    float3 newBaseColor = BlendColor(baseColor.rgb, blendedCurPrev);
                    return lerp(baseColor, float4(BlendLinearLight(newBaseColor, blendColor)+ debugCircle*_Debug, baseColor.a), _BaseNewBaseBlendRatio);

                    // BlendScreen
                }
                ENDCG
            }
        }
}
