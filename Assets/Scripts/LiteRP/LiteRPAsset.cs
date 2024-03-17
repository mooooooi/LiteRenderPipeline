using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/LiteRPAsset")]
public class LiteRPAsset : RenderPipelineAsset<LiteRP>
{
    protected override RenderPipeline CreatePipeline()
    {
        return new LiteRP(this);
    }
}
