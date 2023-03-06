using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace k514
{
    public static class AsyncTaskTool
    {
        #region <Consts>

        /// <summary>
        /// AsyncOperation의 진행 상한
        /// </summary>
        public const float _AsyncOperationProgressUpperBound = 0.89f;
                
        /// <summary>
        /// AsyncOperation의 진행 상한 역수
        /// </summary>
        public const float _ReversedAsyncOperationProgressUpperBound = 1f / _AsyncOperationProgressUpperBound;
        
        #endregion

        #region <Interface>

        public interface IAsyncTaskRequestSpawner<Handler, Params>
        {
            void OnAsyncTaskHandlerRunEvent(Handler p_Handler, Params p_Params);
            void OnAsyncTaskHandlerRetrieved(Handler p_Handler, Params p_Params);
        }

        public interface IAsyncTaskWrapper<Result>
        {
            UniTask Run(CancellationToken p_Token);
            Result GetResult();
            float GetProgressRate();
        }

        #endregion
    }
    
    /// <summary>
    /// 비동기 작업 진행에 따라 처리해야할 이벤트 프리셋
    /// </summary>
    public struct AsyncTaskEventHandler
    {
        #region <Fields>

        public Action<IAsyncTaskRequest> OnTaskBegin;
        public Action<IAsyncTaskRequest, float> OnProgress;
        public Action<IAsyncTaskRequest> OnSuccess;
        public Action<IAsyncTaskRequest> OnFail;
        public Action<IAsyncTaskRequest> OnCancel;

        #endregion

        #region <Constructor>

        public AsyncTaskEventHandler(Action<IAsyncTaskRequest> p_OnTaskBegin,
            Action<IAsyncTaskRequest, float> p_OnProgress,
            Action<IAsyncTaskRequest> p_OnSuccess, Action<IAsyncTaskRequest> p_OnFail,
            Action<IAsyncTaskRequest> p_OnCancel)
        {
            OnTaskBegin = p_OnTaskBegin;
            OnProgress = p_OnProgress;
            OnSuccess = p_OnSuccess;
            OnFail = p_OnFail;
            OnCancel = p_OnCancel;
        }

        #endregion
    }
    
    public struct AsyncTaskSequencePreset
    {
        #region <Fields>

        public IAsyncTaskRequest IAsyncTaskRequest;
        public float Weight;
        public float AccumulatedProgress;
        public float ProgressRate;
        
        #endregion

        #region <Constructors>

        public AsyncTaskSequencePreset(IAsyncTaskRequest p_AsyncTaskRequest, float p_Weight)
        {
            IAsyncTaskRequest = p_AsyncTaskRequest;
            Weight = p_Weight;
            AccumulatedProgress = 0f;
            ProgressRate = 0f;
        }

        #endregion

        #region <Methods>

        public float GetWeightAccumulatedProgress()
        {
            return AccumulatedProgress + Weight * ProgressRate;
        }

        #endregion
    }

    public struct AsyncTaskSequenceEventHandler<M>
    {
        #region <Fields>

        public Action<M> OnSequenceBegin;
        public Action<M, float> OnSequenceProgress;
        public Action<M> OnSequenceTerminate;
        
        public Action<M, IAsyncTaskRequest> OnTaskBegin;
        public Action<M, IAsyncTaskRequest> OnTaskSuccess;
        public Action<M, IAsyncTaskRequest> OnTaskFail;
        public Action<M, IAsyncTaskRequest> OnTaskCancel;
        
        #endregion

        #region <Constructor>

        public AsyncTaskSequenceEventHandler
        (
            Action<M> p_OnSequenceBegin, Action<M, float> p_OnSequenceProgress, Action<M> p_OnSequenceTerminate,
            Action<M, IAsyncTaskRequest> p_OnTaskBegin, Action<M, IAsyncTaskRequest> p_OnTaskSuccess,
            Action<M, IAsyncTaskRequest> p_OnTaskFail, Action<M, IAsyncTaskRequest> p_OnTaskCancel
        )
        {
            OnSequenceBegin = p_OnSequenceBegin;
            OnSequenceProgress = p_OnSequenceProgress;
            OnSequenceTerminate = p_OnSequenceTerminate;
            
            OnTaskBegin = p_OnTaskBegin;
            OnTaskSuccess = p_OnTaskSuccess;
            OnTaskFail = p_OnTaskFail;
            OnTaskCancel = p_OnTaskCancel;
        }

        #endregion
    }
}