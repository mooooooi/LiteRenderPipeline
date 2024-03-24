using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;
using System.Data.Common;

public partial class LiteRP
{
    protected RenderGraph m_RenderGraph;
    // protected ContextContainer m_CtxContainer; 

    protected void InitializeRenderGraph()
    {
        m_RenderGraph = new RenderGraph();
    }

    protected void CleanupRenderGraph()
    {
        m_RenderGraph.Cleanup();
        m_RenderGraph = null;
    }

    protected void StartRenderGraph(
        ScriptableRenderContext ctx,
        CommandBuffer cmd)
    {
        var renderGraphParams = new RenderGraphParameters
        {
            scriptableRenderContext = ctx,
            commandBuffer = cmd,
            currentFrameIndex = Time.renderedFrameCount
        };

        TextureHandle inputTexture = default;
        Material material = default;
        DrawObjectPass(renderGraphParams, inputTexture, material);

    }

    private void DrawObjectPass(RenderGraphParameters renderGraphParams, TextureHandle inputTexture, Material material)
    {
        using (m_RenderGraph.RecordAndExecute(renderGraphParams))
        {
            using (var builder = m_RenderGraph.AddRenderPass<DrawObjectPassData>("DrawObjectPass", out var passData))
            {
                passData.parameter = 2.5f;
                passData.material = material;
                passData.inputTexture = inputTexture;
                var outputTexture = m_RenderGraph.CreateTexture(new TextureDesc(Vector2.one, true, true)
                {
                    colorFormat = GraphicsFormat.R8G8B8A8_UNorm,
                    clearBuffer = true,
                    clearColor = Color.black,
                    name = "Output"
                });
                passData.outputTexture = builder.UseColorBuffer(outputTexture, 0);

                builder.SetRenderFunc((DrawObjectPassData passData, RenderGraphContext ctx) =>
                {
                    var materialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
                    materialPropertyBlock.SetTexture("_MainTexture", passData.inputTexture);
                    materialPropertyBlock.SetFloat("_FloatParam", passData.parameter);

                    CoreUtils.DrawFullScreen(ctx.cmd, passData.material, materialPropertyBlock);
                });

                return outputTexture;

            }
        }
    }
}

// public class LiteRenderGraphRecorder : RenderGraphRecorder
// {

// }