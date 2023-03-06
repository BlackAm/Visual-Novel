#if UNITY_EDITOR && ON_GUI

using UnityEngine;

namespace k514
{
    public partial class TestManager
    {
        #region <Fields>

        private EventTimerCoroutine _Coroutine;
        private int TestValue;
        
        #endregion

        #region <Callbacks>

        void OnAwakeCoroutine()
        {
            var targetControlType = TestControlType.CoroutineTest;
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.Q, CallCoroutine, "코루틴 생성");
            BindKeyTestEvent(targetControlType, ControllerTool.CommandType.W, ToggleCoroutine, "코루틴 기동/정지");
        }

        #endregion

        #region <Methods>

        private void CallCoroutine(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                _Coroutine?.CancelEvent();
                _Coroutine = EventTimerCoroutineManager.GetInstance.GetCoroutine(SystemBoot.TimerType.GameTimer, false);
                _Coroutine.EventTimerCoroutineFlagMask.AddFlag(EventTimerCoroutine.CoroutinProcessFlag.KeepStatusWhenResume);
                _Coroutine.AddEvent
                (
                    (0, 500), 
                    handler =>
                    {
                        Debug.Log("코루틴");
                        return true;
                    }
                );
            }
        }
        
        private void ToggleCoroutine(ControllerTool.ControlEventPreset p_Type, Vector3 p_TryValue)
        {
            if (p_Type.IsInputRelease)
            {
                _Coroutine?.ToggleActiveCoroutine();
            }
        }

        #endregion
    }
}

#endif