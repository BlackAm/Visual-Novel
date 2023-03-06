using UnityEngine;

namespace k514
{
    /*public partial class Patcher
    {
        #region <Callbacks>

        protected override void OnSequenceBegin(AsyncPatchSequence p_AsyncTaskSequence)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case PatchProgressPhase.CheckVersion:
                    BridgeEntryPhaseLoop();
                    break;
            }
        }

        protected override void OnSequenceTerminate(AsyncPatchSequence p_AsyncTaskSequence)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case PatchProgressPhase.CheckVersion:
                    SwitchPhase(PatchProgressPhase.CompareVersion);
                    break;
                case PatchProgressPhase.CompareVersion: 
                    SwitchPhase(PatchProgressPhase.GetAssetList);
                    break;
                case PatchProgressPhase.GetAssetList:
                    SwitchPhase(PatchProgressPhase.PatchFile);
                    break;
                case PatchProgressPhase.PatchFile:
                    SwitchPhase(PatchProgressPhase.PatchTerminate);
                    break;
                case PatchProgressPhase.PatchTerminate:
                    BridgeTerminatePhaseLoop();
                    break;
            }
        }
        
        protected override void OnTaskBegin(AsyncPatchSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case PatchProgressPhase.CompareVersion:
                    switch (taskSeqIndex)
                    {
                        default:
#if !SERVER_DRIVE
                            SetLabelText(SystemLanguage.SystemLanguageType.Patch_TryNetworkConnection);
#endif
                            break;
                    }
                    break;
                case PatchProgressPhase.GetAssetList:
                    switch (taskSeqIndex)
                    {
                        default:
#if !SERVER_DRIVE
                            SetLabelText(SystemLanguage.SystemLanguageType.Patch_CompareVersion);
#endif
                            break;
                    }
                    break;
                case PatchProgressPhase.PatchFile:
                    switch (taskSeqIndex)
                    {
                        default:
#if !SERVER_DRIVE
                            SetLabelText(SystemLanguage.SystemLanguageType.Patch_DownloadFile);
#endif
                            break;
                    }
                    break;
                case PatchProgressPhase.PatchTerminate:
                    switch (taskSeqIndex)
                    {
                        default:
#if !SERVER_DRIVE
                            SetLabelText(SystemLanguage.SystemLanguageType.Patch_Terminate);
#endif
                            break;
                    }
                    break;
            }
        }
        
        protected override void OnTaskSuccess(AsyncPatchSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
            var (taskPhase, taskSeqIndex, taskLastIndex) = p_AsyncTaskSequence.GetSequenceKey();
            switch (taskPhase)
            {
                case PatchProgressPhase.CompareVersion: 
                {
                    if (taskSeqIndex == taskLastIndex)
                    {
                        if (p_Handler is AsyncPatchTaskRequestHandler c_Handler)
                        {
                            var result = c_Handler.GetResult();
                            _PatchCheckResultPreset = result;
                        }
                    }
                    break;
                }
                case PatchProgressPhase.GetAssetList:
                    break;
                case PatchProgressPhase.PatchFile:
                    break;
                case PatchProgressPhase.PatchTerminate:
                    break;
            }
        }
        
        protected override void OnTaskFail(AsyncPatchSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
#if UNITY_EDITOR
            Debug.LogError("[Patch Error] : 패치에 실패했습니다.");
            SwitchPhase(PatchProgressPhase.PatchTerminate);
#else
            // SetLabelText(SystemLanguage.SystemLanguageType.Patch_Fail);
            // BridgeTerminatePhaseLoop();
            Application.Quit();
#endif
        }
        
        protected override void OnTaskCancel(AsyncPatchSequence p_AsyncTaskSequence, IAsyncTaskRequest p_Handler)
        {
        }

        #endregion
    }*/
}