using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder : IRenderGraphRecorder
    {
        public void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            AddDepthOnlyPass(renderGraph, frameData);
            AddSkyboxPass(renderGraph, frameData);
            AddGBufferPass(renderGraph, frameData);
            AddLightingPass(renderGraph, frameData);
            AddBlitPass(renderGraph, frameData);
        }
    }
}