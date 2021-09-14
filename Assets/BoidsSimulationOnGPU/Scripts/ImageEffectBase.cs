using UnityEngine;

[ExecuteAlways, ExecuteInEditMode]
// [ImageEffectAllowedInSceneView]
[RequireComponent(typeof(Camera))]
public class ImageEffectBase : MonoBehaviour
{
    public Material material;
    public PingPongRenderTexture rts;
    public RenderTexture preTexture;
    public float ratio = 1f;

    protected virtual void Start()
    {
        enabled = material && material.shader.isSupported;
        rts = new PingPongRenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetFloat("_Ratio", ratio);
        material.SetTexture("_Prev", rts.Read);
        Graphics.Blit(source, rts.Write, material);
        Graphics.Blit(rts.Write, destination, material);
        rts.Swap();//swap write and read
    }

}