#if !SERVER_DRIVE
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BlackAm
{
    public class ExpBar : AbstractUI
    {
        private Text _expText;
        private Slider _expSlider;
        private Slider _battery;
        private Text _levelText;
        private RectTransform expTextView;

        private void Awake()
        {
            Initialize();
        }

        private void LateUpdate()
        {
            SetBattery();
        }

        public override void Initialize()
        {
            expTextView = GetComponent<RectTransform>("ExpText");
            _expText = GetComponent<Text>("ExpText/Value");
            _expSlider = GetComponent<Slider>("ExpBar/Slider");
            _battery = GetComponent<Slider>("Battery");
            _levelText = GetComponent<Text>("LevelText");

            _expSlider.interactable = false;
            _battery.interactable = false;
            LayoutRebuilder.ForceRebuildLayoutImmediate(expTextView);
        }

        /// <summary>
        /// 경험치 표시 (0~1f)
        /// </summary>
        /// <param name="exp"></param>
        public void SetExp(float exp)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"ExpBar.cs(38) >> 현재 경험치 바: {exp*100}%");
#endif
            if (exp > 1f) exp = 1f;
            else if (exp < 0) exp = 0;

            //var expString = (exp * 100).ToString();
            //var pointIndex = expString.IndexOf('.');
            //if (pointIndex != -1 && expString.Length > pointIndex + 3)
            //{
            //    expString = expString.Substring(0, pointIndex + 3);
            //}
            if(_expText == null)
            {
                Initialize();
            }
            _expText.text = (exp * 100f).ToString("F2");
            _expSlider.value = exp;
            LayoutRebuilder.ForceRebuildLayoutImmediate(expTextView);
        }

        /// <summary>
        /// 베터리 잔량 표시 업데이트 (0~1f)
        /// </summary>
        public void SetBattery()
        {
            _battery.value = SystemInfo.batteryLevel; // SystemInfo 에서 지원하는 배터리 잔량값 (0~1f)
        }
        
        public void SetLevel(int level)
        {
            if (_levelText == null)
            {
                _levelText = GetComponent<Text>("LevelText");
            }

            _levelText.text = $"Lv {level.ToString()}";
        }
    }
}
#endif