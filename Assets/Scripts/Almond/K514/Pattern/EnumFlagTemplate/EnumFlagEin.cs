namespace k514
{
    public static partial class EnumFlag
    {
        #region <TestManager>
        
#if UNITY_EDITOR && ON_GUI
        public static void AddFlag(this ref TestManager.TestExtraInputType p_TargetMask, TestManager.TestExtraInputType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref TestManager.TestExtraInputType p_TargetMask, TestManager.TestExtraInputType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref TestManager.TestExtraInputType p_TargetMask)
        {
            p_TargetMask.TurnFlag(TestManager.TestExtraInputType.None);
        }

        public static void TurnFlag(this ref TestManager.TestExtraInputType p_TargetMask, TestManager.TestExtraInputType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this TestManager.TestExtraInputType p_TargetMask, TestManager.TestExtraInputType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != TestManager.TestExtraInputType.None;
        }
#endif
        
        #endregion
        
        #region <UnitFilterFlag>
        
        public static void AddFlag(this ref UnitFilterTool.UnitFilterFlagType p_TargetMask, UnitFilterTool.UnitFilterFlagType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitFilterTool.UnitFilterFlagType p_TargetMask, UnitFilterTool.UnitFilterFlagType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitFilterTool.UnitFilterFlagType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitFilterTool.UnitFilterFlagType.None);
        }

        public static void TurnFlag(this ref UnitFilterTool.UnitFilterFlagType p_TargetMask, UnitFilterTool.UnitFilterFlagType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitFilterTool.UnitFilterFlagType p_TargetMask, UnitFilterTool.UnitFilterFlagType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitFilterTool.UnitFilterFlagType.None;
        }

        #endregion
        
        #region <MotionTransition>

        public static void AddFlag(this ref ObjectDeployTool.DeployableType p_TargetMask, ObjectDeployTool.DeployableType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ObjectDeployTool.DeployableType p_TargetMask, ObjectDeployTool.DeployableType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ObjectDeployTool.DeployableType p_TargetMask)
        {
            p_TargetMask.TurnFlag(ObjectDeployTool.DeployableType.None);
        }

        public static void TurnFlag(this ref ObjectDeployTool.DeployableType p_TargetMask, ObjectDeployTool.DeployableType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ObjectDeployTool.DeployableType p_TargetMask, ObjectDeployTool.DeployableType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ObjectDeployTool.DeployableType.None;
        }

        #endregion
        
        #region <MotionTransition>

        public static void AddFlag(this ref AnimatorParamStorage.MotionTransitionType p_TargetMask, AnimatorParamStorage.MotionTransitionType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref AnimatorParamStorage.MotionTransitionType p_TargetMask, AnimatorParamStorage.MotionTransitionType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref AnimatorParamStorage.MotionTransitionType p_TargetMask)
        {
            p_TargetMask.TurnFlag(AnimatorParamStorage.MotionTransitionType.None);
        }

        public static void TurnFlag(this ref AnimatorParamStorage.MotionTransitionType p_TargetMask, AnimatorParamStorage.MotionTransitionType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this AnimatorParamStorage.MotionTransitionType p_TargetMask, AnimatorParamStorage.MotionTransitionType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != AnimatorParamStorage.MotionTransitionType.None;
        }

        #endregion
        
        #region <PrefabSpawn>

        public static void AddFlag(this ref ObjectDeployTool.ObjectDeploySurfaceDeployType p_TargetMask, ObjectDeployTool.ObjectDeploySurfaceDeployType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ObjectDeployTool.ObjectDeploySurfaceDeployType p_TargetMask, ObjectDeployTool.ObjectDeploySurfaceDeployType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ObjectDeployTool.ObjectDeploySurfaceDeployType p_TargetMask)
        {
            p_TargetMask.TurnFlag(ObjectDeployTool.ObjectDeploySurfaceDeployType.None);
        }

        public static void TurnFlag(this ref ObjectDeployTool.ObjectDeploySurfaceDeployType p_TargetMask, ObjectDeployTool.ObjectDeploySurfaceDeployType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ObjectDeployTool.ObjectDeploySurfaceDeployType p_TargetMask, ObjectDeployTool.ObjectDeploySurfaceDeployType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ObjectDeployTool.ObjectDeploySurfaceDeployType.None;
        }

        #endregion
        
        #region <ControllerEvent>

        public static void AddFlag(this ref ControllerTool.InputEventType p_TargetMask, ControllerTool.InputEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ControllerTool.InputEventType p_TargetMask, ControllerTool.InputEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ControllerTool.InputEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(ControllerTool.InputEventType.None);
        }

        public static void TurnFlag(this ref ControllerTool.InputEventType p_TargetMask, ControllerTool.InputEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ControllerTool.InputEventType p_TargetMask, ControllerTool.InputEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ControllerTool.InputEventType.None;
        }

        #endregion
        
        #region <TouchEvent>

#if !SERVER_DRIVE
        public static void AddFlag(this ref TouchEventRoot.TouchInputType p_TargetMask, TouchEventRoot.TouchInputType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref TouchEventRoot.TouchInputType p_TargetMask, TouchEventRoot.TouchInputType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref TouchEventRoot.TouchInputType p_TargetMask)
        {
            p_TargetMask.TurnFlag(TouchEventRoot.TouchInputType.None);
        }

        public static void TurnFlag(this ref TouchEventRoot.TouchInputType p_TargetMask, TouchEventRoot.TouchInputType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this TouchEventRoot.TouchInputType p_TargetMask, TouchEventRoot.TouchInputType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != TouchEventRoot.TouchInputType.None;
        }
#endif

        #endregion
        
        #region <HitState>

        public static void AddFlag(this ref HitMessage.HitMessageAttributeFlag p_TargetMask, HitMessage.HitMessageAttributeFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref HitMessage.HitMessageAttributeFlag p_TargetMask, HitMessage.HitMessageAttributeFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref HitMessage.HitMessageAttributeFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(HitMessage.HitMessageAttributeFlag.None);
        }

        public static void TurnFlag(this ref HitMessage.HitMessageAttributeFlag p_TargetMask, HitMessage.HitMessageAttributeFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this HitMessage.HitMessageAttributeFlag p_TargetMask, HitMessage.HitMessageAttributeFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != HitMessage.HitMessageAttributeFlag.None;
        }

        #endregion
                
        #region <Unit>

        public static void AddFlag(this ref Unit.UnitStateType p_TargetMask, Unit.UnitStateType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref Unit.UnitStateType p_TargetMask, Unit.UnitStateType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref Unit.UnitStateType p_TargetMask)
        {
            p_TargetMask.TurnFlag(Unit.UnitStateType.None);
        }

        public static void TurnFlag(this ref Unit.UnitStateType p_TargetMask, Unit.UnitStateType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this Unit.UnitStateType p_TargetMask, Unit.UnitStateType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != Unit.UnitStateType.None;
        }
        
        public static bool HasFlagOnly(this Unit.UnitStateType p_TargetMask, Unit.UnitStateType p_CompareType)
        {
            return (p_TargetMask & ~p_CompareType) == Unit.UnitStateType.None;
        }
        
        public static bool HasAllFlag(this Unit.UnitStateType p_TargetMask, Unit.UnitStateType p_CompareType)
        {
            return (~p_TargetMask & p_CompareType) == Unit.UnitStateType.None;
        }
                
        public static void AddFlag(this ref UnitActionTool.UnitAction.UnitActionFlag p_TargetMask, UnitActionTool.UnitAction.UnitActionFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitActionTool.UnitAction.UnitActionFlag p_TargetMask, UnitActionTool.UnitAction.UnitActionFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitActionTool.UnitAction.UnitActionFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitActionTool.UnitAction.UnitActionFlag.None);
        }

        public static void TurnFlag(this ref UnitActionTool.UnitAction.UnitActionFlag p_TargetMask, UnitActionTool.UnitAction.UnitActionFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitActionTool.UnitAction.UnitActionFlag p_TargetMask, UnitActionTool.UnitAction.UnitActionFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitActionTool.UnitAction.UnitActionFlag.None;
        }
               
        public static void AddFlag(this ref UnitActionTool.MotionRestrictFlag p_TargetMask, UnitActionTool.MotionRestrictFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitActionTool.MotionRestrictFlag p_TargetMask, UnitActionTool.MotionRestrictFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitActionTool.MotionRestrictFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitActionTool.MotionRestrictFlag.None);
        }

        public static void TurnFlag(this ref UnitActionTool.MotionRestrictFlag p_TargetMask, UnitActionTool.MotionRestrictFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitActionTool.MotionRestrictFlag p_TargetMask, UnitActionTool.MotionRestrictFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitActionTool.MotionRestrictFlag.None;
        }
               
        public static void AddFlag(this ref UnitEventHandlerTool.UnitEventType p_TargetMask, UnitEventHandlerTool.UnitEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref UnitEventHandlerTool.UnitEventType p_TargetMask, UnitEventHandlerTool.UnitEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref UnitEventHandlerTool.UnitEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(UnitEventHandlerTool.UnitEventType.None);
        }

        public static void TurnFlag(this ref UnitEventHandlerTool.UnitEventType p_TargetMask, UnitEventHandlerTool.UnitEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this UnitEventHandlerTool.UnitEventType p_TargetMask, UnitEventHandlerTool.UnitEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != UnitEventHandlerTool.UnitEventType.None;
        }
        
        #endregion
                
        #region <HitState>

        public static void AddFlag(this ref EventTimerCoroutine.CoroutinProcessFlag p_TargetMask, EventTimerCoroutine.CoroutinProcessFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref EventTimerCoroutine.CoroutinProcessFlag p_TargetMask, EventTimerCoroutine.CoroutinProcessFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref EventTimerCoroutine.CoroutinProcessFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(EventTimerCoroutine.CoroutinProcessFlag.None);
        }

        public static void TurnFlag(this ref EventTimerCoroutine.CoroutinProcessFlag p_TargetMask, EventTimerCoroutine.CoroutinProcessFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this EventTimerCoroutine.CoroutinProcessFlag p_TargetMask, EventTimerCoroutine.CoroutinProcessFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != EventTimerCoroutine.CoroutinProcessFlag.None;
        }

        #endregion
                
        #region <AI>

        public static void AddFlag(this ref ThinkableTool.AIExtraFlag p_TargetMask, ThinkableTool.AIExtraFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ThinkableTool.AIExtraFlag p_TargetMask, ThinkableTool.AIExtraFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ThinkableTool.AIExtraFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ThinkableTool.AIExtraFlag.None);
        }

        public static void TurnFlag(this ref ThinkableTool.AIExtraFlag p_TargetMask, ThinkableTool.AIExtraFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ThinkableTool.AIExtraFlag p_TargetMask, ThinkableTool.AIExtraFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ThinkableTool.AIExtraFlag.None;
        }

        #endregion
                
        #region <PrefabEvent>

        public static void AddFlag(this ref PrefabEventSender.UnityPrefabEventType p_TargetMask, PrefabEventSender.UnityPrefabEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref PrefabEventSender.UnityPrefabEventType p_TargetMask, PrefabEventSender.UnityPrefabEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref PrefabEventSender.UnityPrefabEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(PrefabEventSender.UnityPrefabEventType.None);
        }

        public static void TurnFlag(this ref PrefabEventSender.UnityPrefabEventType p_TargetMask, PrefabEventSender.UnityPrefabEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this PrefabEventSender.UnityPrefabEventType p_TargetMask, PrefabEventSender.UnityPrefabEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != PrefabEventSender.UnityPrefabEventType.None;
        }

        #endregion
                        
        #region <PlayerEvent>

#if !SERVER_DRIVE
        public static void AddFlag(this ref PlayerManager.PlayerChangeEventType p_TargetMask, PlayerManager.PlayerChangeEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref PlayerManager.PlayerChangeEventType p_TargetMask, PlayerManager.PlayerChangeEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref PlayerManager.PlayerChangeEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(PlayerManager.PlayerChangeEventType.None);
        }

        public static void TurnFlag(this ref PlayerManager.PlayerChangeEventType p_TargetMask, PlayerManager.PlayerChangeEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this PlayerManager.PlayerChangeEventType p_TargetMask, PlayerManager.PlayerChangeEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != PlayerManager.PlayerChangeEventType.None;
        }
#endif

        #endregion
                                
        #region <SceneEvnet>

        public static void AddFlag(this ref SceneEnvironmentManager.SceneVariableEventType p_TargetMask, SceneEnvironmentManager.SceneVariableEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref SceneEnvironmentManager.SceneVariableEventType p_TargetMask, SceneEnvironmentManager.SceneVariableEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref SceneEnvironmentManager.SceneVariableEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(SceneEnvironmentManager.SceneVariableEventType.None);
        }

        public static void TurnFlag(this ref SceneEnvironmentManager.SceneVariableEventType p_TargetMask, SceneEnvironmentManager.SceneVariableEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this SceneEnvironmentManager.SceneVariableEventType p_TargetMask, SceneEnvironmentManager.SceneVariableEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != SceneEnvironmentManager.SceneVariableEventType.None;
        }

        #endregion
                                        
        #region <TouchEvent>

#if !SERVER_DRIVE
        public static void AddFlag(this ref TouchEventRoot.TouchEventType p_TargetMask, TouchEventRoot.TouchEventType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref TouchEventRoot.TouchEventType p_TargetMask, TouchEventRoot.TouchEventType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref TouchEventRoot.TouchEventType p_TargetMask)
        {
            p_TargetMask.TurnFlag(TouchEventRoot.TouchEventType.None);
        }

        public static void TurnFlag(this ref TouchEventRoot.TouchEventType p_TargetMask, TouchEventRoot.TouchEventType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this TouchEventRoot.TouchEventType p_TargetMask, TouchEventRoot.TouchEventType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != TouchEventRoot.TouchEventType.None;
        }
#endif

        #endregion
    }
}