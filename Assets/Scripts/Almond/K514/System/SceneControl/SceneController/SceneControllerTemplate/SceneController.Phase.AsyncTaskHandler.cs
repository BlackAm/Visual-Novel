namespace k514
{
    public partial class SceneController<M, K, T>
    {
        #region <Fields>

        /// <summary>
        /// 페이즈 이벤트 진행에 따른 콜백함수 프리셋
        /// </summary>
        protected AsyncTaskSequenceEventHandler<T> _AsyncTaskSequenceEventHandler;

        #endregion
        
        #region <Callbacks>

        private void OnCreateAsyncTaskHandler()
        {
            _AsyncTaskSequenceEventHandler = 
                new AsyncTaskSequenceEventHandler<T>
                (
                    OnSequenceBegin, OnSequenceProgress, OnSequenceTerminate, 
                    OnTaskBegin, OnTaskSuccess, OnTaskFail, OnTaskCancel
                );
        }

        protected abstract void OnSequenceBegin(T p_AsyncTaskSequence);
        protected void OnSequenceProgress(T p_AsyncTaskSequence, float p_Progress)
        {
#if !SERVER_DRIVE
            SetProgress(_WholeAccumulatedProgressRate + p_Progress);
#endif
        }
        protected abstract void OnSequenceTerminate(T p_AsyncTaskSequence);
        protected abstract void OnTaskBegin(T p_AsyncTaskSequence, IAsyncTaskRequest p_Handler);
        protected abstract void OnTaskSuccess(T p_AsyncTaskSequence, IAsyncTaskRequest p_Handler);
        protected abstract void OnTaskFail(T p_AsyncTaskSequence, IAsyncTaskRequest p_Handler);
        protected abstract void OnTaskCancel(T p_AsyncTaskSequence, IAsyncTaskRequest p_Handler);

        #endregion
    }
}