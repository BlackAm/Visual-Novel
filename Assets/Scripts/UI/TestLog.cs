#if !SERVER_DRIVE
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlackAm
{
    /*public class TestLog : AbstractUI, IPointerDownHandler, IPointerUpHandler
    {
        private Image openDebug;
        public GameObject panel;
        private Text debugText;
        private float clickCount;
        private bool count;
        public InitType _initType;

        public enum InitType
        {
            Intro,
            Main
        }
        
        public override void Initialize()
        {
            if (_initType == InitType.Intro)
            {
                openDebug = GetComponent<Image>("ActiveButton");   
            }
            panel = Find("BG").gameObject;
            debugText = GetComponent<Text>("BG/Scroll View/Viewport/Content/Text");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            count = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            count = false;
            clickCount = 0;
        }

        private void Update()
        {
            if (count)
            {
                clickCount += Time.deltaTime;

                Debug.LogWarning(clickCount);

                if (clickCount > 1.0f)
                {
                    count = false;
                    clickCount = 0;

                    if (panel.activeSelf)
                    {
                        panel.SetActive(false);
                    }
                    else
                    {
                        panel.SetActive(true);
                    }
                }
            }
        }

        /// 로그 추가. 
        public void AddDebugLog(string text)
        {
            debugText.text += "\n" + text;
        }
    }*/
}
#endif