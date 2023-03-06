using System;
using UnityEngine;
using UnityEngine.AI;

namespace k514
{
    public partial class ControlCharacterController : ControlPhysicsBase
    {
        #region <Consts>

        /// <summary>
        /// 체공 상태 변화 스택 상한 : 체공 => 지상
        /// </summary>
        private const int _AerialStateTransitionUpperBound = 0;
        
        /// <summary>
        /// 체공 상태 변화 스택 하한 : 지상 => 체공
        /// </summary>
        private const int _AerialStateTransitionLowerBound = -5;
        
        #endregion
        
        #region <Fields>

        /// <summary>
        /// 캐릭터 컨트롤러 테이블 레코드
        /// </summary>
        private new ICharacterControllerTableRecordBridge _PhysicsRecord;
        
        /// <summary>
        /// 기본 이동 및 경사 이동을 담당하는 컬라이더
        /// </summary>
        protected CharacterController _CharacterController;
        
        /// <summary>
        /// 컨트롤러끼리 겹치는 일을 막기 위한 트리거 컬라이더
        /// </summary>
        protected CapsuleCollider _TriggerController;

        /// <summary>
        /// Aerial상태를 전이시키는 용도로 사용하는 스택
        /// </summary>
        private int _AerialStateTransitionStack;

        #endregion

        #region <Callbacks>

        public override IPhysics OnInitializePhysics(UnitPhysicsDataRoot.UnitPhysicsType p_PhysicsType, Unit p_TargetUnit, IPhysicsTableRecordBridge p_PhysicsPreset)
        {
            base.OnInitializePhysics(p_PhysicsType, p_TargetUnit, p_PhysicsPreset);
            
            _PhysicsRecord = (ICharacterControllerTableRecordBridge) p_PhysicsPreset;
            _CharacterController = p_TargetUnit.GetComponent<CharacterController>();
            if (null == _CharacterController)
            {
                _CharacterController = p_TargetUnit.gameObject.AddComponent<CharacterController>();

                var (valid, modelKey) = _MasterNode._PrefabModelKey;
                if (valid)
                {
                    var modelRecord = (UnitModelDataRecordBridge) PrefabModelDataRoot.GetInstanceUnSafe[modelKey];
                    _CharacterController.radius = modelRecord.FallbackRadius;
                    _CharacterController.height = modelRecord.FallbackHeights;
                    _CharacterController.center = modelRecord.FallbackCenterOffset;
                }
                else
                {
                    _CharacterController.radius = 1f;
                    _CharacterController.height = 2f;
                    _CharacterController.center = Vector3.up;
                }
            }

            var skinWidth = _CharacterController.skinWidth = PhysicsTool.__Default_SkinWidth;
            _CharacterController.minMoveDistance = PhysicsTool.__Default_MinDistance;
            _CharacterController.enabled = false;
            
            Collider = _CharacterController;

            var radius = _CharacterController.radius;
            var height = _CharacterController.height;
            _TriggerController = p_TargetUnit.gameObject.AddComponent<CapsuleCollider>();
            _TriggerController.radius = radius;
            _TriggerController.height = height;
            _TriggerController.center = _CharacterController.center;
            _TriggerController.enabled = false;
            
            Radius = new FloatProperty_Inverse_Sqr(radius, skinWidth);
            Height = new FloatProperty(Mathf.Max(2f * radius, height), 2f * skinWidth);
            SyncPhysicsScale();

            return this;
        }

        protected override void OnModuleNotify()
        {
            base.OnModuleNotify();
            
            _CharacterController.enabled = true;
            _TriggerController.enabled = true;
            SetPhysicsCollideTrigger(true);
        }

        protected override void OnModuleSleep()
        {
            base.OnModuleSleep();
            
            _CharacterController.enabled = false;
            _TriggerController.enabled = false;
        }
        
        public override void OnPreUpdate(float p_DeltaTime)
        {
        }

        public override void OnUpdate_TimeBlock()
        {
        }

        public override void OnUnitHitActionStarted()
        {
        }

        public override void OnUnitHitActionTerminated()
        {
        }

        public override void OnUnitActionStarted()
        {
        }

        public override void OnUnitActionTerminated()
        {
        }

        public override void OnJumpUp()
        {
        }
        
        public override void OnReachedGround(UnitStampPreset p_UnitStampPreset)
        {
        }

        public override void OnFocusUnitChanged(Unit p_PrevFocusUnit, Unit p_FocusUnit)
        {
        }

        /// <summary>
        /// 유닛 사망시 호출되는 콜백
        /// </summary>
        public override void OnUnitDead(bool p_Instant)
        {
            base.OnUnitDead(p_Instant);
            
            SetColliderEnable(false);
        }
        
        /// <summary>
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
        }

        #endregion

        #region <Methods>

        private void SetPhysicsCollideTrigger(bool p_Flag)
        {
            _TriggerController.isTrigger = !p_Flag;
        }

        protected bool IsValidVelocity(Vector3 p_DeltaVelocity)
        {
            return !p_DeltaVelocity.IsReachedValue(0f, PhysicsTool._VelocityYLowerVectorFactor_Negative);
        }

        public override void MoveTo(Vector3 p_DeltaVelocity, float p_DeltaTime)
        {
            if (IsValidVelocity(p_DeltaVelocity))
            {
                #region <Legacy>
/*
                // 기존의 물리엔진에서는 착지 상태에 중력값이 0이기 때문에
                // 연직방향으로의 보정을 해야했었지만,
                // 현재 물리 모듈에서는 착지 상태에서도 중력값이 존재하므로
                // 아래 코드는 폐기되었다.
                switch (_Current_Y_VelocityType)
                {
                    case Y_VelocityType.None:
                    case Y_VelocityType.Minus:
                    case Y_VelocityType.Zero:
                        // y속도가 0 또는 음수인 경우, 연직아래방향으로 최소 이동해야하는 속도만큼 보정해준다.
                        // 그래야 바닥 충돌을 수행하기 때문
                        p_DeltaVelocity.y = Mathf.Min(PhysicsTool._VelocityYLowerVectorFactor, p_DeltaVelocity.y);
                        break;
                    case Y_VelocityType.Plus:
                        // 마찬가지로 y속도가 양수인 경우 천장 충돌을 체크하기 위해, y속도를 보정해준다.
                        p_DeltaVelocity.y = Mathf.Max(PhysicsTool._VelocityYLowerVectorFactor_Negative, p_DeltaVelocity.y);
                        break;
                }

*/
                #endregion
                if (!_CharacterController.enabled) return;
                _CharacterController.Move(p_DeltaVelocity);
                _MasterNode.OnUnitPositionChangeDetected();
            }
        }

        public override void TryUpdateCollision(Vector3 p_UnitVector)
        {
            if(_CharacterController.enabled)
                _CharacterController.Move(p_UnitVector * PhysicsTool._VelocityYLowerVectorFactor_Negative);
        }
        
        #endregion
    }
}