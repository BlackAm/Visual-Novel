using Cysharp.Threading.Tasks;

namespace BlackAm
{
    /// <summary>
    /// 다수의 비동기 작업의 순서를 관리하는 구현체
    /// </summary>
    public class AsyncBootingSequence : AsyncTaskSequenceBase<AsyncBootingSequence, BootStrap.BootingProgressPhase>
    {
        public AsyncBootingSequence(BootStrap.BootingProgressPhase p_Symbol, AsyncTaskSequenceEventHandler<AsyncBootingSequence> p_AsyncTaskSequenceEventHandler, float p_SequenceWeight) : base(p_Symbol, p_AsyncTaskSequenceEventHandler, p_SequenceWeight)
        {
        }
    }
    
    /// <summary>
    /// 비동기 작업 내용을 기술하는 구현체
    /// </summary>
    public class AsyncBootingTaskRequestHandler : AsyncTaskRequestBase<AsyncBootingTaskRequestHandler, DefaultAsyncTaskRequestParams, DefaultAsyncTaskRequestResult>
    {
        protected override bool _TryCheckTaskSuccess()
        {
            return true;
        }

        protected override async UniTask SpawnAsyncTask()
        {
            var tryEvent = _Params.Event;
            if (ReferenceEquals(null, tryEvent))
            {
                _AsyncTaskContainer = new AsyncOperationContainer(_Params.AsyncOperation);
            }
            else
            {
                _AsyncTaskContainer = new AsyncOperationContainer(tryEvent);
            }
            
            await UniTask.CompletedTask;
        }
    }
    
    /// <summary>
    /// 비동기 작업을 생성하고 제어하는 구현체
    /// </summary>
    public class AsyncBootingTaskRequestManager : AsyncTaskSingleton<AsyncBootingTaskRequestManager, AsyncBootingTaskRequestHandler, int, DefaultAsyncTaskRequestParams, DefaultAsyncTaskRequestResult>
    {
        protected override int GetTableSymbol(DefaultAsyncTaskRequestParams p_Params)
        {
            return p_Params.Index;
        }
    }
}