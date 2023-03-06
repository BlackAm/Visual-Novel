namespace k514
{
    public class AnimatorMotionPresetData : UnitAnimationPresetDataBase<AnimatorMotionPresetData, AnimatorMotionPresetData.AnimatableTableRecord>
    {
        public class AnimatableTableRecord : AnimatableTableRecordBase
        {
            public int AnimatorIndex { get; private set; }
        }

        protected override string GetDefaultTableFileName()
        {
            return "AnimatorMotionPresetDataTable";
        }

        public override MultiTableIndexer<int, UnitAnimationDataRoot.AnimatableType, IAnimatableTableRecordBridge> GetMultiGameIndex()
        {
            return UnitAnimationDataRoot.GetInstanceUnSafe.GameDataTableCluster;
        }

        public override void InitIntervalIndex()
        {
            StartIndex = 0;
            EndIndex = 100000;
        }

        public override UnitAnimationDataRoot.AnimatableType GetThisLabelType()
        {
            return UnitAnimationDataRoot.AnimatableType.AnimatorMotion;
        }
    }
}