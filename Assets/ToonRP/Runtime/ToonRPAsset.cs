using UnityEngine;
using UnityEngine.Rendering;

namespace ToonRP
{
    [CreateAssetMenu(menuName = "Rendering/ToonRP/Toon Render Pipeline Asset")]
    public class ToonRPAsset : RenderPipelineAsset<ToonRenderPipeline>
    {
        [Header("General")]
        public string pipelineName = "ToonRenderPipeline";
        
        [Header("Optimization")]
        public bool useSrpBatcher = true;
        
        protected override RenderPipeline CreatePipeline()
        {
            return new ToonRenderPipeline(this);
        }
    }
}
