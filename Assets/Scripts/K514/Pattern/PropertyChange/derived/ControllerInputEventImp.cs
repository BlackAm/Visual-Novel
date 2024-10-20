using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public class ControllerEventSender : PropertyModifyEventSenderImp<ControllerTool.InputEventType,
        ControllerTool.ControlEventPreset>
    {
        public override bool HasEvent(ControllerTool.InputEventType p_Type, ControllerTool.InputEventType p_Compare)
        {
            return p_Type.HasAnyFlagExceptNone(p_Compare);
        }
    }
    
    public class ControllerEventReceiver : PropertyModifyEventReceiverImp<ControllerTool.InputEventType,
        ControllerTool.ControlEventPreset>
    {
        #region <Fields>

        /// <summary>
        /// 특정 키가 입력됬을 때, 입력이 시작된 시간
        /// </summary>
        public float[] InputStartRecord { get; private set; }
        
        /// <summary>
        /// 특정 키가 입력되고 난 후, 경과된 시간
        /// </summary>
        public float[] InputDurationRecord { get; private set; }

        #endregion
        
        #region <Constructor>

        public ControllerEventReceiver() : base()
        {
            InputStartRecord = new float[ControllerTool.GetInstanceUnSafe.KeyCodeScale];
            InputDurationRecord = new float[ControllerTool.GetInstanceUnSafe.KeyCodeScale];
        }

        public ControllerEventReceiver(ControllerTool.InputEventType p_EventType,
            Action<ControllerTool.InputEventType,
                ControllerTool.ControlEventPreset> p_EventHandler) : base(p_EventType, p_EventHandler)
        {
            InputStartRecord = new float[ControllerTool.GetInstanceUnSafe.KeyCodeScale];
            InputDurationRecord = new float[ControllerTool.GetInstanceUnSafe.KeyCodeScale];
        }

        #endregion

        #region <Methods>

        public override void OnPropertyModifyEventReceived(ControllerTool.InputEventType p_Type, ControllerTool.ControlEventPreset p_Preset)
        {
            var controllerEventType = p_Preset.ControllerInputStateType;
            var targetKeyCode = p_Preset.KeyCode;
            switch (controllerEventType)
            {
                case ControllerTool.ControllerInputStateType.PressKey:
                    InputStartRecord[targetKeyCode] = Time.time;
                    InputDurationRecord[targetKeyCode] = 0f;
                    break;
                case ControllerTool.ControllerInputStateType.HoldingKey:
                    InputDurationRecord[targetKeyCode] = Time.time - InputStartRecord[targetKeyCode];
                    break;
                case ControllerTool.ControllerInputStateType.ReleaseKey:
                    InputStartRecord[targetKeyCode] = Time.time;
                    break;
            }
            p_Preset.SetTimeStamp(InputStartRecord[targetKeyCode], InputDurationRecord[targetKeyCode]);
            base.OnPropertyModifyEventReceived(p_Type, p_Preset);
        }

        #endregion
    }
}