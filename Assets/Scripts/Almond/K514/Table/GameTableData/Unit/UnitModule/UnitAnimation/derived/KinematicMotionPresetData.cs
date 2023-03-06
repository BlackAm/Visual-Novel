namespace k514
{
    public class KinematicPresetData : UnitAnimationPresetDataBase<AnimatorMotionPresetData, AnimatorMotionPresetData.AnimatableTableRecord>
    {
        public class AnimatableTableRecord : AnimatableTableRecordBase
        {
        }

        protected override string GetDefaultTableFileName()
        {
            return "KinematicMotionPresetDataTable";
        }

        public override MultiTableIndexer<int, UnitAnimationDataRoot.AnimatableType, IAnimatableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitAnimationDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 100000;
            EndIndex = 200000;
        }

        public override UnitAnimationDataRoot.AnimatableType GetThisLabelType()
        {
            return UnitAnimationDataRoot.AnimatableType.KinematicMotion;
        }
    }
}