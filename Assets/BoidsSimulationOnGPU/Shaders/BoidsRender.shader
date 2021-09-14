Shader "Hidden/BoidsSimulationOnGPU/BoidsRender"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_BoxDepth("BoxDepth", Int) = 32
		_LowerAlphaBound("LowerAlphaBound", Range(0,1)) = 0.2
	}


	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
					
		}
		LOD 200
		Cull Off
		Lighting Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"


			//#pragma surface surf Standard vertex:vert addshadow
			//#pragma instancing_options procedural:setup


			struct Input
			{
				float2 uv_MainTex;
			};
			// Boidの構造体
			struct BoidData
			{
				uint id;
				float3 velocity; // 速度
				float3 position; // 位置
				float4 color;   // カラー
				float4 initColor;//初期カラー
				float life;
				float intLife;
				float lifeDecMultiplier;
				uint interactionEnabled; //インタラクション対象か
				float interactionTime; //インタラクション時間
			};

			//#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			// Boidデータの構造体バッファ
			StructuredBuffer<BoidData> _BoidDataBuffer;
			//#endif

			sampler2D _MainTex; // テクスチャ
			

			half   _Glossiness; // 光沢
			half   _Metallic;   // 金属特性
			fixed4 _Color;      // カラー
			float _BoxDepth;    // シミュレーション奥行
			float _LowerAlphaBound; //最低アルファ値
			

			float3 _ObjectScale; // Boidオブジェクトのスケール

			// オイラー角（ラジアン）を回転行列に変換
			float4x4 eulerAnglesToRotationMatrix(float3 angles)
			{
				float ch = cos(angles.y); float sh = sin(angles.y); // heading
				float ca = cos(angles.z); float sa = sin(angles.z); // attitude
				float cb = cos(angles.x); float sb = sin(angles.x); // bank

				// Ry-Rx-Rz (Yaw Pitch Roll)
				return float4x4(
					ch * ca + sh * sb * sa, -ch * sa + sh * sb * ca, sh * cb, 0,
					cb * sa, cb * ca, -sb, 0,
					-sh * ca + ch * sb * sa, sh * sa + ch * sb * ca, ch * cb, 0,
					0, 0, 0, 1
					);
			}

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color: TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			// 頂点シェーダ
			v2f vert(appdata_t v, uint instanceID : SV_InstanceID) {


				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				// インスタンスIDからBoidのデータを取得
				BoidData boidData = _BoidDataBuffer[instanceID];
				float3 pos = boidData.position.xyz; // Boidの位置を取得
				float3 scl = _ObjectScale;          // Boidのスケールを取得
				float4 col = boidData.color.rgba;
				float life = boidData.life;

				// Scaleと位置を適用
				float4x4 object2world = (float4x4)0;

				// Z位置とInteractionEnabledでスケールを操作
				scl = scl * 0.45 * (-pos.z + _BoxDepth / 2.0) / _BoxDepth + boidData.interactionEnabled * 0.01;
				boidData.life = boidData.life / boidData.intLife;
				// Life でスケール調整
				scl = scl * smoothstep(0, 0.1,boidData.life) * smoothstep(1.0, 0.99, boidData.life);
				

				object2world._11_22_33_44 = float4(scl.xyz, 1.0);



				/*
				* オイラー角での頂点調整 STARTS HERE
				*/
				

				// 速度からY軸についての回転を算出
				float rotY = atan2(boidData.velocity.x, boidData.velocity.z);

				// 速度からX軸についての回転を算出
				float rotX = -asin(boidData.velocity.y / (length(boidData.velocity.xyz) + 1e-8));

				// オイラー角（ラジアン）から回転行列を求める
				float4x4 rotMatrix = eulerAnglesToRotationMatrix(float3(rotX, rotY, 0));

				// 行列に回転を適用
				object2world = mul(rotMatrix, object2world);
				


				/*
				* オイラー角での頂点調整 ENDS HERE
				*/



				// 行列に位置（平行移動）を適用
				object2world._14_24_34 += pos.xyz;

				// 頂点を座標変換
				v.vertex = mul(object2world, v.vertex);

				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				//Alpha値をz値で操作
				
				col.a *= ((-pos.z + _BoxDepth / 2.0) / _BoxDepth)* boidData.life;
				col.a += _LowerAlphaBound;

				col.a = col.a > 1.0 ? 1.0 : col.a;
				o.color = col;


				
				return o;
			}

			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 colour = tex2D(_MainTex, i.texcoord) * i.color;
				return colour;
			}
			//void setup()
			//{
			//}

			// サーフェスシェーダ
			//void surf (Input IN, inout SurfaceOutputStandard o)
			//{
			//	fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			//	o.Albedo = c.rgb;
			//	o.Metallic = _Metallic;
			//	o.Smoothness = _Glossiness;
			//}
			ENDCG
		}
	}
	
}
