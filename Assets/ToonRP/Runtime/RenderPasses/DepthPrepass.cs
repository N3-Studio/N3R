using ToonRP.FrameData;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder
    {
        private static readonly ProfilingSampler DepthPrepassProfilingSampler = new("Depth Prepass");
        private static readonly ShaderTagId DepthPrepassShaderTagId = new("DepthOnly");

        internal class DepthPrepassData
        {
            public RendererListHandle RendererList;
        }

        private static void AddDepthPrepass(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<CameraData>();
            var toonTextures = frameData.GetOrCreate<ToonTextures>();

            var depthDesc = new TextureDesc(cameraData.Camera.pixelWidth, cameraData.Camera.pixelHeight)
            {
                depthBufferBits = DepthBits.Depth32,
                name = "Depth Prepass",
                clearBuffer = true
            };
            
            toonTextures.DepthTexture = renderGraph.CreateTexture(depthDesc);

            using var builder = renderGraph.AddRasterRenderPass<DepthPrepassData>("Depth Prepass", out var passData,
                DepthPrepassProfilingSampler);
            
            builder.AllowPassCulling(false);
            
            builder.SetRenderAttachmentDepth(toonTextures.DepthTexture);
            
            var rendererListDesc = new RendererListDesc(DepthPrepassShaderTagId, cameraData.CullingResults, cameraData.Camera)
            {
                renderQueueRange = RenderQueueRange.opaque, 
                sortingCriteria = SortingCriteria.CommonOpaque
            };
            
            passData.RendererList = renderGraph.CreateRendererList(rendererListDesc);
            
            builder.UseRendererList(passData.RendererList);

            builder.SetRenderFunc((DepthPrepassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.RendererList);
            });
        }
    }
}