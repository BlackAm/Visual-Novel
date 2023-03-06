using System.Collections.Generic;

namespace k514
{
    /// <summary>
    /// 유닛의 액션을 기술하는 인터페이스
    /// </summary>
    public interface IActable : IIncarnateUnit, IAnimationClipEventReceivable
    {
        UnitActionDataRoot.ActableType _ActableType { get; }
        IActableTableRecordBridge _ActableRecord { get; }
        IActable OnInitializeActable(UnitActionDataRoot.ActableType p_ActableType, Unit p_TargetUnit, IActableTableRecordBridge p_ActablePreset);

        ControllerTool.CommandType _DefaultCommand { get; }

        ActableTool.WalkState _CurrentWalkState { get; }
        ActableTool.IdleState _CurrentIdleState { get; }

        (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnHandleMoveAction(ControllerTool.ControlEventPreset p_Preset);
        (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnHandleJumpAction(ControllerTool.ControlEventPreset p_Preset);
        (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnHandleSpellAction(ControllerTool.ControlEventPreset p_Preset);

        (bool, UnitActionTool.UnitAction.UnitTryActionResultType) OnActionTriggered(ActableTool.UnitActionType p_ActionType, ControllerTool.ControlEventPreset p_Preset);
        (bool, ActableTool.UnitActionType) GetCommandType(ControllerTool.CommandType p_CommandType);
        bool HasActionCommand(ControllerTool.CommandType p_CommandType);
        (bool, UnitActionTool.UnitAction.UnitTryActionResultType) IsSpellEnterable(ControllerTool.CommandType p_CommandType);

        void SetMaxJumpCount(int p_Count);
        bool IsJumpedManual();
        void AddJumpCount();
        
        // momo6346 - 선택한 커맨드의 스킬을 변경/삭제 혹은 스킬을 조회 합니다...
        ControllerTool.CommandType FindCommandSkill(ActableTool.UnitActionCluster data);
        Dictionary<ControllerTool.CommandType, ActableTool.UnitActionSpellPreset> GetSkill();
        void SaveSkill(bool sceneMove);
        
        void ResetCooldown();
        void ProgressCooldown(float p_DeltaTime);
        void SetCooldown(ControllerTool.CommandType p_Type, float p_Cooldown, bool p_ResetCooldown);
        bool IsCooldown(ControllerTool.CommandType p_Type);

        void TurnMoveState(ActableTool.WalkState p_Type, AnimatorParamStorage.MotionTransitionType p_MotionTransitionType);
        void TurnIdleState(ActableTool.IdleState p_Type, AnimatorParamStorage.MotionTransitionType p_MotionTransitionType);
        void TurnMoveState(AnimatorParamStorage.MotionTransitionType p_MotionTransitionType);
        void TurnIdleState(AnimatorParamStorage.MotionTransitionType p_MotionTransitionType);

        bool IsMovingState();
        bool IsIdleState();
        float GetMoveStateSpeedRate();
        void CancelUnitAction(AnimatorParamStorage.MotionType p_CancelMotion);
        UnitActionTool.UnitActionPhase GetCurrentPhase();
        void ClearReserveCommandInput();
        List<ControllerTool.CommandType> GetAvailableCommandTypeList();
        ControllerTool.CommandType GetRandomAvailableCommandType();
        (bool, UnitActionTool.UnitAction) GetPrimeUnitAction(ControllerTool.CommandType p_CommandType);
        (bool, UnitActionTool.UnitAction) GetUnitAction(ControllerTool.CommandType p_CommandType);
        (bool, UnitActionTool.UnitAction) GetUnitActionValid(ControllerTool.CommandType p_CommandType);
        (bool, float) GetUnitActionAttackRange(ControllerTool.CommandType p_CommandType);
        (bool, ThinkableTool.AIUnitFindType) GetUnitActionUnitFindType(ControllerTool.CommandType p_CommandType);
        void OnMoveTriggeredWhenDriveSpell(bool p_RestrictFlag);
        ObjectDeployEventRecord GetObjectDeployEventRecord();
        (ControllerTool.CommandType, int) FindAction(int p_ActionIndex);
        (bool, UnitActionTool.UnitAction) GetCurrentUnitAction();
        bool IsUnWeaponModule();
    }
}