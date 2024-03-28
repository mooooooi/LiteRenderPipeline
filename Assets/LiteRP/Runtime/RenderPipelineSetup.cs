using UnityEngine;
using UnityEngine.Rendering;

public class RenderPipelineSetup : MonoBehaviour
{
    public RenderPipelineAsset Asset;
    private void Awake() {
        GraphicsSettings.defaultRenderPipeline = Asset;
        Debug.Log("Setup new RenderPipelineAsset: " + Asset.name, Asset);
    }
}