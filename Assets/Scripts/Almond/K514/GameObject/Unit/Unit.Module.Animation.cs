namespace k514
{
    public partial class Unit : IAnimationClipEventReceivable
    {
        #region <Fields>

        /// <summary>
        /// 현재 선택된 해당 유닛의 모션 제어 모듈
        /// </summary>
        public IAnimatable _AnimationObject;

        /// <summary>
        /// Animation 모듈
        /// </summary>
        private UnitModuleCluster<UnitAnimationDataRoot.AnimatableType, IAnimatable> _AnimationModule;

        #endregion
        
        #region <Callbacks>

        protected void OnAwakeAnimation()
        {
            _AnimationModule 
                = new UnitModuleCluster<UnitAnimationDataRoot.AnimatableType, IAnimatable>(
                    this, UnitModuleDataTool.UnitModuleType.Animation, _PrefabExtraDataRecord.AnimationPresetIdList);
            _AnimationObject = (IAnimatable) _AnimationModule.CurrentSelectedModule;
        }
        
        private void OnPoolingAnimation()
        {
            _AnimationObject = _AnimationModule.SwitchModule();
        }

        private void OnRetrieveAnimation()
        {
            // 애니메이션 오브젝트는 내부에서 애니메이션 클립 에셋을 가지기 때문에
            // Dispose되어서 null이 될 수 있으므로 null 체크를 해준다.
            _AnimationObject?.OnMasterNodeRetrieved();
        }

        #endregion

        #region <Methods>

        protected void SwitchAnimation()
        {
            _AnimationObject = _AnimationModule.SwitchModule();
        }
        
        protected void SwitchAnimation(UnitAnimationDataRoot.AnimatableType p_ModuleType)
        {
            _AnimationObject = _AnimationModule.SwitchModule(p_ModuleType);
        }
        
        protected void SwitchAnimation(int p_Index)
        {
            _AnimationObject = _AnimationModule.SwitchModule(p_Index);
        }

        private void DisposeAnimation()
        {
            if (_AnimationModule != null)
            {
                _AnimationModule.Dispose();
                _AnimationModule = null;
            }

            _AnimationObject = null;
        }

        #endregion
        
        #region <IAnimationClipEventReceivable>
        
        public void OnAnimationStart(float p_Duration)
        {
            _ActableObject.OnAnimationStart(p_Duration);
        }

        public void OnAnimationCue(float p_Duration)
        {
            _ActableObject.OnAnimationCue(p_Duration);
        }

        public void OnAnimationMotionStop(float p_Duration)
        {
            _ActableObject.OnAnimationMotionStop(p_Duration);
        }

        public void OnAnimationEnd(float p_Duration)
        {
            _ActableObject.OnAnimationEnd(p_Duration);
        }

        public void OnAnimatorMove()
        {
            _ActableObject.OnAnimatorMove();
        }

        #endregion
    }
}