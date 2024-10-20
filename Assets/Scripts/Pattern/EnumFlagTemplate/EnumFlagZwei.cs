namespace BlackAm
{
    public partial class EnumFlag
    {
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

        #region <DialogueEndingFlag>

        public static void AddFlag(this ref DialogueGameManager.DialogueEndingFlag p_TargetMask, DialogueGameManager.DialogueEndingFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref DialogueGameManager.DialogueEndingFlag p_TargetMask, DialogueGameManager.DialogueEndingFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref DialogueGameManager.DialogueEndingFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(DialogueGameManager.DialogueEndingFlag.None);
        }

        public static void TurnFlag(this ref DialogueGameManager.DialogueEndingFlag p_TargetMask, DialogueGameManager.DialogueEndingFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this DialogueGameManager.DialogueEndingFlag p_TargetMask, DialogueGameManager.DialogueEndingFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != DialogueGameManager.DialogueEndingFlag.None;
        }

        #endregion
    }
}