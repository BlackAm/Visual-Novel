using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class BootStrap
    {
        #region <Callbacks>

        protected override void OnCreatePhase()
        {
            _PhaseWeightTable 
                = new Dictionary<BootingProgressPhase, float>
                {
                    {BootingProgressPhase.BootStart, 1f},
                    {BootingProgressPhase.SystemOpenProcess, 1f},
                    {BootingProgressPhase.SystemOpenProcess2, 1f},
                    {BootingProgressPhase.SystemOpenProcess3, 1f},
                    {BootingProgressPhase.SystemOpenProcess4, 1f},
                    {BootingProgressPhase.BootTerminate, 1f},
                };

            base.OnCreatePhase();
            
            var enumerator = SystemTool.GetEnumEnumerator<BootingProgressPhase>(SystemTool.GetEnumeratorType.ExceptNone);
            foreach (var progressPhase in enumerator)
            {
                switch (progressPhase)
                {
                    case BootingProgressPhase.BootStart:
                    {
                        var asyncTaskSequence = new AsyncBootingSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams 
                            (
                                BootingStart
#if UNITY_EDITOR
                                , "부팅 연출 및 시스템 테이블을 미리 로드"
#endif
                            ),
                            2f, 0.3f
                        );

                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case BootingProgressPhase.SystemOpenProcess:
                    {
                        #region <SystemOpenProcess>

                        var asyncTaskSequence = new AsyncBootingSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameSingletonLoad0 
#if UNITY_EDITOR
                                , "싱글톤을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameSingletonLoad1 
#if UNITY_EDITOR
                                , "싱글톤을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameSingletonLoad2
#if UNITY_EDITOR
                                , "싱글톤을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameSingletonLoad3
#if UNITY_EDITOR
                                , "싱글톤을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameSingletonLoad4
#if UNITY_EDITOR
                                , "싱글톤을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameSingletonLoad5
#if UNITY_EDITOR
                                , "싱글톤을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                        
                        #endregion
                    }
                    case BootingProgressPhase.SystemOpenProcess2:
                    {
                        #region <SystemOpenProcess2>

                        var asyncTaskSequence = new AsyncBootingSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameTableLoad0
#if UNITY_EDITOR
                                , "게임 테이블을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameTableLoad1
#if UNITY_EDITOR
                                , "게임 테이블을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameTableLoad2
#if UNITY_EDITOR
                                , "게임 테이블을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameTableLoad3
#if UNITY_EDITOR
                                , "게임 테이블을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameTableLoad4
#if UNITY_EDITOR
                                , "게임 테이블을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                GameTableLoad5
#if UNITY_EDITOR
                                , "게임 테이블을 미리 로딩"
#endif
                            ),
                            2f, 0.1f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                        
                        #endregion
                    }
                    case BootingProgressPhase.SystemOpenProcess3:
                    {
                        var asyncTaskSequence = new AsyncBootingSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
#if UNITY_EDITOR
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                BootEditorOnly, 
                                "에디터 모드에서만 동작하는 시스템 초기화"
                            ),
                            2f, 0.1f
                        );
#endif
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                LoadMainGameTable
#if UNITY_EDITOR
                                , "메인 게임 테이블 초기화"
#endif
                            ),
                            2f, 0.5f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case BootingProgressPhase.SystemOpenProcess4:
                    {
                        var asyncTaskSequence = new AsyncBootingSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        /*AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                NetworkInit
#if UNITY_EDITOR
                                , "시스템 네트워크 초기화"
#endif
                            ),
                            2f, 0.5f
                        );*/
                        AsyncBootingTaskRequestManager.GetInstance.AddAsyncTaskSequence
                        (
                            asyncTaskSequence,
                            new DefaultAsyncTaskRequestParams
                            (
                                CameraInit
#if UNITY_EDITOR
                                , "카메라 초기화"
#endif
                            ),
                            1f, 0.5f
                        );
                        
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                    case BootingProgressPhase.BootTerminate:
                    {
                        var asyncTaskSequence = new AsyncBootingSequence(progressPhase, _AsyncTaskSequenceEventHandler, _PhaseWeightTable[progressPhase]);
                        _AsyncTaskTable.Add(progressPhase, asyncTaskSequence);
                        break;
                    }
                }
            }
            
            SwitchPhase(BootingProgressPhase.BootStart);
        }

        #endregion
        
        #region <Methods>

        protected override void SwitchPhase(BootingProgressPhase p_Type)
        {
            base.SwitchPhase(p_Type);
            
            switch (_CurrentPhase)
            {
                case BootingProgressPhase.None:
                    break;
                case BootingProgressPhase.BootStart:
                case BootingProgressPhase.SystemOpenProcess:
                case BootingProgressPhase.SystemOpenProcess2:
                case BootingProgressPhase.SystemOpenProcess3:
                case BootingProgressPhase.SystemOpenProcess4:
                case BootingProgressPhase.BootTerminate:
                    _AsyncTaskTable[_CurrentPhase].StartAsyncTaskSequence();
                    break;
            }
        }

        #endregion
    }
}