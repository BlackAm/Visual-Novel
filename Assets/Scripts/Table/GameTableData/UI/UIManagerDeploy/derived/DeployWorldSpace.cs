#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public class DeployWorldSpace : UIManagerDeployTable<DeployWorldSpace, DeployWorldSpace.SFX_TableRecord>
    {
        public class SFX_TableRecord : TableRecord
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "DeployWorldSpace";
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100;
            EndIndex = 150;
        }

        public override RenderMode GetThisLabelType()
        {
            return RenderMode.WorldSpace;
        }
    }
}
#endif