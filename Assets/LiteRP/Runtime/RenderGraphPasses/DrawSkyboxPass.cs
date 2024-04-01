using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        internal class DrawSkyboxPass
        {
            internal RendererListHandle renderListHandle;
        }

        private static ProfilingSampler s_DrawSkyboxPassSampler = new ProfilingSampler("DrawSkyboxPass");

        private void AddDrawSkyboxPass(RenderGraph renderGraph, CameraData cameraData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<DrawSkyboxPass>(
                "Draw Skybox Pass", out var passData, s_DrawSkyboxPassSampler))
            {
                // 声明资源引用
                var camera = cameraData.Camera;
                var renderList = renderGraph.CreateSkyboxRendererList(camera);
                passData.renderListHandle = renderList;
                builder.UseRendererList(renderList);

                // 导入BackBuffer
                builder.SetRenderAttachment(m_BackBufferColorHandle, 0);
                // 调用渲染指令
                builder.SetRenderFunc(static (DrawSkyboxPass passData, RasterGraphContext ctx) => {
                    ctx.cmd.DrawRendererList(passData.renderListHandle);
                });
            }
        }
    }
}