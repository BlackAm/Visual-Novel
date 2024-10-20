using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public interface IAsyncTaskRequest
    {
        void SetEventHandlerSet(AsyncTaskEventHandler p_AsyncTaskEventHandler);
        AsyncTaskEventHandler GetEventHandlerSet();
        float GetProgressRate();
        UniTask<bool> RunAsyncTask();

#if UNITY_EDITOR
        string GetDescription();
#endif
    }

    /// <summary>
    /// 비동기 작업을 요청했을 때, 그 진행상황 및 
    /// </summary>
    public abstract class AsyncTaskRequestBase<M, K, T> : PoolingObject<M>, IAsyncTaskRequest where M : AsyncTaskRequestBase<M, K, T>
    {
        #region <Fields>

        protected TaskPhase _CurrentPhase;
        private M _This;
        protected K _Params;
        private T _Result;
        protected AsyncOperationContainer _AsyncTaskContainer;
        private AsyncTaskTool.IAsyncTaskRequestSpawner<M, K> _Spawner;
        private AsyncTaskEventHandler _AsyncTaskEventHandler;
        private SafeReference<object, GameEventTimerHandlerWrapper> _AsyncTaskTraceTimer;
        private CancellationTokenSource _AsyncTaskCancelToken;
        private ProgressTimer _LowerBoundTimer;
        
        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
        }

        public override void OnPooling()
        {
            // 토큰은 매번 생성해주어야한다.
            _AsyncTaskCancelToken = new CancellationTokenSource();
        }

        public override void OnRetrieved()
        {
            CancelTask();
            
            _Spawner.OnAsyncTaskHandlerRetrieved(_This, _Params);
            _CurrentPhase = TaskPhase.None;
            _This = null;
            _Params = default;
            _Result = default;
            _AsyncTaskContainer = default;
            _Spawner = default;
            _AsyncTaskEventHandler = default;
            EventTimerTool.ReleaseEventHandler(ref _AsyncTaskTraceTimer);
            _AsyncTaskCancelToken.Dispose();
            _AsyncTaskCancelToken = null;
        }

        private async UniTask OnBegin()
        {
            _This = this as M;
            
            await SpawnAsyncTask();
                    
            _CurrentPhase = TaskPhase.TaskProgressing;
            _AsyncTaskEventHandler.OnTaskBegin?.Invoke(_This);
            _Spawner.OnAsyncTaskHandlerRunEvent(_This, _Params);

#if UNITY_EDITOR
            if(Application.isPlaying)
#endif
            {
                GameEventTimerHandlerManager.GetInstance.SpawnSafeEventTimerHandler(ref _AsyncTaskTraceTimer, this, SystemBoot.TimerType.SystemTimer, false);
                var (_, eventHandler) = _AsyncTaskTraceTimer.GetValue();
                
                if (ReferenceEquals(null, _AsyncTaskEventHandler.OnProgress))
                {
                    eventHandler
                        .AddEvent(
                            EventTimerTool.EventTimerIntervalType.UpdateEveryFrame,
                            handler =>
                            {
                                var requestHandler = handler.Arg1;
                                return requestHandler.OnProgressing(handler.LatestDeltaTime);
                            }, 
                            null, this
                        );
                }
                else
                {
                    eventHandler
                        .AddEvent(
                            EventTimerTool.EventTimerIntervalType.UpdateEveryFrame,
                            handler =>
                            {
                                var requestHandler = handler.Arg1;
                                return requestHandler.OnProgressing2(handler.LatestDeltaTime);
                            }, 
                            null, this
                        );
                }
                eventHandler.StartEvent(); 
            }
#if UNITY_EDITOR
            else
            {
                _LowerBoundTimer.Terminate();
            }
#endif
        }

        private bool OnProgressing(float p_DeltaTime)
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.TaskProgressing:
                    _LowerBoundTimer.Progress(p_DeltaTime);
                    return true;
                default:
                case TaskPhase.None:
                case TaskPhase.TaskTerminate:
                    return false;
            }
        }
        
        private bool OnProgressing2(float p_DeltaTime)
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.TaskProgressing:
                    _LowerBoundTimer.Progress(p_DeltaTime);
                    _AsyncTaskEventHandler.OnProgress.Invoke(_This, _LowerBoundTimer.ProgressRate * GetProgressRate());
                    return true;
                default:
                case TaskPhase.None:
                case TaskPhase.TaskTerminate:
                    return false;
            }
        }

        protected abstract bool _TryCheckTaskSuccess();

        private async UniTask<bool> TryCheckTaskSuccess()
        {
            await UniTask.SwitchToMainThread();

            if (_TryCheckTaskSuccess())
            {
                OnSuccess();
                return true;
            }
            else
            {
                OnFail();
                return false;
            }
        }
        
        private async UniTask<bool> OnTaskBreak()
        {
            await UniTask.SwitchToMainThread();

            if (!ReferenceEquals(null, _AsyncTaskCancelToken))
            {
                if (_AsyncTaskCancelToken.IsCancellationRequested)
                {
                    OnCancel();
                }
                else
                {
                    OnFail();
                }
            }

            return false;
        }
        
        private void OnSuccess()
        {
            _CurrentPhase = TaskPhase.TaskTerminate;
            _AsyncTaskEventHandler.OnSuccess?.Invoke(_This);
        }

        private void OnFail()
        {
            _CurrentPhase = TaskPhase.None;
            _AsyncTaskEventHandler.OnFail?.Invoke(_This);
        }
        
        private void OnCancel()
        {
            _CurrentPhase = TaskPhase.None;
            _AsyncTaskEventHandler.OnCancel?.Invoke(_This);
            RetrieveObject();
        }
        
        #endregion
        
        #region <Methods>

        public void SetEventHandlerSet(AsyncTaskEventHandler p_AsyncTaskEventHandler)
        {
            _AsyncTaskEventHandler = p_AsyncTaskEventHandler;
        }

        public AsyncTaskEventHandler GetEventHandlerSet()
        {
            return _AsyncTaskEventHandler;
        }

        public void SetPreset(AsyncTaskTool.IAsyncTaskRequestSpawner<M, K> p_Spawner, K p_Params, float p_LowerBoundTime)
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.None:
                {
                    _Params = p_Params;
                    _Spawner = p_Spawner;
                    _LowerBoundTimer = p_LowerBoundTime;
                }
                    break;
            }
        }

        public K GetParams()
        {
            return _Params;
        }

        private void CancelTask()
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.None:
                case TaskPhase.TaskTerminate:
                    break;
                case TaskPhase.TaskProgressing:
                    _AsyncTaskCancelToken.Cancel();
                    OnCancel();
                    break;
            }
        }

        protected abstract UniTask SpawnAsyncTask();

        public async UniTask<bool> RunAsyncTask()
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.None:
                {
                    await OnBegin();

                    try
                    {
                        await _AsyncTaskContainer.RunAsync(this, _AsyncTaskCancelToken.Token);
                        await UniTask.WaitUntil(() => _LowerBoundTimer.IsOver()).WithCancellation(_AsyncTaskCancelToken.Token);
                        return await TryCheckTaskSuccess();
                    }
#if UNITY_EDITOR
                    catch (Exception e)
                    {
                        Debug.LogError($"[{GetDescription()}]\n{e.Message}\n{e.StackTrace}");
#else
                    catch
                    {
#endif
                        return await OnTaskBreak();
                    }
                }
                case TaskPhase.TaskProgressing:
                {
                    await OnBegin();

                    try
                    {
                        await UniTask.WaitUntil(() => _AsyncTaskContainer.IsAsyncTaskOver(), PlayerLoopTiming.Update, _AsyncTaskCancelToken.Token);
                        await UniTask.WaitUntil(() => _LowerBoundTimer.IsOver()).WithCancellation(_AsyncTaskCancelToken.Token);
                        return await TryCheckTaskSuccess();
                    }
#if UNITY_EDITOR
                    catch (Exception e)
                    {
                        Debug.LogError($"[{GetDescription()}]\n{e.Message}\n{e.StackTrace}");
#else
                    catch
                    {
#endif
                        return await OnTaskBreak();
                    }
                }
                default:
                case TaskPhase.TaskTerminate:
                {
                    return await TryCheckTaskSuccess();
                }
            }
        }
        
        public async UniTask<bool> RunAsyncTask(AsyncTaskTool.IAsyncTaskRequestSpawner<M, K> p_Spawner, K p_Params, float p_LowerTimeBound)
        {
            SetPreset(p_Spawner, p_Params, p_LowerTimeBound);
            return await RunAsyncTask().WithCancellation(_AsyncTaskCancelToken.Token);
        }

        public void SetResult(T p_Result)
        {
            _Result = p_Result;
        }

        public T GetResult()
        {
            return _Result;
        }

        public float GetProgressRate()
        {
            return _AsyncTaskContainer.GetProgressRate();
        }

#if UNITY_EDITOR
        public string GetDescription()
        {
            return _Params.ToString();
        }
#endif

        #endregion
    }
}