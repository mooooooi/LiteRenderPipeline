using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        internal class DrawTransparentObjectsPass
        {
            internal RendererListHandle renderListHandle;
        }

        private static ProfilingSampler s_DrawTransparentObjectsPassSampler = new ProfilingSampler("DrawTransparentObjectPass");

        private void AddDrawTransparentObjectsPass(RenderGraph renderGraph, CameraData cameraData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<DrawTransparentObjectsPass>(
                "Draw Transparent Objects Pass", out var passData, s_DrawTransparentObjectsPassSampler))
            {
                // 声明资源引用

                // 创建透明对象渲染队列
                var transparentRendererListDesc = new RendererListDesc(
                    LiteRP.ShaderTagId, cameraData.CullingResults, cameraData.Camera)
                {
                    renderQueueRange = RenderQueueRange.transparent,
                    sortingCriteria = SortingCriteria.CommonTransparent
                };
                var transparentRendererList = renderGraph.CreateRendererList(transparentRendererListDesc);
                passData.renderListHandle = transparentRendererList;
                builder.UseRendererList(transparentRendererList);

                // 导入BackBuffer
                builder.SetRenderAttachment(m_BackBufferColorHandle, 0);
                // 调用渲染指令
                builder.SetRenderFunc(static (DrawTransparentObjectsPass passData, RasterGraphContext ctx) => {
                    ctx.cmd.DrawRendererList(passData.renderListHandle);
                });
            }
        }
    }
}