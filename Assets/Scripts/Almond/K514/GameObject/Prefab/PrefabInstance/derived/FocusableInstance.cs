using UnityEngine;

namespace k514
{
    /// <summary>
    /// 위상 변화 및 카메라 컬링이 가능한 프리팹 인스턴스
    /// </summary>
    public abstract partial class FocusableInstance : PrefabInstance, IPositionStateTracker
    {
        #region <Fields>

        /// <summary>
        /// 카메라 뷰 컨트롤을 위한 범위 기술 오브젝트
        /// </summary>
        public IVirtualRange _RangeObject;

        /// <summary>
        /// 카메라 컬링을 위한 랜더 제어 오브젝트
        /// </summary>
        public IRenderable _Renderable;
        
        /// <summary>
        /// 위상 변화를 기록하는 구조체
        /// </summary>
        public PositionStatePreset _PositionState { get; private set; }
        
        /// <summary>
        /// Update 타이밍에 해당 유닛의 위치가 변경되는 이벤트가 발생했는지 표시하는 플래그
        /// </summary>
        private bool _ReservedPosChangeKey;
    
        #endregion

        #region <Callbacks>

        public override void OnSpawning()
        {
            base.OnSpawning();
            
            _PositionState = new PositionStatePreset();
        }

        public override void OnPooling()
        {
            base.OnPooling();
            
            _PositionState.SetInitialize(_Transform.position);
            _ReservedPosChangeKey = false;
        }

        public override void OnRetrieved()
        {
            base.OnRetrieved();
            
            _PositionState.SetInitialize(Vector3.zero);
        }
        
        public override void OnPositionTransition(Vector3 p_TransitionTo)
        {
            base.OnPositionTransition(p_TransitionTo);
            
            _PositionState.SetInitialize(_Transform.position);
        }

        public abstract void OnLateUpdate(float p_DeltaTime);
        
        /// <summary>
        /// 유닛 좌표가 변한 경우 플래그를 세트하는 콜백
        /// </summary>
        public void OnUnitPositionChangeDetected()
        {
            _ReservedPosChangeKey = true;
        }

        /// <summary>
        /// 유닛의 좌표가 변한경우 해당 이벤트를 전파하는 콜백
        /// </summary>
        protected abstract void OnSendUnitPositionUpdate(bool p_EnoughThresholdMoveFlag);

        #endregion

        #region <Methods>
        
        /// <summary>
        /// 해당 오브젝트의 반경 값을 리턴한다.
        /// </summary>
        public float GetRadius()
        {
            return _RangeObject.Radius.CurrentValue;
        }
        
        /// <summary>
        /// 해당 오브젝트의 반경 절반 값을 리턴한다.
        /// </summary>
        public float GetRadius(float p_Factor)
        {
            return p_Factor * _RangeObject.Radius.CurrentValue;
        }

        /// <summary>
        /// 해당 오브젝트의 높이 값을 리턴한다.
        /// </summary>
        public float GetHeight()
        {
            return _RangeObject.Height.CurrentValue;
        }
        
        /// <summary>
        /// 해당 오브젝트의 높이 값을 리턴한다.
        /// </summary>
        public float GetNonScaledHeight(float p_Factor)
        {
            return p_Factor * _RangeObject.Height._DefaultValue;
        }
        
        /// <summary>
        /// 해당 오브젝트의 높이 값을 리턴한다.
        /// </summary>
        public float GetHeight(float p_Factor)
        {
            return p_Factor * _RangeObject.Height.CurrentValue;
        }
        
        /// <summary>
        /// 해당 오브젝트의 높이 벡터를 리턴한다.
        /// </summary>
        public Vector3 GetHeightOffsetVector(float p_Factor)
        {
            return GetHeight(p_Factor) * Vector3.up;
        }
        
        /// <summary>
        /// 해당 오브젝트의 높이 좌표를 리턴한다.
        /// </summary>
        public Vector3 GetHeightVector(float p_Factor)
        {
            return _Transform.position + GetHeightOffsetVector(p_Factor);
        }
        
        /// <summary>
        /// 해당 오브젝트의 중심 좌표를 리턴한다.
        /// </summary>
        public Vector3 GetCenterPosition()
        {
            return _Transform.position + GetHeightOffsetVector(0.5f);
        }
        
        /// <summary>
        /// 해당 오브젝트의 높이 좌표를 리턴한다.
        /// </summary>
        public Vector3 GetTopPosition()
        {
            return _Transform.position + GetHeightOffsetVector(1f);
        }
        
        /// <summary>
        /// Update 중에 유닛의 위치가 변했던 경우를 고려하여 PositionState를 업데이트 하는 메서드
        /// </summary>
        public void CheckUnitPositionChanged()
        {
            if (_ReservedPosChangeKey)
            {
                _ReservedPosChangeKey = false;
                _PositionState.UpdatePosition(_Transform.position);
                
                if (IsPositionChanged_Threshold())
                {
                    OnSendUnitPositionUpdate(true);
                }
                else if (IsPositionChanged())
                {
                    OnSendUnitPositionUpdate(false);
                }
            }
            else
            {
                _PositionState.UpdatePosition();
            }
        }
        
        /// <summary>
        /// 해당 오브젝트의 위상이 바뀌었는지 체크하는 메서드
        /// </summary>
        public bool IsPositionChanged()
        {
            return _PositionState._IsMoved;
        }
        
        /// <summary>
        /// 해당 오브젝트의 높이 위상이 바뀌었는지 체크하는 메서드
        /// </summary>
        public bool IsHeightChanged()
        {
            return _PositionState._IsHeightMoved;
        }
        
        /// <summary>
        /// 해당 오브젝트가 이동중이라고 할만큼 최소한의 좌표 변화가 있었는지 체크하는 메서드
        /// </summary>
        public bool IsPositionChanged_Threshold()
        {
            return _PositionState._IsThresholdMoved;
        }

        #endregion
    }
}