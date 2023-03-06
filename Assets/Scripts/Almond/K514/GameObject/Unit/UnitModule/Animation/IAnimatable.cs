using UnityEngine;

namespace k514
{
    /// <summary>
    /// 애니메이터 및 애니메이션 컨트롤러나 아핀변환 등을 통해 동작하는 게임 오브젝트의 기능을 제공하는 인터페이스
    /// </summary>
    public interface IAnimatable : IIncarnateUnit
    {
        UnitAnimationDataRoot.AnimatableType _AnimatableType { get; }
        IAnimatableTableRecordBridge _AnimatonRecord { get; }
        
        /// <summary>
        /// 애니메이션 모션 진행 중에 다른 모션으로 진행되는 경우, 진행하고 있었던 모션의 회전도를 기준으로 모션이 이어서
        /// 진행하게 되는데, 처음 한 방향을 유지하면서 모션을 실행하기 위해, 초기 회전도 값을 캐싱해놓는 벡터
        /// </summary>
        TransformTool.AffineCachePreset CachedMasterNodeUV { get; }
        IAnimatable OnInitializeAnimation(UnitAnimationDataRoot.AnimatableType p_AnimatableType, Unit p_MasterNode, IAnimatableTableRecordBridge p_AnimationPreset);
        void TryHitMotion();
        void TryHitMotionBreak();
        void SetAnimationEnable(bool p_Flag);
        void SetAnimationFloat(string p_Name, float p_Float);
        void SetAnimationSpeedFactor(float p_Factor);
        void SetMotionSpeedFactor(float p_Factor);
        void ClearAnimationSpeed();
        void SetPlayDefaultMotion(bool p_RestrictFlag);
        bool IsCurrentMotionOnAnimator(AnimatorParamStorage.MotionType p_Type);
        bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type,
            AnimatorParamStorage.MotionTransitionType p_TransitionFlag);
        bool SwitchMotionState(AnimatorParamStorage.MotionType p_Type, int p_Index,
            AnimatorParamStorage.MotionTransitionType p_TransitionFlag);
        void CacheMasterAffine();
        
        bool OnCheckAnimationTransition(UnitActionTool.UnitAction.MotionSequence p_MotionSequence);
        void OnAnimationStart();
        void OnAnimationEnd();
        void OnAnimatorMove();
    }
    
    /// <summary>
    /// 애니메이션 모션의 시작 및 끝 이벤트에 대응하는 인터페이스
    /// </summary>
    public interface IAnimationClipEventReceivable
    {
        void OnAnimationStart(float p_Duration);
        void OnAnimationCue(float p_Duration);
        void OnAnimationMotionStop(float p_Duration);
        void OnAnimationEnd(float p_Duration);
        void OnAnimatorMove();
    }
}