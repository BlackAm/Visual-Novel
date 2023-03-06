using UnityEngine;

namespace k514
{
    public class VolitionalActor_OneShot : VolitionalActorBase
    {
        #region <Fields>

        public new OneShotActorData.ActorTableRecord _RoleRecord { get; private set; }

        #endregion
        
        #region <Callbacks>
        
        public override IVolitional OnInitializeRole(UnitRoleDataRoot.UnitRoleType p_RoleType, Unit p_TargetUnit, IVolitionalTableRecordBridge p_RolePreset)
        {
            base.OnInitializeRole(p_RoleType, p_TargetUnit, p_RolePreset);
            
            _RoleRecord = (OneShotActorData.ActorTableRecord) p_RolePreset;
     
            return this;
        }
        
        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();
            
            Debug.Log("OneShotOperate On");
        }
        
        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
            
            Debug.Log("OneShotOperate Off");
        }
        
        protected override void OnActorRunAct()
        {
        }
        
        #endregion

        #region <Methods>

        #endregion
    }
}