using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public abstract partial class SceneController<M, K, T> : UnitySingleton<SceneController<M, K, T>> where T : IAsyncTaskSequence
    {
        #region <Fields>

        [SerializeField] protected GameObject _LocalSceneCameraGameObject;
        [SerializeField] protected AudioListener _localSceneCameraAudioListener;
        protected SceneControllerTool.SystemSceneType _SceneType;
        
        #endregion
        
        #region <Callbacks>

        protected override void OnCreated()
        {
#if !SERVER_DRIVE
            OnCreatePanel();
            OnCreateAnimation();
#endif
            OnCreateAsyncTaskHandler();
            OnCreatePhase();
        }

        public override void OnInitiate()
        {
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;
#if !SERVER_DRIVE
            OnUpdatePanel(deltaTime);
            OnUpdateAnimation(deltaTime);
#endif
        }

        #endregion

        #region <Methods>

        protected void TurnOffLocalCamera()
        {
            try
            {
                _LocalSceneCameraGameObject.SetActive(false);
            }
            catch
            {
                //
            }
        }

        protected void TurnOffLocalAudioListener()
        {
            try
            {
                _localSceneCameraAudioListener.enabled = false;
            }
            catch
            {
                //
            }
        }
        
        #endregion
        
        #region <Disposable>

        /// <summary>
        /// 해당 오브젝트를 제거한다.
        /// 오브젝트 제거는 매 로딩 종료시에 일어나므로, 해당 파기메서드의 호출은 반드시 게임 시스템의 종료를 의미하는 것은 아니다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
#if !SERVER_DRIVE
            if (_ProgressBar.IsValid)
            {
                _ProgressBar.Dispose();
            }
            _ProgressBar = default;
            
            if (_Fader != null)
            {
                _Fader.Dispose();
                _Fader = null;
            }
#endif
            base.DisposeUnManaged();
            
            Destroy(gameObject);
        }

        #endregion
    }
}