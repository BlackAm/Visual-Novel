namespace k514
{
    public static partial class EnumFlag
    {
        #region <IteratorProgressFlag>
        
        public static void AddFlag(this ref TimerIteratorBase.IteratorProgressFlag p_TargetMask, TimerIteratorBase.IteratorProgressFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref TimerIteratorBase.IteratorProgressFlag p_TargetMask, TimerIteratorBase.IteratorProgressFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref TimerIteratorBase.IteratorProgressFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(TimerIteratorBase.IteratorProgressFlag.None);
        }

        public static void TurnFlag(this ref TimerIteratorBase.IteratorProgressFlag p_TargetMask, TimerIteratorBase.IteratorProgressFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this TimerIteratorBase.IteratorProgressFlag p_TargetMask, TimerIteratorBase.IteratorProgressFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != TimerIteratorBase.IteratorProgressFlag.None;
        }

        #endregion

        #region <UnitStampResultFlag>
        
        public static void AddFlag(this ref UnitTool.UnitStampResultFlag p_TargetMask, UnitTool.UnitStampResultFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitTool.UnitStampResultFlag p_TargetMask, UnitTool.UnitStampResultFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitTool.UnitStampResultFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitTool.UnitStampResultFlag.None);
        }

        public static void TurnFlag(this ref UnitTool.UnitStampResultFlag p_TargetMask, UnitTool.UnitStampResultFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitTool.UnitStampResultFlag p_TargetMask, UnitTool.UnitStampResultFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitTool.UnitStampResultFlag.None;
        }

        #endregion

        #region <UnitAddForceFilterEventType>
        
        public static void AddFlag(this ref UnitTool.UnitAddForceFilterEventType p_TargetMask, UnitTool.UnitAddForceFilterEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitTool.UnitAddForceFilterEventType p_TargetMask, UnitTool.UnitAddForceFilterEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitTool.UnitAddForceFilterEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitTool.UnitAddForceFilterEventType.None);
        }

        public static void TurnFlag(this ref UnitTool.UnitAddForceFilterEventType p_TargetMask, UnitTool.UnitAddForceFilterEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitTool.UnitAddForceFilterEventType p_TargetMask, UnitTool.UnitAddForceFilterEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitTool.UnitAddForceFilterEventType.None;
        }

        #endregion
        
        #region <UnitAddForceProcessType>
        
        public static void AddFlag(this ref UnitTool.UnitAddForceProcessType p_TargetMask, UnitTool.UnitAddForceProcessType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitTool.UnitAddForceProcessType p_TargetMask, UnitTool.UnitAddForceProcessType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitTool.UnitAddForceProcessType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitTool.UnitAddForceProcessType.None);
        }

        public static void TurnFlag(this ref UnitTool.UnitAddForceProcessType p_TargetMask, UnitTool.UnitAddForceProcessType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitTool.UnitAddForceProcessType p_TargetMask, UnitTool.UnitAddForceProcessType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitTool.UnitAddForceProcessType.None;
        }

        #endregion

        #region <UnitFilterParamFlag>
        
        public static void AddFlag(this ref UnitFilterTool.FilterParamsFlag p_TargetMask, UnitFilterTool.FilterParamsFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitFilterTool.FilterParamsFlag p_TargetMask, UnitFilterTool.FilterParamsFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitFilterTool.FilterParamsFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitFilterTool.FilterParamsFlag.None);
        }

        public static void TurnFlag(this ref UnitFilterTool.FilterParamsFlag p_TargetMask, UnitFilterTool.FilterParamsFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitFilterTool.FilterParamsFlag p_TargetMask, UnitFilterTool.FilterParamsFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitFilterTool.FilterParamsFlag.None;
        }

        #endregion
        
        #region <ProjectileProgressFlag>
        
        public static void AddFlag(this ref ProjectileTool.ProjectileProgressFlag p_TargetMask, ProjectileTool.ProjectileProgressFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ProjectileTool.ProjectileProgressFlag p_TargetMask, ProjectileTool.ProjectileProgressFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ProjectileTool.ProjectileProgressFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ProjectileTool.ProjectileProgressFlag.None);
        }

        public static void TurnFlag(this ref ProjectileTool.ProjectileProgressFlag p_TargetMask, ProjectileTool.ProjectileProgressFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ProjectileTool.ProjectileProgressFlag p_TargetMask, ProjectileTool.ProjectileProgressFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ProjectileTool.ProjectileProgressFlag.None;
        }

        #endregion
        
        #region <ProjectileCollisionHandleFlag>
        
        public static void AddFlag(this ref ProjectileTool.ProjectileEventHandleFlag p_TargetMask, ProjectileTool.ProjectileEventHandleFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ProjectileTool.ProjectileEventHandleFlag p_TargetMask, ProjectileTool.ProjectileEventHandleFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ProjectileTool.ProjectileEventHandleFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ProjectileTool.ProjectileEventHandleFlag.None);
        }

        public static void TurnFlag(this ref ProjectileTool.ProjectileEventHandleFlag p_TargetMask, ProjectileTool.ProjectileEventHandleFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ProjectileTool.ProjectileEventHandleFlag p_TargetMask, ProjectileTool.ProjectileEventHandleFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ProjectileTool.ProjectileEventHandleFlag.None;
        }

        #endregion
        
        #region <ProjectileCollisionEventFlag>
        
        public static void AddFlag(this ref ProjectileTool.ProjectileCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileCollisionEventFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ProjectileTool.ProjectileCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileCollisionEventFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ProjectileTool.ProjectileCollisionEventFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ProjectileTool.ProjectileCollisionEventFlag.None);
        }

        public static void TurnFlag(this ref ProjectileTool.ProjectileCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileCollisionEventFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ProjectileTool.ProjectileCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileCollisionEventFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ProjectileTool.ProjectileCollisionEventFlag.None;
        }

        #endregion
        
        #region <ProjectileTraceTargetEventFlag>
        
        public static void AddFlag(this ref ProjectileTool.ProjectileTraceTargetEventFlag p_TargetMask, ProjectileTool.ProjectileTraceTargetEventFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ProjectileTool.ProjectileTraceTargetEventFlag p_TargetMask, ProjectileTool.ProjectileTraceTargetEventFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ProjectileTool.ProjectileTraceTargetEventFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ProjectileTool.ProjectileTraceTargetEventFlag.None);
        }

        public static void TurnFlag(this ref ProjectileTool.ProjectileTraceTargetEventFlag p_TargetMask, ProjectileTool.ProjectileTraceTargetEventFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ProjectileTool.ProjectileTraceTargetEventFlag p_TargetMask, ProjectileTool.ProjectileTraceTargetEventFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ProjectileTool.ProjectileTraceTargetEventFlag.None;
        }

        #endregion
    }
}