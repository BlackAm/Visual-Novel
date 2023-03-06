using UnityEngine;

namespace k514
{
    public partial class ActableBase
    {
        #region <Fields>

        /// <summary>
        /// 최대 점프 카운트
        /// </summary>
        protected int MaxJumpCount;
        
        /// <summary>
        /// 현재 누적된 점프 횟수
        /// </summary>
        protected int CurrentJumpCount;
        
        #endregion

        #region <Callbacks>

        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnHandleJumpAction(ControllerTool.ControlEventPreset p_Preset)
        {
            if (p_Preset.IsInputPress)
            { 
                if(IsAvailableToJump())
                {
                    AddJumpCount();
                    if (!ReferenceEquals(null, _MasterNode._PhysicsObject))
                    {
                        // 이미 체공중인 상태에서, 추가로 점프 입력이 발생한 경우에 모션을 다시 재생해준다.
                        if (_MasterNode.HasState_Or(Unit.UnitStateType.FLOAT) ||
                            _MasterNode._AnimationObject.IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType
                                .JumpDown))
                        {
                            _MasterNode._AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.JumpUp, 0,
                                AnimatorParamStorage.MotionTransitionType.Restrict_ToSameMotion |
                                AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                        }

                        _MasterNode._PhysicsObject.ClearVelocity();
                        _MasterNode._PhysicsObject.ClearPhysicsAutonomyMove();
                        _MasterNode.ForceTo(Vector3.up, 0);
                    }

                    return (true, UnitActionTool.UnitAction.UnitTryActionResultType.Success_EntrySpell);
                }
                else
                {
                    return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_EntrySpell);
                }
            }
            else
            {
                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_HoldCommand);
            }
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 현재 점프 가능한 상태인지 리턴하는 논리 메서드
        /// </summary>
        protected bool IsAvailableToJump()
        {
            return _MasterNode.HasState_Only(Unit.UnitStateType.DefaultJumpAvailableMask)
                   && MaxJumpCount > CurrentJumpCount
                   && !_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL);
        }

        public void SetMaxJumpCount(int p_Count)
        {
            MaxJumpCount = p_Count + 1;
        }

        public bool IsJumpedManual() => CurrentJumpCount > 0;

        public void AddJumpCount() => CurrentJumpCount++;

        #endregion
    }
}