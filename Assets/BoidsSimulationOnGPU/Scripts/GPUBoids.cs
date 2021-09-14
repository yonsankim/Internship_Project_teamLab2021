using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BoidsSimulationOnGPU
{
    public class GPUBoids : MonoBehaviour
    {
        // Boidデータの構造体
        [System.Serializable]
        struct BoidData
        {
            public uint Id;
            public Vector3 Velocity; // 速度
            public Vector3 Position; // 位置
            public Color color;      // 色
            public Color initColor;  // 初期色
            public float life;
            public float intLife;
            public float lifeDecMultiplier;
            public uint interactionEnabled; //インタラクション対象か - bool はnon-blittable
            public float interactionTime; //インタラクション時間
        }


        struct BoidTypesData
        {
            public float CohesionNeighbourhoodRadius;
            public float AlignmentNeighbourhoodRadius;
            public float SeparateNeighbourhoodRadius;
            public float MaxSpeed;
            public float MaxSteerForce;
            public float CohesionWeight;
            public float AlignmentWeight;
            public float SeparateWeight;

        }
        // スレッドグループのスレッドのサイズ
        const int SIMULATION_BLOCK_SIZE = 256;

        #region Boids Parameters
        // 最大オブジェクト数
        [Range(256, 32768)]
        public int MaxObjectNum = 16384;

        // 結合を適用する他の個体との半径
        public float CohesionNeighborhoodRadius = 2.0f;
        // 整列を適用する他の個体との半径
        public float AlignmentNeighborhoodRadius = 2.0f;
        // 分離を適用する他の個体との半径
        public float SeparateNeighborhoodRadius = 1.0f;

        // 速度の最大値
        public float MaxSpeed = 5.0f;
        // 操舵力の最大値
        public float MaxSteerForce = 0.5f;

        // 結合する力の重み
        public float CohesionWeight = 1.0f;
        // 整列する力の重み
        public float AlignmentWeight = 1.0f;
        // 分離する力の重み
        public float SeparateWeight = 3.0f;

        // 壁を避ける力の重み
        public float AvoidWallWeight = 10.0f;
        // 手を避ける力の重み
        public float AvoidHandWeight = 0.08f;


        // 壁の中心座標   
        public Vector3 WallCenter = Vector3.zero;
        // 壁のサイズ
        public Vector3 WallSize = new Vector3(32.0f, 32.0f, 32.0f);



        // Effect Radius
        [Range(0.1f, 1.0f)]
        public float EffectRadius = 0.5f;

        //width and height
        private float Width;
        private float Height;

        //Mouse Position - OBSOLETE
        private float mouseX;
        private float mouseY;

        //onClick event
        private bool clicked;
        private float alphaLevel;
        private float limitedAlphaLevel;

        //Inreaction Time
        [Range(0.1f, 10.0f)]
        public float InteractionTime = 3.0f;

        //Curl Noise - Convergence
        [Range(0.1f, 10.0f)]
        public float Convergence = 4.0f;

        // Min Alpha
        [Range(0.0f, 1.0f)]
        public float MinAlpha = 0.7f;

        [Range(10, 120)]
        public int MaxBoidLife;


        public int GroupNo;


        //Camera OrthographicSize;
        private float OrthographicSize;
        public Vector2 FOneAspectRatio;

        public float NoiseScale;
        public float NoiseAspect;

        #endregion

        #region Built-in Resources
        // Boidsシミュレーションを行うComputeShaderの参照
        public ComputeShader BoidsCS;
        #endregion

        #region Private Resources
        // Boidの操舵力（Force）を格納したバッファ
        ComputeBuffer _boidForceBuffer;
        // Boidの基本データ（速度, 位置, Transformなど）を格納したバッファ
        ComputeBuffer _boidDataBuffer;
        // Boidの種類データを格納したバッファ
        ComputeBuffer _boidTypesBuffer;
        #endregion


        #region Scriptable Objects
        public BoidsType _boidsTypesData;
        
        #endregion

        #region Accessors
        // Boidの基本データを格納したバッファを取得
        public ComputeBuffer GetBoidDataBuffer()
        {
            return this._boidDataBuffer != null ? this._boidDataBuffer : null;
        }
        public ComputeBuffer GetBoidDataTypesBuffer()
        {
            return this._boidTypesBuffer != null ? this._boidTypesBuffer : null;
        }

        // オブジェクト数を取得
        public int GetMaxObjectNum()
        {
            return this.MaxObjectNum;
        }

        // シミュレーション領域の中心座標を返す
        public Vector3 GetSimulationAreaCenter()
        {
            return this.WallCenter;
        }

        // シミュレーション領域のボックスのサイズを返す
        public Vector3 GetSimulationAreaSize()
        {
            return this.WallSize;
        }
        #endregion

        #region MonoBehaviour Functions
        void Start()
        {
            // Set FRAME RATE
            Application.targetFrameRate = 60;


            // バッファを初期化
            InitBuffer();
            if (Camera.main)
            {
                //This enables the orthographic mode
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = 5.0f;
                OrthographicSize = Camera.main.orthographicSize;
                WallSize.y = OrthographicSize * 2;
                float aspectRatio = (FOneAspectRatio.x / FOneAspectRatio.y);
                WallSize.x = WallSize.y * aspectRatio;

            }


            //BoidType boidCohesive_cohesive = _boidsTypesData.boidTypesList[0];

            
            //float CohesiveMaxSpeed = _boidsTypesData.boidTypesList[0].MaxSpeed;
            //Debug.Log(CohesiveMaxSpeed);
  
        }

        void Update()
        {



            // アスペクト
            Width = Screen.width;
            Height = Screen.height;
            float aspectRatio = Width / Height;
            



            // mousePos  corrected aspect from (0,0)(width, height) to (-1, -1)(1, 1)
            mouseX = 2 * (Input.mousePosition.x / Width) - 1;
            mouseY = 2 * (Input.mousePosition.y / Height) - 1;


            //mouse Clicked event
            clicked = Input.GetMouseButton(0);
            if (clicked)
            {
                alphaLevel += Time.deltaTime / 2.0f;
                limitedAlphaLevel = LimitAlphalevel(alphaLevel);
                //    Debug.Log("limitedAlpha: " + limitedAlphaLevel);
                //    Debug.Log("clicked: " + clicked);
            }
            else
            {
                alphaLevel = 0;
                //Debug.Log("limitedAlpha: " + alphaLevel);
                //Debug.Log("clicked: " + clicked);
            }
            // Set Boid Type Buffer
            

            // シミュレーション
            Simulation();

            //Debug.Log("deltaTime: " + Time.deltaTime);

            

        }

        void OnDestroy()
        {
            // バッファを破棄
            ReleaseBuffer();
        }

        void OnDrawGizmos()
        {
            // デバッグとしてシミュレーション領域をワイヤーフレームで描画
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(WallCenter, WallSize);
        }
        #endregion

        #region Private Functions
        // バッファを初期化
        void InitBuffer()
        {
            // バッファを初期化
            _boidDataBuffer = new ComputeBuffer(MaxObjectNum,
                Marshal.SizeOf(typeof(BoidData)));
            _boidForceBuffer = new ComputeBuffer(MaxObjectNum,
                Marshal.SizeOf(typeof(Vector3)));
            _boidTypesBuffer = new ComputeBuffer(MaxObjectNum,
                Marshal.SizeOf(typeof(BoidTypesData)));


            // Boidデータ, Forceバッファを初期化
            var forceArr = new Vector3[MaxObjectNum];
            var boidDataArr = new BoidData[MaxObjectNum];
            
            for (var i = 0; i < MaxObjectNum; i++)
            {
                forceArr[i] = Vector3.zero;
                boidDataArr[i].Id = (uint)(i%GroupNo);
                boidDataArr[i].Position = Random.insideUnitSphere * 1.0f;
                boidDataArr[i].Velocity = Random.insideUnitSphere * 0.1f;
                //boidDataArr[i].color = (boidDataArr[i].Id == 1) ? new Color(0, Random.value, Random.Range(0.7f, 1.0f)) : new Color(Random.value, 0, Random.Range(0.7f, 1.0f));
                boidDataArr[i].color = (boidDataArr[i].Id == 1) ? new Color(0.07f, 0.63f, Random.Range(0.47f, 1.0f)) : new Color(0.98f, 0.93f,0.79f);
                //boidDataArr[i].color = new Color(0, Random.value, Random.Range(0.7f, 1.0f));
                boidDataArr[i].initColor = boidDataArr[i].color;
                boidDataArr[i].life = Random.Range(MaxBoidLife-10, MaxBoidLife);
                boidDataArr[i].intLife = boidDataArr[i].life;
                boidDataArr[i].lifeDecMultiplier = 1.0f;
                boidDataArr[i].interactionEnabled = 0;
                boidDataArr[i].interactionTime = 0.0f;

               



                // https://docs.unity3d.com/ja/2018.4/ScriptReference/Random-insideUnitSphere.html
            }



            _boidForceBuffer.SetData(forceArr);
            _boidDataBuffer.SetData(boidDataArr);
            forceArr = null;
            boidDataArr = null;

            setBoidTypesBuffer();
        }
        void setBoidTypesBuffer()
        {

            

            var boidTypesDataArr = new BoidTypesData[2];

            for (var i = 0; i < 2; i++)
            {
                boidTypesDataArr[i].CohesionNeighbourhoodRadius = _boidsTypesData.boidTypesList[i].CohesionNeighbourhoodRadius;
                boidTypesDataArr[i].AlignmentNeighbourhoodRadius = _boidsTypesData.boidTypesList[i].AlignmentNeighbourhoodRadius;
                boidTypesDataArr[i].SeparateNeighbourhoodRadius = _boidsTypesData.boidTypesList[i].SeparateNeighbourhoodRadius;
                boidTypesDataArr[i].MaxSpeed = _boidsTypesData.boidTypesList[i].MaxSpeed;
                boidTypesDataArr[i].MaxSteerForce = _boidsTypesData.boidTypesList[i].MaxSteerForce;
                boidTypesDataArr[i].CohesionWeight = _boidsTypesData.boidTypesList[i].CohesionWeight;
                boidTypesDataArr[i].AlignmentWeight = _boidsTypesData.boidTypesList[i].AlignmentWeight;
                boidTypesDataArr[i].SeparateWeight = _boidsTypesData.boidTypesList[i].SeparateWeight;
            }

            _boidTypesBuffer.SetData(boidTypesDataArr);
            boidTypesDataArr = null;
  
        }
        // シミュレーション
        void Simulation()
        {
            ComputeShader cs = BoidsCS;
            int id = -1;

            // スレッドグループの数を求める
            int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);

            // 操舵力を計算
            id = cs.FindKernel("ForceCS"); // カーネルIDを取得
            cs.SetInt("_MaxBoidObjectNum", MaxObjectNum);
            cs.SetFloat("_CohesionNeighborhoodRadius", CohesionNeighborhoodRadius);
            cs.SetFloat("_AlignmentNeighborhoodRadius", AlignmentNeighborhoodRadius);
            cs.SetFloat("_SeparateNeighborhoodRadius", SeparateNeighborhoodRadius);
            cs.SetFloat("_MaxSpeed", MaxSpeed);
            cs.SetFloat("_MaxSteerForce", MaxSteerForce);
            cs.SetFloat("_SeparateWeight", SeparateWeight);
            cs.SetFloat("_CohesionWeight", CohesionWeight);
            cs.SetFloat("_AlignmentWeight", AlignmentWeight);
            cs.SetFloat("_NoiseScale", NoiseScale);
            cs.SetFloat( "_NoiseAspect;", NoiseAspect);
            cs.SetVector("_WallCenter", WallCenter);
            cs.SetVector("_WallSize", WallSize);
            cs.SetFloat("_AvoidWallWeight", AvoidWallWeight);
            cs.SetFloat("_Time", Time.time);
            cs.SetInt("_GroupNo", GroupNo);
            cs.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
            cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
            cs.SetBuffer(id, "_BoidTypesBufferRead", _boidTypesBuffer);

            cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行

            // 操舵力から、速度と位置を計算
            id = cs.FindKernel("IntegrateCS"); // カーネルIDを取得
            cs.SetFloat("_DeltaTime", Time.deltaTime);
            cs.SetFloat("_minAlpha", MinAlpha);
            cs.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
            cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
            
            cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行



            if (Input.GetMouseButton(0))
            {
                //Set interaction time
                id = cs.FindKernel("setInteractionCS"); // カーネルIDを取得

                cs.SetFloat("_width", Width);
                cs.SetFloat("_height", Height);
                cs.SetFloat("_mouseX", mouseX);
                cs.SetFloat("_mouseY", mouseY);
                cs.SetFloat("_limitedAlphaLevel", limitedAlphaLevel);
                cs.SetFloat("_effectRadius", EffectRadius);
                cs.SetBool("_clicked", clicked);
                cs.SetMatrix("_worldToCameraMatrix", Camera.main.worldToCameraMatrix);
                cs.SetMatrix("_projectionMatrix", Camera.main.projectionMatrix);
                cs.SetMatrix("_projectionMatrix", Camera.main.projectionMatrix);
                cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
                cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行
            }



            //Apply interaction
            id = cs.FindKernel("applyInteractionCS"); // カーネルIDを取得
            cs.SetFloat("_AvoidHandWeight", AvoidHandWeight);
            cs.SetFloat("_interactionTime", InteractionTime);
            cs.SetFloat("_convergence", Convergence);
            cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
            cs.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行


            //Track boid Life time
            id = cs.FindKernel("trackLifeCS"); // カーネルIDを取得
            cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
            cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
            cs.Dispatch(id, threadGroupSize, 1, 1); // ComputeShaderを実行

        }

        // バッファを解放

        void ReleaseBuffer()
        {
            if (_boidDataBuffer != null)
            {
                _boidDataBuffer.Release();
                _boidDataBuffer = null;
            }

            if (_boidForceBuffer != null)
            {
                _boidForceBuffer.Release();
                _boidForceBuffer = null;
            }

            if (_boidTypesBuffer != null)
            {
                _boidTypesBuffer.Release();
                _boidTypesBuffer = null;
            }
        }



        float LimitAlphalevel(float alphaLevel)
        {
            float limitedAlphaLevel = Mathf.Clamp01(alphaLevel);
            return limitedAlphaLevel;
        }
        #endregion
    } // class
} // namespace