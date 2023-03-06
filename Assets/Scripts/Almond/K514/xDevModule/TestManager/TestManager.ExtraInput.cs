#if UNITY_EDITOR && ON_GUI
using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Consts>

        /// <summary>
        /// 정수 입력 필드
        /// </summary>
        private static readonly Rect ExtraValueField 
            = new Rect(SystemMaintenance.ScreenScaleVector2.x - BoxIntervalSize - BoxSizeHeight * 1.5f, BoxIntervalSize, BoxSizeHeight * 1.5f, BoxSizeHeight * 0.75f - 2);

        /// <summary>
        /// 정수 입력 필드 라벨
        /// </summary>
        private static readonly Rect ExtraValueFieldLabel
            = new Rect(ExtraValueField.x - BoxIntervalSize - BoxSizeWidth * 0.75f, BoxIntervalSize, BoxSizeWidth * 0.75f, BoxSizeHeight * 0.75f - 2);

        #endregion
        
        #region <Fields>
        
        /// <summary>
        /// 현재 선택된 테스트 타입에서 사용할 추가 UI 테이블
        /// </summary>
        private Dictionary<TestControlType, TestGUIExtraInputPreset> _CurrentTestControlExtraGUICollection;

        /// <summary>
        /// 추가 입력된 정수 문자열
        /// </summary>
        private string _CurrentExtraIntInputString;
        
        /// <summary>
        /// 추가 입력된 정수
        /// </summary>
        private int _CurrentExtraIntInput;
        
        /// <summary>
        /// 추가 입력된 열거형상수
        /// </summary>
        private int _CurrentExtraEnumInput;
        
        #endregion

        #region <Enums>

        [Flags]
        public enum TestExtraInputType
        {
            None = 0,
            IntField = 1 << 0,
            EnumField = 1 << 1,
        }

        #endregion
        
        #region <Callbacks>

        private void OnPageSwitched()
        {
            var tryPreset = _CurrentTestControlExtraGUICollection[_CurrentTestControlType];
            _CurrentExtraIntInput = tryPreset.DefaultInt;
            _CurrentExtraIntInputString = _CurrentExtraIntInput.ToString();
        }

        #endregion

        #region <Methods>

        private void DrawExtraInputGUI()
        {
            var cnt = 0;
            
            if (_CurrentTestControlExtraGUICollection[_CurrentTestControlType].Flags.HasAnyFlagExceptNone(TestExtraInputType.IntField))
            {
                var heightOffset = BoxSizeHeight * cnt + BoxIntervalSize;
                var labelRect = ExtraValueFieldLabel;
                var fieldRect = ExtraValueField;
                labelRect.y += heightOffset;
                fieldRect.y += heightOffset;
                
                GUI.Box(labelRect, new GUIContent($"정수 입력 : "));
                _CurrentExtraIntInputString = GUI.TextArea(fieldRect, _CurrentExtraIntInputString);

                if (string.IsNullOrWhiteSpace(_CurrentExtraIntInputString))
                {
                    _CurrentExtraIntInput = 0;
                }
                else
                {
                    if (int.TryParse(_CurrentExtraIntInputString, out var o_ParseStr))
                    {
                        _CurrentExtraIntInput = o_ParseStr;
                    }  
                }

                cnt++;
            }
            
            if (_CurrentTestControlExtraGUICollection[_CurrentTestControlType].Flags.HasAnyFlagExceptNone(TestExtraInputType.EnumField))
            {
                var heightOffset = BoxSizeHeight * cnt + BoxIntervalSize;
                var fieldRect = ExtraValueField;
                var selectionList = _CurrentTestControlExtraGUICollection[_CurrentTestControlType].SelectionList;
                fieldRect.y += heightOffset;
                fieldRect.height += selectionList.Length * BoxSizeHeight;

                _CurrentExtraEnumInput = GUI.SelectionGrid(fieldRect, _CurrentExtraEnumInput, selectionList, 1);

                cnt++;
            }
        }

        private void SetExtraIntInput(TestControlType p_Type, int p_InitialValue)
        {
            var tryPreset = _CurrentTestControlExtraGUICollection[p_Type];
            tryPreset.Flags.AddFlag(TestExtraInputType.IntField);
            tryPreset.DefaultInt = p_InitialValue;
        }
        
        private void SetExtraEnumInput(TestControlType p_Type, Type p_Enum)
        {
            var tryPreset = _CurrentTestControlExtraGUICollection[p_Type];
            tryPreset.Flags.AddFlag(TestExtraInputType.EnumField);
            tryPreset.SelectionList = p_Enum.GetEnumNames();
        }
        
        #endregion

        #region <Class>

        public class TestGUIExtraInputPreset
        {
            public TestExtraInputType Flags;
            public int DefaultInt;
            public string[] SelectionList;
        }

        #endregion
    }
}

#endif