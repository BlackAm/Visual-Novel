using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class Unit : IPriority
    {
        #region <Fields>

        /// <summary>
        /// 현재 선택된 해당 유닛의 사고 기술 모듈
        /// </summary>
        public IThinckable _MindObject;
        
        /// <summary>
        /// Thinkable 모듈
        /// </summary>
        private UnitModuleCluster<UnitAIDataRoot.UnitMindType, IThinckable> _MindModule;
        
        /// <summary>
        /// 유닛 우선도, 높을수록 우선도가 크다.
        /// </summary>
        public int Priority { get; private set; }

        public bool IsQuestTargetUnit;

        #endregion

        #region <Callbacks>

        private void OnAwakeAI()
        {
            _MindModule 
                = new UnitModuleCluster<UnitAIDataRoot.UnitMindType, IThinckable>(
                    this, UnitModuleDataTool.UnitModuleType.AI, _PrefabExtraDataRecord.MindPresetIdList);
            _MindObject = (IThinckable) _MindModule.CurrentSelectedModule;
          
            OnUpdatePriority();
        }

        private void OnPoolingAI()
        {
            _MindObject = _MindModule.SwitchModule();

            IsQuestTargetUnit = false;
        }

        private void OnRetrieveAI()
        {
            _MindObject?.OnMasterNodeRetrieved();
        }

        private void OnUpdatePriority()
        {
            if (IsPlayer)
            {
                Priority = UnitTool.__PLAYER_PRIORITY;
            }
            else
            {
                Priority = _MindObject?._MindRecord.Priority??default;
            }
        }

        /// <summary>
        /// 길찾기 에이전트(NavMeshAgent)가 이동할 목적지를 선정한 경우 호출되는 콜백
        /// </summary>
        public void OnSelectDestination()
        {
        }
        
        /// <summary>
        /// 어떤 방식이든 길찾기 에이전트(NavMeshAgent)가 목적지를 잃은 경우 호출되는 콜백
        /// </summary>
        public void OnReachedDestination()
        {
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerReachedDestination();
            }
#endif
        }

        #endregion

        #region <Methods>

        public void SwitchPersona()
        {
            _MindObject = _MindModule.SwitchModule();
        }
        
        public void SwitchPersona(UnitAIDataRoot.UnitMindType p_ModuleType)
        {
            _MindObject = _MindModule.SwitchModule(p_ModuleType);
        }
        
        public void SwitchPersona(int p_Index)
        {
            _MindObject = _MindModule.SwitchModule(p_Index);
        }

        /// <summary>
        /// 사고 모듈 파기메서드
        /// </summary>
        private void DisposePersona()
        {
            if (_MindModule != null)
            {
                _MindModule.Dispose();
                _MindModule = null;
            }

            _MindObject = null;
        }

        public bool FindEnemyFromPivot(float p_Distance, UnitStateType p_StateFlag)
        {
            return _MindObject.FindEnemyFromPivot(p_Distance, p_StateFlag);
        }
        
        public bool FindEnemyFromPivot(Vector3 p_Position, float p_Distance, UnitStateType p_StateFlag)
        {
            return _MindObject.FindEnemyFromPivot(p_Position, p_Distance, p_StateFlag);
        }
        
        public bool FindEnemyWithParams(FilterParams p_FilterParams)
        {
            return _MindObject.FindEnemyWithParams(p_FilterParams);
        }
        
        /*public (bool, Unit) FindEnemy(ThinkableTool.AIUnitFindType p_FindType, UnitStateType p_StateFlag, bool p_UseCurrentFocusFirst)
        {
            return FindEnemy(_MindObject.GetSearchDistance(), p_FindType, p_StateFlag, p_UseCurrentFocusFirst);
        }
        
        public (bool, Unit) FindEnemy(ThinkableTool.AIState p_Type, ThinkableTool.AIUnitFindType p_FindType, UnitStateType p_StateFlag, bool p_UseCurrentFocusFirst)
        {
            return FindEnemy(_MindObject.GetAIPreset(p_Type).Range, p_FindType, p_StateFlag, p_UseCurrentFocusFirst);
        }
        
        public (bool, Unit) FindEnemy(float p_Distance, ThinkableTool.AIUnitFindType p_FindType, UnitStateType p_StateFlag, bool p_UseCurrentFocusFirst)
        {
            return p_UseCurrentFocusFirst && FocusNode
                ? (true, FocusNode)
                : _MindObject.FindEnemy(p_Distance, p_FindType, p_StateFlag);
        }

        public (bool, Unit) FindEnemy(Vector3 p_Pivot, float p_Distance, ThinkableTool.AIUnitFindType p_FindType, UnitStateType p_StateFlag, bool p_UseCurrentFocusFirst)
        {
            return p_UseCurrentFocusFirst && FocusNode
                ? (true, FocusNode)
                : _MindObject.FindEnemy(p_Pivot, p_Distance, p_FindType, p_StateFlag);
        }

        public (bool, Unit) FindEnemyList(List<int> p_QuestMonsterKey, UnitStateType p_StateFlag, float p_Distance)
        {
            return UnitInteractManager.GetInstance.GetEnemyUnit(this, p_QuestMonsterKey, p_StateFlag, p_Distance, true);
        }*/

        public ThinkableTool.AIState GetCurrentAIState()
        {
            return _MindObject.GetCurrentAIState();
        }

        /*public bool OrderAIAttackTo(Unit p_TargetUnit, bool p_ForceAttack, ControllerTool.CommandType p_CommandType, ThinkableTool.AIReserveCommand p_CommandReserveType = ThinkableTool.AIReserveCommand.SpellEntry_Instant_OnceFlag)
        {
            return _MindObject.AttackTo(p_TargetUnit, p_ForceAttack, p_CommandType, p_CommandReserveType, ThinkableTool.AIReserveHandleType.TurnToNone);
        }

        public async UniTask<bool> OrderAIActSpell(Unit p_TargetUnit, ControllerTool.ControlEventPreset p_Preset, ThinkableTool.AIReserveCommand p_CommandReserveType = ThinkableTool.AIReserveCommand.SpellEntry_Instant_OnceFlag)
        {
            _AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.Punch, AnimatorParamStorage.MotionTransitionType.Restrict);
            await UniTask.NextFrame();
            return _MindObject.ActSpell(p_TargetUnit, p_Preset, p_CommandReserveType, ThinkableTool.AIReserveHandleType.TurnToNone);
        }
        
        public bool OrderAIReserveCommand(ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_CommandReserveType = ThinkableTool.AIReserveCommand.SpellEntry_Instant_OnceFlag, bool p_ReserveRestrict = false)
        {
            return _MindObject.ReserveCommand(p_ReserveCommand, p_CommandReserveType, ThinkableTool.AIReserveHandleType.TurnToNone, p_ReserveRestrict);
        }
        
        public bool OrderAIReserveCommand(ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_CommandReserveType = ThinkableTool.AIReserveCommand.SpellEntry_Instant_OnceFlag, ThinkableTool.AIReserveHandleType p_AIReserveFailHandleType = ThinkableTool.AIReserveHandleType.TurnToNone, bool p_ReserveRestrict = false)
        {
            return _MindObject.ReserveCommand(p_ReserveCommand, p_CommandReserveType, p_AIReserveFailHandleType, p_ReserveRestrict);
        }*/
        
        public bool OrderAIMoveTo(Vector3 p_TargetPosition, bool p_ForceMove)
        {
            return _MindObject.MoveTo(p_TargetPosition, p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(this));
        }
        
        public bool OrderAIMoveTo(Transform p_TargetTransform, bool p_ForceMove)
        {
            return _MindObject.MoveTo(p_TargetTransform.position, p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(this));
        }
        
        public bool OrderAIMoveTo(Unit p_TargetUnit, bool p_ForceMove)
        {
            return _MindObject.MoveTo(p_TargetUnit._Transform.position, p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset.GetStoppingRange(this));
        }
        
        public bool OrderAIMoveTo(Vector3 p_TargetPosition, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            return _MindObject.MoveTo(p_TargetPosition, p_ForceMove, p_AutonomyPathStoppingDistancePreset);
        }
        
        public bool OrderAIMoveTo(Transform p_TargetTransform, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            return _MindObject.MoveTo(p_TargetTransform.position, p_ForceMove, p_AutonomyPathStoppingDistancePreset);
        }
        
        public bool OrderAIMoveTo(Unit p_TargetUnit, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset)
        {
            return _MindObject.MoveTo(p_TargetUnit._Transform.position, p_ForceMove, p_AutonomyPathStoppingDistancePreset);
        }
        
        public void OrderAIIdle(ActableTool.IdleState p_IdleType)
        {
            _MindObject.Idle(p_IdleType);
        }

        public bool IsAIThroughPath()
        {
            return _PhysicsObject.IsAutonomyMoving();
        }

        #endregion
    }
}