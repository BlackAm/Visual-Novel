namespace k514
{
    public partial class ActableBase
    {
        #region <Callbacks>

        public (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnHandleMoveAction(ControllerTool.ControlEventPreset p_Preset)
        {
            if (_MasterNode.HasState_Or(Unit.UnitStateType.DRIVESKILL))
            {
                OnMoveTriggeredWhenDriveSpell(false);
            }

            // 착지모션(JumpDown) 완료 후에 이동 액션을 실행하기 위해, 현재 모션이 착지모션이라면 해당 블록을 실행하지 않는다.
            if (
                !_MasterNode._AnimationObject.IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType.JumpDown) 
                && _MasterNode.HasState_Only(Unit.UnitStateType.DefaultMoveAvailableMask)
            )
            {
                if (p_Preset.IsInputRelease)
                {
                    TurnMoveState(ActableTool.WalkState.Hold, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                    _MasterNode.HaltController(true);
                }
                else
                {
                    // TODO<K514> : 이후에 마을 나오면
                    _MasterNode._PhysicsObject.ClearPhysicsAutonomyMove();
                    var tryUnitMove = _MasterNode.ForceController(p_Preset, GetMoveStateSpeedRate());
                    if (tryUnitMove)
                    {
                        TurnMoveState(true ? ActableTool.WalkState.Run : ActableTool.WalkState.Walk, AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine);
                    }
                }

                return (true, UnitActionTool.UnitAction.UnitTryActionResultType.Success_EntrySpell);
            }
            else
            {
                _MasterNode.HaltController(true);
                return (false, UnitActionTool.UnitAction.UnitTryActionResultType.Fail_EntrySpell);
            }
        }

        #endregion
    }
}