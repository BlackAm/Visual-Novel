#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    public class DeployScreenSpaceCamera : UIManagerDeployTable<DeployScreenSpaceCamera, DeployScreenSpaceCamera.SFX_TableRecord>
    {
        public class SFX_TableRecord : TableRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DeployScreenSpaceCamera";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 50;
        }

        public override RenderMode GetThisLabelType()
        {
            return RenderMode.ScreenSpaceCamera;
        }
    }
}
#endif