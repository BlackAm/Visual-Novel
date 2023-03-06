#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public class UITheater : UIPoolerBase
    {
        #region <Consts>

        private const int TestSize = 20; 

        #endregion
        
        #region <Methods>

        public UI_NamePanel PoolNameLabel(Unit p_TargetUnit, int p_Mark)
        {
            Set_UI_Hide(false, false);
            
            var nameLabel = PoolUI<UI_NamePanel>(UICustomRoot.UIManagerType.TestNamePanel);
            nameLabel.SetFadeDuration(0.05f, 0.05f);
            nameLabel.SetTextColor(Color.white);
            nameLabel.SetTextScale(22);
            nameLabel.SetPanelMark(p_Mark);
            nameLabel.SetTracingTarget(p_TargetUnit, Vector3.zero);
            nameLabel.SetScale(1f);

            return nameLabel;
        }
        
        public void PoolNumberFont(Unit p_TargetUnit, float p_Value, bool p_RandomizeDeploy)
        {
            Set_UI_Hide(false, false);
            
            var damageFont = PoolUI<UI_NumberPanel>(UICustomRoot.UIManagerType.TestNumberPanel);
            damageFont.SetFadeDuration(0.2f, 0.6f, 0.1f);
            damageFont.SetTracingTarget(
                p_TargetUnit._Transform, 
                p_RandomizeDeploy ? 
                    CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, 0f, p_TargetUnit.GetRadius(1f)) + p_TargetUnit.GetHeightOffsetVector(0.5f)
                    : p_TargetUnit.GetHeightOffsetVector(0.5f));
            damageFont.SetDamageSprite(p_Value);
            damageFont.SetScale(1f);
            damageFont.TriggerFadeIn();
        }

        public void PoolSymbolFont(Unit p_TargetUnit, int p_SymbolIndex, bool p_RandomizeDeploy)
        {
            Set_UI_Hide(false, false);
            var damageFont = PoolUI<UI_NumberWithSymbolPanel>(UICustomRoot.UIManagerType.TestNumberSymbolPanel);
            damageFont.SetFadeDuration(0.3f, 0.6f, 0.4f);
            damageFont.SetTracingTarget(
                p_TargetUnit._Transform, 
                p_RandomizeDeploy ? 
                    CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, 0f, p_TargetUnit.GetRadius(0.5f)) + p_TargetUnit.GetHeightOffsetVector(0.7f)
                    : p_TargetUnit.GetHeightOffsetVector(1.1f),
                Vector3.up * 420f
            );
            
            damageFont.SetDisableDamageSprite();
            damageFont.SetDamageSymbolImage(p_SymbolIndex);
            damageFont.SetScale(0.85f);
            damageFont.TriggerFadeIn();
            damageFont.SetColor(new Color32(120, 0, 0, 255));
        }
        
        public void PoolNumberWithSymbolFont(Unit p_TargetUnit, int p_SymbolIndex, float p_Value, bool p_RandomizeDeploy)
        {
            Set_UI_Hide(false, false);
            var damageFont = PoolUI<UI_NumberWithSymbolPanel>(UICustomRoot.UIManagerType.TestNumberSymbolPanel);
            damageFont.SetFadeDuration(0.3f, 0.6f, 0.4f);
            damageFont.SetTracingTarget(
                p_TargetUnit._Transform, 
                p_RandomizeDeploy ? 
                    CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, 0f, p_TargetUnit.GetRadius(0.5f)) + p_TargetUnit.GetHeightOffsetVector(0.7f)
                    : p_TargetUnit.GetHeightOffsetVector(1.1f),
                Vector3.up * 5f
            );
           
            damageFont.SetDamageSprite(p_Value);
            damageFont.SetDamageSymbolImage(p_SymbolIndex);
            damageFont.SetScale(1.5f);
            damageFont.panelType = UI_NumberPanel.PanelType.Damage;
            damageFont.TriggerFadeIn();
            damageFont.SetColor(new Color32(120, 0, 0, 255));
        }

        public void PoolNumberWithSymbolFont(Unit p_TargetUnit, int p_SymbolIndex, float p_Value, bool p_RandomizeDeploy, Color p_color)
        {
            // if (p_TargetUnit.GetUnitHideRender()) return;
            Set_UI_Hide(false, false);
            var damageFont = PoolUI<UI_NumberWithSymbolPanel>(UICustomRoot.UIManagerType.TestNumberSymbolPanel);
            damageFont.SetFadeDuration(0.3f, 0.6f, 0.4f);
            damageFont.SetTracingTarget(
                p_TargetUnit._Transform, 
                p_RandomizeDeploy ? 
                    CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, 0f, p_TargetUnit.GetRadius(0.5f)) + p_TargetUnit.GetHeightOffsetVector(0.7f)
                    : p_TargetUnit.GetHeightOffsetVector(1.1f),
                Vector3.up * 15f
            );
            
            damageFont.SetDamageSprite(p_Value);
            damageFont.SetDamageSymbolImage(p_SymbolIndex);
            damageFont.SetScale(1f);
            damageFont.TriggerFadeIn();
            damageFont.panelType = UI_NumberPanel.PanelType.Potion;
            damageFont.SetColor(p_color);
        }
        
        #endregion
    }
}
#endif