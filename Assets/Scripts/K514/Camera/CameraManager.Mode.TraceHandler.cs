#if !SERVER_DRIVE
using UnityEngine;

namespace BlackAm
{
    public partial class CameraManager
    {
        #region <Consts>

        private const int __FirstPersonFocus_ViewControl_Msec = 100;

        #endregion
        
        #region <Callbacks>

        /// <summary>
        /// 일반 추적 모드시 타겟 transform의 position과 카메라 base wrapper position을 동기화 시킴
        /// </summary>
        public void OnUpdateTracing(float p_DeltaTime)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && ReferenceEquals(TracingTarget, null)) return;
#endif
            BaseWrapper.position = TracingTarget._Transform.position;
            OnCheckTargetMove();
        }

        /// <summary>
        /// 현재 타겟 위치와 베이스 Transform 사이의 변위에 [0, 1] 구간의 smoothdamp를 곱한 값 만큼을 이동하고
        /// 해당 smoothdamp 값을 서서히 증감 시키는 것으로 카메라의 부드러운 이동을 연출시키는 메서드
        ///
        /// 1차 구현 : 타겟 오브젝트의 이동방향에 음수배를 하여 일정 거리에 카메라를 위치시키는 방식,
        /// 이동중이면 반경을 늘리고, 정지하면 반경을 줄이는 것으로 부드러운 연출이 가능했으나
        /// 방향이 바뀌면 카메라가 멀리있을수록 급격하게 움직이므로 폐기
        ///
        /// 2차 구현 : 카메라로부터 타겟 오브젝트로의 방향 벡터를 기준으로 1차 구현 방식을 사용.
        /// 또한 타겟 오브젝트의 방향이 변할 때, 반지름을 줄이고 그 외의 경우 반지름을 늘이는 방식 채용.
        /// 그러나 카메라의 base transform과 타겟 오브젝트가 겹칠 때, 즉 타겟 오브젝트가 방향을 반대로 걸어서
        /// 카메라의 base transform이랑 겹칠 때 카메라가 엉뚱한 방향으로 궤도를 그리는 문제가 발생.
        ///
        /// 3차 구현 : 2차 구현에서 엉뚱한 궤도 문제는 컨트롤러 문제였다. 해당 컨트롤러 문제를 해결한 후에
        /// 반대방향으로 이동하는 경우 가끔씩 끊기는 이동을 하는 문제가 남았다.
        ///
        /// 이슈 1 : base wrapper랑 타겟 오브젝트 좌표가 근접한 경우, 두 사이 벡터가 노멀라이즈 되는 과정에서
        /// 엉뚱한 방향으로 튀는 문제. safeVector 메서드를 이용해 해결
        ///
        /// 이슈 2 : 카메라~타겟 벡터와 타겟이동방향 벡터 방향이 일치하지 않는 경우를 2차, 3차 방식으로 효율적으로 케어할 방법이 없음.
        /// 다시 1차 구현 기반으로 회귀
        ///
        /// 4차 구현 : 1차 구현의 유일한 문제점인, 급격한 카메라 위치 변경을 막기 위해 카메라의 위치를 선정하는 타겟 오브젝트 이동하는
        /// 벡터를 러프하는 방식으로 개선할 것.
        ///
        /// 그러나 러프라고 하는게 시점과 끝점이 고정되어야 지정한 시구간동안 연출이 가능한건데, 지속적으로 각도가 바뀌는 환경에서
        /// 러프가 예상보다 짧게 = 빠르게 움직임므로 부자연스러움. 폐기
        ///
        /// 5차 구현 : 1차 구현의 유일한 문제점을 해결하기 위해 다음과 같은 방식 적용
        /// 1. 방향이 변하지 않은 경우 1차구현 방식 적용
        /// 2. 방향이 바뀐 경우, 반경을 0으로 하는 시퀀스를 지속하다. 0이 된 경우, 해당 시점의 방향을 기준으로 운동함
        ///
        /// 5차 구현 방법으로 해결되기 했는데, 좀 이상하게 움직임
        /// 위로 가다가 위오른쪽을 가게 하면, 위 => 위오른쪽이 아닌 위 => 중앙 => 위오른쪽 으로 무조건 중앙을 거쳐서 카메라가 움직이게 되버림;
        ///
        /// 6차 구현 : 5차구현의 한계를 해결하기 위해, 4차 구현 당시 구현했던 궤도 러프 구조체를 추가.
        ///
        /// 다만 러프 각도가 90도를 넘어가면 과하게 회전하는 문제가 발생(반대방향으로 가면 반원궤도를 그려야 하니까)
        ///
        /// 7차 구현 : 다시 2차 구현으로 돌아가서, 카메라를 중심으로 하되
        /// 기존의 2차 한계가 3차 구현에서 해결한 컨트롤러 문제였으므로, 해당 방식으로 재시도
        ///
        /// 타겟 오브젝트의 속도가 크면, 방향을 반대로 돌렸을 때 _Current_Camera_TracingObject_DirectionState를 카메라 위치가 갱신되기 전에
        /// 갱신하기 때문에 반지름이 충분히 줄어들지 않았는데 상태가 전이되는 문제가 있었음. 
        ///
        /// 8차 구현 : x, y, z 값에 대해 타겟 유닛이 해당 방향으로 이동하면 각 값을 그 방향으로 증가시키는 방식0
        /// 각 성분은 상/하한을 가지고 있으며, 움직이지 않는 경우에는 각 값을 일정량 줄이는 방식.
        ///
        /// 9차 구현 : 8차에서 부드럽게 움직이는 문제는 해결됬지만 급격하게 방향이 바뀌는 경우와 조금 방향이 바뀌는 경우에 방향이 보정되는데 동일한
        /// 시간이 걸리므로 방향이 자주 바뀌는 환경에서 부자연스러운 연출이 나옴.
        ///
        /// => 속도가 충분히 큰 물체를 추적하는 경우에 x축 z축 이동이 따로 일어나서, x축 중심으로 이동하다가 z축 중심으로 이동하게 애니메이션이 변화하는
        /// 현상이 발생해서 움직임이 부자연스러운 문제가 있음. 
        /// 
        /// </summary>
        public void OnUpdateSmoothTracing(float p_DeltaTime)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && ReferenceEquals(TracingTarget, null)) return;
#endif
            var targetPosition = TracingTarget._Transform.position;
            var deltaTime = p_DeltaTime;
            
            // 추적 대상의 이동이 감지되는 경우
            if (TracingTarget.IsPositionChanged())
            {
                // 추적 대상이 이동하고 있을 때에는 지연 추적을 위해 _SmoothTracingCameraOffset 증가 값이 _SmoothLerpRadiusIncreaseSpeed/s 로 고정된다.
                _SmoothTracingCameraOffset -= TracingTarget._PositionState._CurrentDeltaDirectionUnitVector * _CurrentSceneCameraConfigure.SmoothLerpRadiusIncreaseSpeed * deltaTime;
                // 최대 지연 추적 범위가 되도록 클램프해준다.
                _SmoothTracingCameraOffset = _SmoothTracingCameraOffset.ClampVector(_TracingSmoothRadiusNegative, _TracingSmoothRadius);
            }
            else
            {
                var decreaseRadiusDelta = _CurrentSceneCameraConfigure.SmoothLerpRadiusDecreaseSpeed * deltaTime;
                _SmoothTracingCameraOffset = _SmoothTracingCameraOffset.ZeroLerpSameRate(decreaseRadiusDelta);
            }
            
            BaseWrapper.position = targetPosition + _SmoothTracingCameraOffset;
            OnCheckTargetMove();
        }

        /// <summary>
        /// 1인칭 시점 추적 모드
        /// </summary>
        public void OnUpdateFirstPersonTracing(float p_DeltaTime)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && ReferenceEquals(TracingTarget, null)) return;
#endif
            BaseWrapper.position = TracingTarget._Transform.position;
            var lookVector = TracingTarget._Transform.forward;
            
            // 방향을 즉시 동기화 시키면, 화면이 갑자기 움직여서 어지럽기 때문에 러프시킨다.
            SetDegreeLerp(CameraWrapperType.Root, lookVector, 0, __FirstPersonFocus_ViewControl_Msec);
            SetCameraDistanceZoomLerp(CameraWrapperType.Root, 0f, 0, __FirstPersonFocus_ViewControl_Msec);
            OnCheckTargetMove();
        }
        
        #endregion
    }
}
#endif