using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace ToonRP.FrameData
{
    public class ToonTextures : ContextItem
    {
        public TextureHandle DepthTexture;
        
        public override void Reset()
        {
            DepthTexture = TextureHandle.nullHandle;
        }
    }
}