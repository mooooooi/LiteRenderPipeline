using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

public partial class LiteRenderGraphRecorder
{
    internal class ClearRenderTargetPassData
    {
        internal RTClearFlags ClearFlags;
        internal Color BackgroundColor;
        internal float Depth;
        internal uint Stencil;
    }

    private static ProfilingSampler s_ClearRenderTargetPassSampler = new ProfilingSampler("ClearRenderTarget");

    private void AddClearRenderTargetPass(RenderGraph renderGraph, CameraData cameraData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<ClearRenderTargetPassData>(
            "Clear Render Target", out var passData, s_ClearRenderTargetPassSampler))
        {
            passData.ClearFlags = cameraData.GetClearFlags();
            passData.BackgroundColor = cameraData.GetBackgroundColor();
            passData.Depth = 1;
            passData.Stencil = 0;

            builder.SetRenderAttachment(m_BackBufferColorHandle, 0);
            // 调用渲染指令
            builder.SetRenderFunc(static (ClearRenderTargetPassData passData, RasterGraphContext ctx) =>
            {
                ctx.cmd.ClearRenderTarget(passData.ClearFlags, passData.BackgroundColor, passData.Depth, passData.Stencil);
            });
        }
    }
}