#if !SERVER_DRIVE
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Almond.Util;

namespace BlackAm
{
    public partial class ControlStick : TouchEventJoystickButton
    {
        private const float __Alpha_UpperBound = 0.75f;
        private const float __Alpha_Speed = 3f;
        
        private Image _controlStickBaseImage;
        private float _CurrentAlpha;
        private bool _IsDragged;

        public override void OnSpawning()
        {
            base.OnSpawning();
            /*_controlStickBaseImage = MainGameUI.Instance.mainUI._ControlStickBase.GetComponent<Image>();
            _controlStickBaseImage.SetImageAlpha(0f);*/
        }

        protected override void OnKeyCodeEventPointerDown(PointerEventData p_EventData)
        {
            base.OnKeyCodeEventPointerDown(p_EventData);
            /*MainGameUI.Instance.mainUI._ControlStickBase.localPosition = p_EventData.position;
            _BaseOffset = MainGameUI.Instance.mainUI._ControlStickBase.transform.localPosition.XYVector2();
            _IsDragged = true;*/
        }

        protected override void OnKeyCodeEventPointerUp(PointerEventData p_EventData)
        {
            base.OnKeyCodeEventPointerUp(p_EventData);
            _IsDragged = false;
            _CurrentAlpha = 0f;
            _controlStickBaseImage.SetImageAlpha(_CurrentAlpha);
            _Handle.SetImageAlpha(_CurrentAlpha);
        }

        public override void OnUpdateUI(float p_DeltaTime)
        {
            base.OnUpdateUI(p_DeltaTime);
            /*if (_IsDragged)
            {
                var temp = (Input.touchCount == 0) ? (Vector2) Input.mousePosition : Input.GetTouch(0).position;
                // 클릭 위치 ==> 움직인 위치가 같으면 드래그 한 것.
                if (!MainGameUI.Instance.mainUI._ControlStickBase.position.Equals(temp))
                {
                    _Handle.SetImageAlpha(1f);
                    _CurrentAlpha =
                        Mathf.Min(__Alpha_UpperBound, _CurrentAlpha + __Alpha_Speed * p_DeltaTime);
                    _controlStickBaseImage.SetImageAlpha(_CurrentAlpha);
                }
                
            }*/
        }

        public override string GetHandlePivotName()
        {
            return "Pivot";
        }
    }
}
#endif