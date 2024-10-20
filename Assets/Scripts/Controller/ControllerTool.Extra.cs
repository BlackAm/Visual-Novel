using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 컨트롤 이벤트를 입력 디바이스에 따라 분배하는 기능을 담당하는 매니저 클래스
    /// </summary>
    public partial class ControllerTool
    {
        public void RemoveControllerEventReceiver(InputEventType p_InputType,
                ControllerEventReceiver p_Event)
        {
            if(_InputEventSenderCollection.ContainsKey(p_InputType))
            {
                _InputEventSenderCollection[p_InputType].RemoveReceiver(p_Event);
            }
        }
    }
}