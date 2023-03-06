namespace k514
{
    public interface IAnimatableTableBridge : ITableBase
    {
    }    
    
    public interface IAnimatableTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
    }

    public class UnitAnimationDataRoot : UnitModuleDataRootBase<UnitAnimationDataRoot, UnitAnimationDataRoot.AnimatableType, IAnimatableTableBridge, IAnimatableTableRecordBridge>
    {
        #region <Enums>

        public enum AnimatableType
        {
            AnimatorMotion,
            KinematicMotion,
        }

        #endregion
        
        #region <Methods>

        protected override UnitModuleDataTool.UnitModuleType GetUnitModuleType()
        {
            return UnitModuleDataTool.UnitModuleType.Animation;
        }

        public override (AnimatableType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case AnimatableType.AnimatorMotion:
                        return (labelType, new ControlAnimator().OnInitializeAnimation(labelType, p_Target, record));
                    case AnimatableType.KinematicMotion:
                        return (labelType, new ControlAffine().OnInitializeAnimation(labelType, p_Target, record));
                }
            }

            return default;
        }
        
        #endregion
    }
}