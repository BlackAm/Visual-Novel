namespace k514
{
    public partial class EnumFlag
    {
        
        #region <SceneControlFlag>
        
        public static void AddFlag(this ref SceneControllerTool.SceneControlFlag p_TargetMask, SceneControllerTool.SceneControlFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref SceneControllerTool.SceneControlFlag p_TargetMask, SceneControllerTool.SceneControlFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref SceneControllerTool.SceneControlFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(SceneControllerTool.SceneControlFlag.None);
        }

        public static void TurnFlag(this ref SceneControllerTool.SceneControlFlag p_TargetMask, SceneControllerTool.SceneControlFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this SceneControllerTool.SceneControlFlag p_TargetMask, SceneControllerTool.SceneControlFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != SceneControllerTool.SceneControlFlag.None;
        }

        #endregion

        #region <ProjectileUnitCollisionEventFlag>
        
        public static void AddFlag(this ref ProjectileTool.ProjectileUnitCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileUnitCollisionEventFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ProjectileTool.ProjectileUnitCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileUnitCollisionEventFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ProjectileTool.ProjectileUnitCollisionEventFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ProjectileTool.ProjectileUnitCollisionEventFlag.None);
        }

        public static void TurnFlag(this ref ProjectileTool.ProjectileUnitCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileUnitCollisionEventFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ProjectileTool.ProjectileUnitCollisionEventFlag p_TargetMask, ProjectileTool.ProjectileUnitCollisionEventFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ProjectileTool.ProjectileUnitCollisionEventFlag.None;
        }

        #endregion

#if !SERVER_DRIVE
        #region <CameraViewControlEventType>

        public static void AddFlag(this ref CameraManager.CameraViewControlProgressType p_TargetMask, CameraManager.CameraViewControlProgressType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref CameraManager.CameraViewControlProgressType p_TargetMask, CameraManager.CameraViewControlProgressType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref CameraManager.CameraViewControlProgressType p_TargetMask)
        {
            p_TargetMask.TurnFlag(CameraManager.CameraViewControlProgressType.None);
        }

        public static void TurnFlag(this ref CameraManager.CameraViewControlProgressType p_TargetMask, CameraManager.CameraViewControlProgressType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this CameraManager.CameraViewControlProgressType p_TargetMask, CameraManager.CameraViewControlProgressType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != CameraManager.CameraViewControlProgressType.None;
        }

        #endregion
#endif    
        
        #region <ControlMessageFlag>

        public static void AddFlag(this ref ControllerTool.ControlMessageFlag p_TargetMask, ControllerTool.ControlMessageFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref ControllerTool.ControlMessageFlag p_TargetMask, ControllerTool.ControlMessageFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref ControllerTool.ControlMessageFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(ControllerTool.ControlMessageFlag.None);
        }

        public static void TurnFlag(this ref ControllerTool.ControlMessageFlag p_TargetMask, ControllerTool.ControlMessageFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this ControllerTool.ControlMessageFlag p_TargetMask, ControllerTool.ControlMessageFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != ControllerTool.ControlMessageFlag.None;
        }

        #endregion
        
        #region <ActorModuleProgressFlag>

        public static void AddFlag(this ref VolitionalTool.ActorModuleProgressFlag p_TargetMask, VolitionalTool.ActorModuleProgressFlag p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref VolitionalTool.ActorModuleProgressFlag p_TargetMask, VolitionalTool.ActorModuleProgressFlag p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref VolitionalTool.ActorModuleProgressFlag p_TargetMask)
        {
            p_TargetMask.TurnFlag(VolitionalTool.ActorModuleProgressFlag.None);
        }

        public static void TurnFlag(this ref VolitionalTool.ActorModuleProgressFlag p_TargetMask, VolitionalTool.ActorModuleProgressFlag p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this VolitionalTool.ActorModuleProgressFlag p_TargetMask, VolitionalTool.ActorModuleProgressFlag p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != VolitionalTool.ActorModuleProgressFlag.None;
        }

        #endregion

        #region <MasterNodeRelateType>

        public static void AddFlag(this ref PrefabInstanceTool.MasterNodeRelateType p_TargetMask, PrefabInstanceTool.MasterNodeRelateType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref PrefabInstanceTool.MasterNodeRelateType p_TargetMask, PrefabInstanceTool.MasterNodeRelateType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref PrefabInstanceTool.MasterNodeRelateType p_TargetMask)
        {
            p_TargetMask.TurnFlag(PrefabInstanceTool.MasterNodeRelateType.None);
        }

        public static void TurnFlag(this ref PrefabInstanceTool.MasterNodeRelateType p_TargetMask, PrefabInstanceTool.MasterNodeRelateType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this PrefabInstanceTool.MasterNodeRelateType p_TargetMask, PrefabInstanceTool.MasterNodeRelateType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != PrefabInstanceTool.MasterNodeRelateType.None;
        }

        #endregion
        
        #region <FocusNodeRelateType>

        public static void AddFlag(this ref PrefabInstanceTool.FocusNodeRelateType p_TargetMask, PrefabInstanceTool.FocusNodeRelateType p_TryMask)
        {
            p_TargetMask |= p_TryMask;
        }

        public static void RemoveFlag(this ref PrefabInstanceTool.FocusNodeRelateType p_TargetMask, PrefabInstanceTool.FocusNodeRelateType p_TryMask)
        {
            p_TargetMask &= ~p_TryMask;
        }

        public static void ClearFlag(this ref PrefabInstanceTool.FocusNodeRelateType p_TargetMask)
        {
            p_TargetMask.TurnFlag(PrefabInstanceTool.FocusNodeRelateType.None);
        }

        public static void TurnFlag(this ref PrefabInstanceTool.FocusNodeRelateType p_TargetMask, PrefabInstanceTool.FocusNodeRelateType p_TryMask)
        {
            p_TargetMask = p_TryMask;
        }

        public static bool HasAnyFlagExceptNone(this PrefabInstanceTool.FocusNodeRelateType p_TargetMask, PrefabInstanceTool.FocusNodeRelateType p_CompareType)
        {
            return (p_TargetMask & p_CompareType) != PrefabInstanceTool.FocusNodeRelateType.None;
        }

        #endregion
    }
}