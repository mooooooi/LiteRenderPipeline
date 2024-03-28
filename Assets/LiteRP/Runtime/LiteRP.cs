using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public partial class LiteRP : RenderPipeline
{
    protected LiteRPAsset m_Asset;
    public static ShaderTagId ShaderTagId = new ShaderTagId("SRPDefaultUnlit");

    public LiteRP(LiteRPAsset asset)
    {
        m_Asset = asset;
        m_RenderGraph = new RenderGraph();

        InitializeRenderGraph();
    }

    protected override void Dispose(bool disposing)
    {
        CleanupRenderGraph();

        GC.SuppressFinalize(this);
        base.Dispose(disposing);
    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        throw new System.Exception("Not implemented!");
    }

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        BeginContextRendering(context, cameras);

        foreach (var cam in cameras)
        {
            RenderSingmeCamera(context, cam);
        }

        m_RenderGraph.EndFrame();
        EndContextRendering(context, cameras);
    }

    private void RenderSingmeCamera(ScriptableRenderContext context, Camera cam)
    {
        BeginCameraRendering(context, cam);

        if (!PrepareFrameData(context, cam))
            return;

        var cmd = CommandBufferPool.Get(cam.name);
        context.SetupCameraProperties(cam);

        // if (!GenericFillCommandBuffer(context, cam, cmd))
        // {
        //     cmd.Clear();
        //     CommandBufferPool.Release(cmd);
        //     return;
        // }
        RecordAndExcuteRenderGraph(context, cmd, cam);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
        context.Submit();
        EndCameraRendering(context, cam);
    }

    private bool GenericFillCommandBuffer(ScriptableRenderContext context, Camera cam, CommandBuffer cmd)
    {
        if (!cam.TryGetCullingParameters(out var cullingParams))
            return false;
        // Culling
        var cullingRet = context.Cull(ref cullingParams);
        // ClearFlags
        context.SetupCameraProperties(cam);
        var clearSkyBox = cam.clearFlags == CameraClearFlags.Skybox;
        var clearDepth = cam.clearFlags != CameraClearFlags.Nothing;
        var clearColor = cam.clearFlags == CameraClearFlags.SolidColor;
        cmd.ClearRenderTarget(clearDepth, clearColor, CoreUtils.ConvertSRGBToActiveColorSpace(cam.backgroundColor));

        // Rendering
        var sortingSettings = new SortingSettings(cam);
        var drawSettings = new DrawingSettings(ShaderTagId, sortingSettings);
        var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
        var renderListParams = new RendererListParams(cullingRet, drawSettings, filterSettings);
        var renderList = context.CreateRendererList(ref renderListParams);
        cmd.DrawRendererList(renderList);

        // Rendering skybox
        if (clearSkyBox && RenderSettings.skybox != null)
        {
            var skyRenderList = context.CreateSkyboxRendererList(cam);
            cmd.DrawRendererList(skyRenderList);
        }
        return true;
    }

}
