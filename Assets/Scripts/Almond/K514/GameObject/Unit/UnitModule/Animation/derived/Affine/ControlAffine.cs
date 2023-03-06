using System;

namespace k514
{
    public class ControlAffine : AnimatableBase
    {
        #region <Fields>

        private AffineKinematicType _CurrentKinematic;
        private bool _isValid;
        
        #endregion

        #region <Enums>

        public enum AffineKinematicType
        {
            None,
            
            Linear,
            Bezier,
            Circle,
            Junk_Yard_Dog,
        }

        #endregion
        
        #region <Callbacks>

        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();
            
            _CurrentKinematic = AffineKinematicType.None;
        }

        public override bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type, int p_Index, AnimatorParamStorage.MotionTransitionType p_TransitionFlag)
        {
            throw new NotImplementedException();
        }

        public override void CacheMasterAffine()
        {
            throw new NotImplementedException();
        }

        public override bool OnCheckAnimationTransition(UnitActionTool.UnitAction.MotionSequence p_MotionSequence)
        {
            throw new NotImplementedException();
        }

        public override void OnAnimationStart()
        {
            throw new NotImplementedException();
        }

        public override void OnAnimationEnd()
        {
            throw new NotImplementedException();
        }

        public override void OnAnimatorMove()
        {
            throw new NotImplementedException();
        }

        protected override void OnModuleSleep()
        {
            throw new NotImplementedException();
        }

        public override void OnPreUpdate(float p_DeltaTime)
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate(float p_DeltaTime)
        {
            throw new NotImplementedException();
        }

        public override void OnUpdate_TimeBlock()
        {
            throw new NotImplementedException();
        }

        public override void OnStriked(Unit p_Trigger, HitResult p_HitResult)
        {
            throw new NotImplementedException();
        }

        public override void OnHitted(Unit p_Trigger, HitResult p_HitResult)
        {
            throw new NotImplementedException();
        }

        public override void OnUnitHitActionStarted()
        {
            throw new NotImplementedException();
        }

        public override void OnUnitHitActionTerminated()
        {
            throw new NotImplementedException();
        }

        public override void OnUnitActionStarted()
        {
            throw new NotImplementedException();
        }

        public override void OnUnitActionTerminated()
        {
            throw new NotImplementedException();
        }

        public override void OnUnitDead(bool p_Instant)
        {
            throw new NotImplementedException();
        }

        public override void OnJumpUp()
        {
            throw new NotImplementedException();
        }

        public override void OnReachedGround(UnitStampPreset p_UnitStampPreset)
        {
            throw new NotImplementedException();
        }

        public override void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset)
        {
            throw new NotImplementedException();
        }

        public override void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
            throw new NotImplementedException();
        }

        protected override void DisposeUnManaged()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region <Methods>
        
        public override void SetAnimationEnable(bool p_Flag)
        {
            _isValid = p_Flag;
        }

        public override void SetAnimationFloat(string p_Name, float p_Float)
        {
            throw new NotImplementedException();
        }

        public override void SetPlayDefaultMotion(bool p_RestrictFlag)
        {
            throw new NotImplementedException();
        }

        public override bool IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType p_Type)
        {
            throw new NotImplementedException();
        }

        public override bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type, AnimatorParamStorage.MotionTransitionType p_TransitionFlag)
        {
            throw new NotImplementedException();
        }

        public override void SetAnimationSpeedFactor(float p_Factor)
        {
            throw new NotImplementedException();
        }

        public override void SetMotionSpeedFactor(float p_Factor)
        {
            throw new NotImplementedException();
        }

        public override void ClearAnimationSpeed()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}