using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유닛의 사고 법칙을 기술하는 인터페이스
    /// </summary>
    public interface IThinckable : IIncarnateUnit
    {
        UnitAIDataRoot.UnitMindType _MindType { get; }
        IThinkableTableRecordBridge _MindRecord { get; }
        IThinckable OnInitializeAI(UnitAIDataRoot.UnitMindType p_MindType, Unit p_TargetUnit, IThinkableTableRecordBridge p_MindPreset);
        PhysicsTool.UpdateAutonomyPhysicsResult OnUpdateAutonomyPhysics(float p_RemainingDistance, float p_StoppingDistance);
        void OnAutonomyPhysicsPathOver();
        // bool HasEnemy();
        float GetAISpeedRate();
        /*void SetEnemy(Unit p_TargetUnit, PrefabInstanceTool.FocusNodeRelateType p_TraceUnitRelateType);
        void ClearEnemy();
        bool AttackTo(Unit p_TargetUnit, bool p_ForceAttack, ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType);
        bool ActSpell(Unit p_TargetUnit, ControllerTool.ControlEventPreset p_Preset, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType);
        void ReserveCommand(ThinkableTool.AIReserveHandleType p_Type);
        bool ReserveCommand(ControllerTool.CommandType p_ReserveCommand, ThinkableTool.AIReserveCommand p_ReserveType, ThinkableTool.AIReserveHandleType p_FailHandleType, bool p_ReserveRestrictFlag);*/
        bool MoveTo(Vector3 p_TargetPosition, bool p_ForceMove, PhysicsTool.AutonomyPathStoppingDistancePreset p_AutonomyPathStoppingDistancePreset);
        void StopMove(ActableTool.IdleState p_IdleType);
        void Idle(ActableTool.IdleState p_IdleType);
        bool ReturnPosition(bool p_ForceMove, bool p_SwitchInstance);
        ThinkableTool.AIState GetCurrentAIState();
        bool HasAIExtraFlag(ThinkableTool.AIExtraFlag p_Type);
        void SetAIFlagMask(ThinkableTool.AIExtraFlag p_FlagMask);
        void SetNeverSleep(bool p_Flag);
        void SetWander(bool p_Flag);
        void SetCheckEncounter(bool p_Flag);
        void SetCounter(bool p_Flag);
        void SetAggressive(bool p_Flag);
        void SetJunkYardDog(bool p_Flag);
        void SetRemoteOrder(bool p_Flag);
        bool FindEnemyFromPivot(float p_Distance, Unit.UnitStateType p_StateFlag);
        bool FindEnemyFromPivot(Vector3 p_Pivot, float p_Distance, Unit.UnitStateType p_StateFlag);
        bool FindEnemyWithParams(FilterParams p_FilterParams);
        /*(bool, Unit) FindEnemy(float p_Distance, ThinkableTool.AIUnitFindType p_FindType,
            Unit.UnitStateType p_StateFlag);
        (bool, Unit) FindEnemy(Vector3 p_Pivot, float p_Distance, ThinkableTool.AIUnitFindType p_FindType,
            Unit.UnitStateType p_StateFlag);*/
        void OnAutonomyPhysicsPendingDelay();
        void OnAutonomyPhysicsPendingOver(bool p_PathCompleted);
        void OnAutonomyPhysicsPendingDeadline();
        void OnAutonomyPhysicsStuck();
        void LoadAIPresetFromTableRecord();
        ThinkableTool.AIStatePreset GetAIPreset(ThinkableTool.AIState p_State);
        // float GetSearchDistance();
        void SetSlaveMasterUnit(Unit p_Target);
        void ClearReserveCommand();
        //bool CheckEnterableNextAction();
        void SetAIRange(ThinkableTool.AIState p_State, float p_Value);
    }
}