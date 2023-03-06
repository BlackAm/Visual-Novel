using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    public partial class BootStrap
    {
        #region <Callbacks>

        protected override void OnEntryPhaseLoop()
        {
        }
        
        protected override void _OnTerminatePhaseLoop()
        {
            SystemBoot.GetInstance.OnBootingSuccess();
        }

        #endregion

        #region <Method/Phase/BootStart>

        private async UniTask BootingStart(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("부팅 연출 및 시스템 테이블 로딩을 시작합니다.");
#endif
#if !SERVER_DRIVE
            SetPlayVideo();
#endif
            await SystemBoot.PreloadSystemTableAsync();
#if UNITY_EDITOR
            Debug.LogWarning("시스템 테이블 로딩에 성공했습니다.");
#endif
        }

        #endregion

        #region <Method/Phase/SOP>
        
        private async UniTask GameSingletonLoad0(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩을 시작합니다.(0)");
#endif
            await SystemBoot.GetInstance.PreloadGameSingletonAsync0();
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩에 성공했습니다.(0)");
#endif
        }
        
        private async UniTask GameSingletonLoad1(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩을 시작합니다.(1)");
#endif
            await SystemBoot.GetInstance.PreloadGameSingletonAsync1();
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩에 성공했습니다.(1)");
#endif
        }
        
        private async UniTask GameSingletonLoad2(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩을 시작합니다.(2)");
#endif
            await SystemBoot.GetInstance.PreloadGameSingletonAsync2();
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩에 성공했습니다.(2)");
#endif
        }
        
        private async UniTask GameSingletonLoad3(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩을 시작합니다.(3)");
#endif
            await SystemBoot.GetInstance.PreloadGameSingletonAsync3();
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩에 성공했습니다.(3)");
#endif
        }
        
        private async UniTask GameSingletonLoad4(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩을 시작합니다.(4)");
#endif
            await SystemBoot.GetInstance.PreloadGameSingletonAsync4();
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩에 성공했습니다.(4)");
#endif
        }
        
        private async UniTask GameSingletonLoad5(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩을 시작합니다.(5)");
#endif
            await SystemBoot.GetInstance.PreloadGameSingletonAsync5();
#if UNITY_EDITOR
            Debug.LogWarning("싱글톤 로딩에 성공했습니다.(5)");
#endif
        }
        
        #endregion        
        
        #region <Method/Phase/SOP2>

        private async UniTask GameTableLoad0(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩을 시작합니다.(0)");
#endif
            await SystemBoot.GetInstance.PreloadGameTableAsync0();
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩에 성공했습니다.(0)");
#endif
        }
                
        private async UniTask GameTableLoad1(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩을 시작합니다.(1)");
#endif
            await SystemBoot.GetInstance.PreloadGameTableAsync1();
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩에 성공했습니다.(1)");
#endif
        }
                
        private async UniTask GameTableLoad2(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩을 시작합니다.(2)");
#endif
            await SystemBoot.GetInstance.PreloadGameTableAsync2();
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩에 성공했습니다.(2)");
#endif
        }
                
        private async UniTask GameTableLoad3(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩을 시작합니다.(3)");
#endif
            await SystemBoot.GetInstance.PreloadGameTableAsync3();
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩에 성공했습니다.(3)");
#endif
        }
                
        private async UniTask GameTableLoad4(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩을 시작합니다.(4)");
#endif
            await SystemBoot.GetInstance.PreloadGameTableAsync4();
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩에 성공했습니다.(4)");
#endif
        }
                
        private async UniTask GameTableLoad5(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩을 시작합니다.(5)");
#endif
            await SystemBoot.GetInstance.PreloadGameTableAsync5();
#if UNITY_EDITOR
            Debug.LogWarning("게임 테이블 로딩에 성공했습니다.(5)");
#endif
        }

        #endregion

        #region <Method/Phase/SOP3>

#if UNITY_EDITOR
        private async UniTask BootEditorOnly(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            if (SystemFlag.IsSystemReleaseMode())
            {
                Debug.LogWarning("Notice : 배포 모드로 실행됬습니다.");
            }
            else
            {
                Debug.LogWarning("Notice : 개발 모드로 실행됬습니다.");
            }
    #if ON_GUI
            Debug.LogWarning("Notice : 테스트 매니저를 생성합니다.");
            await TestManager.GetInstance();
    #else
            await UniTask.CompletedTask;
    #endif
        }
#endif

        private async UniTask LoadMainGameTable(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("메인 게임 테이블 로딩을 시작합니다.");
#endif
            await SystemBoot.GetInstance.LoadMainGameTableAsync();
            Debug.LogError("메인 게임 테이블 로딩에 성공했습니다.");
#if UNITY_EDITOR
            Debug.LogWarning("메인 게임 테이블 로딩에 성공했습니다.");
#endif
        }
        
        #endregion
        
        #region <Method/Phase/SOP4>

        private async UniTask NetworkInit(IAsyncTaskRequest p_AsyncTaskRequest)
        {
#if UNITY_EDITOR
            Debug.LogWarning("시스템 네트워크 초기화를 시작합니다.");
#endif
            // await NetworkTool.InitNetworkPreset();
#if UNITY_EDITOR
            Debug.LogWarning("시스템 네트워크 초기화에 성공했습니다.");
#endif
        }
        
        private async UniTask CameraInit(IAsyncTaskRequest p_AsyncTaskRequest)
        {
            await UniTask.CompletedTask;
            
#if UNITY_EDITOR
            Debug.LogWarning("카메라 초기화를 시작합니다.");
#endif
            CameraManager.GetInstanceUnSafe.AddAudioListener();
#if UNITY_EDITOR
            Debug.LogWarning("카메라 초기화에 성공했습니다.");
#endif
        }

        #endregion
    }
}