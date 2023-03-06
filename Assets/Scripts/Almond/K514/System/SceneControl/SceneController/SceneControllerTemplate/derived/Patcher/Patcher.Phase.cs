using System.Collections.Generic;

namespace k514
{
    /*public partial class Patcher
    {
        #region <Fields>
        
        /// <summary>
        /// 패치 정보를 가지는 프리셋
        /// </summary>
        private AsyncPatchTaskRequestResult _PatchCheckResultPreset;

        #endregion

        #region <Callbacks>

        protected override void OnCreatePhase()
        {
            _PhaseWeightTable 
                = new Dictionary<PatchProgressPhase, float>
                {
                    {PatchProgressPhase.CheckVersion, 0.25f},
                    {PatchProgressPhase.CompareVersion, 0.5f},
                    {PatchProgressPhase.GetAssetList, 3f},
                    {PatchProgressPhase.PatchFile, 3f},
                    {PatchProgressPhase.PatchTerminate, 0.25f},
                };

            base.OnCreatePhase();

            var enumerator = SystemTool.GetEnumEnumerator<PatchProgressPhase>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var progressPhase in enumerator)
            {
                switch (progressPhase)
                {
                    case PatchProgressPhase.CheckVersion:
                    {
                        var asyncTaskSequence = new AsyncPatchSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncPatchTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                TryGetVersionIndex
#if UNITY_EDITOR
                                , "패치 서버와 연결 시도"
#endif
                            ),
                            1f, 0.3f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case PatchProgressPhase.CompareVersion:
                    {
                        var asyncTaskSequence = new AsyncPatchSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncPatchTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                CompareVersion
#if UNITY_EDITOR
                                , "패치 서버와 연결 시도"
#endif
                            ),
                            1f, 0.3f
                        );
                
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case PatchProgressPhase.GetAssetList:
                    {
                        var asyncTaskSequence = new AsyncPatchSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncPatchTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                TryGetVersionAssetList
#if UNITY_EDITOR
                                , "패치 서버와 버전 비교"
#endif
                            ),
                            1f, 0.3f
                        );
                
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case PatchProgressPhase.PatchFile:
                    {
                        var asyncTaskSequence = new AsyncPatchSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncPatchTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                PatchFile
#if UNITY_EDITOR
                                , "패치 파일 다운로드 및 패치 적용, 테스트"
#endif
                            ),
                            1f, 1f
                        );
                
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case PatchProgressPhase.PatchTerminate:
                    {
                        var asyncTaskSequence = new AsyncPatchSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncPatchTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                PatchTerminate
#if UNITY_EDITOR
                                , "패치 종료"
#endif
                            ),
                            1f, 0.5f
                        );
                
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                }
            }
            
            SwitchPhase(PatchProgressPhase.CheckVersion);
        }

        #endregion

        #region <Methods>

        protected override void SwitchPhase(PatchProgressPhase p_Type)
        {
            base.SwitchPhase(p_Type);
            
            switch (_CurrentPhase)
            {
                case PatchProgressPhase.None:
                    break;
                case PatchProgressPhase.CheckVersion:
                case PatchProgressPhase.CompareVersion:
                case PatchProgressPhase.GetAssetList:
                case PatchProgressPhase.PatchFile:
                case PatchProgressPhase.PatchTerminate:
                    _AsyncTaskTable[_CurrentPhase].StartAsyncTaskSequence();
                    break;
            }
        }

        #endregion
    }*/
}