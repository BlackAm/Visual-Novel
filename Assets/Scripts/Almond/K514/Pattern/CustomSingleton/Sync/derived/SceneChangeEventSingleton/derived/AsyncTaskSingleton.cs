using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 유니티 Scene 변경과 상호작용하는 싱글톤 추상 클래스
    /// </summary>
    public abstract class AsyncTaskSingleton<Me, Handler, Symbol, Params, Result> : SceneChangeEventSingleton<Me>, AsyncTaskTool.IAsyncTaskRequestSpawner<Handler, Params> 
        where Me : AsyncTaskSingleton<Me, Handler, Symbol, Params, Result>, new()
        where Handler : AsyncTaskRequestBase<Handler, Params, Result>, new()
    {
        #region <Fields>
        
        /// <summary>
        /// 핸들러 풀러
        /// </summary>
        private ObjectPooler<Handler> _HandlerPooler;
        
        /// <summary>
        /// 핸들러 중복 방지용 테이블
        /// </summary>
        private Dictionary<Symbol, Handler> _HandlerTable;

        #endregion

        #region <Callbacks>

        protected override void OnCreated()
        {
            _HandlerPooler = new ObjectPooler<Handler>();
            _HandlerTable = new Dictionary<Symbol, Handler>();
        }

        public override void OnInitiate()
        {
            _HandlerPooler.RetrieveAllObject();
        }
        
        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
        }

        public override void OnSceneTerminated()
        {
        }

        public override void OnSceneTransition()
        {
        }
        
        public void OnAsyncTaskHandlerRunEvent(Handler p_Handler, Params p_Params)
        {
            var trySymbol = GetTableSymbol(p_Params);
            if (!_HandlerTable.ContainsKey(trySymbol))
            {
                _HandlerTable.Add(trySymbol, p_Handler);
            }
        }

        public void OnAsyncTaskHandlerRetrieved(Handler p_Handler, Params p_Params)
        {
            var trySymbol = GetTableSymbol(p_Params);
            if (_HandlerTable.TryGetValue(trySymbol, out var o_Record) && ReferenceEquals(o_Record, p_Handler))
            {
                _HandlerTable.Remove(trySymbol);
            }
        }
        
        #endregion

        #region <Methods>

        protected abstract Symbol GetTableSymbol(Params p_Params);

        public async UniTask<(bool, Handler)> RunHandler(Params p_Params, float p_LowerBoundTime)
        {
            var trySymbol = GetTableSymbol(p_Params);
            if (_HandlerTable.TryGetValue(trySymbol, out var o_Handler))
            {
                var result = await o_Handler.RunAsyncTask(this, p_Params, p_LowerBoundTime);
                return (result, o_Handler);
            }
            else
            {
                var spawned = _HandlerPooler.GetObject();
                var result = await spawned.RunAsyncTask(this, p_Params, p_LowerBoundTime);
                return (result, spawned);
            }
        }
        
        public async UniTask<bool> RunHandler(AsyncTaskEventHandler p_EventPreset, Params p_Params, float p_LowerBoundTime)
        {
            var trySymbol = GetTableSymbol(p_Params);
            if (_HandlerTable.TryGetValue(trySymbol, out var o_Handler))
            {
                return await o_Handler.RunAsyncTask(this, p_Params, p_LowerBoundTime);
            }
            else
            {
                var spawned = _HandlerPooler.GetObject();
                spawned.SetEventHandlerSet(p_EventPreset);
                return await spawned.RunAsyncTask(this, p_Params, p_LowerBoundTime);
            }
        }

        public void AddAsyncTaskSequence<M, K>(AsyncTaskSequenceBase<M, K> p_TargetSequence, Params p_Params, float p_Weight, float p_LowerBoundTime) where M : AsyncTaskSequenceBase<M, K>
        {
            var trySymbol = GetTableSymbol(p_Params);
            if (_HandlerTable.TryGetValue(trySymbol, out var o_Handler))
            {
            }
            else
            {
                var spawned = _HandlerPooler.GetObject();
                spawned.SetPreset(this, p_Params, p_LowerBoundTime);
                p_TargetSequence.AddAsyncTask(new AsyncTaskSequencePreset(spawned, p_Weight));
            }
        }

        public void CancelRequest(Params p_Params)
        {
            var trySymbol = GetTableSymbol(p_Params);
            if (_HandlerTable.TryGetValue(trySymbol, out var o_Handler))
            {
                o_Handler.RetrieveObject();
            }
        }
        
        public void ClearAllRequest()
        {
            _HandlerPooler.RetrieveAllObject();
        }
        
        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
            _HandlerPooler.RetrieveAllObject();

            base.DisposeUnManaged();
        }

        #endregion
    }
}