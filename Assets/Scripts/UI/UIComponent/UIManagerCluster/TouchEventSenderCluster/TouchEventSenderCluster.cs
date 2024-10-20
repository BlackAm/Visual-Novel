#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// IUIManagerCluster를 확장하여, 터치 이벤트에 관련된 ITouchEvent를 등록하는 기능을 제공하는 인터페이스
    /// </summary>
    public interface ITouchEventSenderCluster : IUIManagerCluster
    {
        /// <summary>
        /// 해당 수납 구현체에 터치 이벤트 송신자를 등록시키는 메서드
        /// </summary>
        M RegistInput<M>(string p_ButtonName, TouchEventRoot.TouchInputType p_EventType) where M : KeyCodeTouchEventSenderBase;

        /// <summary>
        /// 해당 수납 구현체에 터치 키코드 이벤트 송신자를 등록시키는 메서드
        /// </summary>
        M RegistKeyCodeInput<M>(string p_ButtonName, TouchEventRoot.TouchMappingKeyCodeType p_TouchKeyCodeType, int p_SlotNumber, ControllerTool.InputEventType p_EventType) where M : KeyCodeTouchEventSenderBase;
        
        /// <summary>
        /// momo6346
        /// 해당 수납 구현체에 터치 키코드 이벤트 송신자를 제거하는 메소드
        /// </summary>
        M SetKeyCodeInput<M>(string p_ButtonName, int keyCode, ControllerTool.InputEventType p_EventType) where M : KeyCodeTouchEventSenderBase;
    }
    
    /// <summary>
    /// ITouchEventSenderCluster 구현체
    /// </summary>
    public abstract class TouchEventSenderCluster : UIManagerClusterBase, ITouchEventSenderCluster
    {
        #region <Methods>

        public M RegistInput<M>(string p_ButtonName, TouchEventRoot.TouchInputType p_EventType) where M : KeyCodeTouchEventSenderBase
        {
#if UNITY_EDITOR
            if (p_EventType == TouchEventRoot.TouchInputType.KeyCodeEvent)
            {
                Debug.LogError("적합하지 않은 메서드를 통해, UI 이벤트 송신자를 초기화하고 있습니다.");
            }
#endif
            var spawnedTouchEventSender = _Transform.Find(p_ButtonName).gameObject.AddComponent<M>();
            spawnedTouchEventSender.TouchEventFlagMask.AddFlag(p_EventType);
            spawnedTouchEventSender.CheckAwake();

            AddSlaveNode(spawnedTouchEventSender);
            return spawnedTouchEventSender;
        }

        public M RegistKeyCodeInput<M>(string p_ButtonName, TouchEventRoot.TouchMappingKeyCodeType p_TouchKeyCodeType, int p_SlotNumber, ControllerTool.InputEventType p_EventType) where M : KeyCodeTouchEventSenderBase
        {
            var targetKeyCode = TouchEventRoot.GetInstanceUnSafe[p_TouchKeyCodeType, p_SlotNumber].KeyCode;
            var spawnedTouchEventSender =
                _Transform.Find(p_ButtonName).gameObject.AddComponent<M>().SetKeyCode(targetKeyCode, p_EventType) 
                    as M;
            
            spawnedTouchEventSender.TouchEventFlagMask.AddFlag(TouchEventRoot.TouchInputType.KeyCodeEvent);
            spawnedTouchEventSender.CheckAwake();
            
            AddSlaveNode(spawnedTouchEventSender);
            return spawnedTouchEventSender;
        }
        
        // momo6346 - 스킬 슬롯변경을 위해 스킬등록을 교체하는 기능.
        public M SetKeyCodeInput<M>(string p_ButtonName, int keyCode, ControllerTool.InputEventType p_EventType) where M : KeyCodeTouchEventSenderBase
        {
            var spawnedTouchEventSender =
                _Transform.Find(p_ButtonName).gameObject.GetComponent<M>().SetKeyCode((KeyCode)keyCode, p_EventType) 
                    as M;
            spawnedTouchEventSender.TouchEventFlagMask.TurnFlag(TouchEventRoot.TouchInputType.KeyCodeEvent);
            
            AddSlaveNode(spawnedTouchEventSender);
            return spawnedTouchEventSender;
        }

        #endregion
    }
}
#endif