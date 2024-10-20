using System;
using UnityEngine;

namespace BlackAm
{
    /*public partial class Maintenance
    {
        #region <Callbacks>

        protected override void OnSequenceBegin(AsyncMaintenanceSequence p_AsyncTaskSequence)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case MaintenanceProgressPhase.TaskStart: 
                    BridgeEntryPhaseLoop();
                    break;
            }
        }

        protected override void OnSequenceTerminate(AsyncMaintenanceSequence p_AsyncTaskSequence)
        {            
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case MaintenanceProgressPhase.TaskStart: 
                    SwitchPhase(MaintenanceProgressPhase.TaskTerminate);
                    break;
                case MaintenanceProgressPhase.TaskTerminate:
                    BridgeTerminatePhaseLoop();
                    break;
            }
        }
        
        protected override void OnTaskBegin(AsyncMaintenanceSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }
        
        protected override void OnTaskSuccess(AsyncMaintenanceSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }
        
        protected override void OnTaskFail(AsyncMaintenanceSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
#if UNITY_EDITOR
            Debug.LogError("[Maintenance Error] : ");
            SwitchPhase(MaintenanceProgressPhase.TaskTerminate);
#else
            // Application.Quit();
#endif
        }
        
        protected override void OnTaskCancel(AsyncMaintenanceSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }

        #endregion
    }*/
}