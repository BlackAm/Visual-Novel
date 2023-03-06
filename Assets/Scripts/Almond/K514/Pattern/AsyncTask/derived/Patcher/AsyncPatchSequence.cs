using Cysharp.Threading.Tasks;

namespace k514
{
    /// <summary>
    /// 다수의 비동기 작업의 순서를 관리하는 구현체
    /// </summary>
    /*public class AsyncPatchSequence : AsyncTaskSequenceBase<AsyncPatchSequence, Patcher.PatchProgressPhase>
    {
        public AsyncPatchSequence(Patcher.PatchProgressPhase p_Symbol, AsyncTaskSequenceEventHandler<AsyncPatchSequence> p_AsyncTaskSequenceEventHandler, float p_SequenceWeight) : base(p_Symbol, p_AsyncTaskSequenceEventHandler, p_SequenceWeight)
        {
        }
    }
    
    /// <summary>
    /// 비동기 작업 내용을 기술하는 구현체
    /// </summary>
    public class AsyncPatchTaskRequestHandler : AsyncTaskRequestBase<AsyncPatchTaskRequestHandler, DefaultAsyncTaskRequestParams, AsyncPatchTaskRequestResult>
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
    public class AsyncPatchTaskRequestManager : AsyncTaskSingleton<AsyncPatchTaskRequestManager, AsyncPatchTaskRequestHandler, int, DefaultAsyncTaskRequestParams, AsyncPatchTaskRequestResult>
    {
        protected override int GetTableSymbol(DefaultAsyncTaskRequestParams p_Params)
        {
            return p_Params.Index;
        }
    }
    
    /// <summary>
    /// 비동기 작업 결과물을 기술하는 구조체
    /// </summary>
    public struct AsyncPatchTaskRequestResult
    {
        #region <Fields>

        public PatchTool.PatchPreset PatchPreset;
        
        #endregion

        #region <Methods>

        public void SetVersion(int p_Client, int p_Server)
        {
            PatchPreset = new PatchTool.PatchPreset(p_Client, p_Server);
        }

        public void SetPatchMode(PatchTool.PatchMode p_PatchMode)
        {
            PatchPreset.SwitchPatchMode(p_PatchMode);
        }

        #endregion
    }*/
}