using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public interface IAsyncTaskSequence
    {
    }

    public abstract class AsyncTaskSequenceBase<M, K> : IAsyncTaskSequence where M : AsyncTaskSequenceBase<M, K>
    {
        #region <Fields>

        private M _This;
        
        /// <summary>
        /// 해당 비동기 작업 순환자를 구분하는 심볼
        /// </summary>
        private K Symbol;
        
        /// <summary>
        /// 비동기 작업 풀
        /// </summary>
        public List<AsyncTaskSequencePreset> _AsyncTaskSequence;

        /// <summary>
        /// 시퀀스 이벤트 핸들러 셋
        /// </summary>
        private AsyncTaskSequenceEventHandler<M> _AsyncTaskSequenceEventHandler;
        
        /// <summary>
        /// 페이즈 이벤트 핸들러 셋
        /// </summary>
        private AsyncTaskEventHandler _AsyncTaskEventHandler;
        
        /// <summary>
        /// 해당 시퀀스 자체의 가중치 값
        /// </summary>
        public float _SequenceWeight;
        
        /// <summary>
        /// 현재 진행 중인 비동기 작업 인덱스
        /// </summary>
        private int _CurrentAsyncTaskIndex;
        
        /// <summary>
        /// 현재 비동기 작업 풀에 등록된 비동기 작업 숫자
        /// </summary>
        private int _AsyncTaskCount;
        
        /// <summary>
        /// 현재 페이즈
        /// </summary>
        private TaskPhase _CurrentPhase;
        
        #endregion

        #region <Constructor>

        public AsyncTaskSequenceBase(K p_Symbol, AsyncTaskSequenceEventHandler<M> p_AsyncTaskSequenceEventHandler, float p_SequenceWeight)
        {
            _This = this as M;
            Symbol = p_Symbol;
            _SequenceWeight = p_SequenceWeight;
            _AsyncTaskSequence = new List<AsyncTaskSequencePreset>();
            _AsyncTaskSequenceEventHandler = p_AsyncTaskSequenceEventHandler;
            _AsyncTaskEventHandler = new AsyncTaskEventHandler(OnSequenceBegin, OnSequenceProgress, OnTaskSuccess, OnTaskFailed, OnTaskCanceled);
        }

        #endregion

        #region <Callbacks>

        private void OnSequenceBegin(IAsyncTaskRequest p_Handler)
        {
            if (_CurrentAsyncTaskIndex == 0)
            {
                _OnSequenceBegin();
            }

            _AsyncTaskSequenceEventHandler.OnTaskBegin?.Invoke(_This, p_Handler);
        }
        
        private void _OnSequenceBegin()
        {
            _AsyncTaskSequenceEventHandler.OnSequenceBegin?.Invoke(_This);
        }

        private void OnSequenceProgress(IAsyncTaskRequest p_Handler, float p_Progress)
        {
            var currentSeqPreset = _AsyncTaskSequence[_CurrentAsyncTaskIndex];
            currentSeqPreset.ProgressRate = p_Progress;
            _AsyncTaskSequence[_CurrentAsyncTaskIndex] = currentSeqPreset;
            
            _AsyncTaskSequenceEventHandler.OnSequenceProgress?.Invoke(_This, currentSeqPreset.GetWeightAccumulatedProgress());
        }
        
        private void OnTaskSuccess(IAsyncTaskRequest p_Handler)
        {
            _AsyncTaskSequenceEventHandler.OnTaskSuccess?.Invoke(_This, p_Handler);
            CallNextAsyncTask();
        }
        
        private void OnTaskFailed(IAsyncTaskRequest p_Handler)
        {
            _AsyncTaskSequenceEventHandler.OnTaskFail?.Invoke(_This, p_Handler);
        }
                
        private void OnTaskCanceled(IAsyncTaskRequest p_Handler)
        {
            _AsyncTaskSequenceEventHandler.OnTaskCancel?.Invoke(_This, p_Handler);
        }
        
        private void OnSequenceComplete()
        {
            _CurrentPhase = TaskPhase.TaskTerminate;
            _AsyncTaskSequenceEventHandler.OnSequenceTerminate?.Invoke(_This);
        }

        #endregion
        
        #region <Methods>

        public (K, int, int) GetSequenceKey()
        {
            return (Symbol, _CurrentAsyncTaskIndex, _AsyncTaskCount - 1);
        }

        public void AddAsyncTask(AsyncTaskSequencePreset p_AsyncTask)
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.None:
                    p_AsyncTask.IAsyncTaskRequest.SetEventHandlerSet(_AsyncTaskEventHandler);
                    _AsyncTaskSequence.Add(p_AsyncTask);
                    break;
                case TaskPhase.TaskProgressing:
                case TaskPhase.TaskTerminate:
                    break;
            }
        }

        private bool IsAsyncTaskSequenceOver()
        {
            return _CurrentPhase == TaskPhase.TaskTerminate;
        }

        public float TryGetProgressRate()
        {
            if (IsAsyncTaskSequenceOver())
            {
                return _SequenceWeight;
            }
            else
            {
                var currentSeqPreset = _AsyncTaskSequence[_CurrentAsyncTaskIndex];
                return currentSeqPreset.GetWeightAccumulatedProgress();
            }
        }
        
        public void StartAsyncTaskSequence()
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.None:
                {
                    _AsyncTaskCount = _AsyncTaskSequence.Count;
                    if (_AsyncTaskCount > 0)
                    {
                        var _asyncTaskWholeWeight = 0f;
                        foreach (var preset in _AsyncTaskSequence)
                        {
                            _asyncTaskWholeWeight += preset.Weight;
                        }
                        
                        var _inv_AsyncTaskWholeWeight = _SequenceWeight / _asyncTaskWholeWeight; 
                        for (int i = 0; i < _AsyncTaskCount; i++)
                        {
                            var trySeqPreset = _AsyncTaskSequence[i];
                            trySeqPreset.Weight *= _inv_AsyncTaskWholeWeight;
                            if (i > 0)
                            {
                                var prevSeqPreset = _AsyncTaskSequence[i - 1];
                                trySeqPreset.AccumulatedProgress += prevSeqPreset.Weight + prevSeqPreset.AccumulatedProgress;
                            }
                            _AsyncTaskSequence[i] = trySeqPreset;
                        }
                        
                        _CurrentPhase = TaskPhase.TaskProgressing;
                        _CurrentAsyncTaskIndex = 0;
                        MoveNext();
                    }
                    else
                    {
                        _OnSequenceBegin();
                        OnSequenceComplete();
                    }
                }
                    break;
                case TaskPhase.TaskProgressing:
                case TaskPhase.TaskTerminate:
                    break;
            }
        }
        
        private void CallNextAsyncTask()
        {
            switch (_CurrentPhase)
            {
                case TaskPhase.TaskProgressing:
                    _CurrentAsyncTaskIndex++;
                    if (_CurrentAsyncTaskIndex < _AsyncTaskCount)
                    {
                        MoveNext();
                    }
                    else
                    {
                        OnSequenceComplete();
                    }
                    break;
                case TaskPhase.None:
                case TaskPhase.TaskTerminate:
                    break;
            }
        }
        
        private void MoveNext()
        {
            var tryCurrentTask = _AsyncTaskSequence[_CurrentAsyncTaskIndex];
            var handler = tryCurrentTask.IAsyncTaskRequest;

#if UNITY_EDITOR
            if (CustomDebug.PrintAsyncPoolTask)
            {
                if (_CurrentAsyncTaskIndex == _AsyncTaskCount)
                {
                    if (_AsyncTaskCount == 0)
                    {
                        Debug.Log($"AsyncTask [{Symbol}] Terminated at {Time.time}. But This Sequence has no any Task.");
                    }
                    else
                    {
                        Debug.Log($"AsyncTask [{Symbol}] Terminated at {Time.time}");
                    }
                }
                else
                {
                    Debug.Log($"AsyncTask [{Symbol}]: {_CurrentAsyncTaskIndex + 1}/{_AsyncTaskCount}, Reached Last at {Time.time}");
                    Debug.Log($"Task Description : {handler.GetDescription()}");
                }
            }
#endif
            handler.RunAsyncTask();
        }

        #endregion
    }
}