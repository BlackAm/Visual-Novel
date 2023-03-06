using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public interface IActableTableBridge : ITableBase
    {
    }    
    
    public interface IActableTableRecordBridge : ITableBaseRecord
    {
        int KEY { get; }
        Dictionary<ControllerTool.CommandType, ActableTool.UnitActionCluster> UnitActionRecords { get; }
        ControllerTool.CommandType DefaultCommand { get; }
    }

    public class UnitActionDataRoot : UnitModuleDataRootBase<UnitActionDataRoot, UnitActionDataRoot.ActableType, IActableTableBridge, IActableTableRecordBridge>
    {
        #region <Enums>

        public enum ActableType
        {
            None,

            PassivePhaseTransition, 
            AutonomyPhaseTransition,
            UnWeaponPhaseTransition,
        }

        #endregion

        #region <Methods>

        protected override UnitModuleDataTool.UnitModuleType GetUnitModuleType()
        {
            return UnitModuleDataTool.UnitModuleType.Action;
        }

        public override (ActableType, IIncarnateUnit) SpawnModule(Unit p_Target, int p_TableIndex)
        {
            var (valid, labelType) = GetLabelType(p_TableIndex);
            if (valid)
            {
                var record = this[p_TableIndex];
                switch (labelType)
                {
                    case ActableType.PassivePhaseTransition:
                        return (labelType, new PassivePhaseTransitionActable().OnInitializeActable(labelType, p_Target, record));
                    case ActableType.AutonomyPhaseTransition:
                        return (labelType, new AutonomyPhaseTransitionActable().OnInitializeActable(labelType, p_Target, record));
                    case ActableType.UnWeaponPhaseTransition:
                        return (labelType, new UnWeaponPhaseTransitionActable().OnInitializeActable(labelType, p_Target, record));
                }
            }

            return default;
        }

        #endregion
    }
}