using UnityEngine;

namespace k514
{
    public partial class ThinkableBase
    {
        #region <Methods>
        
        /*public abstract bool AttackTo(Unit p_TargetUnit, bool p_ForceAttack, ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType);
        public abstract bool ActSpell(Unit p_TargetUnit, ControllerTool.ControlEventPreset p_Preset, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType);
        public abstract void ReserveCommand(ThinkableTool.AIReserveHandleType p_Type);
        public abstract bool ReserveCommand(ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType, bool p_ReserveRestrictFlag);*/
        public abstract bool MoveTo(Vector3 p_TargetPosition, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset);

        public void StopMove(ActableTool.IdleState p_IdleType)
        {
            if (!_MasterNode.HasState_Or(Unit.UnitStateType.UnitAIStoppableFilterMask))
            {
                _MasterNode._PhysicsObject.ClearPhysicsAutonomyMove();
                _MasterNode._ActableObject.TurnIdleState(p_IdleType, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
            }
        }

        public virtual void Idle(ActableTool.IdleState p_IdleType)
        {
            StopMove(p_IdleType);
        }

        public abstract bool ReturnPosition(bool p_ForceMove, bool p_SwitchInstance);

        /// <summary>
        /// 유닛 액션 모듈이 예약된 다음 스킬을 발동하려고 할 때, 발동 가능 여부를 선정하는 기준 중에 하나가 되는 논리 메서드
        /// </summary>
        // public abstract bool CheckEnterableNextAction();

        #endregion
    }
}