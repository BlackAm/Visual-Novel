using System;
using UnityEngine;
using Random = UnityEngine.Random;
 
namespace k514
{
    public class VolitionalChamp : VolitionalBase
    {
        #region <Consts>
 
        private const int PreFixCount = 4;
        private const int NoneAttrPreFixStart = 140010;
        private const int FireAttrPreFixStart = 140020;
        private const int WaterAttrPreFixStart = 140030;
        private const int LightAttrPreFixStart = 140040;
        private const int DarknessAttrPreFixStart = 140050;
         
        #endregion
         
        #region <Fields>
 
        private new UnitRoleChampionPresetData.RoleTableRecord _RoleRecord;
        private string PrefixName;
    
        #endregion
         
        #region <Callbacks>
 
        public override IVolitional OnInitializeRole(UnitRoleDataRoot.UnitRoleType p_RoleType, Unit p_TargetUnit, IVolitionalTableRecordBridge p_RolePreset)
        {
            base.OnInitializeRole(p_RoleType, p_TargetUnit, p_RolePreset);
            _RoleRecord = (UnitRoleChampionPresetData.RoleTableRecord) p_RolePreset;
             
            return this;
        }
 
        public override string GetRoleName()
        {
            return $" {PrefixName} {LanguageManager.GetInstanceUnSafe[_MasterNode._Default_UnitInfo.UnitNameId].content}";
        }
 
        protected override void OnModuleNotify()
        {
            _MasterNode.SetScale(_RoleRecord.UnitScale);
             
            var preFixIndex = 0;
 
            PrefixName = LanguageManager.GetInstanceUnSafe[preFixIndex].content;

#if !SERVER_DRIVE
            var vfxIndex = _RoleRecord.AuraVfxSpawnIndex;
            // _MasterNode.AttachVfx(UnitTool.UnitAttachedVfxType.ChampionAura, vfxIndex, Vector3.zero);
#endif
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