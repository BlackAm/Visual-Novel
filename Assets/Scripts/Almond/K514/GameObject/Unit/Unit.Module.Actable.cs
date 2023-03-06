using System.Collections.Generic;
using UI2020;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>

        /// <summary>
        /// 현재 선택된 해당 유닛의 액션 기술 모듈
        /// </summary>
        public IActable _ActableObject;

        /// <summary>
        /// Actable 모듈
        /// </summary>
        protected UnitModuleCluster<UnitActionDataRoot.ActableType, IActable> _ActableModule;

        #endregion
        
        #region <Callbacks>

        protected virtual void OnAwakeActable()
        {
            LoadActable(_PrefabExtraDataRecord.UnitActionRecordIdList);
        }

        protected virtual void OnPoolingActable()
        {
            _ActableObject = _ActableModule.SwitchModule();
            OnPoolingController();
        }

        private void OnRetrieveActable()
        {
            _ActableObject?.OnMasterNodeRetrieved();
        }
        
        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnActionTriggered(ControllerTool.ControlEventPreset p_Preset, bool p_RestrictActivateFlag)
        {
            var (validCommand, ActionType) = _ActableObject.GetCommandType(p_Preset.CommandType);
            if (validCommand)
            {
                return OnActionTriggered(ActionType, p_Preset, p_RestrictActivateFlag);
            }
            else
            {
                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_InvalidCommandType);
            }
        }
        
        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnActionTriggered(ActableTool.UnitActionType p_ActionType, ControllerTool.ControlEventPreset p_Preset, bool p_RestrictActivateFlag)
        {
            if (p_RestrictActivateFlag)
            {
                p_Preset.AddMessageFlag(ControllerTool.ControlMessageFlag.RestrictActivateSpell);
            }
            return _ActableObject.OnActionTriggered(p_ActionType, p_Preset);
        }
        
        /// <summary>
        /// 최초에 한번 상태가 스펠 액션 상태로 전이된 경우 호출되는 콜백
        /// </summary>
        public void OnUnitActionStarted(ControllerTool.CommandType p_CommandType)
        {
            _PhysicsObject.ClearVelocity();
            TryUpdateSqrDistanceTable_And_ReportPositionChangeDetected();
            
            AddState(UnitStateType.DRIVESKILL);

            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUnitActionStarted();
            }
        }
        
       /// <summary>
       /// 최초에 스펠 시전 상태로 전이된 위의 콜백 OnUnitActionStarted 이후 그리고
       /// 스펠 시전 중에 커맨드가 같은 커맨드 혹은 다른 커맨드로도 변경된 경우에 호출되는 콜백
       /// </summary>
        public void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
            _PhysicsObject.ClearVelocity();
            TryUpdateSqrDistanceTable_And_ReportPositionChangeDetected();
            
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUnitActionTransitioned(p_PrevCommandType, p_CurrentCommandType);
            }
        }
        
        public void OnUnitActionTerminated()
        {
            RemoveState(UnitStateType.DRIVESKILL);
            
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnUnitActionTerminated();
            }
        }

        /// <summary>
        /// 지정한 트리거 타입의 쿨다운이 생성된 경우 호출되는 콜백
        /// </summary>
        public void OnTriggerCoolDownStart(ControllerTool.CommandType p_CommandType)
        {
        }

        /// <summary>
        /// 지정한 트리거 타입의 쿨다운이 진행된 경우 호출되는 콜백
        /// </summary>
        public virtual void OnTriggerCoolDownRateChanged(ControllerTool.CommandType p_CommandType, float p_ProgressRate01) 
        {

        }

        /// <summary>
        /// 지정한 트리거 타입의 쿨다운이 종료된 경우 호출되는 콜백
        /// </summary>
        public void OnTriggerCoolDownOver(ControllerTool.CommandType p_CommandType)
        {
        }

        #endregion
        
        #region <Methods>

        protected void LoadActable(List<int> p_IndexList)
        {
            _ActableModule 
                = new UnitModuleCluster<UnitActionDataRoot.ActableType, IActable>(
                    this, UnitModuleDataTool.UnitModuleType.Action, p_IndexList);
            _ActableObject = (IActable) _ActableModule.CurrentSelectedModule;
        }

        protected void SwitchActable()
        {
            _ActableObject = _ActableModule.SwitchModule();
        }
        
        public void SwitchActable(UnitActionDataRoot.ActableType p_ModuleType)
        {
            _ActableObject = _ActableModule.SwitchModule(p_ModuleType);
#if !SERVER_DRIVE
            //MainGameUI.Instance.mainUI.InitActionImage(false);
#endif
        }
        
        protected void SwitchActable(int p_Index)
        {
            _ActableObject = _ActableModule.SwitchModule(p_Index);
        }
        
        public ControllerTool.CommandType FindCommandSkill(ActableTool.UnitActionCluster data)
        {
            return _ActableObject.FindCommandSkill(data);
        }
        
        public Dictionary<ControllerTool.CommandType, ActableTool.UnitActionSpellPreset> GetSkill()
        {
            return _ActableObject.GetSkill();
        }

        public void SaveSkill(bool sceneMove)
        {
            _ActableObject.SaveSkill(sceneMove);
        }

        private void DisposeActable()
        {
            if (_ActableModule != null)
            {
                _ActableModule.Dispose();
                _ActableModule = null;
            }

            _ActableObject = null;
        }

        #endregion
    }
}