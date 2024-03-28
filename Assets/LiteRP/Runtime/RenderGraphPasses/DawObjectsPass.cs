using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

public partial class LiteRenderGraphRecorder
{
    internal class DrawObjectsPassData
    {
        internal RendererListHandle opaueRendererListHandle;
        internal RendererListHandle transparentRendererListHandle;
    }

    private static ProfilingSampler s_DrawObjectsPassSampler = new ProfilingSampler("DrawObjectPass");

    private void AddDrawObjectsPass(RenderGraph renderGraph, CameraData cameraData)
    {
        using (var builder = renderGraph.AddRasterRenderPass<DrawObjectsPassData>(
            "Draw Objects Pass", out var passData, s_DrawObjectsPassSampler))
        {
            // 声明资源引用

            // 创建不透明对象渲染队列
            var opaueRendererListDesc = new RendererListDesc(
                LiteRP.ShaderTagId, cameraData.CullingResults, cameraData.Camera)
            {
                renderQueueRange = RenderQueueRange.opaque,
                sortingCriteria = SortingCriteria.CommonOpaque
            };
            var opaueRendererList = renderGraph.CreateRendererList(opaueRendererListDesc);
            passData.opaueRendererListHandle = opaueRendererList;
            builder.UseRendererList(opaueRendererList);

            // 创建透明对象渲染队列
            var transparentRendererListDesc = new RendererListDesc(
                LiteRP.ShaderTagId, cameraData.CullingResults, cameraData.Camera)
            {
                renderQueueRange = RenderQueueRange.transparent,
                sortingCriteria = SortingCriteria.CommonTransparent
            };
            var transparentRendererList = renderGraph.CreateRendererList(transparentRendererListDesc);
            passData.transparentRendererListHandle = transparentRendererList;
            builder.UseRendererList(transparentRendererList);

            // 导入BackBuffer
            builder.SetRenderAttachment(m_BackBufferColorHandle, 0);
            // 设置全局渲染状态
            builder.AllowPassCulling(false);
            // 调用渲染指令
            builder.SetRenderFunc(static (DrawObjectsPassData passData, RasterGraphContext ctx) => {
                ctx.cmd.DrawRendererList(passData.opaueRendererListHandle);
                ctx.cmd.DrawRendererList(passData.transparentRendererListHandle);
            });
        }
    }
}