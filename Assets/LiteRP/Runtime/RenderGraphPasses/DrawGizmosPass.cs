#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder
    {
        internal class DrawGizmosPass
        {
            internal RendererListHandle renderListHandle;
        }

        private static ProfilingSampler s_DrawGizmosPassSampler = new ProfilingSampler("DrawGizmosPass");

        private void AddGizmosPass(RenderGraph renderGraph, CameraData cameraData, GizmoSubset gizmoSubset)
        {
            if (!Handles.ShouldRenderGizmos() || cameraData.Camera.sceneViewFilterMode == Camera.SceneViewFilterMode.ShowFiltered)
                return;
            using (var builder = renderGraph.AddRasterRenderPass<DrawSkyboxPass>(
                "Draw Gizmos Pass", out var passData, s_DrawGizmosPassSampler))
            {
                // 声明资源引用
                var camera = cameraData.Camera;
                passData.renderListHandle = renderGraph.CreateGizmoRendererList(camera, gizmoSubset);
                builder.UseRendererList(passData.renderListHandle);

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
#endif