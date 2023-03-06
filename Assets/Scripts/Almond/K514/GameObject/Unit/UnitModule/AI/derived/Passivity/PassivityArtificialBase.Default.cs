namespace k514
{
    public partial class PassivityAIBase
    {
        #region <Callbacks>
        
        public override void OnUpdate(float p_DeltaTime)
        {
        }

        public override void OnUpdate_TimeBlock()
        {
        }

        public override void OnJumpUp()
        {
        }

        public override void OnUnitHitActionStarted()
        {
        }
        
        public override void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
        }
        
        public override void OnUnitHitActionTerminated()
        {
        }

        public override void OnUnitActionStarted()
        {
        }

        public override void OnUnitActionTerminated()
        {
        }

        #endregion
        
        #region <Methods>

        public override void LoadAIPresetFromTableRecord()
        {
        }

        public override float GetAISpeedRate()
        {
            return 1f;
        }

        public override ThinkableTool.AIStatePreset GetAIPreset(ThinkableTool.AIState p_State)
        {
            return default;
        }

        public override void SetSlaveMasterUnit(Unit p_Target)
        {
        }

        public override void ClearReserveCommand()
        {
        }

        public override void SetRemoteOrder(bool p_Flag)
        {
        }
        
        public override ThinkableTool.AIState GetCurrentAIState()
        {
            return ThinkableTool.AIState.None;
        }

        public override bool HasAIExtraFlag(ThinkableTool.AIExtraFlag p_Type)
        {
            return false;
        }

        public override void SetAIFlagMask(ThinkableTool.AIExtraFlag p_FlagMask)
        {
        }

        public override void SetNeverSleep(bool p_Flag)
        {
        }

        public override void SetWander(bool p_Flag)
        {
        }

        public override void SetCheckEncounter(bool p_Flag)
        {
        }

        public override void SetCounter(bool p_Flag)
        {
        }

        public override void SetAggressive(bool p_Flag)
        {
        }

        public override void SetJunkYardDog(bool p_Flag)
        {
        }

        #endregion
    }
}