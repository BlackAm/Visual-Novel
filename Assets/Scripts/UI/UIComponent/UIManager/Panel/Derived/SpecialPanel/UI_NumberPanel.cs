#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class UI_NumberPanel : UI_PanelBase
    {
        #region <Consts>

        private const int NumberFontCount = 8;
        private static readonly int UpperBound = 10.PowInt(NumberFontCount) - 1;

        #endregion
        
        #region <Fields>

        private Image[] _ImageSet;
        private (Vector3 t_TracingStart, Vector3 t_TracingOffset) _TracePreset;
        private (bool t_MoveFlag, Vector3 t_MoveOffsetSpeed, Vector3 t_MoveOffset) _MovePreset;
        private (float t_FadeInDuration, float t_WholeDuration, float t_FadeOutDuration) _DurationTuple;
        private (ProgressTimer t_FadeIn, ProgressTimer t_Whole, ProgressTimer t_FadeOut) _LerpTimer;
        private UI_NumberPanel_Phase _CurrentPhase;
        public PanelType panelType;
        
        #endregion

        #region <Enums>

        private enum UI_NumberPanel_Phase
        {
            None,
            FadeIn,
            Display,
            FadeOut,
        }
        
        /// 사용 타입.
        public enum PanelType
        {
            Damage,
            Potion              
        }

        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            _ImageSet = new Image[NumberFontCount];
            
            var wrapper = transform.FindRecursive("Align").Item2;
            for (int i = 0; i < NumberFontCount; i++)
            {
                var childOne = new GameObject($"N{i}");
                childOne.transform.SetParent(wrapper, false);
                _ImageSet[i] = childOne.AddComponent<Image>();
            }

            base.OnSpawning();
        }

        public override void OnPooling()
        {
            base.OnPooling();
            
            _CurrentPhase = UI_NumberPanel_Phase.None;
            /*
            SetAllImageLinearClear(2f);
            _LerpTimer.t_FadeIn.Reset();
            _LerpTimer.t_Whole.Reset();
            */
            _TracePreset = default;
            _MovePreset = default;
        }

        /// momo6346 - UI 강조 효과 수정. 
        public override void OnUpdateUI(float p_DeltaTime)
        {
            switch (_CurrentPhase)
            {
                case UI_NumberPanel_Phase.None:
                    break;
                case UI_NumberPanel_Phase.FadeIn:
                    if (_LerpTimer.t_FadeIn.ElapsedTime > 0f)
                    {
                        if (!gameObject.activeSelf)
                        {
                            gameObject.SetActive(true);
                            _LerpTimer.t_FadeIn.Reset();
                            _LerpTimer.t_Whole.Reset();
                            return;
                        }
                    }
                    
                    if (_LerpTimer.t_FadeIn.IsOver())
                    {
                        /// momo6346 - _LerpTimer.t_FadeIn가 고르지 않아서 추가했습니다.
                        SetAllImageLerpScale(0.3f);
                      
                        _CurrentPhase++;
                        TrySyncPosition();
                    }
                    else
                    {
                        switch (panelType)
                        {
                            case PanelType.Damage:
                                OnUpdateMoveOffset(p_DeltaTime * 1.5f);
                                if (_LerpTimer.t_FadeIn.ProgressRate > 0f)
                                {
                                    //SetAllImageLerpScale(ObjectScale.CurrentValue - (_LerpTimer.t_FadeIn.ProgressRate * ObjectScale.CurrentValue), 0.125f);
                                    SetAllImageLerpScale(Mathf.Lerp(ObjectScale.CurrentValue, 0.3f, _LerpTimer.t_FadeIn.ProgressRate));   
                                }
                                _LerpTimer.t_FadeIn.Progress(p_DeltaTime * 1.5f);
                                _LerpTimer.t_Whole.Progress(p_DeltaTime * 1.5f);
                        
                                TrySyncPosition();
                                break;
                            case PanelType.Potion:
                                OnUpdateMoveOffset(p_DeltaTime);
                                SetAllImageLerpScale(ObjectScale.CurrentValue - (_LerpTimer.t_FadeIn.ProgressRate * ObjectScale.CurrentValue), 0.5f);
                                _LerpTimer.t_FadeIn.Progress(p_DeltaTime);
                                _LerpTimer.t_Whole.Progress(p_DeltaTime);
                        
                                TrySyncPosition();
                                break;
                        }
                    }
                    break;
                case UI_NumberPanel_Phase.Display:
                    OnUpdateMoveOffset(p_DeltaTime);
                    if (_LerpTimer.t_Whole.RemaindTime < _LerpTimer.t_FadeOut.WholeTime)
                    {
                        _CurrentPhase++;
                        TrySyncPosition();
                    }
                    else
                    {
                        OnUpdateMoveOffset(p_DeltaTime);
                        _LerpTimer.t_Whole.Progress(p_DeltaTime);
                        TrySyncPosition();
                    }

                    break;
                case UI_NumberPanel_Phase.FadeOut:
                    OnUpdateMoveOffset(p_DeltaTime);
                    if (_LerpTimer.t_FadeOut.IsOver())
                    {
                        _CurrentPhase = UI_NumberPanel_Phase.None;
                        /// 비활성
                        RetrieveObject();
                    }
                    else
                    {
                        _LerpTimer.t_FadeOut.Progress(p_DeltaTime);
                        //transform.position += Vector3.up;
                        _LerpTimer.t_Whole.Progress(p_DeltaTime);
                        TrySyncPosition();
                    }
                    break;
            }
        }

        private void OnUpdateMoveOffset(float p_DeltaTime)
        {
            if (_MovePreset.t_MoveFlag)
            {
                _MovePreset.t_MoveOffset += _MovePreset.t_MoveOffsetSpeed * p_DeltaTime;
            }
        }

        #endregion
        
        #region <Methods>

        private void TrySyncPosition()
        {
            // 구간[Paral : 0f, Perp : 1f]
            var cameraRotateDotValue = CameraManager.GetInstanceUnSafe._TraceUp_CameraLook_DotValue_Abs;
            // 구간[Near : 1f, Far : 0f]
            var cameraZoomRate = 2f - CameraManager.GetInstanceUnSafe._CurrentZoomRate;
            var screenOffsetRate = 1f + cameraRotateDotValue * cameraZoomRate;
            
            if (_MovePreset.t_MoveFlag && _CurrentPhase == UI_NumberPanel_Phase.FadeOut)
            {
                _RectTransform.SetAddScreenPos(screenOffsetRate * _MovePreset.t_MoveOffset);
            }
            else
            {
                _RectTransform.SetScreenPos(_TracePreset.t_TracingStart + _TracePreset.t_TracingOffset);
            }
        }

        public void SetFadeDuration(float p_FadeInDuration, float p_WholeDuration, float p_FadeOutDuration)
        {
            _LerpTimer = (
                ProgressTimer.GetProgressTimer(p_FadeInDuration),
                ProgressTimer.GetProgressTimer(p_WholeDuration), 
                ProgressTimer.GetProgressTimer(p_FadeOutDuration)
            );
        }
        
        /// <summary>
        /// 페이드 인 트리거 메서드
        /// </summary>
        public void TriggerFadeIn()
        {
            _LerpTimer.t_FadeIn.Reset();
            _CurrentPhase = UI_NumberPanel_Phase.FadeIn;
            SetAllImageLerpScale(ObjectScale.CurrentValue);
        }

        /// <summary>
        /// 페이드 아웃 트리거 메서드
        /// </summary>
        public void TriggerFadeOut()
        {
            _LerpTimer.t_FadeOut.Reset(); 
            _CurrentPhase = UI_NumberPanel_Phase.FadeOut;
        }
        
        public void SetTracingTarget(Transform p_Target, Vector3 p_TracingOffset)
        {
            _TracePreset = (p_Target.position, p_TracingOffset);
        }
        
        public void SetTracingTarget(Transform p_Target, Vector3 p_TracingOffset, Vector3 p_MoveOffset)
        {
            _TracePreset = (p_Target.position, p_TracingOffset);
            _MovePreset = (true, p_MoveOffset, Vector3.zero);
        }

        public void SetDisableDamageSprite()
        {
            for (int i = 0; i < NumberFontCount; i++)
            {
                _ImageSet[i].gameObject.SetActiveSafe(false);
            } 
        }

        public void SetDamageSprite(float p_Value)
        {
            var intVal = (int)Mathf.Abs(p_Value);
            if (intVal > UpperBound)
            {
                for (int i = 0; i < NumberFontCount; i++)
                {
                    _ImageSet[i].gameObject.SetActiveSafe(true);
                    _ImageSet[i].sprite = SpriteSheetManager.GetInstanceUnSafe.GetNumberSprite(9);
                } 
            }
            else if(intVal < 1)
            {
                _ImageSet[0].gameObject.SetActiveSafe(true);
                _ImageSet[0].sprite = SpriteSheetManager.GetInstanceUnSafe.GetNumberSprite(0);
                
                for (int i = 1; i < NumberFontCount; i++)
                {
                    _ImageSet[i].gameObject.SetActiveSafe(false);
                } 
            }
            else
            {
                var prevPow = 1;
                for (int i = 0; i < NumberFontCount; i++)
                {
                    if (intVal > 0)
                    {
                        var baseValue = 10.PowInt(i + 1);
                        var remaind = intVal % baseValue;
                        if (remaind > 0)
                        {
                            intVal -= remaind;
                            _ImageSet[i].gameObject.SetActiveSafe(true);
                            if (prevPow > 1)
                            {
                                remaind /= prevPow;
                            }
                            _ImageSet[i].sprite = SpriteSheetManager.GetInstanceUnSafe.GetNumberSprite(remaind);
                        }
                        else
                        {
                            _ImageSet[i].gameObject.SetActiveSafe(true);
                            _ImageSet[i].sprite = SpriteSheetManager.GetInstanceUnSafe.GetNumberSprite(0);
                        }

                        prevPow = baseValue;
                    }
                    else
                    {
                        _ImageSet[i].gameObject.SetActiveSafe(false);
                    }
                }
            }
        }

        #endregion
    }
}
#endif