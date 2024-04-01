using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        internal class DrawOpaueObjectsPassData
        {
            internal RendererListHandle renderListHandle;
        }

        private static ProfilingSampler s_DrawOpaueObjectsPassSampler = new ProfilingSampler("DrawOpaueObjectPass");

        private void AddDrawOpaueObjectsPass(RenderGraph renderGraph, CameraData cameraData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<DrawOpaueObjectsPassData>(
                "Draw Objects Pass", out var passData, s_DrawOpaueObjectsPassSampler))
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
                passData.renderListHandle = opaueRendererList;
                builder.UseRendererList(opaueRendererList);

                // 导入BackBuffer
                builder.SetRenderAttachment(m_BackBufferColorHandle, 0);
                // 调用渲染指令
                builder.SetRenderFunc(static (DrawOpaueObjectsPassData passData, RasterGraphContext ctx) =>
                {
                    ctx.cmd.DrawRendererList(passData.renderListHandle);
                });
            }
        }
    }
}