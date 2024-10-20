using UnityEngine;

namespace BlackAm
{
    public partial class SamplePositionTool
    {
        private static void Initialize_StreamSampling()
        {
        }
        
        public static void GetWideStreamSampling(ObjectDeployTimeline p_ObjectDeployTimeline, TransformTool.AffineCachePreset p_StartPivot, float p_Predelay, ObjectDeployTimelinePreset p_ObjectDeployTimelinePreset)
        {
            var perpendicularUV = p_StartPivot.Right;
            var count = p_ObjectDeployTimelinePreset.Count;
            var interval = Mathf.Max(p_ObjectDeployTimelinePreset.Interval, CustomMath.Epsilon);
            var scale = p_ObjectDeployTimelinePreset.Scale * p_StartPivot.ScaleFactor;

            p_ObjectDeployTimeline.AddTimelineAffine(p_Predelay, p_StartPivot);
            for (int i = 1; i < count; i++)
            {
                var tryTimeStamp = i * interval + p_Predelay;
                var offsetRightUV = i * scale * perpendicularUV;
                var leftAffine = p_StartPivot;
                leftAffine.AddWorldPositionOffset(-offsetRightUV);
                p_ObjectDeployTimeline.AddTimelineAffine(tryTimeStamp, leftAffine);
     
                var rightAffine = p_StartPivot;
                rightAffine.AddWorldPositionOffset(offsetRightUV);
                p_ObjectDeployTimeline.AddTimelineAffine(tryTimeStamp, rightAffine);
            }
        }
    }
}