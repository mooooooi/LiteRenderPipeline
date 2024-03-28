using UnityEngine;
using UnityEngine.Rendering;

public class CameraData : ContextItem
{
    public Camera Camera;
    public CullingResults CullingResults;
    public override void Reset()
    {
        Camera = default;
        CullingResults = default;
    }

    public RTClearFlags GetClearFlags()
    {
        var clearFlags = Camera.clearFlags;
        if (clearFlags == CameraClearFlags.Depth)
            return RTClearFlags.DepthStencil;
        if (clearFlags == CameraClearFlags.Nothing)
            return RTClearFlags.None;
        return RTClearFlags.All;
    }

    public Color GetBackgroundColor()
    {
        return CoreUtils.ConvertSRGBToActiveColorSpace(Camera.backgroundColor);
    }
}