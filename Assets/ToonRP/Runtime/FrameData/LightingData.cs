using UnityEngine;
using UnityEngine.Rendering;

namespace ToonRP.FrameData
{
    public class LightingData : ContextItem
    {
        public Vector4 MainLightDirection;
        
        public Vector4 MainLightColor;

        public override void Reset()
        {
            MainLightDirection = Vector4.zero;
            MainLightColor = Vector4.zero;
        }
    }
}