using UnityEngine;

namespace k514
{
    public abstract class AnimatableBase : UnitModuleBase, IAnimatable
    {
        ~AnimatableBase()
        {
            Dispose();
        }
        
        #region <Fields>

        public UnitAnimationDataRoot.AnimatableType _AnimatableType { get; private set; }
        public IAnimatableTableRecordBridge _AnimatonRecord { get; private set; }
        public TransformTool.AffineCachePreset CachedMasterNodeUV { get; protected set; }

        #endregion

        #region <Callbacks>

        public virtual IAnimatable OnInitializeAnimation(UnitAnimationDataRoot.AnimatableType p_AnimatableType, Unit p_MasterNode, IAnimatableTableRecordBridge p_AnimationPreset)
        {
            UnitModuleType = UnitModuleDataTool.UnitModuleType.Animation;
            _AnimatableType = p_AnimatableType;
            _MasterNode = p_MasterNode;
            _AnimatonRecord = p_AnimationPreset;
            
            return this;
        }
        
        protected override void OnModuleNotify()
        {
            ClearAnimationSpeed();
            SetAnimationEnable(true);
        }

        public virtual void TryHitMotion()
        {
            _MasterNode.OnHitActionStart();
        }

        public virtual void TryHitMotionBreak()
        {
            _MasterNode.OnHitActionTerminate();
        }
        
        public override void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
        }

        public override void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition)
        {
        }
        
        public abstract bool OnCheckAnimationTransition(UnitActionTool.UnitAction.MotionSequence p_MotionSequence);
        public abstract void OnAnimationStart();
        public abstract void OnAnimatorMove();
        public abstract void OnAnimationEnd();
        
        #endregion
       
        #region <Methods>
        
        public abstract void CacheMasterAffine();
        public abstract void SetAnimationEnable(bool p_Flag);
        public abstract void SetAnimationSpeedFactor(float p_Factor);
        public abstract void SetMotionSpeedFactor(float p_Factor);
        public abstract void ClearAnimationSpeed();
        public abstract void SetAnimationFloat(string p_Name, float p_Float);
        public abstract void SetPlayDefaultMotion(bool p_RestrictFlag);
        public abstract bool IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType p_Type);
        public abstract bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type,
            AnimatorParamStorage.MotionTransitionType p_TransitionFlag);
        public abstract bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type, int p_Index,
            AnimatorParamStorage.MotionTransitionType p_TransitionFlag);

        #endregion
    }
}