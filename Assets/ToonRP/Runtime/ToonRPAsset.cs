using UnityEngine;
using UnityEngine.Rendering;

namespace ToonRP
{
    [CreateAssetMenu(menuName = "Rendering/ToonRP/Toon Render Pipeline Asset")]
    public class ToonRPAsset : RenderPipelineAsset<ToonRenderPipeline>
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new ToonRenderPipeline();
        }
    }
}
