using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP.FrameData
{
    public class ResourceData : ContextItem
    {
        public TextureHandle ColorTexture;
        
        public TextureHandle DepthTexture;

        public TextureHandle BackBufferColor;

        public TextureHandle[] GBuffer = new TextureHandle[4];
        
        public override void Reset()
        {
            ColorTexture = TextureHandle.nullHandle;
            DepthTexture = TextureHandle.nullHandle;
            BackBufferColor = TextureHandle.nullHandle;
            
            for (var i = 0; i < GBuffer.Length; i++)
            {
                GBuffer[i] = TextureHandle.nullHandle;
            }
        }
    }
}