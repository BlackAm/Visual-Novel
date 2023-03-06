using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public abstract partial class ControlPhysicsBase : UnitModuleBase, IPhysics
    {
        ~ControlPhysicsBase()
        {
            Dispose();
        }
        
        #region <Fields>

        /// <summary>
        /// 기준 아핀
        /// </summary>
        protected Transform _Transform;
        
        /// <summary>
        /// 물리 모듈 타입
        /// </summary>
        public UnitPhysicsDataRoot.UnitPhysicsType _PhysicsType { get; private set; }
        
        /// <summary>
        /// 물리 모듈 레코드
        /// </summary>
        public IPhysicsTableRecordBridge _PhysicsRecord { get; private set; }

        /// <summary>
        /// 충돌 연산자
        /// </summary>
        public Collider Collider { get; protected set; }
        
        /// <summary>
        /// 반경 프리셋
        /// </summary>
        public FloatProperty_Inverse_Sqr Radius { get; protected set; }
        
        /// <summary>
        /// 높이 프리셋
        /// </summary>
        public FloatProperty Height { get; protected set; }
        
        /// <summary>
        /// 질량 프리셋
        /// </summary>
        public FloatProperty_Inverse Mass { get; protected set; }
        
        public Vector3 _TargetPosition { get; protected set; }

        public virtual UnityEngine.AI.NavMeshAgent _NavMeshAgent { get; protected set; }

        /// <summary>
        /// 스케일 배율 값
        /// </summary>
        public float PhysicsScaleFactor
        {
            get => _backingPhysicsScaleFactor;
            set
            {
                _backingPhysicsScaleFactor = value;
                Radius = Radius.ApplyScale(_backingPhysicsScaleFactor);
                Height = Height.ApplyScale(_backingPhysicsScaleFactor);
                Mass = Mass.ApplyScale(_backingPhysicsScaleFactor);
            }
        }

        /// <summary>
        /// 스케일 배율 프로퍼티 필드
        /// </summary>
        private float _backingPhysicsScaleFactor;

        #endregion

        #region <Enums>

        #endregion
        
        #region <Callbacks>

        public virtual IPhysics OnInitializePhysics(UnitPhysicsDataRoot.UnitPhysicsType p_PhysicsType, Unit p_TargetUnit, IPhysicsTableRecordBridge p_PhysicsPreset)
        {
            UnitModuleType = UnitModuleDataTool.UnitModuleType.Physics;
            _PhysicsType = p_PhysicsType;
            _MasterNode = p_TargetUnit;
            _PhysicsRecord = p_PhysicsPreset;
            _Transform = p_TargetUnit._Transform;
            _backingPhysicsScaleFactor = 1f;
            
            _VelocityRecord = new Dictionary<PhysicsTool.AccelerationType, Vector3>();
            _Acceleration = new Dictionary<PhysicsTool.AccelerationType, List<Vector3>>();
            _DampingTimer = new Dictionary<PhysicsTool.AccelerationType, StackTimer<PhysicsTool.AccelerationType, PhysicsTool.UnitAddForceParams>>();
            _ForcedUnit = new Dictionary<PhysicsTool.AccelerationType, List<Unit>>();
            _ForceTimer = new List<SafeReference<object, GameEventTimerHandlerWrapper>>();
            _ForceValidationTable = new Dictionary<PhysicsTool.AccelerationType, bool>();
            
            foreach (var accelerationType in PhysicsTool._AccelerationTypeEnumerator)
            {
                _VelocityRecord.Add(accelerationType, Vector3.zero);
                _Acceleration.Add(accelerationType, new List<Vector3>());
                _DampingTimer.Add(accelerationType, new StackTimer<PhysicsTool.AccelerationType, PhysicsTool.UnitAddForceParams>(_StackInterval, accelerationType, OnDampingTerminated, OnDampingTick, true));
                _ForcedUnit.Add(accelerationType, new List<Unit>());
                _ForceValidationTable.Add(accelerationType, false);
            }
            
            Mass = new FloatProperty_Inverse(_PhysicsRecord.Mass, 0f);
            SyncPhysicsScale();

            return this;
        }

        protected override void OnModuleNotify()
        {
            SyncPhysicsScale();
            
            ClearVelocity();
            Update_Y_VelocityType();
            ApplyDampingRate(0f);
            SetGravityFlag(PhysicsTool.GravityType.Applied);

            _MasterNode.OnCheckOverlapObject();
        }

        protected override void OnModuleSleep()
        {
            CancelAllPreDelayForceEvent();
            ResetAerialState();
        }
        
        public override void OnUpdate(float p_DeltaTime)
        {
//            ApplyAerialState(p_DeltaTime);
//            UpdateVelocity(p_DeltaTime);
//            ApplyVelocity(p_DeltaTime);
//            ApplyDampingRate(1f);
        }
        
        public void OnFixedUpdate(float p_DeltaTime)
        {
            ApplyAerialState(p_DeltaTime);
            UpdateVelocity(p_DeltaTime);
            ApplyVelocity(p_DeltaTime);
            ApplyDampingRate(1f);
        }
        
        public override void OnPivotChanged(PositionStatePreset p_PositionStatePreset, bool p_SyncPosition)
        {
            if (p_SyncPosition)
            {
                _MasterNode.OnCheckOverlapObject();
            }
        }

        public override void OnUnitActionTransitioned(ControllerTool.CommandType p_PrevCommandType, ControllerTool.CommandType p_CurrentCommandType)
        {
        }

        public override void OnStriked(Unit p_Trigger, HitResult p_HitResult)
        {
        }

        public override void OnUnitDead(bool p_Instant)
        {
            ClearVelocity();
        }

        public override void OnCheckOverlapObject(UnitStampPreset p_UnitStampPreset)
        {
            if (p_UnitStampPreset.ResultFlag.HasAnyFlagExceptNone(UnitTool.UnitStampResultFlag.Overlapped))
            {
                var tryPosition = PhysicsTool
                    .GetHighestObjectPosition_RayCast(_Transform.position, GameManager.Obstacle_Terrain_UnitEC_LayerMask, QueryTriggerInteraction.Collide).Item2;
                
                _MasterNode.OnPositionTransition(tryPosition);
            }
        }

        #endregion
        
        #region <Methods>

        /// <summary>
        /// 충돌 연산자의 충돌정보를 갱신시키는 메서드
        /// </summary>
        public abstract void TryUpdateCollision(Vector3 p_UnitVector);

        /// <summary>
        /// 해당 물리모듈에 속도를 지정한 시간유닛만큼 가하는 메서드
        /// </summary>
        public abstract void MoveTo(Vector3 p_DeltaVelocity, float p_DeltaTime);
        
        /// <summary>
        /// 해당 물리모듈이 착지 상태인지 검증하는 논리 메서드
        /// </summary>
        public bool IsGround()
        {
            return _AerialState == PhysicsTool.AerialState.OnGround;
        }

        /// <summary>
        /// 지정한 물리모듈의 충돌반경을 스케일업하는 메서드
        /// </summary>
        public void SetPhysicsScale(float p_ScaleRate)
        {
            PhysicsScaleFactor = _MasterNode.ObjectScale._DefaultValue * p_ScaleRate;
        }
        
        /// <summary>
        /// 지정한 물리모듈의 충돌반경을 유닛의 스케일과 동기화시키는 메서드
        /// </summary>
        public void SyncPhysicsScale()
        {
            SetPhysicsScale(_MasterNode.ObjectScale._ScaleFactor);
        }

        #endregion
    }
}