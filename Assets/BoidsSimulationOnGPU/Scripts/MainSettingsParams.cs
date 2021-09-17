using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrefsGUI;

namespace BoidsSimulationOnGPU{
    public class MainSettingsParams : PrefsGUISampleBase
    {

        private void Awake()
        {
            _boidsGPUFolds.Add("GPUBoids", () =>
            {
                maxObjectNum.OnGUI();
                emitCount.OnGUI();
                avoidWallWeight.OnGUI(); ;
                avoidHandWeight.OnGUISlider();
                wallCenter.OnGUI();
                wallSize.OnGUI();
                effectRadius.OnGUISlider(); 
                interactionTime.OnGUISlider();
                convergence.OnGUISlider();
                minAlpha.OnGUISlider();
                maxBoidLife.OnGUI();
                fOneAspect.OnGUI();
                noiseScale.OnGUISlider();
                noiseAspect.OnGUISlider();
                particleColor.OnGUI();


            });


            _boidsTypeFolds.Add("BoidsType", () =>
            {
                cohesionNeighbourhoodRadius1.OnGUI();
                alignmentNeighbourhoodRadius1.OnGUI();
                separateNeighbourhoodRadius1.OnGUI();
                maxSpeed1.OnGUI();
                maxSteerForce1.OnGUISlider();
                cohesionWeight1.OnGUI();
                alignmentWeight1.OnGUI();
                separateWeight1.OnGUI();
                
                cohesionNeighbourhoodRadius2.OnGUI();
                alignmentNeighbourhoodRadius2.OnGUI();
                separateNeighbourhoodRadius2.OnGUI();
                maxSpeed2.OnGUI();
                maxSteerForce2.OnGUISlider();
                cohesionWeight2.OnGUI();
                alignmentWeight2.OnGUI();
                separateWeight2.OnGUI();



            });
            _boidsRendererFolds.Add("BoidsRender", () =>
            {

                objectScale.OnGUI();

            });

            _imageEffectBaseFolds.Add("ImageEffectBase", () =>
            {
                prevCurBelndRatio.OnGUISlider(0, 1);
                baseNewBaseBlendRatio.OnGUISlider(0, 1);
                debug.OnGUISlider(0, 1);


            });
            _imageEffectBaseFolds.Add("ImageEffectBaseSpout", () =>
            {

                debug1.OnGUISlider(0, 1);


            });



            Prefs.Load(FileName);
            SetData();
            enabled = false;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            SetData();
        }

        public string FileName = "MainSetting";

        [SerializeField]
        GPUBoids _gpuboids;


        [SerializeField]
        BoidsType _boidTypes;
        
        [SerializeField]
        BoidsRender _boidsRender;


        [SerializeField]
        ImageEffectBase _imageEffectBase;

        [SerializeField]
        ImageEffectBase _imageEffectBaseSpout;


        #region Folds
        GUIUtil.Folds _boidsGPUFolds = new GUIUtil.Folds();
        GUIUtil.Folds _boidsTypeFolds = new GUIUtil.Folds();
        GUIUtil.Folds _boidsRendererFolds = new GUIUtil.Folds();
        GUIUtil.Folds _imageEffectBaseFolds = new GUIUtil.Folds();
        GUIUtil.Folds _imageEffectBaseSpoutFolds = new GUIUtil.Folds();

        #endregion

        #region PrefsGUI
        [Header("BoidsGPU")]
        public PrefsInt maxObjectNum = new PrefsInt("MaxObjectNum", 20000);
        public PrefsInt emitCount = new PrefsInt("EmitCount", 100);
        public PrefsFloat  avoidWallWeight= new PrefsFloat("AvoidWallWeight", 250);
        public PrefsFloat  avoidHandWeight= new PrefsFloat("AvoidHandWeight", 0.08f);
        public PrefsVector3 wallCenter = new PrefsVector3("WallsCenter", new Vector3(0.0f, 0.0f, 0.0f));
        public PrefsVector3 wallSize = new PrefsVector3("WallSize", new Vector3(32, 20, 12));
        public PrefsFloat effectRadius = new PrefsFloat("EffectRadius", 0.45f);
        public PrefsFloat interactionTime = new PrefsFloat("InteractionTime", 3.0f);
        public PrefsFloat convergence = new PrefsFloat("Convergence", 0.1f);
        public PrefsFloat minAlpha = new PrefsFloat("MinAlpha", 0.9f);
        public PrefsInt maxBoidLife = new PrefsInt("MaxBoidLife", 120);
        public PrefsVector2 fOneAspect = new PrefsVector2("FOneAspect", new Vector2(6690, 2160));
        public PrefsFloat noiseScale = new PrefsFloat("NoiseScale", 1.0f);
        public PrefsFloat noiseAspect = new PrefsFloat("NoiseAspect", 1.0f);
        public PrefsColor particleColor = new PrefsColor("particleColor", new Color(0.98f, 0.93f, 0.79f));


        [Header("BoidsType")]
        public PrefsFloat cohesionNeighbourhoodRadius1 = new PrefsFloat("CohesionNeighbourhoodRadius1", 2.67f);
        public PrefsFloat alignmentNeighbourhoodRadius1 = new PrefsFloat("AlignmentNeighbourhoodRadius1", 2.0f);
        public PrefsFloat separateNeighbourhoodRadius1 = new PrefsFloat("AeparateNeighbourhoodRadius1", 2.0f);
        public PrefsFloat maxSpeed1 = new PrefsFloat("MaxSpeed1", 7.52f);
        public PrefsFloat maxSteerForce1 = new PrefsFloat("MaxSteerForce1", 0.5f);
        public PrefsFloat cohesionWeight1 = new PrefsFloat("CohesionWeight1", 2.0f);
        public PrefsFloat alignmentWeight1 = new PrefsFloat("AlignmentWeight1", 1.0f);
        public PrefsFloat separateWeight1 = new PrefsFloat("SeparateWeight1", 3.0f);
        
        public PrefsFloat cohesionNeighbourhoodRadius2= new PrefsFloat("CohesionNeighbourhoodRadius2",3.0f);
        public PrefsFloat alignmentNeighbourhoodRadius2 = new PrefsFloat("AlignmentNeighbourhoodRadius2", 2.0f);
        public PrefsFloat separateNeighbourhoodRadius2= new PrefsFloat("AeparateNeighbourhoodRadius2", 2.0f);
        public PrefsFloat maxSpeed2= new PrefsFloat("MaxSpeed2", 5.0f);
        public PrefsFloat maxSteerForce2= new PrefsFloat("MaxSteerForce2", 0.5f);
        public PrefsFloat cohesionWeight2 = new PrefsFloat("CohesionWeight",2.0f);
        public PrefsFloat alignmentWeight2 = new PrefsFloat("AlignmentWeight2", 1.0f);
        public PrefsFloat separateWeight2 = new PrefsFloat("SeparateWeight2", 2.63f);


        [Header("BoidsRenderer")]
        public PrefsVector3 objectScale = new PrefsVector3("ObjectScale", new Vector3(0.04f, 0.06f, 0.75f));


        [Header("ImageEffect")]
        public PrefsFloat prevCurBelndRatio = new PrefsFloat("PrevCurBelndRatio", 1.0f);
        public PrefsFloat baseNewBaseBlendRatio = new PrefsFloat("BaseNewBaseBlendRatio", 0.3f);
        public PrefsFloat debug = new PrefsFloat("Debug", 1.0f);
        
        [Header("ImageEffectSpoud")]

        public PrefsFloat debug1 = new PrefsFloat("Debug1", 1.0f);



        //public PrefsFloat prefsFloat = new PrefsFloat("PrefsFloat");
        //public PrefsString prefsString = new PrefsString("PrefsString");
        //public PrefsColor prefsColor = new PrefsColor("PrefsColor");
        //public PrefsVector2 prefsVector2 = new PrefsVector2("PrefsVector2");
        //public PrefsVector3 prefsVector3 = new PrefsVector3("PrefsVector3");
        //public PrefsVector4 prefsVector4 = new PrefsVector4("PrefsVector4");

        #endregion

        protected override void OnGUIInternal()
        {
            GUI.skin.label.fontSize = 12;
            _boidsGPUFolds.OnGUI();
            // ------------------------------------------------------------
            // BoidsType
            // ------------------------------------------------------------
            _boidsTypeFolds.OnGUI();
            // ------------------------------------------------------------
            // BoidsRender
            // ------------------------------------------------------------
            _boidsRendererFolds.OnGUI();
            // ------------------------------------------------------------
            // ImageEffectBase
            // ------------------------------------------------------------
            _imageEffectBaseFolds.OnGUI();
            // ------------------------------------------------------------
            // ImageEffectBaseSpout
            // ------------------------------------------------------------
            _imageEffectBaseSpoutFolds.OnGUI();

            if (GUILayout.Button("Save"))
                Prefs.Save(FileName);
            if (GUILayout.Button("Reset"))
                Prefs.DeleteAll();
        }


        void SetData()
        {
            if(_gpuboids != null)
            {
                _gpuboids.MaxObjectNum = maxObjectNum;
                _gpuboids.emitCount = emitCount;
                _gpuboids.AvoidWallWeight = avoidWallWeight;
                _gpuboids.AvoidHandWeight = avoidHandWeight;
                _gpuboids.WallCenter = wallCenter;
                _gpuboids.WallSize = wallSize;
                _gpuboids.EffectRadius = effectRadius;
                _gpuboids.InteractionTime = interactionTime;
                _gpuboids.Convergence = convergence;
                _gpuboids.MinAlpha = minAlpha;
                _gpuboids.MaxBoidLife = maxBoidLife;
                _gpuboids.FOneAspectRatio = fOneAspect;
                _gpuboids.NoiseScale = noiseScale;
                _gpuboids.NoiseAspect = noiseAspect;
                _gpuboids.ParticleColor = particleColor;


            }
            if(_boidTypes != null)
            {
                _boidTypes.boidTypesList[0].CohesionNeighbourhoodRadius = cohesionNeighbourhoodRadius1;
                _boidTypes.boidTypesList[0].AlignmentNeighbourhoodRadius = alignmentNeighbourhoodRadius1;
                _boidTypes.boidTypesList[0].SeparateNeighbourhoodRadius = separateNeighbourhoodRadius1;
                _boidTypes.boidTypesList[0].MaxSpeed = maxSpeed1;
                _boidTypes.boidTypesList[0].MaxSteerForce = maxSteerForce1;
                _boidTypes.boidTypesList[0].CohesionWeight = cohesionWeight1;
                _boidTypes.boidTypesList[0].AlignmentWeight = alignmentWeight1;
                _boidTypes.boidTypesList[0].SeparateWeight = separateWeight1;

                _boidTypes.boidTypesList[1].CohesionNeighbourhoodRadius = cohesionNeighbourhoodRadius2;
                _boidTypes.boidTypesList[1].AlignmentNeighbourhoodRadius = alignmentNeighbourhoodRadius2;
                _boidTypes.boidTypesList[1].SeparateNeighbourhoodRadius = separateNeighbourhoodRadius2;
                _boidTypes.boidTypesList[1].MaxSpeed = maxSpeed2;
                _boidTypes.boidTypesList[1].MaxSteerForce = maxSteerForce2;
                _boidTypes.boidTypesList[1].CohesionWeight = cohesionWeight2;
                _boidTypes.boidTypesList[1].AlignmentWeight = alignmentWeight2;
                _boidTypes.boidTypesList[1].SeparateWeight = separateWeight2;

            }

            if (_boidsRender != null)
            {
                _boidsRender.ObjectScale = objectScale;

            }

            if (_imageEffectBase != null)
            {
                _imageEffectBase.PrevCurBelndRatio = prevCurBelndRatio;
                _imageEffectBase.BaseNewBaseBlendRatio = baseNewBaseBlendRatio;
                _imageEffectBase.Debug = debug;

            }

            if (_imageEffectBaseSpout != null)
            {

                _imageEffectBaseSpout.Debug = debug;

            }
        }
    }
}
