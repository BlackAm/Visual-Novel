using System;
using UnityEngine;

namespace BlackAm
{
    public partial class BootStrap
    {
        #region <Callbacks>

        protected override void OnSequenceBegin(AsyncBootingSequence p_AsyncTaskSequence)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case BootingProgressPhase.BootStart: 
                    BridgeEntryPhaseLoop();
                    break;
            }
        }

        protected override void OnSequenceTerminate(AsyncBootingSequence p_AsyncTaskSequence)
        {            
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case BootingProgressPhase.BootStart: 
                    SwitchPhase(BootingProgressPhase.SystemOpenProcess);
                    break;
                case BootingProgressPhase.SystemOpenProcess:
                    SwitchPhase(BootingProgressPhase.SystemOpenProcess2);
                    break;
                case BootingProgressPhase.SystemOpenProcess2:
                    SwitchPhase(BootingProgressPhase.SystemOpenProcess3);
                    break;
                case BootingProgressPhase.SystemOpenProcess3:
                    SwitchPhase(BootingProgressPhase.SystemOpenProcess4);
                    break;
                case BootingProgressPhase.SystemOpenProcess4:
                    SwitchPhase(BootingProgressPhase.BootTerminate);
                    break;
                case BootingProgressPhase.BootTerminate:
                    BridgeTerminatePhaseLoop();
                    break;
            }
        }
        
        protected override void OnTaskBegin(AsyncBootingSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }
        
        protected override void OnTaskSuccess(AsyncBootingSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }
        
        protected override void OnTaskFail(AsyncBootingSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
#if UNITY_EDITOR
            Debug.LogError("[Boot Error] : 시스템 초기화에 실패했습니다.");
            SwitchPhase(BootingProgressPhase.BootTerminate);
#else
            // Application.Quit();
#endif
        }
        
        protected override void OnTaskCancel(AsyncBootingSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }

        #endregion
    }
}