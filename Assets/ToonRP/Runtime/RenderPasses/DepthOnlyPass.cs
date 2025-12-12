using ToonRP.FrameData;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder
    {
        private static readonly ProfilingSampler DepthOnlyPassProfilingSampler = new("Depth Only Pass");
        private static readonly ShaderTagId DepthOnlyPassShaderTagId = new("DepthOnly");

        internal class DepthOnlyPassData
        {
            public RendererListHandle RendererList;
        }

        private static void AddDepthOnlyPass(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<CameraData>();
            var resourceData = frameData.Get<ResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<DepthOnlyPassData>("Depth Only Pass", out var passData,
                DepthOnlyPassProfilingSampler);
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderAttachmentDepth(resourceData.DepthTexture);
            
            var rendererListDesc = new RendererListDesc(DepthOnlyPassShaderTagId, cameraData.CullingResults, cameraData.Camera)
            {
                renderQueueRange = RenderQueueRange.opaque, 
                sortingCriteria = SortingCriteria.CommonOpaque
            };
            
            passData.RendererList = renderGraph.CreateRendererList(rendererListDesc);
            
            builder.UseRendererList(passData.RendererList);

            builder.SetRenderFunc((DepthOnlyPassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.RendererList);
            });
        }
    }
}