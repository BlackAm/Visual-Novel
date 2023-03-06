using System;
using UnityEngine;
using Random = UnityEngine.Random;
 
namespace k514
{
    public abstract partial class VolitionalActorBase : VolitionalBase
    {
        #region <Fields>

        public new IActorTableRecordBridge _RoleRecord { get; private set; }
        private VolitionalTool.ActorModuleProgressFlag _ActorModuleFlagMask;
        private ProgressTimer _ProgressTimer;
        private bool _IsValid;
        
        #endregion
        
        #region <Callbacks>
        
        public override IVolitional OnInitializeRole(UnitRoleDataRoot.UnitRoleType p_RoleType, Unit p_TargetUnit, IVolitionalTableRecordBridge p_RolePreset)
        {
            base.OnInitializeRole(p_RoleType, p_TargetUnit, p_RolePreset);
            
            _RoleRecord = (IActorTableRecordBridge) p_RolePreset;
            _ProgressTimer.Initialize(_RoleRecord.LifeSpan);
            return this;
        }
        
        protected override void OnModuleNotify()
        {
            ResetAct();

            if (_ActorModuleFlagMask.HasAnyFlagExceptNone(VolitionalTool.ActorModuleProgressFlag.AutoRunAct))
            {
                RunAct();
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
            if (_IsValid)
            {
                if (_ProgressTimer.IsOver())
                {
                    OnActorTimerOver();
                }
                else
                {
                    _ProgressTimer.Progress(UnitInteractManager.__TimeBlock_Interval);
                }
            }
        }

        protected abstract void OnActorRunAct();

        private void OnActorTimerOver()
        {
            ResetAct();

            switch (_RoleRecord.ActorTimeOverEventType)
            {
                case VolitionalTool.ActorTimeOverEventType.DeadEnd:
                    UnitInteractManager.GetInstance.ReserveDead(_MasterNode);
                    break;
                case VolitionalTool.ActorTimeOverEventType.ShowMustGoOn:
                    break;
                case VolitionalTool.ActorTimeOverEventType.ResetModule:
                    _MasterNode.SwitchRole();
                    break;
            }
        }

        #endregion

        #region <Methods>

        public void RunAct()
        {
            if (!_IsValid)
            {
                _IsValid = true;
                OnActorRunAct();
            }
        }

        private void ResetAct()
        {
            _ActorModuleFlagMask = _RoleRecord.ActorModuleProgressFlag;
            _ProgressTimer.Reset();
            _IsValid = false; 
        }

        public override string GetRoleName()
        {
            return string.Empty;
        }

        #endregion
    }
}