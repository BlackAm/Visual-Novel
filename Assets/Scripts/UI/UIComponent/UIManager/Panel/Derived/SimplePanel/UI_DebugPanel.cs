#if !SERVER_DRIVE
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class UI_DebugPanel : UI_PanelBase
    {
        #region <Fields>

        private Text FPS;
        private Text SpawnedNumber;
        private List<DebugUIWrapper> DebugWrapperGroup;
        
        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            DebugWrapperGroup = new List<DebugUIWrapper>();

            var nameBase = "Wrapper";
            for (int i = 0; i < 3; i++)
            {
                var WrapperName = $"Image/{nameBase}{i}";
                var titleText = _Transform.Find($"{WrapperName}/TitleText").GetComponent<Text>();
                var valueText = _Transform.Find($"{WrapperName}/ValueText").GetComponent<Text>();
                DebugWrapperGroup.Add(new DebugUIWrapper(titleText, valueText));
            }
            
            DebugWrapperGroup[0].SetTitleText("FPS");
            DebugWrapperGroup[0].SetValueText(0.ToString());
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
            DebugWrapperGroup[0].SetValueText(((int)(1f / Time.unscaledDeltaTime)).ToString());
        }

        #endregion

        #region <Methods>

        public void SetFirstContentTitle(string p_Text)
        {            
            DebugWrapperGroup[1].SetTitleText(p_Text);
        }
        
        public void SetFirstContentValue(string p_Text)
        {            
            DebugWrapperGroup[1].SetValueText(p_Text);
        }
        
        public void SetSecondContentTitle(string p_Text)
        {            
            DebugWrapperGroup[2].SetTitleText(p_Text);
        }
        
        public void SetSecondContentValue(string p_Text)
        {            
            DebugWrapperGroup[2].SetValueText(p_Text);
        }

        #endregion
        
        #region <Structs>

        public struct DebugUIWrapper
        {
            public Text Title;
            public Text Value;

            public DebugUIWrapper(Text p_Title, Text p_Value)
            {
                Title = p_Title;
                Value = p_Value;
                ClearContents();
            }

            public void SetTitleText(string p_Text)
            {
                Title.text = p_Text;
            }
            
            public void SetValueText(string p_Text)
            {
                Value.text = p_Text;
            }

            public void ClearContents()
            {
                SetTitleText(string.Empty);
                SetValueText(string.Empty);
            }
        }

        #endregion
    }
}
#endif