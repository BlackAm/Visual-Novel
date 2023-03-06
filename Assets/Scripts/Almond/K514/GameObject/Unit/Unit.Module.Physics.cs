using UnityEngine;

namespace k514
{
    public partial class Unit
    {
        #region <Fields>
        
        private IPhysics _BackingPhysicsObject;

        /// <summary>
        /// 현재 선택된 해당 유닛의 물리 연산 모듈
        /// </summary>
        public IPhysics _PhysicsObject
        {
            get => _BackingPhysicsObject;
            protected set
            {
                _BackingPhysicsObject = value;
                _RangeObject = value;
            }
        }

        /// <summary>
        /// Physics 모듈
        /// </summary>
        private UnitModuleCluster<UnitPhysicsDataRoot.UnitPhysicsType, IPhysics> _PhysicsModule;

        /// <summary>
        /// 밟음 구조체
        /// </summary>
        protected UnitStampPreset _LatestStampPreset;

        public UnitSizeType _UnitSizeType;

        public Vector3 _TargetPosition;

        private bool _ShowingWay;

        private int _CurrentEffectCount;
        
        #endregion

        #region <Callbacks>

        #region <Enums>

        public enum UnitSizeType
        {
            Small,
            Normal,
            Big,
            VeryBig,
        }

        #endregion

        private void OnAwakePhysics()
        {
            _PhysicsModule 
                = new UnitModuleCluster<UnitPhysicsDataRoot.UnitPhysicsType, IPhysics>(
                    this, UnitModuleDataTool.UnitModuleType.Physics, _PrefabExtraDataRecord.PhysicsPresetIdList);
            _PhysicsObject = (IPhysics) _PhysicsModule.CurrentSelectedModule;
        }

        private void OnPoolingPhysics()
        {
            _PhysicsObject = _PhysicsModule.SwitchModule();
            _UnitSizeType = _PrefabExtraDataRecord.UnitSizeType;
        }
        
        private void OnRetrievePhysics()
        {
            _PhysicsObject.OnMasterNodeRetrieved();
            _LatestStampPreset = default;
            _TargetPosition = default;
            _ShowingWay = false;
            SetEffectCount(0);
        }

        /// <summary>
        /// 프레임이 아닌 고정된 시간 주기로 호출되는 콜백
        /// 부하가 크기 때문에 물리모듈에만 적용한다.
        /// </summary>
        public void OnFixedUpdate(float p_DeltaTime)
        {
            _PhysicsObject.OnFixedUpdate(p_DeltaTime);
        }
        
        public void OnJumpUpAreal()
        {
            // 스킬 시전 중이 아니라면, 점프모션을 수행한다.
            if (!HasState_Or(UnitStateType.DRIVESKILL | UnitStateType.STUCK))
            {
                // 점프 모션이 다른 모션으로 치환된 상태이고,
                // 치환된 모션의 y가 변화는 과정에서 해당 콜백이 계속 호출되는 경우
                // 동일한 모션이 무한반복될 수 있는데 해당 현상을 막기위해
                // 다른 모션으로만 전이하는 플래그를 사용하여 모션을 재생시킨다.
                _AnimationObject.SwitchMotionState(AnimatorParamStorage.MotionType.JumpUp, 0, 
                    AnimatorParamStorage.MotionTransitionType.Restrict_ToDiffentMotion | AnimatorParamStorage.MotionTransitionType.Bypass_StateMachine | AnimatorParamStorage.MotionTransitionType.AndMask);
                
                AddState(UnitStateType.FLOAT);
                for (int i = 0; i < _ModuleCount; i++)
                {
                    _ModuleList[i].CurrentSelectedModule.OnJumpUp();
                }
            }
            // 스킬 시전 중에 모션 등에 의해 낙하하는 경우, 점프 모션을 거치지 않고 체공 로직을 수행한다.
            else
            {
                AddState(UnitStateType.FLOAT);
                for (int i = 0; i < _ModuleCount; i++)
                {
                    _ModuleList[i].CurrentSelectedModule.OnJumpUp();
                }
            }  
        }
           
        public void OnReversed_Y_Velocity()
        {
        }
        
        public void OnReachedGround()
        {
            RemoveState(UnitStateType.FLOAT);
   
            OnStampingObjects();
            /*for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnReachedGround(_LatestStampPreset);
            }*/
        }

        protected virtual void OnStampingObjects()
        {
            // 해당 유닛이 다른 유닛 위에 착지한 경우에
            var (valid, stamped) = _LatestStampPreset.TryGetStampedUnit();
            if (valid)
            {
            }
        }
        
        public void OnCheckOverlapObject()
        {
            UpdateLatestStampPreset();

            /*for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnCheckOverlapObject(_LatestStampPreset);
            }*/
        }

        #endregion
    
        #region <Methods>

        protected void SwitchPhysics()
        {
            _PhysicsObject = _PhysicsModule.SwitchModule();
        }
        
        protected void SwitchPhysics(UnitPhysicsDataRoot.UnitPhysicsType p_ModuleType)
        {
            _PhysicsObject = _PhysicsModule.SwitchModule(p_ModuleType);
        }
        
        protected void SwitchPhysics(int p_Index)
        {
            _PhysicsObject = _PhysicsModule.SwitchModule(p_Index);
        }
        
        /// <summary>
        /// 물리 모듈 파기 메서드
        /// </summary>
        private void DisposePhysics()
        {
            if (_PhysicsModule != null)
            {
                _PhysicsModule.Dispose();
                _PhysicsModule = null;
            }

            _PhysicsObject = null;
        }

        /// <summary>
        /// 해당 유닛의 스케일을 변경하며, 동시에 변경한 스케일 값을 물리모듈에 전파하도록 재정의된 메서드
        /// </summary>
        public override void SetScale(float p_ScaleRate)
        {
            base.SetScale(p_ScaleRate);

            if (PoolState != PoolState.None)
            {
                _PhysicsObject.SetPhysicsScale(p_ScaleRate);
            }
        }
        
        /// <summary>
        /// 스케일이 적용된 이동속도를 리턴하는 메서드
        /// </summary>
        public float GetScaledMovementSpeed()
        {
            // return GetSizeSpeed() * _BattleStatusPreset.t_Current.GetMultipliedMovementSpeed();
            return GetSizeSpeed();
        }

        public float GetSizeSpeed()
        {
            switch (_UnitSizeType)
            {
                case UnitSizeType.Small:
                    return 0.8f;
                case UnitSizeType.Normal:
                    return 1f;
                case UnitSizeType.Big:
                    return 1.3f;
                case UnitSizeType.VeryBig:
                    return 1.7f;
            }

            return 1f;
        }

        /// <summary>
        /// 물리모듈을 일정한 속도로 움직이는 메서드
        /// </summary>
        public void MoveTo(Vector3 p_DeltaForce, float p_DeltaTime)
        {
            _PhysicsObject.MoveTo(p_DeltaForce, p_DeltaTime);
        }

        /// <summary>
        /// 물리모듈에 지정한 벡터만큼의 속도를 더하는 메서드
        /// </summary>
        public void ForceTo(Vector3 p_ForceVector, int p_AddForceIndex)
        {
            _PhysicsObject.AddVelocity(PhysicsTool.AccelerationType.Default, ObjectScale.CurrentValue * p_ForceVector, new PhysicsTool.UnitAddForceParams(p_AddForceIndex, this));
        }

        /// <summary>
        /// 물리모듈의 컨트롤러 속도계를 0벡터로 하는 메서드
        /// </summary>
        public void HaltController(bool p_ClearAutonomyMove)
        {
            _PhysicsObject.ClearVelocity(PhysicsTool.AccelerationType.SyncWithController, false);

            if (p_ClearAutonomyMove)
            {
                _PhysicsObject.ClearPhysicsAutonomyMove();
            }
        }

        /// <summary>
        /// 물리모듈이 현재 지면 위에 있는지 검증하는 메서드
        /// </summary>
        public bool IsGround()
        {
            return _PhysicsObject.IsGround();
        }

        public UnitStampPreset UpdateLatestStampPreset()
        {
            _LatestStampPreset = new UnitStampPreset(this);
            return _LatestStampPreset;
        }
        
        public UnitStampPreset GetLatestStampPreset()
        {
            return _LatestStampPreset;
        }

        /// <summary>
        /// 충돌범위 바깥쪽, 주변 offset 만큼 범위 안의 랜덤한 지점의 좌표를 리턴하는 메서드
        /// </summary>
        public Vector3 GetRandomAroundPosition(float p_Offset)
        {
            var radius = GetRadius();
            return _Transform.RandomSymmetricPosition(CustomMath.XYZType.ZX, radius, radius + p_Offset);
        }
        
        /// <summary>
        /// 포커스 유닛의 충돌범위 바깥쪽, 주변 offset 만큼 범위 안의 랜덤한 지점의 좌표를 리턴하는 메서드
        /// 포커스가 없다면 0벡터를 리턴한다.
        /// </summary>
        public Vector3 GetFocusAroundOffsetVector(float p_Offset)
        {
            if (FocusNode)
            {
                var targetAI = FocusNode.Node;
                var targetRange = targetAI.GetRadius();
                return targetAI._Transform.RandomSymmetricPosition(CustomMath.XYZType.ZX, targetRange, targetRange + p_Offset);
            }
            else
            {
                return Vector3.zero;
            }
        }
        
        public void SetTargetPosition(Vector3 p_TargetPosition)
        {
            _TargetPosition = p_TargetPosition;
        }

        public Vector3 GetTargetPosition()
        {
            return _TargetPosition;
        }

        public bool HasTargetPosition()
        {
            return GetTargetPosition() != Vector3.zero;
        }

        public bool IsShowingWay()
        {
            return _ShowingWay;
        }

        public void SetShowingWay(bool p_Flag)
        {
            _ShowingWay = p_Flag;
        }

        public void SetEffectCount(int p_Count)
        {
            _CurrentEffectCount = p_Count;
        }

        public int GetEffectCount()
        {
            return _CurrentEffectCount;
        }

        #endregion
    }
}