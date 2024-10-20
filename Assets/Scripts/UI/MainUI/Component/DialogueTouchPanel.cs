using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackAm
{
     public partial class MainUI
    {
        private TouchEventReceiver _touchEventReceiver;

        private void Initialize_DialogueTouchPanel()
        {
            _touchEventReceiver = TouchEventManager.GetInstance.GetEventReceiver<TouchEventReceiver>(
                TouchEventRoot.TouchEventType.DialogueActioned, OnDialogueTouched);
            RegistInput<TouchEventDragButtonWithGesture>("DialogueTouchPanel", TouchEventRoot.TouchInputType.DialogueEvent);
        }
        
        private void OnDialogueTouched(TouchEventRoot.TouchEventType p_Type, TouchEventManager.TouchEventPreset p_Preset)
        {
            switch (p_Preset.PointerEventData.button)
            {
                case PointerEventData.InputButton.Left:
                    DialogueGameManager.GetInstance._KeyInputObject.ActionKeyInput(ControllerTool.CommandType.L_Click);
                    break;
                case PointerEventData.InputButton.Right:
                    DialogueGameManager.GetInstance._KeyInputObject.ActionKeyInput(ControllerTool.CommandType.R_Click);
                    break;
            }
        }
    }
}