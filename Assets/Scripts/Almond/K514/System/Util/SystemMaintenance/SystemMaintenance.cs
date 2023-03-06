using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace k514
{
    public static partial class SystemMaintenance
    {
        #region <Consts>

        /// <summary>
        /// 전체 비동기 테스크 취소용 토큰 소스
        /// </summary>
        private static CancellationTokenSource _SystemTaskCancellationTokenSource;
        
        /// <summary>
        /// 전체 비동기 테스크 취소용 토큰
        /// </summary>
        public static CancellationToken _SystemTaskCancellationToken { get; private set; }

        /// <summary>
        /// 패치파일 확장자
        /// </summary>
        public const string PatchFileExt = ".kdm";
        
        /// <summary>
        /// 업데이트에 사용하지 않을 확장자 그룹
        /// </summary>
        public static readonly List<string> ExtBlackList = new List<string> {".meta"};

        /// <summary>
        /// 에셋번들 메니피스트 파일 확장자
        /// </summary>
        public const string BundleManifestFileExt = ".manifest";

#if UNITY_EDITOR
        /// <summary>
        /// 해당 프로젝트가 지원하는 플랫폼 리스트
        /// </summary>
        public static List<BuildTargetGroup> BuildTargetGroups = new List<BuildTargetGroup>
            {BuildTargetGroup.Android, BuildTargetGroup.Standalone, BuildTargetGroup.Switch};

        /// <summary>
        /// 유니티 기본 셰이더 이름 목록
        /// </summary>
        public static string[] DefaultShaderNameList 
            = new[]
            {
                "Legacy Shaders/Diffuse",
                "Hidden/CubeBlur",
                "Hidden/CubeCopy",
                "Hidden/CubeBlend",
                "Sprites/Default",
                "UI/Default",
                
/*
                "Hidden/VideoComposite",
                "Hidden/VideoDecode",
                "Hidden/Compositing",
                "Hidden/VideoDecodeAndroid"
*/
        };
#endif

        #endregion
        
        #region <Methods>
        
        /// <summary>
        /// 시스템 동작 관련 디렉터리를 생성하는 메서드
        /// </summary>
        public static void InitSystemMaintenance()
        {
            CancelAllTask();
#if UNITY_EDITOR
            InitSystemConfig();
            InitEditorDirectory();
#endif
            InitBundleDirectory();
        }

        /// <summary>
        /// 현재 실행되는 모드가 실제 플랫폼에서 실행되는 로직을 따라야하는지 여부를 리턴하는 메서드
        /// 다음과 같은 조건에서 참이다.
        ///
        ///    1. 유니티 에디터에서 실행되지만 배포 모드인 경우
        ///    2. 실제 플랫폼에서 실행중인 경우
        /// 
        /// </summary>
        public static bool IsPlayOnTargetPlatform()
        {
#if UNITY_EDITOR
            // Case 1
            return SystemFlag.IsSystemReleaseMode();
#else
            // Case 2
            return true;
#endif
        }
        
        /// <summary>
        /// Headless 모드로 실행되어있는지 검증하는 논리 메서드
        /// </summary>
        public static bool IsHeadless() 
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }

        /// <summary>
        /// 게임 시스템을 초기화시키는 메서드
        /// </summary>
        public static void ReleaseSystem()
        {
            CancelAllTask();
            SingletonTool.ClearActiveSingleton();
        }

        public static void CancelAllTask()
        {
            if (ReferenceEquals(null, _SystemTaskCancellationTokenSource))
            {
                _SystemTaskCancellationTokenSource = new CancellationTokenSource();
                _SystemTaskCancellationToken = _SystemTaskCancellationTokenSource.Token;
            }
            else
            {
                if (_SystemTaskCancellationTokenSource.IsCancellationRequested)
                {
                }
                else
                {
                    _SystemTaskCancellationTokenSource.Cancel();
                }
                
                _SystemTaskCancellationTokenSource = new CancellationTokenSource();
                _SystemTaskCancellationToken = _SystemTaskCancellationTokenSource.Token;
            }
        }
        
        /// <summary>
        /// 업데이트에 포함시키지 않는 확장자를 가진 파일명인지 검증하는 메서드
        /// </summary>
        private static bool IsBlockedExt(string fileName)
        {
            foreach (var blockedExt in ExtBlackList)
            {
                if (fileName.EndsWith(blockedExt)) return true;
            }
            return false;
        }
        
        #endregion
    }
}