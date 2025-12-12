using ToonRP.FrameData;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder
    {
        private static readonly ProfilingSampler GBufferPassProfilingSampler = new("G-Buffer Pass");
        private static readonly ShaderTagId GBufferPassShaderTagId = new("GBuffer");

        internal class GBufferPassData
        {
            public RendererListHandle RendererList;
        }

        private static void AddGBufferPass(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<CameraData>();
            var resourceData = frameData.Get<ResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<GBufferPassData>("G-Buffer Pass", out var passData,
                GBufferPassProfilingSampler);
            
            builder.AllowPassCulling(false);
            
            for (var i = 0; i < resourceData.GBuffer.Length; i++)
            {
                builder.SetRenderAttachment(resourceData.GBuffer[i], i);
            }
            
            // builder.SetRenderAttachmentDepth(resourceData.DepthTexture);
            
            var rendererListDesc = new RendererListDesc(GBufferPassShaderTagId, cameraData.CullingResults, cameraData.Camera)
            {
                renderQueueRange = RenderQueueRange.opaque, 
                sortingCriteria = SortingCriteria.CommonOpaque
            };
            
            passData.RendererList = renderGraph.CreateRendererList(rendererListDesc);
            
            builder.UseRendererList(passData.RendererList);
            
            builder.SetRenderFunc((GBufferPassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.RendererList);
            });
        }
    }
}