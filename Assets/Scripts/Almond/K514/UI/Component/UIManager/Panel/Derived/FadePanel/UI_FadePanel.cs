#if !SERVER_DRIVE
using System;
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public class UI_FadePanel : UI_PanelBase
    {
        #region <Consts>

        /// <summary>
        /// 기본 페이딩 지속시간
        /// </summary>
        private const float _DefaultFadeTime = 1f;
        
        /// <summary>
        /// 페이더 알파 1
        /// </summary>
        protected static readonly Color _FaderBlackBound = new Color(0f, 0f, 0f, 1f);
    
        /// <summary>
        /// 페이더 알파 0
        /// </summary>
        protected static readonly Color _FaderBlackBoundClear = new Color(0f, 0f, 0f, 0f);

        /// <summary>
        /// 페이트 애니메이션 예약시 지연딜레이 하한 값(초)
        /// </summary>
        private const float _FadeAnimation_PreDelay_LowerBound = 0.05f;
        
        #endregion
        
        #region <Fields>

        /// <summary>
        /// 페이더용 이미지
        /// </summary>
        protected Image _PanelFader;

        /// <summary>
        /// 페이더 애니메이션 타입
        /// </summary>
        protected FadeAnimationType _FadeAnimationType;

        /// <summary>
        /// 페이드 애니메이션 러프 타이머
        /// </summary>
        private ProgressTimer _FadeTimer;
        
        /// <summary>
        /// 선딜레이 타이머
        /// </summary>
        private ProgressTimer _PreDelayTimer;
        /// <summary>
        /// 현재 진행중인 애니메이션 페이즈
        /// </summary>
        private FadePhase _CurrentFadePhase;
        
        /// <summary>
        /// 선딜레이 이후 전이될 예약된 페이즈
        /// </summary>
        private FadePhase _ReservedPhase;
        
        /// <summary>
        /// 지정된 선딜레이
        /// </summary>
        private float _PreDelay;

        /// <summary>
        /// 페이드 종료시 수행할 이벤트
        /// </summary>
        private Action OnEntryOver, OnEscapeOver;
        
        #endregion
        
        #region <Enums>

        /// <summary>
        /// 진행할 페이드 애니메이션 타입
        /// </summary>
        public enum FadeAnimationType
        {
            /// <summary>
            /// 애니메이션 개시시, 종료시에 모두 검은 화면인 타입
            /// </summary>
            StaticBlack_StaticBlack,
            
            /// <summary>
            /// 애니메이션 개시시에는 검은 화면이고, 종료시에 페이드 인(알파) 애니메이션을 수행하는 타입
            /// </summary>
            StaticBlack_DynamicAlpha,
            
            /// <summary>
            /// 애니메이션 개시시에는 페이드 인(알파) 애니메이션을 수행하고, 종료시에 페이드 아웃(블랙) 애니메이션을 수행하는 타입
            /// </summary>
            DynamicAlpha_DynamicBlack,
            
            /// <summary>
            /// 애니메이션 개시시에는 페이드 아웃(블랙) 애니메이션을 수행하고, 종료시에 페이드 인(알파) 애니메이션을 수행하는 타입
            /// </summary>
            DynamicBlack_DynamicAlpha,
        }

        /// <summary>
        /// 애니메이션 진행 페이즈
        /// </summary>
        public enum FadePhase
        {
            /// <summary>
            /// 정지/초기화
            /// </summary>
            None,
            
            /// <summary>
            /// 선딜레이 대기
            /// </summary>
            PreDelay,
            
            /// <summary>
            /// 애니메이션 시작
            /// </summary>
            Entry,
            
            /// <summary>
            /// 애니메이션 종료
            /// </summary>
            Escape,
        }

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            _PanelFader = _Transform.Find("Fader").GetComponent<Image>();
            SetFadeAnimationType(FadeAnimationType.DynamicAlpha_DynamicBlack);
            SetFadeTime(_DefaultFadeTime);
            _OnSceneTransitionNoDisable = true;
        }

        public override void OnDisableUI()
        {
            base.OnDisableUI();
            
            _CurrentFadePhase = FadePhase.None;
            _ReservedPhase = FadePhase.None;
            _PreDelayTimer.Reset();
            _FadeTimer.Reset();
            OnEscapeOver = null;
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
            switch (_CurrentFadePhase)
            {
                case FadePhase.None:
                    break;
                case FadePhase.PreDelay:
                    if (_PreDelayTimer.IsOver())
                    {
                        _CurrentFadePhase = _ReservedPhase;
                    }
                    else
                    {
                        _PreDelayTimer.Progress(p_DeltaTime);
                    }
                    break;
                case FadePhase.Entry:
                    if (_FadeTimer.IsOver())
                    {
                        OnEntryAnimationTerminate();
                    }
                    else
                    {
                        _FadeTimer.Progress(p_DeltaTime);
                        OnEntryAnimationProgress(_FadeTimer.ProgressRate);
                    }
                    break;
                case FadePhase.Escape:
                    if (_FadeTimer.IsOver())
                    {
                        OnEscapeAnimationTerminate();
                    }
                    else
                    {
                        _FadeTimer.Progress(p_DeltaTime);
                        OnEscapeAnimationProgress(_FadeTimer.ProgressRate);
                    }
                    break;
            }
        }
        
        #endregion

        #region <Callbacks/FadeAnimation>

        /// <summary>
        /// Entry 연출을 시작하기 전에 수행하는 초기화 콜백
        /// </summary>
        public void OnEntryAnimationBegin()
        {
            Set_UI_Hide(false);
            _FadeTimer.Reset();
            
            switch (_FadeAnimationType)
            {
                case FadeAnimationType.DynamicBlack_DynamicAlpha:
                    SetFadeProgressClear(1f);
                    break;
                case FadeAnimationType.StaticBlack_StaticBlack:
                case FadeAnimationType.StaticBlack_DynamicAlpha:
                case FadeAnimationType.DynamicAlpha_DynamicBlack:
                    SetFadeProgressBlack(1f);
                    break;
            }
            _PanelFader.enabled = true;
        }
        
        /// <summary>
        /// Entry 연출때 수행할 애니메이션이 있다면 수행하는 콜백
        /// </summary>
        public void OnEntryAnimationProgress(float p_ProgressRate01)
        {
            switch (_FadeAnimationType)
            {
                case FadeAnimationType.StaticBlack_StaticBlack:
                case FadeAnimationType.StaticBlack_DynamicAlpha:
                    break;
                case FadeAnimationType.DynamicAlpha_DynamicBlack:
                    SetFadeProgressClear(p_ProgressRate01);
                    break;
                case FadeAnimationType.DynamicBlack_DynamicAlpha:
                    SetFadeProgressBlack(p_ProgressRate01);
                    break;
            }
        }
        
        /// <summary>
        /// Entry 연출 종료시 호출되는 콜백
        /// </summary>
        public void OnEntryAnimationTerminate()
        {
            _CurrentFadePhase = FadePhase.None;
                        
            if (ReferenceEquals(null, OnEntryOver))
            {
                Set_UI_Hide(true);
            }
            else
            {
                OnEntryOver.Invoke();
                OnEntryOver = null;
            }
            
            switch (_FadeAnimationType)
            {
                case FadeAnimationType.StaticBlack_StaticBlack:
                case FadeAnimationType.StaticBlack_DynamicAlpha:
                    break;
                case FadeAnimationType.DynamicAlpha_DynamicBlack:
                    SetFadeProgressClear(1f);
                    break;
                case FadeAnimationType.DynamicBlack_DynamicAlpha:
                    SetFadeProgressBlack(1f);
                    break;
            }
        }
        
        /// <summary>
        /// Escape 연출을 시작하기 전에 수행하는 초기화 콜백
        /// </summary>
        public void OnEscapeAnimationBegin()
        {
            Set_UI_Hide(false);
            _FadeTimer.Reset();

            switch (_FadeAnimationType)
            {
                case FadeAnimationType.StaticBlack_StaticBlack:
                case FadeAnimationType.StaticBlack_DynamicAlpha:
                case FadeAnimationType.DynamicBlack_DynamicAlpha:
                    SetFadeProgressBlack(1f);
                    break;
                case FadeAnimationType.DynamicAlpha_DynamicBlack:
                    SetFadeProgressClear(1f);
                    break;
            }
        }
        
        /// <summary>
        /// Escape 연출때 수행할 애니메이션이 있다면 수행하는 콜백
        /// </summary>
        public void OnEscapeAnimationProgress(float p_ProgressRate01)
        {
            switch (_FadeAnimationType)
            {
                case FadeAnimationType.StaticBlack_StaticBlack:
                    break;
                case FadeAnimationType.StaticBlack_DynamicAlpha:
                case FadeAnimationType.DynamicBlack_DynamicAlpha:
                    SetFadeProgressClear(p_ProgressRate01);
                    break;
                case FadeAnimationType.DynamicAlpha_DynamicBlack:
                    SetFadeProgressBlack(p_ProgressRate01);
                    break;
            }
        }

        /// <summary>
        /// Escape 연출 종료시 호출되는 콜백
        /// </summary>
        public void OnEscapeAnimationTerminate()
        {
            _CurrentFadePhase = FadePhase.None;

            if (ReferenceEquals(null, OnEscapeOver))
            {
                Set_UI_Hide(true);
            }
            else
            {
                OnEscapeOver.Invoke();
                OnEscapeOver = null;
            }
            
            switch (_FadeAnimationType)
            {
                case FadeAnimationType.StaticBlack_StaticBlack:
                    break;
                case FadeAnimationType.StaticBlack_DynamicAlpha:
                case FadeAnimationType.DynamicBlack_DynamicAlpha:
                    SetFadeProgressClear(1f);
                    break;
                case FadeAnimationType.DynamicAlpha_DynamicBlack:
                    SetFadeProgressBlack(1f);
                    break;
            }
        }
        
        #endregion
        
        #region <Methods>

        private void SetFadeProgressClear(float p_ProgressRate01)
        {
            _PanelFader.color = Color.Lerp(_FaderBlackBound, _FaderBlackBoundClear, p_ProgressRate01);
        }

        private void SetFadeProgressBlack(float p_ProgressRate01)
        {
            SetFadeProgressClear(1f - p_ProgressRate01);
        }
        
        public void SetFadeTime(float p_Duration)
        {
            _FadeTimer.Initialize(p_Duration);
        }

        public void SetFadeAnimationType(FadeAnimationType p_FadeAnimationType)
        {
            _FadeAnimationType = p_FadeAnimationType;
        }
        
        public void CastEntryAnimation(Action p_OnEntryOver, float p_PreDelay = 0f)
        {
            if (p_PreDelay < _FadeAnimation_PreDelay_LowerBound)
            {
                _PreDelayTimer.Initialize(p_PreDelay);
            
                _CurrentFadePhase = FadePhase.PreDelay;
                _ReservedPhase = FadePhase.Entry;
            }
            else
            {
                _CurrentFadePhase = FadePhase.Entry;
            }

            OnEntryOver = p_OnEntryOver;
            OnEntryAnimationBegin();
        }
        
        public void CastEscapeAnimation(Action p_OnEscapeOver, float p_PreDelay = 0f)
        {
            if (p_PreDelay < _FadeAnimation_PreDelay_LowerBound)
            {
                _PreDelayTimer.Initialize(p_PreDelay);
                
                _CurrentFadePhase = FadePhase.PreDelay;
                _ReservedPhase = FadePhase.Escape;
            }
            else
            {
                _CurrentFadePhase = FadePhase.Escape;
            }

            OnEscapeOver = p_OnEscapeOver;
            OnEscapeAnimationBegin();
        }
        
        #endregion
    }
}
#endif