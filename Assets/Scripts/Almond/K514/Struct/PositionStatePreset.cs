using UnityEngine;

namespace k514
{
    /// <summary>
    /// 특정 벡터의 현재~이전 좌표 및 사이 방향, 변위량을 계산하여 이동여부 등의 정보를 정리하는 클래스
    /// </summary>
    public class PositionStatePreset
    {
        #region <Consts>

        private const float _PositionChangeThreshold = 0.33f;

        #endregion
        
        #region <Fields>

        public Vector3 _PrevPosition;
        public Vector3 _CurrentPosition;
        public Vector3 _PrevThresholdPosition;

        /// <summary>
        /// 해당 값은 벡터가 마지막으로 움직였던 값을 갖는다. 즉 isMoved == false 인 경우, 값이 0이 되는게 아니라
        /// 이전에 지녔던 값을 그대로 갖는다.
        /// </summary>
        public Vector3 _CurrentDeltaDirection;
        public Vector3 _CurrentDeltaDirectionUnitVector;
        
        /// <summary>
        /// 해당 값은 벡터가 마지막으로 움직였던 값을 갖는다. 즉 isMoved == false 인 경우, 값이 0이 되는게 아니라
        /// 이전에 지녔던 값을 그대로 갖는다.
        /// </summary>
        public float _CurrentDeltaDirectionSqrMagnitude;
        
        /// <summary>
        /// 좌표가 이동되었는지 체크하는 플래그
        /// </summary>
        public bool _IsMoved;

        /// <summary>
        /// 지정된 임계값 이상 움직인 경우에 세트되는 플래그
        /// </summary>
        public bool _IsThresholdMoved;

        /// <summary>
        /// 좌표의 y성분이 변했는지 체크하는 플래그
        /// </summary>
        public bool _IsHeightMoved;
        
        /// <summary>
        /// 지정한 Transform의 움직임이 발생한 경우, 위의 IsMoved 플래그를 세트할 임계점 상한 제곱값
        /// </summary>
        public float _MoveCheckThresholdSqr;

        #endregion

        #region <Constructors>
        
        public PositionStatePreset() : this(_PositionChangeThreshold)
        {
        }
        
        public PositionStatePreset(float p_Threshold)
        {
            SetInitialize(Vector3.zero);
            _MoveCheckThresholdSqr = p_Threshold * p_Threshold;
        }
  
        #endregion
        
        #region <Methods>

        public void SetInitialize(Vector3 p_Position)
        {
            _PrevPosition = p_Position;
            _CurrentPosition = p_Position;
            _PrevThresholdPosition = p_Position;
            UpdatePosition();
        }

        public void UpdatePosition(Vector3 p_Position)
        {
            _PrevPosition = _CurrentPosition;
            _CurrentPosition = p_Position;
            
            var tryCurrentDeltaDirection = _PrevPosition.GetDirectionVectorTo(_CurrentPosition);
            var tryCurrentDeltaDirectionSqrMagnitude = tryCurrentDeltaDirection.sqrMagnitude;
            _IsMoved = tryCurrentDeltaDirectionSqrMagnitude > CustomMath.Epsilon;
            _IsThresholdMoved = _PrevThresholdPosition.GetSqrDistanceTo(_CurrentPosition) > _MoveCheckThresholdSqr;
            _IsHeightMoved = !(_PrevPosition.y - _CurrentPosition.y).IsReachedZero();
            
            if (_IsMoved)
            {
                _CurrentDeltaDirection = tryCurrentDeltaDirection;
                _CurrentDeltaDirectionUnitVector = _CurrentDeltaDirection.normalized;
                _CurrentDeltaDirectionSqrMagnitude = tryCurrentDeltaDirectionSqrMagnitude;
            }
            if (_IsThresholdMoved)
            {
                _PrevThresholdPosition = _CurrentPosition;
            }
        }

        public void UpdatePosition()
        {
            _CurrentDeltaDirection = _CurrentDeltaDirectionUnitVector = Vector3.zero;
            _CurrentDeltaDirectionSqrMagnitude = 0f;
            _IsMoved = _IsThresholdMoved = _IsHeightMoved = false;
        }

        #endregion
    }

    /// <summary>
    /// 지정한 두 벡터의 방향 관계(일치, 반대방향, 그외)를 내적을 이용해 구하는 구조체
    /// </summary>
    public struct DirectionStatePreset
    {
        #region <Fields>

        public Vector3 _PivotDirectionUnitVector;
        public Vector3 _TargetDirectionUnitVector;
        public float _DotValue;
        private DirectionState _PrevDirectionState;
        
        #endregion

        #region <Enums>

        public enum DirectionState
        {
            Identical, 
            Non_Identical_Positive, 
            Non_Identical_Negative, 
            Inverse
        }

        #endregion

        #region <Methods>
   
        /// <summary>
        /// 지정한 두 벡터의 방향 관계(일치, 반대방향, 그외)를 outmode로 리턴하는 메서드
        /// 지정한 두 벡터는 구조체에 저장되며, 만약 이전에 측정된 방향관계와 같은 관계가 연산된 경우 거짓을 리턴한다.
        /// 즉, 방향관계가 변하면 참을 리턴한다.
        /// </summary>
        public bool UpdateDirection(Vector3 p_PivotUnitVector, Vector3 p_TargetUnitVector, out DirectionState o_DirectionState)
        {
            o_DirectionState = UpdateDirection(p_PivotUnitVector, p_TargetUnitVector);
            if (_PrevDirectionState == o_DirectionState)
            {
                return false;
            }
            else
            {
                _PrevDirectionState = o_DirectionState;
                return true;
            }
        }
        
        /// <summary>
        /// 지정한 두 벡터의 방향 관계(일치, 반대방향, 그외)를 outmode로 리턴하는 메서드
        /// </summary>
        public DirectionState UpdateDirection(Vector3 p_PivotUnitVector, Vector3 p_TargetUnitVector)
        {
            _PivotDirectionUnitVector = p_PivotUnitVector;
            _TargetDirectionUnitVector = p_TargetUnitVector;
            _DotValue = Vector3.Dot(_PivotDirectionUnitVector, _TargetDirectionUnitVector);
            if (CheckDirectionIdentical(p_PivotUnitVector, p_TargetUnitVector))
            {
                return DirectionState.Identical;
            }
            else if(_DotValue.IsReachedValue(-1f, Mathf.Epsilon))
            {
                return DirectionState.Inverse;
            }
            else if(_DotValue > 0f)
            {
                return DirectionState.Non_Identical_Positive;
            }
            else
            {
                return DirectionState.Non_Identical_Negative;
            }
        }

        /// <summary>
        /// 지정한 두 벡터의 방향 관계(일치, 반대방향, 그외)를 outmode로 리턴하는 메서드
        /// </summary>
        public bool CheckDirectionIdentical(Vector3 p_PivotUnitVector, Vector3 p_TargetUnitVector)
        {
            _PivotDirectionUnitVector = p_PivotUnitVector;
            _TargetDirectionUnitVector = p_TargetUnitVector;
            _DotValue = Vector3.Dot(_PivotDirectionUnitVector, _TargetDirectionUnitVector);
            return _DotValue.IsReachedOne();
        }
        
        #endregion
    }

    /// <summary>
    /// 특정 방향벡터를 지정한 방향벡터가 되도록 회전시키며 회전 상태 등을 제어하는 기능을 가지는 구조체
    /// </summary>
    public struct OrbitTracingPreset
    {
        #region <Consts>

        private const float LerpDecreaseRate = 1f;

        #endregion
        
        #region <Fields>

        public Vector3 _CurrentDirectionUnitVector;
        private Vector3 _PivotDirectionUnitVector;
        private Vector3 _TargetDirectionUnitVector;
        private ProgressTimer _LerpTimer;
        private DirectionStatePreset _DirectionStatePreset;
        public OrbitTracingState _OrbitTracingState { get; private set; }

        #endregion

        #region <Enums>

        public enum OrbitTracingState
        {
            TraceOver,
            Tracing,
        }

        #endregion
        
        #region <Methods>

        public void Initialize(float p_LerpDuration)
        {
            _LerpTimer.Initialize(p_LerpDuration);
        }

        public void SetPivotTargetPosition(Vector3 p_PivotDirectionUnitVector, Vector3 p_TargetDirectionUnitVector)
        {
            _LerpTimer.Reset();
            _OrbitTracingState = OrbitTracingState.Tracing;
            _PivotDirectionUnitVector = p_PivotDirectionUnitVector;
            _TargetDirectionUnitVector = p_TargetDirectionUnitVector;
        }

        public void Reset()
        {
            _CurrentDirectionUnitVector = _PivotDirectionUnitVector;
            _OrbitTracingState = OrbitTracingState.TraceOver;
            _LerpTimer.Reset();
        }

        public void UpdateOrbitVector(float p_DeltaTime)
        {
            switch (_OrbitTracingState)
            {
                case OrbitTracingState.TraceOver:
                    break;
                case OrbitTracingState.Tracing:
                    var directionType = _DirectionStatePreset.UpdateDirection(_CurrentDirectionUnitVector, _TargetDirectionUnitVector);
                    switch (directionType)
                    {
                        case DirectionStatePreset.DirectionState.Identical:
                            _OrbitTracingState = OrbitTracingState.TraceOver;
                            break;
                        case DirectionStatePreset.DirectionState.Non_Identical_Positive:
                        case DirectionStatePreset.DirectionState.Non_Identical_Negative:
                        case DirectionStatePreset.DirectionState.Inverse:
                            _LerpTimer.ProgressClamped(p_DeltaTime);
                            _CurrentDirectionUnitVector = Vector3.Slerp(_PivotDirectionUnitVector, _TargetDirectionUnitVector, _LerpTimer.ProgressRate);
                            break;
                    }
                    break;
            }
        }
        
        #endregion
    }
}