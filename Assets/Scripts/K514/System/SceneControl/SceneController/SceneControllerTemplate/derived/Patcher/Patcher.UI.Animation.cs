#if !SERVER_DRIVE

namespace BlackAm
{
    /*public partial class Patcher
    {
        #region <Consts>

        private const string _Maid1ImageName = "YouseiMaid1";
        private const string _Maid2ImageName = "YouseiMaid2";
        private const string _PatchouliImageName = "Pache";
        private const string _RemiliaImageName = "Remi";
        private const string _SakuyaImageName = "Sakuya-san";
        private const string _NueImageName = "Nue";
        private const string _KogasaImageName = "Kogasa";
        private const string _YoshikaImageName = "Yoshika";
        
        #endregion

        #region <Fields>

        private SpriteSheetCarrier _FG_SpriteSheet;
        private SpriteSheetCarrier _BG_Character_SpriteSheet;
        
        private SpriteAnimationPreset _ForeGroundAnimation;
        private SpriteAnimationPreset _Y1Animation;
        private SpriteAnimationPreset _Y2Animation;
        private SpriteAnimationPreset _SAnimation;
        private SpriteAnimationPreset _PAnimation;
        private SpriteAnimationPreset _RAnimation;
        private SpriteAnimationPreset _KAnimation;
        private SpriteAnimationPreset _NAnimation;
        private SpriteAnimationPreset _YAnimation;

        #endregion

        #region <Callbacks>

        protected override void OnCreateAnimation()
        {
            base.OnCreateAnimation();
            
            _FG_SpriteSheet = new SpriteSheetCarrier($"{SystemMaintenance.GetDependencyResourcePathBranch(DependencyResourceSubType.Sprite)}LoadingScene");
            _BG_Character_SpriteSheet = new SpriteSheetCarrier($"{SystemMaintenance.GetDependencyResourcePathBranch(DependencyResourceSubType.Sprite)}BGChara");
            
            _ForeGroundAnimation = _FG_SpriteSheet.GetSpriteAnimationPreset(transform, _ForegroundImageName, 1, -1, 2f, 0);
            _ForeGroundAnimation.SpriteIterator.SetIntervalBound(7, 0);

            // 요정 메이드 1
            _Y1Animation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _Maid1ImageName, 0, 4, 0.6f, 0);
            _Y1Animation.SpriteIterator.SetLoopIntervalRandomize(true, (0.5f, 2f), default);
            
            // 요정 메이드 2
            _Y2Animation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _Maid2ImageName, 5, 9, 0.6f, 0);
            _Y2Animation.SpriteIterator.SetLoopIntervalRandomize(true, (0.5f, 2f), default);

            // 파츄리
            _PAnimation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _PatchouliImageName, 10, 17, 1f, 0); 
            _PAnimation.SpriteIterator.SetLoopIntervalRandomize(true, (0.5f, 5f), default);

            // 레밀
            _RAnimation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _RemiliaImageName, 18, 22, 1f, 0); 
            _RAnimation.SpriteIterator.SetPingPong(true);
            _RAnimation.SpriteIterator.SetLoopIntervalRandomize(true, (1f, 2f), (4f, 5f));
            
            // 사쿠야
            _SAnimation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _SakuyaImageName, 23, 27, 0.6f, 0); 
            _SAnimation.SpriteIterator.SetLoopIntervalRandomize(true, (0.5f, 2f), default);

            // 누에
            _NAnimation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _NueImageName, 28, 35, 0.6f, 0); 
            _NAnimation.SpriteIterator.SetLoopIntervalRandomize(true, (2f, 4f), default);

            // 코가사
            _KAnimation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _KogasaImageName, 36, 39, 0.6f, 0); 
                        
            // 요시카
            _YAnimation = _BG_Character_SpriteSheet.GetSpriteAnimationPreset(transform, _YoshikaImageName, 40, 43, 0.6f, 0); 
        }

        protected override void OnUpdateAnimation(float p_DeltaTime)
        {
            _ForeGroundAnimation.OnUpdateSpriteIterator(p_DeltaTime);
            
            _Y1Animation.OnUpdateSpriteIterator(p_DeltaTime);
            _Y2Animation.OnUpdateSpriteIterator(p_DeltaTime);
            _SAnimation.OnUpdateSpriteIterator(p_DeltaTime);
            _PAnimation.OnUpdateSpriteIterator(p_DeltaTime);
            _RAnimation.OnUpdateSpriteIterator(p_DeltaTime);
            _NAnimation.OnUpdateSpriteIterator(p_DeltaTime);
            _KAnimation.OnUpdateSpriteIterator(p_DeltaTime);
            _YAnimation.OnUpdateSpriteIterator(p_DeltaTime);
        }

        #endregion
    }*/
}
#endif
