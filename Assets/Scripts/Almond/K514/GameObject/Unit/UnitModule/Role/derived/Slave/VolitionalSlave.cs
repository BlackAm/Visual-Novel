using System;
using UnityEngine;
using Random = UnityEngine.Random;
 
namespace k514
{
    public class VolitionalSlave : VolitionalBase
    {
        #region <Fields>
 
        private new UnitRoleSlavePresetData.RoleTableRecord _RoleRecord;
        
        #endregion
         
        #region <Callbacks>
 
        public override IVolitional OnInitializeRole(UnitRoleDataRoot.UnitRoleType p_RoleType, Unit p_TargetUnit, IVolitionalTableRecordBridge p_RolePreset)
        {
            base.OnInitializeRole(p_RoleType, p_TargetUnit, p_RolePreset);
            _RoleRecord = (UnitRoleSlavePresetData.RoleTableRecord) p_RolePreset;
             
            return this;
        }
 
        public override string GetRoleName()
        {
            return $" 펫 {LanguageManager.GetInstanceUnSafe[_MasterNode._Default_UnitInfo.UnitNameId].content}";
        }
 
        protected override void OnModuleNotify()
        {
            _MasterNode.SetScale(_RoleRecord.UnitScale);
 
            switch (_RoleRecord.MasterType)
            {
                case UnitRoleSlavePresetData.MasterType.None:
                case UnitRoleSlavePresetData.MasterType.FirstEncounter:
                case UnitRoleSlavePresetData.MasterType.Player:
#if !SERVER_DRIVE
                    _MasterNode._MindObject.SetSlaveMasterUnit(PlayerManager.GetInstance.Player);
#endif
                    break;
            }
        }
 
        protected override void OnModuleSleep()
        {
        }
 
        public override void OnPreUpdate(float p_DeltaTime)
        {
        }
 
        public override void OnUpdate(float p_DeltaTime)
        {
        }
 
        public override void OnUpdate_TimeBlock()
        {
        }
 
        public override void OnStriked(Unit p_Trigger, HitResult p_HitResult)
        {
        }
 
        public override void OnHitted(Unit p_Trigger, HitResult p_HitResult)
        {
        }
 
        public override void OnUnitHitActionStarted()
        {
        }
 
        public override void OnUnitHitActionTerminated()
        {
        }
 
        public override void OnUnitActionStarted()
        {
        }
 
        public override void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
        }
 
        public override void OnUnitActionTerminated()
        {
        }
 
        public override void OnUnitDead(bool p_Instant)
        {
        }
 
        public override void OnJumpUp()
        {
        }
 
        public override void OnReachedGround(UnitStampPreset p_UnitStampPreset)
        {
        }

        public override void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset)
        {
        }

        public override void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
        }
 
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
        }
 
        #endregion
    }
}