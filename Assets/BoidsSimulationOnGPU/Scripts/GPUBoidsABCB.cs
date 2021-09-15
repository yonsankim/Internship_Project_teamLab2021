using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BoidsSimulationOnGPU
{
    public class GPUBoidsABCB : GPUBoids
    {


        ComputeBuffer _pooledBoidBuffer;
        ComputeBuffer _boidCountBuffer;


        uint[] boidCount;
        uint frame = 0;

        // スレッドグループのスレッドのサイズ
        const int SIMULATION_BLOCK_SIZE = 256;
        public int emitCount = 24;


        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            Debug.Log(EffectRadius);
        }

        // Update is called once per frame
        protected override void Update()
        {
            frame += 1;


            if (frame % 5 == 0)
            {
                Emit();
            }

            base.Update();

        }

        protected override void InitBuffer()
        {
            ComputeShader cs = BoidsCS;
            int id = -1;
            
            // スレッドグループの数を求める
            int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);


            _pooledBoidBuffer = new ComputeBuffer(MaxObjectNum,Marshal.SizeOf(typeof(uint)), ComputeBufferType.Append);
            _pooledBoidBuffer.SetCounterValue(0);


            _boidCountBuffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(uint)), ComputeBufferType.IndirectArguments);
            boidCount = new uint[] { 0 };
            _boidCountBuffer.SetData(boidCount);

            //id = cs.FindKernel("ForceCS"); // カーネルIDを取得
            //cs.SetBuffer(id, "_DeadBoidBuffer", _pooledBoidBuffer);
            //cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);


            id = cs.FindKernel("trackLifeCS");
            //cs.SetBuffer(id, "_DeadBoidDataBuffer", _pooledBoidBuffer);
            //cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);




            //id = cs.FindKernel("applyInteractionCS");
            //cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);


            //id = cs.FindKernel("setInteractionCS");
            //cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);

            //id = cs.FindKernel("IntegrateCS");
            //cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);



            id = cs.FindKernel("Initialize");
            cs.SetBuffer(id, "_DeadBoidDataBuffer", _pooledBoidBuffer);
            cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);
            cs.SetInt("_ MaxBoidLife", MaxBoidLife);
 



            // ComputeShaderを実行
            cs.Dispatch(id, threadGroupSize, 1, 1);






            ///////////////////////////////////////////////////
            ///
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



            _boidForceBuffer.SetData(forceArr);
            _boidDataBuffer.SetData(boidDataArr);
            forceArr = null;
            boidDataArr = null;
            

            setBoidTypesBuffer();


           

        }

        protected override void Simulation()
        {

            ComputeShader cs = BoidsCS;
            int id = -1;

            // スレッドグループの数を求める
            int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);
            
            

            base.Simulation();
        }



        


        void OnGUI()
        {
            ComputeBuffer.CopyCount(_pooledBoidBuffer, _boidCountBuffer, 0);
            _boidCountBuffer.GetData(boidCount);
            GUILayout.Label("Pooled(Dead) Particles : " + boidCount[0]);
            GUI.skin.label.fontSize = 300;
            
        }

        void Emit()
        {

            ComputeShader cs = BoidsCS;
            int id = -1;

            // スレッドグループの数を求める
            int threadGroupSize = Mathf.CeilToInt(MaxObjectNum / SIMULATION_BLOCK_SIZE);

            id = cs.FindKernel("EmitCS"); // カーネルIDを取得
            cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
            cs.SetBuffer(id, "_PooledBoidDataBuffer", _pooledBoidBuffer);
            cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
            cs.Dispatch(id, emitCount / 8, 1, 1);
        }
    }
}
