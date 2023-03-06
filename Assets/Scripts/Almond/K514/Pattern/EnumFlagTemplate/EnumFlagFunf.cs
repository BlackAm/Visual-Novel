namespace k514
{
    public static partial class EnumFlag
    {
        #region <IteratorProgressFlag>
        
        public static void AddFlag(this ref SceneDataTool.SceneVariablePropertyType p_TargetMask, SceneDataTool.SceneVariablePropertyType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref SceneDataTool.SceneVariablePropertyType p_TargetMask, SceneDataTool.SceneVariablePropertyType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref SceneDataTool.SceneVariablePropertyType p_TargetMask)
        {
            p_TargetMask.TurnFlag(SceneDataTool.SceneVariablePropertyType.None);
        }

        public static void TurnFlag(this ref SceneDataTool.SceneVariablePropertyType p_TargetMask, SceneDataTool.SceneVariablePropertyType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this SceneDataTool.SceneVariablePropertyType p_TargetMask, SceneDataTool.SceneVariablePropertyType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != SceneDataTool.SceneVariablePropertyType.None;
        }

        #endregion

    }
}