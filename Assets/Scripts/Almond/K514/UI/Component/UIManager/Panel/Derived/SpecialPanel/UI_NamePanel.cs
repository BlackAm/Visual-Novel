#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public class UI_NamePanel : UI_TextPanel
    {
        #region <Fields>

        private (Unit t_TracingTarget, Vector3 t_TracingOffset) _TracePreset;
        private (float t_FadeInDuration, float t_FadeOutDuration) _DurationTuple;
        private (ProgressTimer t_FadeIn, ProgressTimer t_FadeOut) _LerpTimer;
        private UI_NameLabel_Phase _CurrentPhase;
        private UnitEventReceiver _UnitEventReceiver;
        private Image _Mark;
        private bool _FixedVisibleFlag;
        private bool _IsVisible;
        
        #endregion

        #region <Enums>

        private enum UI_NameLabel_Phase
        {
            None,
            FadeIn,
            FadeOut,
        }

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            _Mark = _Transform.FindRecursive<Image>("Mark").Item2;
            _UnitEventReceiver = new UnitEventReceiver(UnitEventHandlerTool.UnitEventType.UINameLabelEventFlag, OnUnitEventTriggered);
            OnPanelHasNoMarkImage();
        }

        public override void OnPooling()
        {
            base.OnPooling();
            
            _CurrentPhase = UI_NameLabel_Phase.None;
            _IsVisible = true;
            _FixedVisibleFlag = false;
            _TracePreset = default;

            Set_UI_Hide(true);
            SetAllTextLinearClear(1f);
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            
            _UnitEventReceiver.ClearSenderGroup();
        }

        public override void OnEnableUI()
        {
            base.OnEnableUI();
            
            TrySyncPosition();
        }
        
        public override void OnUpdateUI(float p_DeltaTime)
        {
            base.OnUpdateUI(p_DeltaTime);

            if (_HideFlag)
            {
            }
            else
            {
                switch (_CurrentPhase)
                {
                    case UI_NameLabel_Phase.None:
                        break;
                    case UI_NameLabel_Phase.FadeIn:
                        if (_LerpTimer.t_FadeIn.IsOver())
                        {
                            _CurrentPhase = UI_NameLabel_Phase.None;
                            SetAllTextLinearBlack(1f);
                        }
                        else
                        {
                            _LerpTimer.t_FadeIn.Progress(p_DeltaTime);
                            SetAllTextLinearBlack(_LerpTimer.t_FadeIn.ProgressRate);
                        }
                        break;
                    case UI_NameLabel_Phase.FadeOut:
                        if (_LerpTimer.t_FadeOut.IsOver())
                        {
                            _CurrentPhase = UI_NameLabel_Phase.None;
                            RetrieveObject();
                        }
                        else
                        {
                            _LerpTimer.t_FadeOut.Progress(p_DeltaTime); 
                            SetAllTextLinearClear(_LerpTimer.t_FadeOut.ProgressRate);
                        }
                        break;
                }
            }
        }

        private void OnUnitEventTriggered(UnitEventHandlerTool.UnitEventType p_EventType, UnitEventMessage p_UnitEventMessage)
        {
            // 해당 UI가 추적 대상을 가지는 경우에
            if (_TracePreset.t_TracingTarget.IsValid())
            {
                // 해당 UI가 보여야하는 경우, 위치를 추적 대상에 동기화시키는 로컬 플래그
                var syncPosWhenVisibleFlag = false;
                // 해당 UI가 보여야하는 경우, FadeOut 연출을 동작시키는 로컬 플래그
                var fadeOutWhenVisibleFlag = false;
                // 해당 UI가 보여야하는 경우, 텍스트 값을 추적 대상의 이름과 동기화시키는 로컬 플래그
                var syncNameFlag = false;
                // 해당 UI를 보이게 만드는 로컬 플래그
                var updateVisibleFlag = false;
                // 이전 플래그 캐시
                var prevFixedFlag = _FixedVisibleFlag;
                var prevHideFlag = _HideFlag;
                
                switch (p_EventType)
                {
                    case UnitEventHandlerTool.UnitEventType.PositionChanged:
                    case UnitEventHandlerTool.UnitEventType.CameraMovedWhenThisUnitRendering:
                        syncPosWhenVisibleFlag = true;
                        break;
                    case UnitEventHandlerTool.UnitEventType.UnitDead:
                        fadeOutWhenVisibleFlag = true;
                        break;
                    case UnitEventHandlerTool.UnitEventType.UnitNameChanged:
                        syncNameFlag = true;
                        break;
                    
                    case UnitEventHandlerTool.UnitEventType.OutOfScreen:
                        updateVisibleFlag = true;
                        _IsVisible = !p_UnitEventMessage.BoolValue;
                        break;
                    case UnitEventHandlerTool.UnitEventType.DistanceCulling:
                        updateVisibleFlag = true;
                        _IsVisible = !p_UnitEventMessage.BoolValue;
                        break;
                    
                    case UnitEventHandlerTool.UnitEventType.UnitDisabled:
                        updateVisibleFlag = true;
                        if (p_UnitEventMessage.BoolValue)
                        {
                            // UnitDisabled 이벤트가 발생(true)했으므로, Visible은 그 반대값 false가 된다.
                            _IsVisible = false;
                        }
                        else
                        {
                            _IsVisible = CameraManager.GetInstanceUnSafe.IsCameraSeenObject(_TracePreset.t_TracingTarget, true);
                        }
                        break;
                    case UnitEventHandlerTool.UnitEventType.SwitchHideUINameLabel:
                        updateVisibleFlag = true;
                        _FixedVisibleFlag = p_UnitEventMessage.BoolValue;
                        if (p_UnitEventMessage.BoolValue2)
                        {
                            _IsVisible = false;
                        }
                        else
                        {
                            _IsVisible = CameraManager.GetInstanceUnSafe.IsCameraSeenObject(_TracePreset.t_TracingTarget, true);
                        }
                        break;
                    case UnitEventHandlerTool.UnitEventType.SwitchColorUINameLabel:
                        SetTextColor(p_UnitEventMessage.ColorValue);
                        break;
                    case UnitEventHandlerTool.UnitEventType.UnitRetrieved:
                        RetrieveObject();
                        return;
                }

                /*if(_IsVisible && _TracePreset.t_TracingTarget.GetUnitHideRender())
                {
                    updateVisibleFlag = true;
                    _IsVisible = false;
                }*/
                
                // 선정된 플래그를 바탕으로 해당 UI를 제어한다.
                /* UI 가시화 처리 */
                if (updateVisibleFlag)
                {
                    // 고정 플래그가 false => true로 전이한 경우, 확정적으로 Visible상태가 업데이트 된다.
                    var fixedFlagSetted = prevFixedFlag != _FixedVisibleFlag && _FixedVisibleFlag;
                    UpdateVisible(fixedFlagSetted);
                }
                
                /* UI 동기화 처리 */
                // HideFlag는 FixedVisibleFlag에 의해 VisibleFlag와 항상 동기화 되어있지 않음.
                // 예를 들어, VisibleFlag가 Set되어도 FixedVisibleFlag가 먼저 Set되어 있었다면, 해당 이벤트가 HideFlag에 전달되지 않음.
                var hideFlagTransitioned = prevHideFlag != _HideFlag;
                if (_HideFlag)
                {
                    // 현재 UI가 비활성 상태여도 이름 정보는 동기화 시켜준다.
                    if (syncNameFlag)
                    {
                        TrySyncName();
                    }
                }
                else
                {
                    // Hide 상태에서 Visible 상태로 최초 전이한 경우에 로컬 플래그가 적용된다.
                    if (!hideFlagTransitioned && syncPosWhenVisibleFlag)
                    {
                        TrySyncPosition();
                    }

                    if (fadeOutWhenVisibleFlag)
                    {
                        TriggerFadeOut();
                    }
                
                    if (syncNameFlag)
                    {
                        TrySyncName();
                    }
                }
            }
            // 해당 UI가 추적 대상을 가지는 않는 경우에
            else
            {
                switch (p_EventType)
                {
                    case UnitEventHandlerTool.UnitEventType.UnitRetrieved:
                        RetrieveObject();
                        return;
                }
            }
        }

        private void OnPanelHasNoMarkImage()
        {
            _Mark.sprite = null;
        }

        #endregion
        
        #region <Methods>

        private void UpdateVisible(bool p_RestrictFlag)
        {
            if (!_FixedVisibleFlag || p_RestrictFlag)
            {
                Set_UI_Hide(!_IsVisible);
            }
        }

        public void SetPanelMark(int p_ImageIndex)
        {
            var sprite =
                ImageNameTableData.GetInstanceUnSafe.GetResource(p_ImageIndex, ResourceType.Image,
                    ResourceLifeCycleType.Scene).Item2;

            if (!ReferenceEquals(null, sprite))
            {
                _Mark.sprite = sprite;
            }
            else
            {
                OnPanelHasNoMarkImage();
            }

            UpdateVisible(false);
        }

        public void TrySyncPosition()
        {
            var tryTraceTarget = _TracePreset.t_TracingTarget;
            if (tryTraceTarget.IsValid())
            {
                // 구간[Para : 0f, Perp : 1f]
                var cameraRotateDotValue = CameraManager.GetInstanceUnSafe._TraceUp_CameraLook_DotValue_Abs;
                // 구간[Near : 1f, Far : 0f]
                var cameraZoomRate = 2f - CameraManager.GetInstanceUnSafe._CurrentZoomRate;
                var screenOffsetRate = 1f + cameraRotateDotValue * cameraZoomRate;
                
                _RectTransform.SetScreenPos(tryTraceTarget.GetTopPosition() + _TracePreset.t_TracingOffset);
                _RectTransform.SetAddScreenPos(50f * screenOffsetRate * Vector3.up);
            }
        }

        private void TrySyncName()
        {
            SetTextContent(_TracePreset.t_TracingTarget.GetUnitName());
        }

        /// <summary>
        /// 해당 UI의 페이드 인/아웃 시간을 정하는 메서드
        /// </summary>
        public void SetFadeDuration(float p_FadeInDuration, float p_FadeOutDuration)
        {
            _LerpTimer = (ProgressTimer.GetProgressTimer(p_FadeInDuration), ProgressTimer.GetProgressTimer(p_FadeOutDuration));
        }

        /// <summary>
        /// 추적할 아핀 객체 및 오프셋을 설정하는 메서드
        /// </summary>
        public void SetTracingTarget(Unit p_Target, Vector3 p_TracingOffset)
        {
            _TracePreset = (p_Target, p_TracingOffset);
            p_Target.AddEventReceiver(_UnitEventReceiver);

            TrySyncName();
            TrySyncPosition();
            TriggerFadeIn();
        }
        
        /// <summary>
        /// 페이드 인 트리거 메서드
        /// </summary>
        public void TriggerFadeIn()
        {
            if (_IsVisible)
            {
                _LerpTimer.t_FadeIn.Reset();
                _CurrentPhase = UI_NameLabel_Phase.FadeIn;
            }
        }

        /// <summary>
        /// 페이드 아웃 트리거 메서드
        /// </summary>
        public void TriggerFadeOut()
        {
            if (_IsVisible)
            {
                _LerpTimer.t_FadeOut.Reset(); 
                _CurrentPhase = UI_NameLabel_Phase.FadeOut;
            }
            else
            {
                RetrieveObject();
            }
        }
        public override void SetTextColor(Color p_Color)
        {
            _MainText.color = p_Color;
        }
        #endregion
    }
}
#endif