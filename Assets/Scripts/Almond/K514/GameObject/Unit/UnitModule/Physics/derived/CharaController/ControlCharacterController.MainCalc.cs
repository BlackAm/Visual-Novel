using UnityEngine;

namespace k514
{
    public partial class ControlCharacterController
    {
        #region <Methods>

        protected override void ApplyVelocity(float p_DeltaTime)
        {
            MoveTo(_CurrentVelocity * p_DeltaTime, p_DeltaTime);
        }

        /// <summary>
        /// 현재 물리 모듈의 체공 상태를 체크하는 메서드
        /// 체공 상태가 변했다면 참을 리턴한다.
        /// </summary>
        protected override bool UpdateAerialState()
        {
            var latestStampPreset = _MasterNode.UpdateLatestStampPreset();
            
            // 컬라이더 하단부에 지면 혹은 다른 유닛 충돌이 검증된 경우
            if (latestStampPreset.ResultFlag != UnitTool.UnitStampResultFlag.None)
            {
                switch (_AerialStateTransitionStack)
                {
                    case var case0 when case0 >= _AerialStateTransitionUpperBound :
                        switch (_AerialState)
                        {
                            case PhysicsTool.AerialState.None:
                            case PhysicsTool.AerialState.OnAerial:
                                _AerialState = PhysicsTool.AerialState.OnGround;
                                return true;
                            default :
                            case PhysicsTool.AerialState.OnGround:
                                return false;
                        }
                    case var case1 when case1 >= 0 :
                        _AerialStateTransitionStack++;
                        return false;
                    default :
                        _AerialStateTransitionStack = 0;
                        return false;
                }
            }
            else
            {
                // 충돌이 검증되지 않았다면 중력을 더해준다.
                AddGravity();
                
                // 점프를 수동 시전한 경우, 곧바로 상태를 전이시켜야한다.
                if (_MasterNode._ActableObject.IsJumpedManual())
                {
                    _AerialStateTransitionStack = _AerialStateTransitionLowerBound;
                }
                
                switch (_AerialStateTransitionStack)
                {
                    case var case0 when case0 <= _AerialStateTransitionLowerBound :
                        switch (_AerialState)
                        {
                            case PhysicsTool.AerialState.None:
                            case PhysicsTool.AerialState.OnGround:
                                _AerialState = PhysicsTool.AerialState.OnAerial;
                                return true;
                            default :
                            case PhysicsTool.AerialState.OnAerial:
                                return false;
                        }
                    case var case1 when case1 <= 0 :
                        _AerialStateTransitionStack--;
                        return false;
                    default :
                        _AerialStateTransitionStack = 0;
                        return false;
                }
            }
        }

        protected override void ApplyAerialState(float p_DeltaTime)
        {
            var hasStateChanged = UpdateAerialState();

            switch (_AerialState)
            {
                case PhysicsTool.AerialState.None:
                case PhysicsTool.AerialState.OnGround:
                    // 착지 이외 상태에서 착지 상태로 전이된 경우
                    if (hasStateChanged)
                    {
                        // 착지시 현재 속도에서 y성분을 제거한다.
                        OverlapVelocityYLower(0f);
                        // 최소 중력값을 덮어쓴다.
                        OverlapDefaultGravityVelocity(p_DeltaTime);     
                        // 착지 콜백을 호출해준다.
                        _MasterNode.OnReachedGround();
                    }
                    break;
                case PhysicsTool.AerialState.OnAerial:
                    // 체공 이외 상태에서 체공 상태로 전이된 경우
                    if (hasStateChanged)
                    {
                        // 중력 속도를 제거해준다.
                        ClearVelocity(PhysicsTool.AccelerationType.Gravity, false);
                        // 체공 시작 콜백을 호출해준다.
                        _MasterNode.OnJumpUpAreal();
                    }
                    break;
            }
        }

        /// <summary>
        /// 현재 중력 적용 방식에 따라, 중력 가속도를 더하는 메서드
        /// </summary>
        private void AddGravity()
        {
            switch (_GravityFlag)
            {
                case PhysicsTool.GravityType.Applied:
                    AddAcceleration(PhysicsTool.AccelerationType.Gravity, Mass.CurrentValue * UnitInteractManager.GetInstance.GravityVector);
                    break;
                case PhysicsTool.GravityType.Anti_HitBreak:
                case PhysicsTool.GravityType.Anti_Perfect:
                    break;
            }
        }

        /// <summary>
        /// 현재 적용중인 중력속도를 덮어쓰는 메서드
        /// </summary>
        protected void OverlapDefaultGravityVelocity(float p_DeltaTime)
        {
            OverlapVelocity(PhysicsTool.AccelerationType.Gravity, Mass.CurrentValue * p_DeltaTime * UnitInteractManager.GetInstance.GravityVector);
        }

        public CharacterController GetCharacterController()
        {
            return _CharacterController;
        }
        public CapsuleCollider GetTriggerController()
        {
            return _TriggerController;
        }
        public void SetColliderEnable(bool state)
        {
            _CharacterController.enabled = state;
            _TriggerController.enabled = state;
        }

        #endregion
    }
}