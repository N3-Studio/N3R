using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder : IRenderGraphRecorder
    {
        public void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            AddSkyboxPass(renderGraph, frameData);
            AddDepthOnlyPass(renderGraph, frameData);
            AddGBufferPass(renderGraph, frameData);
            AddBlitPass(renderGraph, frameData);
        }
    }
}