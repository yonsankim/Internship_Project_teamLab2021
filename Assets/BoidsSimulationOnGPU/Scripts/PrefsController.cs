using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrefsGUI;
namespace BoidsSimulationOnGPU
{
    public class PrefsController : MonoBehaviour
    {
        public List<PrefsSet> PrefsList = new List<PrefsSet>();
        private void Update()
        {
            foreach (PrefsSet ps in PrefsList)
            {
                if (Input.GetKeyUp(ps.KeyCode))
                {
                    ps.PrefsGUI.enabled ^= true;
                }
            }
        }
    }
    [System.Serializable]
    public class PrefsSet
    {
        public string Name;
        public PrefsGUISampleBase PrefsGUI;
        public KeyCode KeyCode;
    }
}