namespace k514
{
    public class AnimationProjector : SimpleProjector
    {
        #region <Fields>
        
#if !SERVER_DRIVE
        private SpriteIterator _SpriteIterator;
#endif

        #endregion

        #region <Callbacks>
        
#if !SERVER_DRIVE
        public override void OnPooling()
        {
            base.OnPooling();
            _SpriteIterator.Reset();
        }

        protected override void OnLoadFirstImage()
        {
            var projectorExtraData = (PrefabExtraData_AnimationProjector.PrefabExtraDataAnimationProjectorRecord) _PrefabKey._PrefabExtraPreset._PrefabExtraDataRecord;
            _SpriteIterator = new SpriteIterator(projectorExtraData.AnimationSpriteType);
            SetTexture(_SpriteIterator.GetCurrentSprite().texture);
        }

        protected override void OnFadeIn(float p_Dt)
        {
            base.OnFadeIn(p_Dt);
            OnUpdateSpriteIterator(p_Dt);
        }

        protected override void OnProjection(float p_Dt)
        {
            base.OnProjection(p_Dt);
            OnUpdateSpriteIterator(p_Dt);
        }

        protected override void OnFadeOut(float p_Dt)
        {
            base.OnFadeOut(p_Dt);
            OnUpdateSpriteIterator(p_Dt);
        }

        private void OnUpdateSpriteIterator(float p_Dt)
        {
            switch (_SpriteIterator.ProgressIterating(p_Dt))
            {
                case TimerIteratorBase.IterateResultType.ProgressNext:
                case TimerIteratorBase.IterateResultType.ProgressOver:
                    SetTexture(_SpriteIterator.GetCurrentSprite().texture);
                    break;
            }
        }

        protected override void OnDispose()
        {
            if (!ReferenceEquals(null, _SpriteIterator))
            {
                SetTexture(null);
                _SpriteIterator.Dispose();
                _SpriteIterator = null;
            }
        }
#endif
      
        #endregion
    }
}