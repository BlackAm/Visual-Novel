using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    /*public partial class Maintenance
    {
        #region <Callbacks>

        protected override void OnCreatePhase()
        {
            _PhaseWeightTable 
                = new Dictionary<MaintenanceProgressPhase, float>
                {
                    {MaintenanceProgressPhase.TaskStart, 1f},
                    {MaintenanceProgressPhase.TaskTerminate, 1f},
                };

            base.OnCreatePhase();
            
            var enumerator = SystemTool.GetEnumEnumerator<MaintenanceProgressPhase>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var progressPhase in enumerator)
            {
                switch (progressPhase)
                {
                    case MaintenanceProgressPhase.TaskStart:
                    {
                        var asyncTaskSequence = new AsyncMaintenanceSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncMaintenanceTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams 
                            (
                                TaskStart
#if UNITY_EDITOR
                                , "T Begin"
#endif
                            ),
                            2f, 0.3f
                        );

                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case MaintenanceProgressPhase.TaskTerminate:
                    {
                        var asyncTaskSequence = new AsyncMaintenanceSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncMaintenanceTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams 
                            (
                                TaskTerminate
#if UNITY_EDITOR
                                , "T Terminate"

#endif
                            ),
                            2f, 0.3f
                        );

                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                }
            }
            
            SwitchPhase(MaintenanceProgressPhase.TaskStart);
        }

        #endregion
        
        #region <Methods>

        protected override void SwitchPhase(MaintenanceProgressPhase p_Type)
        {
            base.SwitchPhase(p_Type);
            
            switch (_CurrentPhase)
            {
                case MaintenanceProgressPhase.None:
                    break;
                case MaintenanceProgressPhase.TaskStart:
                case MaintenanceProgressPhase.TaskTerminate:
                    _AsyncTaskTable[_CurrentPhase].StartAsyncTaskSequence();
                    break;
            }
        }

        #endregion
    }*/
}