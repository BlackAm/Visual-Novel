#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine.UI;

namespace k514
{
    public abstract class UI_MessageBoxBase : UIManagerBase
    {
        #region <Fields>
        
        /// <summary>
        /// 해당 메시지박스가 보유한 텍스트 리스트
        /// </summary>
        protected List<Text> _AllTextGroup;
        
        /// <summary>
        /// 해당 메시지박스가 보유한 버튼 리스트
        /// </summary>
        protected List<Button> _AllButtonGroup;

        #endregion
        
        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            _AllTextGroup = new List<Text>();
            _AllButtonGroup = new List<Button>();
            
            _Transform.GetComponentsInChildren(_AllTextGroup);
            _Transform.GetComponentsInChildren(_AllButtonGroup);
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
        }
        
        #endregion

        #region <Methods>
               
        protected void SetAllTextContentsClear()
        {
            foreach (var text in _AllTextGroup)
            {
                text.text = string.Empty;
            }
        }

        #endregion
    }
}
#endif