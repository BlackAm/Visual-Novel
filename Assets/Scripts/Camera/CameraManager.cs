#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    /// <summary>
    /// 카메라 수명 제어 및 카메라 연출 이벤트를 제어하는 싱글톤 클래스
    /// </summary>
    public partial class CameraManager : SceneChangeEventAsyncSingleton<CameraManager>
    {
        #region <Consts>
             
        /// <summary>
        /// 래퍼 타입 중, 아핀 변환 구조를 가지는 타입 리스트
        /// </summary>
        private static readonly List<CameraWrapperType> AffineWrapperTypeList = new List<CameraWrapperType>
        {
            CameraWrapperType.Root,
            CameraWrapperType.Directional_0
        };

        #endregion
        
        #region <Fields>

        /// <summary>
        /// [카메라 래퍼 타입, 카메라 래퍼 Transform] 컬렉션
        /// </summary>
        private Dictionary<CameraWrapperType, Transform> CameraWrapperTransformCollection;

        /// <summary>
        /// 해당 카메라 매니저가 이벤트를 등록하는 이벤트 타이머
        /// </summary>
        private EventTimer CameraEventTimer;

        /// <summary>
        /// 카메라가 현재 씬에서 동작해야 하는지를 표시하는 플래그
        /// </summary>
        private bool IsCameraValid;

        /// <summary>
        /// 씬 설정이 변경된 경우, 해당 이벤트를 수신받는 오브젝트
        /// </summary>
        public SceneVariableChangeEventReceiver SceneVariableChangeEventReceiver { get; private set; }

        /// <summary>
        /// 현재 적용중인 씬 변위 세팅 레코드
        /// </summary>
        private SceneVariableData.TableRecord _CurrentSceneVariableRecord;

        /// <summary>
        /// 현재 적용 중인 카메라 설정 레코드
        /// </summary>
        private CameraConfigureData.TableRecord _CurrentSceneCameraConfigure;

        #endregion

        #region <Enums>

        /// <summary>
        /// 메인 카메라 연출을 위해 카메라를 감싸는 래퍼 Transform 타입
        /// </summary>
        public enum CameraWrapperType
        {
            /// <summary>
            /// 카메라의 RootWrapper가 향하는 기본 방향 == 카메라의 시선 끝에 위치한 Transform
            /// </summary>
            Base,
            
            /// <summary>
            /// 카메라의 RootWrapper, 항상 Base 쪽을 일정한 각도와 거리에서 바라보도록 위치가 정해진다.
            /// </summary>
            Root,
            
            /// <summary>
            /// 카메라의 일시적인 아핀변환
            /// </summary>
            Directional_0,
                       
            /// <summary>
            /// 카메라 흔들림
            /// </summary>
            Shake,
        }

        public CameraWrapperType[] CameraWrapperTypeEnumerator;
        
        #endregion
        
        #region <Callbacks>

        protected override async UniTask OnCreated()
        {
            // 카메라는 맨 마지막에 활성화되도록 우선도를 100으로 설정한다.
            Priority = 100;
            
            // 관련 컬렉션 초기화
            CameraWrapperTransformCollection = new Dictionary<CameraWrapperType, Transform>();
            CameraAffineTransformCollection = new Dictionary<CameraWrapperType, CameraAffineTransform>();
            
            // 이벤트 수신 오브젝트 초기화
            SceneVariableChangeEventReceiver =
                SceneEnvironmentManager.GetInstance
                    .GetEventReceiver<SceneVariableChangeEventReceiver>(
                        SceneEnvironmentManager.SceneVariableEventType.OnSceneVariableChanged, OnMapVariableChanged);
            
            // 반복문 순환자 초기화
            CameraWrapperTypeEnumerator = SystemTool.GetEnumEnumerator<CameraWrapperType>(SystemTool.GetEnumeratorType.GetAll);
            CameraAffineTypeEnumerator = SystemTool.GetEnumEnumerator<CameraAffineTransformType>(SystemTool.GetEnumeratorType.GetAll);

            // 해당 카메라에서 사용할 EventTimer 선정
            CameraEventTimer = SystemBoot.GameEventTimer;
            
            await UniTask.SwitchToMainThread();
            
            // 래퍼 타입 순서에 맞게 각 래퍼 Transform을 계층 관계로 구성시킨다.
            var wrapperCount = CameraWrapperTypeEnumerator.Length;
            for (int i = 0; i < wrapperCount; i++)
            {
                Transform spawnedTransform = null;
                CameraAffineTransform spawnedCameraAffineObject = null;
                var targetWrapperType = CameraWrapperTypeEnumerator[i];
                var isAffineWrapper = AffineWrapperTypeList.Contains(targetWrapperType);
                
                if (isAffineWrapper)
                {
                    spawnedCameraAffineObject = new CameraAffineTransform(targetWrapperType); 
                    spawnedTransform = spawnedCameraAffineObject.Head;
                }
                else
                {
                    spawnedTransform = new GameObject(targetWrapperType.ToString()).transform;
                }
                
                CameraWrapperTransformCollection[targetWrapperType] = spawnedTransform;
                CameraAffineTransformCollection.Add(targetWrapperType, spawnedCameraAffineObject);
                
                if (i != 0)
                {
                    var prevType = CameraWrapperTypeEnumerator[i - 1];
                    Transform prevTransform = null;
                    
                    if (AffineWrapperTypeList.Contains(prevType))
                    {
                        prevTransform = CameraAffineTransformCollection[prevType].Rear;
                    }
                    else
                    {
                        prevTransform = CameraWrapperTransformCollection[prevType];
                    }

                    spawnedTransform.SetParent(prevTransform, false);
                }
            }

            // 각 컬렉션에 들어가있는 Wrapper Transform의 별명 레퍼런스 초기화
            BaseWrapper = CameraWrapperTransformCollection[CameraWrapperType.Base];
            ViewControlWrapper = CameraWrapperTransformCollection[CameraWrapperType.Root];
            ShakeWrapper = CameraWrapperTransformCollection[CameraWrapperType.Shake];

            // 각 기능별로 구분된 부분 클래스 초기화
            OnCreateMainCameraPartial();
            OnCreateEventPartial();
            OnCreateAffineTransformPartial();
            OnCreateBaseWrapperPartial();
            OnCreateViewControlPartial();
            OnCreateFixingFocusPartial();
            OnCreateShakeWrapperPartial();
            OnCreateMathPartial();
            OnCreateRenderPartial();

            OnCreateExtraPartial();
#if APPLY_PPS
            if (Application.isPlaying || !Application.isEditor)
            {
                OnCreatePPSWrapperPartial();
            }
#endif

#if UNITY_EDITOR
            if (!Application.isPlaying) 
            {
                return;
            }
#endif
            // 카메라 오브젝트를 불변으로 만들어준다.
            Object.DontDestroyOnLoad(BaseWrapper.gameObject);
        }

        public override async UniTask OnInitiate()
        {
            await UniTask.CompletedTask;
        }

        public override async UniTask OnScenePreload()
        {
            await UniTask.CompletedTask;
        }

        public override void OnSceneStarted()
        {
            IsCameraValid = true;

            SetAudioListener(true);
            ResetCullingMask();
            ResetViewControl();

            // 카메라 모드가 없는 상태에서 씬이 시작되는 경우
            if (CurrentCameraMode == CameraMode.None)
            {
                var (pos, rot) = SceneEnvironmentManager.GetInstance.GetPlayerStartPreset();
                var (valid, correctPos) = ObjectDeployTool.CorrectAffinePreset(pos, ObjectDeployTool.ObjectDeploySurfaceDeployType.ForceSurface);
                BaseWrapper.position = valid ? correctPos.Position : pos;
            }
        }

        public override void OnSceneTerminated()
        {
            IsCameraValid = false;
            
            OnResetBaseWrapperPartial();
            OnResetStateWrapperPartial();
            OnResetShakeWrapperPartial();
            OnResetAffineTransformPartial();
        }

        public override void OnSceneTransition()
        {
            // 월드 카메라 초기화
            SetCameraBlind();
            SetCameraBasePositionZero();
            SetAudioListener(false);
        }

        private void OnMapVariableChanged(SceneEnvironmentManager.SceneVariableEventType p_EventType,
            SceneVariableData.TableRecord p_Record)
        {
            var currentSceneSettingData = SceneEnvironmentManager.GetInstance.CurrentSceneSettingRecord;
            _CurrentSceneCameraConfigure = CameraConfigureData.GetInstanceUnSafe.GetTableData(currentSceneSettingData.CameraConfigureIndex);
            _CurrentSceneVariableRecord = p_Record;
            
            UpdateCameraSetting();
        }

        public void OnUpdateCameraManager(float p_DeltaTime)
        {    
            if (IsCameraIdle) return;

            switch (CurrentCameraMode)
            {
                case CameraMode.None:
                    break;
                case CameraMode.ObjectTracing:
                    OnUpdateTracing(p_DeltaTime);
                    break;
                case CameraMode.ObjectTracingSmoothLerp:
                    OnUpdateSmoothTracing(p_DeltaTime);
                    break;
                case CameraMode.FirstPersonTracing:
                    OnUpdateFirstPersonTracing(p_DeltaTime);
                    break;
            }

            switch (_ViewControlRotationDirection)
            {
                case ArrowType.None:
                    break;
                case ArrowType.SoloUp:
                case ArrowType.SoloLeft:
                case ArrowType.SoloDown:
                case ArrowType.SoloRight:
                case ArrowType.UpLeft:
                case ArrowType.LeftDown:
                case ArrowType.DownRight:
                case ArrowType.RightUp:
                    OnUpdateViewControl(p_DeltaTime);
                    break;
            }
        }

        #endregion

        #region <Methods>

        /// <summary>
        /// 현재 씬에 대한 레코드로부터 카메라를 초기화 시키는 메서드
        /// </summary>
        private void UpdateCameraSetting()
        {
            UpdateMainCameraSetting();
            UpdateAffineSetting();
#if APPLY_PPS
            UpdatePostProcessStackSetting();
#endif
            UpdateTraceTargetSetting();
            UpdateRenderSetting();
        }

        public void SetCameraBasePositionZero()
        {
            BaseWrapper.position = Vector3.zero;
        }

        #endregion
    }
}
#endif