namespace k514
{
    public partial class EnumFlag
    {
        #region <Wandering>

        public static void AddFlag(this ref ThinkableTool.AIWanderingType p_TargetMask, ThinkableTool.AIWanderingType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ThinkableTool.AIWanderingType p_TargetMask, ThinkableTool.AIWanderingType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ThinkableTool.AIWanderingType p_TargetMask)
        {
            p_TargetMask.TurnFlag(ThinkableTool.AIWanderingType.None);
        }

        public static void TurnFlag(this ref ThinkableTool.AIWanderingType p_TargetMask, ThinkableTool.AIWanderingType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ThinkableTool.AIWanderingType p_TargetMask, ThinkableTool.AIWanderingType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ThinkableTool.AIWanderingType.None;
        }

        #endregion
        
        #region <HitParameter>

        public static void AddFlag(this ref UnitHitTool.HitParameterType p_TargetMask, UnitHitTool.HitParameterType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitHitTool.HitParameterType p_TargetMask, UnitHitTool.HitParameterType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitHitTool.HitParameterType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitHitTool.HitParameterType.None);
        }

        public static void TurnFlag(this ref UnitHitTool.HitParameterType p_TargetMask, UnitHitTool.HitParameterType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitHitTool.HitParameterType p_TargetMask, UnitHitTool.HitParameterType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitHitTool.HitParameterType.None;
        }

        #endregion
        
        #region <HitExtra>

        public static void AddFlag(this ref UnitHitTool.HitExtraAttributeType p_TargetMask, UnitHitTool.HitExtraAttributeType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitHitTool.HitExtraAttributeType p_TargetMask, UnitHitTool.HitExtraAttributeType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitHitTool.HitExtraAttributeType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitHitTool.HitExtraAttributeType.None);
        }

        public static void TurnFlag(this ref UnitHitTool.HitExtraAttributeType p_TargetMask, UnitHitTool.HitExtraAttributeType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitHitTool.HitExtraAttributeType p_TargetMask, UnitHitTool.HitExtraAttributeType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitHitTool.HitExtraAttributeType.None;
        }

        #endregion
        
        #region <HitBuff>

        public static void AddFlag(this ref UnitHitTool.HitBuffType p_TargetMask, UnitHitTool.HitBuffType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitHitTool.HitBuffType p_TargetMask, UnitHitTool.HitBuffType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitHitTool.HitBuffType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitHitTool.HitBuffType.None);
        }

        public static void TurnFlag(this ref UnitHitTool.HitBuffType p_TargetMask, UnitHitTool.HitBuffType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitHitTool.HitBuffType p_TargetMask, UnitHitTool.HitBuffType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitHitTool.HitBuffType.None;
        }

        #endregion
        
        #region <HitResult>

        public static void AddFlag(this ref HitResult.HitResultFlag p_TargetMask, HitResult.HitResultFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref HitResult.HitResultFlag p_TargetMask, HitResult.HitResultFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref HitResult.HitResultFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(HitResult.HitResultFlag.None);
        }

        public static void TurnFlag(this ref HitResult.HitResultFlag p_TargetMask, HitResult.HitResultFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this HitResult.HitResultFlag p_TargetMask, HitResult.HitResultFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != HitResult.HitResultFlag.None;
        }

        #endregion

        #region <SystemLateHandleEventType>

        public static void AddFlag(this ref SystemTool.SystemOnceFrameEventType p_TargetMask, SystemTool.SystemOnceFrameEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref SystemTool.SystemOnceFrameEventType p_TargetMask, SystemTool.SystemOnceFrameEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref SystemTool.SystemOnceFrameEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(SystemTool.SystemOnceFrameEventType.None);
        }

        public static void TurnFlag(this ref SystemTool.SystemOnceFrameEventType p_TargetMask, SystemTool.SystemOnceFrameEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this SystemTool.SystemOnceFrameEventType p_TargetMask, SystemTool.SystemOnceFrameEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != SystemTool.SystemOnceFrameEventType.None;
        }

        #endregion

        #region <UnitActionClusterType>

        public static void AddFlag(this ref ActableTool.UnitActionClusterFlag p_TargetMask, ActableTool.UnitActionClusterFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ActableTool.UnitActionClusterFlag p_TargetMask, ActableTool.UnitActionClusterFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ActableTool.UnitActionClusterFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ActableTool.UnitActionClusterFlag.None);
        }

        public static void TurnFlag(this ref ActableTool.UnitActionClusterFlag p_TargetMask, ActableTool.UnitActionClusterFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ActableTool.UnitActionClusterFlag p_TargetMask, ActableTool.UnitActionClusterFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ActableTool.UnitActionClusterFlag.None;
        }

        #endregion
        
        #region <FilterResultType>

        public static void AddFlag(this ref UnitFilterTool.FilterResultType p_TargetMask, UnitFilterTool.FilterResultType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitFilterTool.FilterResultType p_TargetMask, UnitFilterTool.FilterResultType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitFilterTool.FilterResultType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitFilterTool.FilterResultType.None);
        }

        public static void TurnFlag(this ref UnitFilterTool.FilterResultType p_TargetMask, UnitFilterTool.FilterResultType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitFilterTool.FilterResultType p_TargetMask, UnitFilterTool.FilterResultType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitFilterTool.FilterResultType.None;
        }

        #endregion
        
        #region <ObjectDeployFlag>

        public static void AddFlag(this ref ObjectDeployTool.ObjectDeployFlag p_TargetMask, ObjectDeployTool.ObjectDeployFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ObjectDeployTool.ObjectDeployFlag p_TargetMask, ObjectDeployTool.ObjectDeployFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ObjectDeployTool.ObjectDeployFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ObjectDeployTool.ObjectDeployFlag.None);
        }

        public static void TurnFlag(this ref ObjectDeployTool.ObjectDeployFlag p_TargetMask, ObjectDeployTool.ObjectDeployFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ObjectDeployTool.ObjectDeployFlag p_TargetMask, ObjectDeployTool.ObjectDeployFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ObjectDeployTool.ObjectDeployFlag.None;
        }

        #endregion

        #region <EventDeployFlag>

        public static void AddFlag(this ref ObjectDeployEventPreset.EventDeployFlag p_TargetMask, ObjectDeployEventPreset.EventDeployFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ObjectDeployEventPreset.EventDeployFlag p_TargetMask, ObjectDeployEventPreset.EventDeployFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ObjectDeployEventPreset.EventDeployFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ObjectDeployEventPreset.EventDeployFlag.None);
        }

        public static void TurnFlag(this ref ObjectDeployEventPreset.EventDeployFlag p_TargetMask, ObjectDeployEventPreset.EventDeployFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ObjectDeployEventPreset.EventDeployFlag p_TargetMask, ObjectDeployEventPreset.EventDeployFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ObjectDeployEventPreset.EventDeployFlag.None;
        }

        #endregion

        #region <CameraEventType>

#if !SERVER_DRIVE
        public static void AddFlag(this ref CameraManager.CameraEventType p_TargetMask, CameraManager.CameraEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref CameraManager.CameraEventType p_TargetMask, CameraManager.CameraEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref CameraManager.CameraEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(CameraManager.CameraEventType.None);
        }

        public static void TurnFlag(this ref CameraManager.CameraEventType p_TargetMask, CameraManager.CameraEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this CameraManager.CameraEventType p_TargetMask, CameraManager.CameraEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != CameraManager.CameraEventType.None;
        }
#endif

        #endregion
        
        #region <RenderGroupType>

#if !SERVER_DRIVE
        public static void AddFlag(this ref RenderableTool.RenderGroupType p_TargetMask, RenderableTool.RenderGroupType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref RenderableTool.RenderGroupType p_TargetMask, RenderableTool.RenderGroupType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref RenderableTool.RenderGroupType p_TargetMask)
        {
            p_TargetMask.TurnFlag(RenderableTool.RenderGroupType.None);
        }

        public static void TurnFlag(this ref RenderableTool.RenderGroupType p_TargetMask, RenderableTool.RenderGroupType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this RenderableTool.RenderGroupType p_TargetMask, RenderableTool.RenderGroupType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != RenderableTool.RenderGroupType.None;
        }
#endif

        #endregion
        
        #region <ShaderControlType>

        public static void AddFlag(this ref RenderableTool.ShaderControlType p_TargetMask, RenderableTool.ShaderControlType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref RenderableTool.ShaderControlType p_TargetMask, RenderableTool.ShaderControlType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref RenderableTool.ShaderControlType p_TargetMask)
        {
            p_TargetMask.TurnFlag(RenderableTool.ShaderControlType.None);
        }

        public static void TurnFlag(this ref RenderableTool.ShaderControlType p_TargetMask, RenderableTool.ShaderControlType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this RenderableTool.ShaderControlType p_TargetMask, RenderableTool.ShaderControlType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != RenderableTool.ShaderControlType.None;
        }

        #endregion
        
        #region <AIReserveCommand>

        public static void AddFlag(this ref ThinkableTool.AIReserveCommand p_TargetMask, ThinkableTool.AIReserveCommand p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ThinkableTool.AIReserveCommand p_TargetMask, ThinkableTool.AIReserveCommand p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ThinkableTool.AIReserveCommand p_TargetMask)
        {
            p_TargetMask.TurnFlag(ThinkableTool.AIReserveCommand.None);
        }

        public static void TurnFlag(this ref ThinkableTool.AIReserveCommand p_TargetMask, ThinkableTool.AIReserveCommand p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ThinkableTool.AIReserveCommand p_TargetMask, ThinkableTool.AIReserveCommand p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ThinkableTool.AIReserveCommand.None;
        }

        #endregion
    }
}