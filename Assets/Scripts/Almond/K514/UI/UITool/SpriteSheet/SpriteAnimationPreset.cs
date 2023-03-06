#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public struct SpriteAnimationPreset : _IDisposable
    {
        #region <Fields>

        public Image TargetImage;
        public SpriteIterator SpriteIterator { get; set; }

        #endregion

        #region <Constructor>

        public SpriteAnimationPreset(Image p_Image)
        {
            IsDisposed = false;
            TargetImage = p_Image;
            SpriteIterator = default;
        }
            
        public SpriteAnimationPreset(Image p_Image, UITool.AnimationSpriteType p_AnimationSpriteType)
        {
            IsDisposed = false;
            TargetImage = p_Image;
            SpriteIterator = new SpriteIterator(p_AnimationSpriteType);
        }
            
        public SpriteAnimationPreset(Image p_Image, List<Sprite> p_SpriteSet, float p_AnimationDuration, int p_LoopCount)
        {
            IsDisposed = false;
            TargetImage = p_Image;
            SpriteIterator = new SpriteIterator(p_SpriteSet, p_AnimationDuration, p_LoopCount);
        }

        #endregion

        #region <Callbacks>

        public void OnUpdateSpriteIterator(float p_DeltaTime)
        {
            switch (SpriteIterator.ProgressIterating(p_DeltaTime))
            {
                case TimerIteratorBase.IterateResultType.ProgressNext:
                case TimerIteratorBase.IterateResultType.ProgressOver:
                    SetImage();
                    break;
            }
        }

        #endregion
            
        #region <Methods>

        public void SetSpriteIterator(SpriteIterator p_SpriteIterator)
        {
            if (!ReferenceEquals(null, SpriteIterator))
            {
                SpriteIterator.Dispose();
                SpriteIterator = null;
            }

            SpriteIterator = p_SpriteIterator;
        }
            
        public void SetImage()
        {
            TargetImage.sprite = SpriteIterator.GetCurrentSprite();
        }

        public void ClearImage()
        {
            TargetImage.sprite = null;
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
            if (!ReferenceEquals(null, SpriteIterator))
            {
                SpriteIterator.Dispose();
                SpriteIterator = null;
            }
        }

        #endregion
    }
}
#endif