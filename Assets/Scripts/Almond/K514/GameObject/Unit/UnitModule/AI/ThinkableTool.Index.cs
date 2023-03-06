using System.Collections.Generic;

namespace k514
{
    public interface IThinkableTableBridge : ITableBase
    {
    }    
    
    public interface IThinkableTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
        int Priority { get; }
        ThinkableTool.AIUnitFindType DefaultUnitFindType { get; }
    }
    
    public interface IThinkableAutonomyTableBridge : IThinkableTableBridge
    {
    }    
    
    public interface IThinkableAutonomyTableRecordBridge : IThinkableTableRecordBridge
    {
        ThinkableTool.AIExtraFlag AIExtraFlag { get; }
        Dictionary<ThinkableTool.AIState, ThinkableTool.AIStatePreset> StatePresetRecord{ get; }
        ThinkableTool.AITracePivotSelectType AITracePivotSelectType { get; }
        ThinkableTool.AITracePivotSelectType AITracePivotSelectTypeWhenTargetMoving { get; }
        float WanderingRadius { get; }
        int WanderingIntervalDsec { get; }
        float Carefulness{ get; }
    }
    
    public interface IThinkablePassivityTableBridge : IThinkableTableBridge
    {
    }    
    
    public interface IThinkablePassivityTableRecordBridge : IThinkableTableRecordBridge
    {
        float SearchDistance { get; }
        bool FindTargetWhenActSpellFlag { get; }
    }
    
    public class UnitAIDataRoot : UnitModuleDataRootBase<UnitAIDataRoot, UnitAIDataRoot.UnitMindType, IThinkableTableBridge, IThinkableTableRecordBridge>
    {
        #region <Enums>

        public enum UnitMindType
        {
            None,
            Passivity_Base,
            Autonomy_MeleeSimple,
            Autonomy_Coward
        }

        #endregion

        #region <Methods>

        protected override UnitModuleDataTool.UnitModuleType GetUnitModuleType()
        {
            return UnitModuleDataTool.UnitModuleType.AI;
        }

        public override (UnitMindType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case UnitMindType.Passivity_Base :
                        return (labelType, new PassivityAIBase().OnInitializeAI(labelType, p_Target, record));
                    case UnitMindType.Autonomy_MeleeSimple :
                        return (labelType, new AutonomyAI_Melee().OnInitializeAI(labelType, p_Target, record));
                    case UnitMindType.Autonomy_Coward :
                        return (labelType, new AutonomyAI_Coward().OnInitializeAI(labelType, p_Target, record));
                }
            }

            return default;
        }

        #endregion
    }
}