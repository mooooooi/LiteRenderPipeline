using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRenderGraphRecorder : IRenderGraphRecorder, IDisposable
    {
        private TextureHandle m_BackBufferColorHandle = TextureHandle.nullHandle;
        private RTHandle m_TargetColorHandle = null;

        public void Dispose()
        {
            RTHandles.Release(m_TargetColorHandle);
            GC.SuppressFinalize(this);
        }

        public void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<CameraData>();
            var clearFlag = cameraData.Camera.clearFlags;

            CreateRenderTargets(renderGraph, cameraData);
            AddSetupCameraPropertiesPass(renderGraph, cameraData);
            if (clearFlag != CameraClearFlags.Nothing)
            {
                AddClearRenderTargetPass(renderGraph, cameraData);
            }
                
            AddDrawOpaueObjectsPass(renderGraph, cameraData);
            if (clearFlag == CameraClearFlags.Skybox && RenderSettings.skybox != null)
            {
                AddDrawSkyboxPass(renderGraph, cameraData);
            }
            AddDrawTransparentObjectsPass(renderGraph, cameraData);

    #if UNITY_EDITOR
            AddGizmosPass(renderGraph, cameraData, GizmoSubset.PreImageEffects);
            AddGizmosPass(renderGraph, cameraData, GizmoSubset.PostImageEffects);
    #endif
        }

        private void CreateRenderTargets(RenderGraph renderGraph, CameraData cameraData)
        {
            var targetColorId = BuiltinRenderTextureType.CameraTarget;
            if (m_TargetColorHandle == null)
            {
                m_TargetColorHandle = RTHandles.Alloc(targetColorId, "BackBuffer Color");
            }

            var importColorParams = new ImportResourceParams()
            {
                clearOnFirstUse = true,
                discardOnLastUse = false,
                clearColor = cameraData.GetBackgroundColor()
            };

            var colorRT_sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
            var rtInfo = new RenderTargetInfo()
            {
                width = Screen.width,
                height = Screen.height,
                volumeDepth = 1,
                msaaSamples = 1,
                format = GraphicsFormatUtility.GetGraphicsFormat(RenderTextureFormat.Default, colorRT_sRGB)
            };
            m_BackBufferColorHandle = renderGraph.ImportTexture(m_TargetColorHandle, rtInfo, importColorParams);
        }
    }
}