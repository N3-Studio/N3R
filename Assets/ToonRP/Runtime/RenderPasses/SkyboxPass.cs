using ToonRP.FrameData;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder
    {
        private static readonly ProfilingSampler SkyboxPassProfilingSampler = new("Skybox Pass");

        internal class SkyboxPassData
        {
            public RendererListHandle SkyboxRenderListHandle;
        }

        private static void AddSkyboxPass(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<CameraData>();
            var resourceData = frameData.Get<ResourceData>();
            
            using var builder = renderGraph.AddRasterRenderPass<SkyboxPassData>("Skybox", out var passData, 
                SkyboxPassProfilingSampler);

            passData.SkyboxRenderListHandle = renderGraph.CreateSkyboxRendererList(cameraData.Camera);
            builder.UseRendererList(passData.SkyboxRenderListHandle);

            builder.SetRenderAttachment(resourceData.ColorTexture, 0);

            builder.AllowPassCulling(false);
            
            builder.SetRenderFunc((SkyboxPassData data, RasterGraphContext context) =>
            {
                context.cmd.DrawRendererList(data.SkyboxRenderListHandle);
            });
        }
    }
}