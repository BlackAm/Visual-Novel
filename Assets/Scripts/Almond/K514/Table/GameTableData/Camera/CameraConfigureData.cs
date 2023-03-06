#if !SERVER_DRIVE
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace k514
{
    /// <summary>
    /// 카메라를 가속하는 속도로 흔드는 연출에 관한 테이블 데이터 클래스
    /// </summary>
    public class CameraConfigureData : GameTable<CameraConfigureData, int, CameraConfigureData.TableRecord>
    {
        public class TableRecord : GameTableRecordBase
        {
            /// <summary>
            /// 카메라 뷰 컨트롤 [세로, 가로]회전 속도배율 마스크 벡터
            /// </summary>
            public Vector2 CameraRotationSpeedRateMask { get; private set; }
            
            /// <summary>
            /// 뷰컨트롤 줌인/아웃 속력
            /// </summary>
            public float ZoomSpeed { get; private set; }
            
            /// <summary>
            /// 뷰컨트롤 회전 속력
            /// </summary>
            public float RotationSpeed { get; private set; }
            
            /// <summary>
            /// 뷰컨트롤 드래그 지속시, 회전 속력 최저 배율
            /// </summary>
            public float RotationSpeedMinRate { get; private set; }
            
            /// <summary>
            /// 뷰컨트롤 드래그 지속시, 속력 증가 배율
            /// </summary>
            public float RotationSpeedRate { get; private set; }
            
            /// <summary>
            /// 뷰컨트롤 드래그 지속시, 회전 속력 최대 배율
            /// </summary>
            public float RotationSpeedMaxRate { get; private set; }

            /// <summary>
            /// 카메라 뷰 컨트롤 초기화시 선딜레이
            /// </summary>
            public uint ResetLerpPreMsec { get; private set; }            
            
            /// <summary>
            /// 카메라 뷰 컨트롤 초기화시 지속시간
            /// </summary>
            public uint ResetLerpMsec { get; private set; }

            /// <summary>
            /// 카메라 스무딩 중, 초점 추적 거리가 늘어나는 속력
            /// </summary>
            public float SmoothLerpRadiusIncreaseSpeed { get; private set; }
            
            /// <summary>
            /// 카메라 스무딩 중, 초점 추적 거리가 줄어드는 속력
            /// </summary>
            public float SmoothLerpRadiusDecreaseSpeed { get; private set; }
                        
            /// <summary>
            /// 카메라 초점 추적을 기술하는 오브젝트
            /// </summary>
            public CameraManager.TraceTargetPreset TraceTargetPreset { get; private set; }
            
            /// <summary>
            /// 카메라 은면제거 로직, far plane 평면 클리어 플래그
            /// </summary>
            public CameraClearFlags CameraClearFlags { get; private set; }

#if UNITY_EDITOR
            public override async UniTask OnRecordAdded()
            {
                await base.OnRecordAdded();

                // 터치조작과 달리 에디터에서 마우스 휠에 의한 줌은 속도가 느리므로 보정해준다.
                ZoomSpeed *= 10f;
            }
#endif
        }

        protected override string GetDefaultTableFileName()
        {
            return "CameraConfigure";
        }

        public override TableTool.TableFileType GetTableFileType()
        {
            return TableTool.TableFileType.Xml;
        }
    }
}
#endif