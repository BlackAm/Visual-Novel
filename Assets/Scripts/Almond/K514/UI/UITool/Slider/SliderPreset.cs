#if !SERVER_DRIVE
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public struct SliderPreset : _IDisposable
    {
        #region <Fields>

        private Slider _MainSlider;
        private TextMeshProUGUI _MainLabel;
        private Image _BackGround;
        private Image _HandleImage;
        private SpriteAnimationPreset _BackGroundSprite;
        private SpriteAnimationPreset _HandleSprite;
        public bool IsValid;
            
        #endregion

        #region <Constructors>

        public SliderPreset(Transform p_Wrapper)
        {
            _MainSlider = p_Wrapper.GetComponentInChildren<Slider>();
            _MainLabel = p_Wrapper.GetComponentInChildren<TextMeshProUGUI>();

            if (ReferenceEquals(null, _MainSlider))
            {
                _BackGround = default;
                _HandleImage = default;
                _BackGroundSprite = default;
                _HandleSprite = default;
                IsValid = false;
            }
            else
            {
                _MainSlider.value = 0f;
                    
                var sliderTransform = _MainSlider.transform;
                _BackGround = sliderTransform.Find("BackGround").GetComponent<Image>();
                _HandleImage = sliderTransform.Find("Handle/Image").GetComponent<Image>();
                _BackGroundSprite = new SpriteAnimationPreset(_BackGround);
                _HandleSprite = new SpriteAnimationPreset(_HandleImage);
                _HandleImage.enabled = false;
                _MainSlider.interactable = false;
                    
                IsValid = true;
            }

            IsDisposed = false;
        }
            
        #endregion

        #region <Methods>

        public void SetProgress(float p_ProgressRage01)
        {
            if (!ReferenceEquals(null, _MainSlider))
            {
                _MainSlider.value = p_ProgressRage01;
            }
        }

        public void SetText(string p_Text)
        {
            if (!ReferenceEquals(null, _MainLabel))
            {
                _MainLabel.text = p_Text;
            }
        }
            
        #endregion
            
        #region <Disposable>
        
        /// <summary>
        /// dispose 패턴 onceFlag
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// dispose 플래그를 초기화 시키는 메서드
        /// </summary>
        public void Rejunvenate()
        {
            IsDisposed = false;
        }
            
        /// <summary>
        /// 인스턴스 파기 메서드
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            else
            {
                IsDisposed = true;
                DisposeUnManaged();
            }
        }

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        private void DisposeUnManaged()
        {
            _BackGroundSprite.Dispose();
            _BackGroundSprite = default;
                
            _HandleSprite.Dispose();
            _HandleSprite = default;
        }

        #endregion
    }
}
#endif