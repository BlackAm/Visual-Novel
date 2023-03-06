#if !SERVER_DRIVE
using UnityEngine;

namespace k514
{
    public class DeployScreenSpaceOverlay : UIManagerDeployTable<DeployScreenSpaceOverlay, DeployScreenSpaceOverlay.SFX_TableRecord>
    {
        public class SFX_TableRecord : TableRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DeployScreenSpaceOverlay";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 50;
            EndIndex = 100;
        }

        public override RenderMode GetThisLabelType()
        {
            return RenderMode.ScreenSpaceOverlay;
        }
    }
}
#endif