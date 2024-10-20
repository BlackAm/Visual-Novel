namespace BlackAm
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