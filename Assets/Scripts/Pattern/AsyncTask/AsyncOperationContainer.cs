using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public struct AsyncOperationContainer
    {
        #region <Fields>

        private AsyncOperation _AsyncOperation;
        private Func<IAsyncTaskRequest, UniTask> _Action;
        private Func<IAsyncTaskRequest, UniTask<AsyncOperation>> _AsyncOperationAction;
        private AsyncTaskType _AsyncTaskType;
        private bool _AsyncTaskOverFlag;
        
        #endregion

        #region <Enum>

        private enum AsyncTaskType
        {
            Delegate,
            DelegateAsyncOp,
            AsyncOp
        }

        #endregion
        
        #region <Constructors>

        public AsyncOperationContainer(AsyncOperation p_AsyncOperation)
        {
            _Action = default;
            _AsyncOperation = p_AsyncOperation;
            _AsyncOperationAction = default;
            _AsyncTaskType = AsyncTaskType.AsyncOp;
            _AsyncTaskOverFlag = true;
        }
        
        public AsyncOperationContainer(Func<IAsyncTaskRequest, UniTask> p_Action)
        {
            _Action = p_Action;
            _AsyncOperation = default;
            _AsyncOperationAction = default;
            _AsyncTaskType = AsyncTaskType.Delegate;
            _AsyncTaskOverFlag = true;
        }
        
        public AsyncOperationContainer(Func<IAsyncTaskRequest, UniTask<AsyncOperation>> p__AsyncOperationAction)
        {
            _Action = default;
            _AsyncOperation = default;
            _AsyncOperationAction = p__AsyncOperationAction;
            _AsyncTaskType = AsyncTaskType.DelegateAsyncOp;
            _AsyncTaskOverFlag = true;
        }
        
        #endregion

        #region <Methods>
        
        public async UniTask RunAsync(IAsyncTaskRequest p_AsyncTaskRequest, CancellationToken p_Token)
        {
            _AsyncTaskOverFlag = false;

            switch (_AsyncTaskType)
            {
                case AsyncTaskType.Delegate:
                    await _Action(p_AsyncTaskRequest).WithCancellation(p_Token);
                    break;
                case AsyncTaskType.DelegateAsyncOp:
                    _AsyncOperation = await _AsyncOperationAction(p_AsyncTaskRequest).WithCancellation(p_Token);
                    _AsyncTaskType = AsyncTaskType.AsyncOp;
                    await _AsyncOperation.WithCancellation(p_Token);
                    break;
                case AsyncTaskType.AsyncOp:
                    await _AsyncOperation.WithCancellation(p_Token);
                    break;
            }

            _AsyncTaskOverFlag = true;
        }

        public float GetProgressRate()
        {
            switch (_AsyncTaskType)
            {
                default:
                case AsyncTaskType.Delegate:
                    return 1f;
                case AsyncTaskType.DelegateAsyncOp:
                    return 0f;
                case AsyncTaskType.AsyncOp:
                    return _AsyncOperation.progress;
            }
        }

        public bool IsAsyncTaskOver()
        {
            return _AsyncTaskOverFlag;
        }

        #endregion
    }
}