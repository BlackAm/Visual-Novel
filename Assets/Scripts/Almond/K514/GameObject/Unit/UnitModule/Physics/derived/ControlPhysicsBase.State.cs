using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class ControlPhysicsBase
    {
        #region <Fields>

        /// <summary>
        /// 중력 타입
        /// </summary>
        protected PhysicsTool.GravityType _GravityFlag;

        /// <summary>
        /// 현재 체공 타입
        /// </summary>
        protected PhysicsTool.AerialState _AerialState;

        /// <summary>
        /// 연직 방향 타입
        /// </summary>
        protected CustomMath.Significant _Current_Y_VelocityType;

        /// <summary>
        /// 외력이 가해진 경우, 해당 외력의 유효성을 표기하는 테이블
        /// </summary>
        private Dictionary<PhysicsTool.AccelerationType, bool> _ForceValidationTable;

        #endregion
        
        #region <Callbacks>

        public override void OnHitted(Unit p_Trigger, HitResult p_HitResult)
        {
            ClearVelocity(PhysicsTool.AccelerationType.SyncWithController, false);

            var (hitForceValid, hitForceIndex) = p_HitResult.GetHitParameter(UnitHitTool.HitParameterType.HitForce);
            if (hitForceValid)
            {
#if UNITY_EDITOR
                if (CustomDebug.PrintUnitHitLog)
                {
                    Debug.Log($"[HitForce] : {p_Trigger?.GetUnitName()} => {_MasterNode.GetUnitName()} / {p_HitResult.ForceVector.ToVectorString()}");
                    CustomDebug.DrawArrow(_MasterNode.GetCenterPosition(), _MasterNode.GetCenterPosition() + p_HitResult.ForceVector, 0.15f, Color.red, 5f);
                }
#endif
                ReserveVelocity(p_HitResult.ForceVector, new PhysicsTool.UnitAddForceParams(hitForceIndex, _MasterNode, p_Trigger));
            }

            var (hitStuckValid, hitStuckIndex) = p_HitResult.GetHitParameter(UnitHitTool.HitParameterType.HitStuck);
            if (hitStuckValid)
            {
                switch (_GravityFlag)
                {
                    case PhysicsTool.GravityType.Applied:
                        break;
                    case PhysicsTool.GravityType.Anti_HitBreak:
                        SetGravityFlag(PhysicsTool.GravityType.Applied);
                        break;
                    case PhysicsTool.GravityType.Anti_Perfect:
                        break;
                }
            }
        }

        #endregion

        #region <Methods>

        protected abstract void ApplyAerialState(float p_DeltaTime);
        protected abstract bool UpdateAerialState();
        public void SetGravityFlag(PhysicsTool.GravityType p_GravityFlag)
        {
            if (_GravityFlag != p_GravityFlag)
            {
                _GravityFlag = p_GravityFlag;
                switch (_GravityFlag)
                {
                    case PhysicsTool.GravityType.Applied:
                        break;
                    case PhysicsTool.GravityType.Anti_HitBreak:
                    case PhysicsTool.GravityType.Anti_Perfect:
                        ClearVelocity(PhysicsTool.AccelerationType.Gravity, false);
                        break;
                }
            }
        }

        private void ResetAerialState()
        {
            _AerialState = PhysicsTool.AerialState.None;
        }

        public void Update_Y_VelocityType()
        {
            var currentYVelocity = _CurrentVelocity.y;
            if (currentYVelocity > 0f)
            {
                _Current_Y_VelocityType = CustomMath.Significant.Plus;
            }
            else if (currentYVelocity < 0f)
            {
                _Current_Y_VelocityType = CustomMath.Significant.Minus;
            }
            else
            {
                _Current_Y_VelocityType = CustomMath.Significant.Zero;
            }
        }
        
        /// <summary>
        /// 연직 아래 방향 속도에 반작용을 적용시키는 메서드
        /// </summary>
        public void OverlapVelocityYLower(float p_Factor)
        {
            // 현재 가속도들의 연직방향 원소 값 y를 0으로 해준다.
            // 만약 탄성을 일으키는 경우에는, 파라미터 값의 비율만큼 값이 반전된다.
            foreach (var accelerationType in PhysicsTool._AccelerationTypeEnumerator)
            {
                var targetVelocity = _VelocityRecord[accelerationType];
                if (targetVelocity.y < 0f)
                {
                    _VelocityRecord[accelerationType] = Vector3.Scale(targetVelocity, new Vector3(1f, -p_Factor, 1f));
                }
                else
                {
                    _VelocityRecord[accelerationType] = Vector3.Scale(targetVelocity, new Vector3(1f, 0f, 1f));
                }
            }

            // 현재 속도 역시 가속도 처럼 연직 값을 0으로 하거나 반전시켜준다.
            if (_CurrentVelocity.y < 0f)
            {
                _CurrentVelocity = Vector3.Scale(_CurrentVelocity, new Vector3(1f, -p_Factor, 1f));
            }
            else
            {
                _CurrentVelocity = Vector3.Scale(_CurrentVelocity, new Vector3(1f, 0f, 1f));
            }
        }
        
        #endregion
    }
}