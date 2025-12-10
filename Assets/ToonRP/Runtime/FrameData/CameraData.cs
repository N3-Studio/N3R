using UnityEngine;
using UnityEngine.Rendering;

namespace ToonRP.FrameData
{
    public class CameraData : ContextItem
    {
        public Camera Camera;
        public CullingResults CullingResults;
        
        public override void Reset()
        {
            Camera = null;
            CullingResults = default;
        }
    }
}