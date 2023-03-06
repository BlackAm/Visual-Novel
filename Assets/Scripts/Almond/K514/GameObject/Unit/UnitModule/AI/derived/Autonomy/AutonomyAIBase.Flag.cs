namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Fields>

        /// <summary>
        /// 해당 인공지능 유닛의 행동 변수 플래그마스크
        /// </summary>
        public ThinkableTool.AIExtraFlag _AIExtraFlagMask;

        /// <summary>
        /// 추적할 길찾기 위치 선정 타입
        /// </summary>
        protected ThinkableTool.AITracePivotSelectType _AITracePivotSelectType;
        protected ThinkableTool.AITracePivotSelectType _AITracePivotSelectTypeWhenTargetMoving;

        #endregion
        
        #region <Methods>
        protected void LoadAIExtraFlagFromTableRecord()
        {
            SetAIFlagMask(_MindRecord.AIExtraFlag);
            _AITracePivotSelectType = _MindRecord.AITracePivotSelectType;
            _AITracePivotSelectTypeWhenTargetMoving = _MindRecord.AITracePivotSelectTypeWhenTargetMoving;
        }
        
        public override bool HasAIExtraFlag(ThinkableTool.AIExtraFlag p_Type)
        {
            return _AIExtraFlagMask.HasAnyFlagExceptNone(p_Type);
        }
        
        public override void SetAIFlagMask(ThinkableTool.AIExtraFlag p_FlagMask)
        {
            _AIExtraFlagMask.TurnFlag(p_FlagMask);
            TryCheckAwakeModule(false);
        }
        
        public override void SetNeverSleep(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.NeverSleep);
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.NeverSleep);
            }
            
            TryCheckAwakeModule(false);
        }

        public override void SetWander(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.Wandering);
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.Wandering);
            }
        }

        public override void SetCheckEncounter(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.EncounterCheck);
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.EncounterCheck);
            }
        }
        
        public override void SetCounter(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.Counter);
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.Counter);
            }
        }
        
        public override void SetAggressive(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.Aggressive);
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.Aggressive);
            }
        }
        
        public override void SetJunkYardDog(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.JunkYardDog);
                foreach (var aiState in ThinkableTool._AIState_Enumerator)
                {
                    switch (aiState)
                    {
                        case ThinkableTool.AIState.None:
                        case ThinkableTool.AIState.Attack:
                        case ThinkableTool.AIState.Idle:
                            break;
                        case ThinkableTool.AIState.Notice:
                        case ThinkableTool.AIState.Trace:
                        case ThinkableTool.AIState.Move:
                            var tryPreset = _StatePresetRecord[aiState];
                            tryPreset.SetRange(500f);
                            _StatePresetRecord[aiState] = tryPreset;
                            break;
                    }   
                }
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.JunkYardDog);
                LoadAIPresetFromTableRecord();
            }
        }
        
        public override void SetRemoteOrder(bool p_Flag)
        {
            if (p_Flag)
            {
                _AIExtraFlagMask.AddFlag(ThinkableTool.AIExtraFlag.RemoteOrder);
            }
            else
            {
                _AIExtraFlagMask.RemoveFlag(ThinkableTool.AIExtraFlag.RemoteOrder);
            }
        }
        
        #endregion
    }
}