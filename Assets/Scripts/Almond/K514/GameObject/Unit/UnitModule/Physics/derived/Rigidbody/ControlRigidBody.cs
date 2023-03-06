using System;
using UnityEngine;
using UnityEngine.AI;

namespace k514
{
    public class ControlRigidBody : ControlPhysicsBase
    {
        #region <Fields>

        /// <summary>
        /// 강체 컴포넌트 테이블 레코드
        /// </summary>
        private new RigidBodyPresetData.PhysicsTableRecord _PhysicsRecord;
        
        /// <summary>
        /// 유니티 물리 엔진을 수행하는 강체컴포넌트
        /// </summary>
        protected Rigidbody _RigidBody;
        
        #endregion
        
        #region <Callbacks>
        
        public override IPhysics OnInitializePhysics(UnitPhysicsDataRoot.UnitPhysicsType p_PhysicsType, Unit p_TargetUnit, IPhysicsTableRecordBridge p_PhysicsPreset)
        {
            base.OnInitializePhysics(p_PhysicsType, p_TargetUnit, p_PhysicsPreset);
            
            _PhysicsRecord = (RigidBodyPresetData.PhysicsTableRecord) p_PhysicsPreset;
            _RigidBody = p_TargetUnit.GetComponent<Rigidbody>();

            return this;
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
        /// 인스턴스가 파기될 때 수행할 작업을 기술한다.
        /// </summary>
        protected override void DisposeUnManaged()
        {
        }

        #endregion

        #region <Methods>

        public override void TryUpdateCollision(Vector3 p_UnitVector)
        {
        }

        public override void MoveTo(Vector3 p_DeltaVelocity, float p_DeltaTime)
        {
        }

        protected override void ApplyAerialState(float p_DeltaTime)
        {
        }

        protected override bool UpdateAerialState()
        {
            return false;
        }

        protected override void ApplyVelocity(float p_DeltaTime)
        {
        }

        #endregion
    }
}