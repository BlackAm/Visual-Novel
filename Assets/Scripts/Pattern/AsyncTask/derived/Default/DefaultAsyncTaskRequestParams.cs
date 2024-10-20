using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public class DefaultAsyncTaskRequestParams
    {
        #region <Fields>

        public int Index;
        public Func<IAsyncTaskRequest, UniTask> Event;
        public Func<IAsyncTaskRequest, UniTask<AsyncOperation>> AsyncOperation;
#if UNITY_EDITOR
        public string Description;
#endif
        
        #endregion

        #region <Constructors>

        public DefaultAsyncTaskRequestParams(
            Func<IAsyncTaskRequest, UniTask> p_Event
#if UNITY_EDITOR
            , string p_Description
#endif
        )
        {
            Index = p_Event.GetHashCode();
            Event = p_Event;
            AsyncOperation = default;
#if UNITY_EDITOR
            Description = p_Description;
#endif
        }

        public DefaultAsyncTaskRequestParams(
            Func<IAsyncTaskRequest, UniTask<AsyncOperation>> p_AsyncOperation
#if UNITY_EDITOR
            , string p_Description
#endif
        )
        {
            Index = p_AsyncOperation.GetHashCode();
            Event = default;
            AsyncOperation = p_AsyncOperation;
#if UNITY_EDITOR
            Description = p_Description;
#endif
        }
        
        #endregion

        #region <Operator>

#if UNITY_EDITOR
        public override string ToString()
        {
            return Description;
        }
#endif

        #endregion
    }
}