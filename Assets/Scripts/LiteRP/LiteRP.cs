using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

public partial class LiteRP : RenderPipeline
{
    protected LiteRPAsset m_Asset;
    protected ShaderTagId m_ShaderTagId = new ShaderTagId("LiteRPLightModeTag");

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

        EndContextRendering(context, cameras);
    }

    private void RenderSingmeCamera(ScriptableRenderContext context, Camera cam)
    {
        BeginCameraRendering(context, cam);
        if (!cam.TryGetCullingParameters(out var cullingParams))
            return;
        // Culling
        var cullingRet = context.Cull(ref cullingParams);
        // ClearFlags
        var cmd = CommandBufferPool.Get(cam.name);
        context.SetupCameraProperties(cam);

        GenericFillCommandBuffer(context, cam, cullingRet, cmd);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();
        CommandBufferPool.Release(cmd);
        context.Submit();
        EndCameraRendering(context, cam);
    }

    private void GenericFillCommandBuffer(ScriptableRenderContext context, Camera cam, CullingResults cullingRet, CommandBuffer cmd)
    {
        var clearSkyBox = cam.clearFlags == CameraClearFlags.Skybox;
        var clearDepth = cam.clearFlags != CameraClearFlags.Nothing;
        var clearColor = cam.clearFlags == CameraClearFlags.SolidColor;
        cmd.ClearRenderTarget(clearDepth, clearColor, CoreUtils.ConvertSRGBToActiveColorSpace(cam.backgroundColor));

        // Rendering
        var sortingSettings = new SortingSettings(cam);
        var drawSettings = new DrawingSettings(m_ShaderTagId, sortingSettings);
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
    }

}
