using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class CharacterImage : AbstractUI
    {
        #region <Fields>

        private (ProgressTimer t_FadeIn, ProgressTimer t_FadeOut) _LerpTimer;
        private CharacterImageFadePhase _CurrentPhase;
        protected Image _MainImage;
        private (AssetPreset, Sprite) _SpritePreset;
        private bool _IsVisible;
        
        public int imageIndex;
        public Vector3 imagePosition;

        #endregion

        #region <Enums>

        public enum CharacterImageFadePhase
        {
            None,
            FadeIn,
            FadeOut,
        }

        #endregion

        #region <Callbacks>

        public override void InitializeAffine(float p_Scale)
        {
            base.InitializeAffine(p_Scale);
            
            
        }

        public override void Initialize()
        {
            _MainImage = GetComponent<Image>();
        }

        public override void OnPooling()
        {
            _CurrentPhase = CharacterImageFadePhase.None;
            _IsVisible = true;
            
            Set_UI_Hide(false);
            SetImageLinearClear(1f);
            
            imageIndex = 0;
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
        }
        
        public override void OnUpdateUI(float p_DeltaTime)
        {
            if (_HideFlag)
            {
            }
            else
            {
                switch (_CurrentPhase)
                {
                    case CharacterImageFadePhase.None:
                        break;
                    case CharacterImageFadePhase.FadeIn:
                        if (_LerpTimer.t_FadeIn.IsOver())
                        {
                            _CurrentPhase = CharacterImageFadePhase.None;
                            SetImageLinearBlack(1f);
                        }
                        else
                        {
                            _LerpTimer.t_FadeIn.Progress(p_DeltaTime);
                            SetImageLinearBlack(_LerpTimer.t_FadeIn.ProgressRate);
                        }
                        break;
                    case CharacterImageFadePhase.FadeOut:
                        if (_LerpTimer.t_FadeOut.IsOver())
                        {
                            _CurrentPhase = CharacterImageFadePhase.None;
                        }
                        else
                        {
                            _LerpTimer.t_FadeOut.Progress(p_DeltaTime); 
                            SetImageLinearClear(_LerpTimer.t_FadeOut.ProgressRate);
                        }
                        break;
                }
            }
        }

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 해당 UI의 페이드 인/아웃 시간을 정하는 메서드
        /// </summary>
        public void SetFadeDuration((float, float) p_FadeTuple)
        {
            _LerpTimer = (ProgressTimer.GetProgressTimer(p_FadeTuple.Item1), ProgressTimer.GetProgressTimer(p_FadeTuple.Item2));
        }

        public void SetImage(int p_ImageKey, bool p_IsActionFade)
        {
            SetSprite(p_ImageKey);
            if (p_IsActionFade && DialogueGameManager.GetInstance._PlayModeObject._PlayMode == DialogueGameManager.DialoguePlayMode.BasicPlay)
            {
                SetImageLinearBlack(0f);
                TriggerFadeIn();
            }
            else
            {
                SetImageLinearBlack(1f);
            }
        }

        public void SetImagePosition(Vector3 p_Position)
        {
            _RectTransform.SetScreenPos(p_Position);

        }
        
        public void TriggerFadeIn()
        {
            if (_IsVisible)
            {
                _LerpTimer.t_FadeIn.Reset();
                _CurrentPhase = CharacterImageFadePhase.FadeIn;
            }
        }
        
        public void TriggerFadeOut()
        {
            if (_IsVisible)
            {
                _LerpTimer.t_FadeOut.Reset(); 
                _CurrentPhase = CharacterImageFadePhase.FadeOut;
            }
            else
            {
                RetrieveObject();
            }
        }

        public void ApplyVisible()
        {
            Set_UI_Hide(!_IsVisible);
        }
        
        protected void SetImageLinearBlack(float p_ProgressRate01)
        {
            _MainImage.SetImageAlpha(p_ProgressRate01);
        }
        
        protected void SetImageLinearClear(float p_ProgressRate01)
        {
            SetImageLinearBlack(1f - p_ProgressRate01);
        }
        
        private void CheckSpritePresetValidation()
        {
            if (_SpritePreset.Item1.IsValid)
            {
                LoadAssetManager.GetInstanceUnSafe.UnloadAsset(_SpritePreset.Item1);
                _SpritePreset = default;
                _MainImage.sprite = null;
            }
        }

        public void SetSprite(int p_Index)
        {
            CheckSpritePresetValidation();

            _SpritePreset =
                ImageNameTableData.GetInstanceUnSafe.GetResource(p_Index, ResourceType.Image, ResourceLifeCycleType.Free_Condition);
            
            _MainImage.sprite = _SpritePreset.Item2;
            imageIndex = p_Index;
        }

        public (Character, CharacterImage) SetCharacterImage(int p_Key)
        {
            var changeCharacterImage = ChangeCharacterImagePresetData.GetInstanceUnSafe[p_Key];

            return SetCharacterImage(changeCharacterImage.Character, changeCharacterImage.ImagePosition,
                changeCharacterImage.ImageKey, changeCharacterImage.ImageScale, changeCharacterImage.ActionFade,
                changeCharacterImage.FadeTuple);
        }

        public (Character, CharacterImage) SetCharacterImage(Character p_Character, Vector3 p_Position, int p_ImageKey, float p_Scale, bool p_IsActionFade, (float, float) p_FadeTuple)
        {
            SetActive(true);
            InitializeAffine(p_Scale);
            SetImagePosition(p_Position);
            SetFadeDuration(p_FadeTuple);
            SetImage(p_ImageKey, p_IsActionFade);

            DialogueGameManager.GetInstance.currentDialogueEventData.CharacterImageSave[p_Character] =
                new SaveLoadManager.CharacterImageSaveData(p_Position, p_ImageKey, p_Scale, p_FadeTuple);
            
            return (p_Character, this);
        }
        
        

        #endregion
    }
}