using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BlackAm
{
    public partial class SceneController<M, K, T>
    {
        #region <Callbacks>

        protected abstract void OnEntryPhaseLoop();
        protected abstract void _OnTerminatePhaseLoop();

        private async void OnTerminatePhaseLoop()
        {
            TurnOffLocalAudioListener();
            _OnTerminatePhaseLoop();
            await UniTask.NextFrame();
            Dispose();
        }

        #endregion
        
        #region <Methods>

        protected void BridgeEntryPhaseLoop()
        {
#if SERVER_DRIVE
            OnEntryPhaseLoop();
#else
            if (ReferenceEquals(null, _Fader))
            {
                OnEntryPhaseLoop();
            }
            else
            {
                _Fader.CastEntryAnimation(OnEntryPhaseLoop);
            }
#endif
        }
        
        protected void BridgeTerminatePhaseLoop()
        {
#if SERVER_DRIVE
            OnTerminatePhaseLoop();
#else
            if (ReferenceEquals(null, _Fader))
            {
                OnTerminatePhaseLoop();
            }
            else
            {
                _Fader.CastEscapeAnimation(OnTerminatePhaseLoop);
            }
#endif
        }

#if UNITY_EDITOR
        protected async UniTask VoidTest(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            Debug.LogWarning("[Editor Only] 대기");
            await UniTask.Delay(1000);
            Debug.LogWarning("[Editor Only] 대기 종료");
        }
#endif
        
        #endregion
    }
}