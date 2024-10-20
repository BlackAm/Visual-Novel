#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackAm
{
    public interface IIndexableTouchEventTableBridge : ITableBase
    {
    }

    public interface IIndexableTouchEventRecordBridge : ITableBaseRecord
    {
        KeyCode KeyCode { get; }
    }
    
    public partial class TouchEventManager : PropertySceneChangeEventSingleton<TouchEventSender, TouchEventRoot.TouchEventType, TouchEventManager.TouchEventPreset, TouchEventManager>
    {
        #region <Callbacks>

        protected override void OnCreated()
        {
            _InputStateCollection = new Dictionary<ControllerTool.InputEventType, bool[]>();
            _MobileStateCollection = new Dictionary<ControllerTool.InputEventType, ControllerTool.TouchGestureType>();
            _DashStateCollection = new Dictionary<ControllerTool.InputEventType, bool>();
            
            var targetEnumerator = ControllerTool.GetInstanceUnSafe.InputEventType_Enumerator;
            var targetTable = InputEventDeviceMap.GetInstanceUnSafe.GetTable();
            foreach (var inputEventType in targetEnumerator)
            {
                var targetRecordDeviceType = targetTable[inputEventType].DeviceType;
                switch (targetRecordDeviceType)
                {
                    case ControllerTool.InputControllerType.TouchPanel:
                    case ControllerTool.InputControllerType.Keyboard_TouchPanel:
                        _InputStateCollection.Add(inputEventType, new bool[ControllerTool.GetInstanceUnSafe.KeyCodeScale]);
                        _MobileStateCollection.Add(inputEventType, ControllerTool.TouchGestureType.None);
                        _DashStateCollection.Add(inputEventType, false);
                        break;
                }
            }
            
            OnAwakeKeyCode();
        }

        public override void OnInitiate()
        {
            ResetInputState();
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
        }
        
        public override void OnSceneTransition()
        {
        }
        
        #endregion
        
        #region <Structs>

        public struct TouchEventPreset
        {
            #region <Fields>

            public Vector3 WorldVector;
            public PointerEventData PointerEventData;

            #endregion

            #region <Constructors>

            public TouchEventPreset(Vector3 p_WorldVector, PointerEventData p_PointerEventData)
            {
                WorldVector = p_WorldVector;
                PointerEventData = p_PointerEventData;
            }

            public TouchEventPreset(PointerEventData p_PointerEventData)
            {
                WorldVector = Vector3.zero;
                PointerEventData = p_PointerEventData;
            }

            #endregion
        }

        #endregion
    }
}
#endif