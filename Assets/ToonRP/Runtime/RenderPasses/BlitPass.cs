using ToonRP.FrameData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder
    {
        private static readonly ProfilingSampler BlitPassProfilingSampler = new("Blit Pass");

        private static readonly Shader BlitShader = Shader.Find("ToonRP/Blit");
        private static readonly Material BlitMaterial = CoreUtils.CreateEngineMaterial(BlitShader);

        internal class BlitPassData
        {
            public TextureHandle ColorTextureHandle;
        }

        private static void AddBlitPass(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<ResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<BlitPassData>("Blit Pass", out var passData,
                BlitPassProfilingSampler);
    
            builder.SetRenderAttachment(resourceData.BackBufferColor, 0);

            passData.ColorTextureHandle = resourceData.ColorTexture;
            
            builder.UseTexture(passData.ColorTextureHandle);
            
            builder.AllowGlobalStateModification(true);

            builder.SetRenderFunc((BlitPassData data, RasterGraphContext context) =>
            {
                context.cmd.SetGlobalTexture(Shader.PropertyToID("_BlitTexture"), data.ColorTextureHandle);
                var scaleBias = new Vector4(1, 1, 0, 0);

                var needFlip = SystemInfo.graphicsUVStartsAtTop;
    
                if (needFlip)
                {
                    scaleBias = new Vector4(1, -1, 0, 1);
                }

                context.cmd.SetGlobalVector(Shader.PropertyToID("_BlitScaleBias"), scaleBias);
                context.cmd.DrawProcedural(Matrix4x4.identity, BlitMaterial, 0, MeshTopology.Triangles, 3, 1);
            });
        }
    }
}