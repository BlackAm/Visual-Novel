#if !SERVER_DRIVE
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class UI_TextPanel : UI_PanelBase
    {
        #region <Fields>

        public Text _MainText;
        protected Color _DefaultColor;
        
        #endregion
        
        #region <Callbacks>
        
        public override void OnSpawning()
        {
            base.OnSpawning();
            _MainText = _Transform.FindRecursive<Text>("MainText").Item2;
            _DefaultColor = _MainText.color;
        }


        public override void OnPooling()
        {
            base.OnPooling();
            
            SetTextContent(string.Empty);
            SetTextVisible(true);
            SetTextColor(new Color(1,1,1,1));
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
        }

        #endregion

        #region <Methods>

        public void SetTextContent(string p_Contents)
        {
            _MainText.text = p_Contents;
        }
        
        public void SetTextScale(int p_Size)
        {
            _MainText.fontSize = p_Size;
        }
        
        public void SetTextVisible(bool p_Flag)
        {
            _MainText.enabled = p_Flag;
        }

        public virtual void SetTextColor(Color p_Color)
        {
            if (p_Color == default)
            {
                _MainText.color = _DefaultColor;
            }
            else
            {
                _MainText.color = p_Color;
            }
        }

        #endregion
    }
}
#endif