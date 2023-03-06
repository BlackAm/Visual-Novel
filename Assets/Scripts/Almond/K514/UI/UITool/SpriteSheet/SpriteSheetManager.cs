#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public class SpriteSheetManager : AsyncSingleton<SpriteSheetManager>
    {
        #region <Consts>

        public const int CRITICAL_INDEX = 10;
        public const int MISSED_INDEX = 11;
        public const int STUN_INDEX = 12;
        public const int BLEED_INDEX = 13;
        public const int POISON_INDEX = 14;

        #endregion
        
        #region <Fields>

        private static SpriteSheetCarrier NumberSpriteSet;

        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            // 데미지 폰트
            /*var imageResourceNameTable = await ImageNameTableData.GetInstance();
            await UniTask.SwitchToMainThread();
            var spriteAssetPreset = imageResourceNameTable.GetResources(8, ResourceType.Image, ResourceLifeCycleType.Free_Condition);
            NumberSpriteSet = new SpriteSheetCarrier(spriteAssetPreset);*/
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        #endregion

        #region <Methods>

        public Sprite GetNumberSprite(int p_Index)
        {
            return NumberSpriteSet.GetSprite(p_Index);
        }
        
        #endregion

        #region <Disposable>

        protected override void DisposeUnManaged()
        {
            NumberSpriteSet?.Dispose();
            NumberSpriteSet = null;
            
            base.DisposeUnManaged();
        }

        #endregion
    }
}
#endif