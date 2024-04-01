using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using System.Data.Common;
using UnityEngine.Rendering.RenderGraphModule;

namespace LiteRP
{
    public partial class LiteRP
    {
        protected RenderGraph m_RenderGraph;
        protected LiteRenderGraphRecorder m_RenderGraphRecorder;
        protected ContextContainer m_CtxContainer;

        protected void InitializeRenderGraph()
        {
            m_RenderGraph = new RenderGraph("LRP");
            m_RenderGraphRecorder = new LiteRenderGraphRecorder();
            m_CtxContainer = new ContextContainer();
        }

        protected void CleanupRenderGraph()
        {
            m_RenderGraph.Cleanup();
            m_RenderGraph = null;

            m_RenderGraphRecorder?.Dispose();
            m_RenderGraphRecorder = null;

            m_CtxContainer.Dispose();
            m_CtxContainer = null;
        }

        private bool PrepareFrameData(ScriptableRenderContext ctx, Camera camera)
        {
            if (!camera.TryGetCullingParameters(out var cullingParams))
                return false;
            var cullingRet = ctx.Cull(ref cullingParams);
            var cameraData = m_CtxContainer.GetOrCreate<CameraData>();
            cameraData.Camera = camera;
            cameraData.CullingResults = cullingRet;
            return true;
        }

        protected void RecordAndExcuteRenderGraph(
            ScriptableRenderContext ctx,
            CommandBuffer cmd,
            Camera camera)
        {
            var renderGraphParams = new RenderGraphParameters
            {
                executionName = camera.name,
                scriptableRenderContext = ctx,
                commandBuffer = cmd,
                currentFrameIndex = Time.frameCount
            };

            m_RenderGraph.BeginRecording(renderGraphParams);
            m_RenderGraphRecorder.RecordRenderGraph(m_RenderGraph, m_CtxContainer);
            m_RenderGraph.EndRecordingAndExecute();
        }
    }
}