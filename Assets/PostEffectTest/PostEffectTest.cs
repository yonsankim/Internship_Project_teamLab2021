using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class PostEffectTest : MonoBehaviour {
    public Material mat;
    public PingPongRenderTexture rts;
    public RenderTexture preTexture;
    public float ratio = 1f;
    private void Start() {
        rts = new PingPongRenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst) {
        mat.SetFloat("_Ratio", ratio);
        mat.SetTexture("_Prev", rts.Read);
        Graphics.Blit(src, rts.Write, mat);
        Graphics.Blit(rts.Write, dst);
        rts.Swap();
    }
    private void OnDestroy() {
        rts.Dispose();
    }
}
