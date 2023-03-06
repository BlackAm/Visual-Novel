using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <Fields>

        /// <summary>
        /// 현재 유닛 이동 모드
        /// </summary>
        public ActableTool.WalkState _CurrentWalkState { get; protected set; }
        
        /// <summary>
        /// 현재 유닛 대기 모드
        /// </summary>
        public ActableTool.IdleState _CurrentIdleState { get; protected set; }

        /// <summary>
        /// 유니티 애니메이션은 스크립트와 별도의 루프에서 모션 클립의 타임 스탬프 콜백을 통해 통신하게 되는데
        /// 해당 콜백은 로직으로 제어할 수 없기 때문에, 플래그를 이용하여 특정한 타이밍의 콜백만 허용하기 위해
        /// 여러 플래그를 정의해야하고 그러한 플래그를 아래의 마스크로 관리하는중
        /// </summary>
        private UnitActionTool.MotionRestrictFlag _MotionRestrictFlagMask;
        
        #endregion

        #region <Callbacks>
        
        private void OnMoveStateChanged(AnimatorParamStorage.MotionTransitionType p_MotionTransitionType)
        {
            switch (_CurrentWalkState)
            {
                case ActableTool.WalkState.Hold:
                    OnIdleStateChanged(p_MotionTransitionType);
                    break;
                case ActableTool.WalkState.Walk:
                    _MasterNode._AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.MoveWalk, p_MotionTransitionType);
                    break;
                case ActableTool.WalkState.Run:
                    _MasterNode._AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.MoveRun, p_MotionTransitionType);
                    break;
            }
        }
        
        private void OnIdleStateChanged(AnimatorParamStorage.MotionTransitionType p_MotionTransitionType)
        {
            switch (_CurrentIdleState)
            {
                case ActableTool.IdleState.Relax:
                    _MasterNode._AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.RelaxIdle, p_MotionTransitionType);
                    break;
                case ActableTool.IdleState.Combat:
                    _MasterNode._AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.CombatIdle, p_MotionTransitionType);
                    break;
                case ActableTool.IdleState.Groggy:
                    _MasterNode._AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.GroggyIdle, p_MotionTransitionType);
                    break;
            }
        }

        #endregion
        
        #region <Methods>
        
        public void TurnMoveState(ActableTool.WalkState p_Type, AnimatorParamStorage.MotionTransitionType p_MotionTransitionType)
        {
            _CurrentWalkState = p_Type;
            OnMoveStateChanged(p_MotionTransitionType);
        }

        public void TurnIdleState(ActableTool.IdleState p_Type, AnimatorParamStorage.MotionTransitionType p_MotionTransitionType)
        {
            _CurrentIdleState = p_Type;
            TurnMoveState(ActableTool.WalkState.Hold, p_MotionTransitionType);
        }

        public void TurnMoveState(AnimatorParamStorage.MotionTransitionType p_MotionTransitionType)
        {
            if (IsIdleState())
            {
                TurnMoveState(ActableTool.WalkState.Walk, p_MotionTransitionType);
            }
            else
            {
                TurnMoveState(_CurrentWalkState, p_MotionTransitionType);
            }
        }
        
        public void TurnIdleState(AnimatorParamStorage.MotionTransitionType p_MotionTransitionType)
        {
            TurnIdleState(_CurrentIdleState, p_MotionTransitionType);
        }

        public bool IsMovingState()
        {
            return _CurrentWalkState != ActableTool.WalkState.Hold;
        }
        
        public bool IsIdleState()
        {
            return _CurrentWalkState == ActableTool.WalkState.Hold;
        }

        public float GetMoveStateSpeedRate()
        {
            switch (_CurrentWalkState)
            {
                case ActableTool.WalkState.Walk:
                    return 0.5f;
                case ActableTool.WalkState.Run:
                    return ActableTool.__DASH_SPEED_RATE;
                default :
                    return 0f;
            } 
        }

        public bool IsUnWeaponModule()
        {
            return _MasterNode._ActableObject._ActableType == UnitActionDataRoot.ActableType.UnWeaponPhaseTransition;
        }

        #endregion
    }
}