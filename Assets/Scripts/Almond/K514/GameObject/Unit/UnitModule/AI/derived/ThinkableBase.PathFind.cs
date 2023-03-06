using UnityEngine;

namespace k514
{
    public partial class ThinkableBase
    {
        #region <Fields>

        /// <summary>
        /// 특정 유닛을 추적하는 상태에서, 추적 경로를 다양하게 하기 위한 오프셋
        /// </summary>
        protected Vector3 _TraceOffsetVector;

        /// <summary>
        /// 특정 유닛을 추적하는데 사용하는 정지 거리 프리셋
        /// </summary>
        protected PhysicsTool.AutonomyPathStoppingDistancePreset _StoppingDistancePreset;

        #endregion

        #region <Callbacks>

        public virtual PhysicsTool.UpdateAutonomyPhysicsResult OnUpdateAutonomyPhysics(float p_RemainingDistance, float p_StoppingDistance)
        {
            return p_RemainingDistance < p_StoppingDistance ? PhysicsTool.UpdateAutonomyPhysicsResult.CheckNextMoveDestination : PhysicsTool.UpdateAutonomyPhysicsResult.ProgressNavMeshControl;
        }

        public virtual void OnAutonomyPhysicsPathOver()
        {
            Idle(_MasterNode._ActableObject._CurrentIdleState);
        }

        public void OnAutonomyPhysicsPendingDelay()
        {
            // 대기 상태에서 대기 상태로 재진입을 피하기 위해 모션 강제 전이 플래그는 사용하지 않는다.
            // _MasterNode._ActableObject.TurnIdleState(HasEnemy() ? ActableTool.IdleState.Combat : ActableTool.IdleState.Relax, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
        }

        public void OnAutonomyPhysicsPendingOver(bool p_MoveCompleted)
        {
            if (!p_MoveCompleted)
            {
                // 착지모션등을 캔슬하기 위해 모션 강제 전이 플래그를 사용한다.
                // _MasterNode._ActableObject.TurnMoveState(_MasterNode.HasAnyAuthority(UnitTool.UnitAuthorityFlag.EveryPlayer) || HasEnemy() ? ActableTool.WalkState.Run : ActableTool.WalkState.Walk, AnimatorParamStorage.MotionTransitionType.Restrict);
            }
        }

        public void OnAutonomyPhysicsPendingDeadline()
        {
            OnBreakAutonomyPath();
        }

        public void OnAutonomyPhysicsStuck()
        {
            OnBreakAutonomyPath();
        }

        protected virtual void OnAutonomyPathFindFailed()
        {
            Idle(ActableTool.IdleState.Relax);
        }

        protected virtual void OnBreakAutonomyPath()
        {
            Idle(ActableTool.IdleState.Relax);
        }

        #endregion

        #region <Methods>


        
        #endregion
    }
}