#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace k514
{
    public class UI_ImagePanel : UI_PanelBase
    {
        #region <Fields>

        protected Image _MainImage;
        private (AssetPreset, Sprite) _SpritePreset;
        
        #endregion
        
        #region <Callbacks>
        
        public override void OnSpawning()
        {
            base.OnSpawning();
            _MainImage = _Transform.FindRecursive<Image>("MainImage").Item2;
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            CheckSpritePresetValidation();
        }
        
        public override void OnUpdateUI(float p_DeltaTime)
        {
        }

        #endregion

        #region <Methods>

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
        }

        public void SetImageColor(Color p_Color)
        {
            _MainImage.color = p_Color;
        }

        #endregion
    }
}
#endif