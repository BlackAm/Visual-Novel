#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public abstract class UI_PanelBase : UIManagerBase
    {
        #region <Fields>
        
        /// <summary>
        /// 해당 판넬이 보유한 이미지 리스트
        /// </summary>
        protected List<Image> _AllImageGroup;
        
        /// <summary>
        /// 해당 판넬이 보유한 텍스트 리스트
        /// </summary>
        protected List<Text> _AllTextGroup;

        /// <summary>
        /// 해당 판넬의 이미지 컴포넌트의 색이 변경됬는지 표시하는 플래그
        /// </summary>
        protected bool _IsColorChanged;

        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            _AllImageGroup = new List<Image>();
            _AllTextGroup = new List<Text>();
            
            _Transform.GetComponentsInChildren(_AllImageGroup);
            _Transform.GetComponentsInChildren(_AllTextGroup);
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            
            ClearColor();
        }

        #endregion

        #region <Methods>

        protected void SetAllImageLerpScale(float p_ProgressRate01, float limitSize = 0f)
        {
            if (p_ProgressRate01 < limitSize) return;
            foreach (var image in _AllImageGroup)
            {
                //image.SetImageAlpha(p_ProgressRate01);
                //(p_ProgressRate01);
                _RectTransform.localScale = ObjectScale.CurrentValue * new Vector3(p_ProgressRate01, p_ProgressRate01, 0);
            }
        }

        /// 사이즈를 적용합니다.
        public void SetAllImageLerpScale(float size)
        {
            foreach (var image in _AllImageGroup)
            {
                _RectTransform.localScale = ObjectScale.CurrentValue * new Vector3(size, size, 0);
            }
        }
        
        protected void SetAllImageLinearBlack(float p_ProgressRate01)
        {
            foreach (var image in _AllImageGroup)
            {
                image.SetImageAlpha(p_ProgressRate01);
            }
        }
        
        protected void SetAllImageLinearClear(float p_ProgressRate01)
        {
            SetAllImageLinearBlack(1f - p_ProgressRate01);
        }

        protected void SetAllTextLinearBlack(float p_ProgressRate01)
        {
            foreach (var text in _AllTextGroup)
            {
                text.SetTextAlpha(p_ProgressRate01);
            }
        }
        
        protected void SetAllTextLinearClear(float p_ProgressRate01)
        {
            SetAllTextLinearBlack(1f - p_ProgressRate01);
        }
               
        protected void SetAllTextContentsClear()
        {
            foreach (var text in _AllTextGroup)
            {
                text.text = string.Empty;
            }
        }

        public virtual void SetColor(Color p_Color)
        {
            _IsColorChanged = true;
            
            foreach (var image in _AllImageGroup)
            {
                image.color = p_Color;
            }
        }
        
        private void ClearColor()
        {
            if (_IsColorChanged)
            {
                SetColor(Color.white);
                _IsColorChanged = false;
            }
        }

        #endregion
    }
}
#endif