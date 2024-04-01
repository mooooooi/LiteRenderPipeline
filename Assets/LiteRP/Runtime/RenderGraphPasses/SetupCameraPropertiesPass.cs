using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        internal class SetupCameraPropertiesPass
        {
            internal CameraData cameraData;
        }

        private static ProfilingSampler s_SetupCameraPropertiesPassSampler = new ProfilingSampler("SetupCameraPropertiesPass");

        private void AddSetupCameraPropertiesPass(RenderGraph renderGraph, CameraData cameraData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<SetupCameraPropertiesPass>(
                "Setup Camera Properties Pass", out var passData, s_SetupCameraPropertiesPassSampler))
            {
                // 声明资源引用
                passData.cameraData = cameraData;
                // 导入BackBuffer
                builder.SetRenderAttachment(m_BackBufferColorHandle, 0);
                // 调用渲染指令
                builder.SetRenderFunc(static (SetupCameraPropertiesPass passData, RasterGraphContext ctx) => {
                    ctx.cmd.SetupCameraProperties(passData.cameraData.Camera);
                });
            }
        }
    }
}