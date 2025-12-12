using ToonRP.FrameData;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP
{
    public partial class ToonRenderGraphRecorder
    {
        private static readonly ProfilingSampler LightingPassProfilingSampler = new("Lighting Pass");
        private static readonly ShaderTagId LightingPassShaderTagId = new("Lighting");
        private static readonly Shader LightingShader = Shader.Find("ToonRP/Lit");
        private static readonly Material LightingMaterial = CoreUtils.CreateEngineMaterial(LightingShader);

        internal class LightingPassData
        {
            public TextureHandle GBuffer0;
            public TextureHandle GBuffer1;
            public TextureHandle GBuffer2;
            public TextureHandle DepthTexture;
    
            public TextureHandle CameraColor;

            public Vector4 MainLightPosition;
            public Vector4 MainLightColor;
        }

        private static void AddLightingPass(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<ResourceData>();
            var lightingData = frameData.Get<LightingData>();

            using var builder = renderGraph.AddRasterRenderPass<LightingPassData>("Lighting Pass", out var passData,
                LightingPassProfilingSampler);
            
            builder.AllowPassCulling(false);
            
            passData.GBuffer0 = resourceData.GBuffer[0];
            passData.GBuffer1 = resourceData.GBuffer[1];
            passData.GBuffer2 = resourceData.GBuffer[2];
            passData.DepthTexture = resourceData.DepthTexture;

            passData.MainLightPosition = lightingData.MainLightDirection;
            passData.MainLightColor = lightingData.MainLightColor;

            builder.UseTexture(passData.GBuffer0);
            builder.UseTexture(passData.GBuffer1);
            builder.UseTexture(passData.GBuffer2);
            builder.UseTexture(passData.DepthTexture);
            
            passData.CameraColor = resourceData.ColorTexture;
            builder.SetRenderAttachment(passData.CameraColor, 0);
            
            builder.AllowGlobalStateModification(true);
            
            builder.SetRenderFunc((LightingPassData data, RasterGraphContext context) =>
            {
                context.cmd.SetGlobalTexture(Shader.PropertyToID("_GBuffer0"), data.GBuffer0);
                context.cmd.SetGlobalTexture(Shader.PropertyToID("_GBuffer1"), data.GBuffer1);
                context.cmd.SetGlobalTexture(Shader.PropertyToID("_GBuffer2"), data.GBuffer2);
                context.cmd.SetGlobalTexture(Shader.PropertyToID("_CameraDepthTexture"), data.DepthTexture);
                
                context.cmd.SetGlobalVector(Shader.PropertyToID("_MainLightPosition"), data.MainLightPosition);
                context.cmd.SetGlobalVector(Shader.PropertyToID("_MainLightColor"), data.MainLightColor);
                
                if (LightingMaterial)
                {
                    context.cmd.DrawProcedural(Matrix4x4.identity, LightingMaterial, 2, MeshTopology.Triangles, 3, 1);
                }
            });
        }
    }
}