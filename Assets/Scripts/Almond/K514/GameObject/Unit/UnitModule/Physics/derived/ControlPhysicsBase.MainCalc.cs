using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class ControlPhysicsBase
    {
        #region <Fields>
                
        /// <summary>
        /// 현재 총 속도
        /// </summary>
        public Vector3 _CurrentVelocity { get; protected set; }
        
        /// <summary>
        /// 타입별 속도 테이블
        /// </summary>
        protected Dictionary<PhysicsTool.AccelerationType, Vector3> _VelocityRecord;
        
        /// <summary>
        /// 타입별 가속도 테이블
        /// </summary>
        protected Dictionary<PhysicsTool.AccelerationType, List<Vector3>> _Acceleration;

        #endregion

        #region <Methods>

        /// <summary>
        /// 지정한 타입의 가속도를 테이블 값에 더하는 메서드
        /// </summary>
        public void AddAcceleration(PhysicsTool.AccelerationType p_Type, Vector3 p_Acc)
        {
            _Acceleration[p_Type].Add(p_Acc);
        }

        /// <summary>
        /// 지정한 타입의 속도를 테이블 값에 더하는 메서드
        /// </summary>
        public void AddVelocity(PhysicsTool.AccelerationType p_Type, Vector3 p_Velocity, PhysicsTool.UnitAddForceParams p_UnitAddForceParams)
        {
            _VelocityRecord[p_Type] += p_Velocity;
            _ForceValidationTable[p_Type] = true;
            
            if (p_UnitAddForceParams.IsValid())
            {
                AddDampingStackTimer(p_Type, p_UnitAddForceParams);
            }
        }
        
        /// <summary>
        /// AddForce 테이블을 참조하여 속도값을 더하는 메서드
        /// </summary>
        public void AddVelocity(PhysicsTool.UnitAddForceParams p_UnitAddForceParams)
        {
            if (p_UnitAddForceParams.IsValid())
            {
                var recordIndex = p_UnitAddForceParams.UnitAddForceRecordIndex;
                var addForceRecord = UnitAddForceData.GetInstanceUnSafe[recordIndex];
                var forceDirectionType = addForceRecord.ForceDirectionType;
                var force = addForceRecord.Force;
                var formedForce = UnitTool.GetForceVector(_MasterNode, true, forceDirectionType, force);

                ReserveVelocity(formedForce, p_UnitAddForceParams);
            }
        }

        /// <summary>
        /// 지정한 타입의 속도를 테이블 값에 덮어쓰는 메서드
        /// </summary>
        public void OverlapVelocity(PhysicsTool.AccelerationType p_Type, Vector3 p_Velocity)
        {
            _VelocityRecord[p_Type] = p_Velocity;
        }

        /// <summary>
        /// 모든 타입의 속도를 테이블에서 지우고, 현재 총 속도도 초기화 시키는 메서드
        /// </summary>
        public void ClearVelocity()
        {
            foreach (var accelerationType in PhysicsTool._AccelerationTypeEnumerator)
            {
                ClearVelocity(accelerationType, false);
            }

            _CurrentVelocity = Vector3.zero;
        }
        
        /// <summary>
        /// 지정한 타입의 속도를 테이블에서 지우는 메서드
        /// </summary>
        public void ClearVelocity(PhysicsTool.AccelerationType p_Type, bool p_CorrectVelocity)
        {
            if (p_CorrectVelocity)
            {
                _CurrentVelocity -= _VelocityRecord[p_Type];
            }

            _VelocityRecord[p_Type] = Vector3.zero;
            _ForceValidationTable[p_Type] = false;
            
            var stackTimer = _DampingTimer[p_Type];
            var forcedUnitList = _ForcedUnit[p_Type];
            
            if (stackTimer.IsValid)
            {
                foreach (var forcedUnit in forcedUnitList)
                {
                    forcedUnit._PhysicsObject.ClearVelocity(p_Type, true);
                }  
            }
            
            forcedUnitList.Clear();
            stackTimer.OnTerminateStackTimer();
        }

        /// <summary>
        /// 모든 타입의 속도의 Y 성분을 지우고, 현재 총 속도의 Y 성분도 지우는 메서드
        /// </summary>
        public void Clear_Y_Velocity()
        {
            foreach (var accelerationType in PhysicsTool._AccelerationTypeEnumerator)
            {
                var targetVelocity = _VelocityRecord[accelerationType];
                _VelocityRecord[accelerationType] = Vector3.Scale(targetVelocity, new Vector3(1f, 0f, 1f));
            }
            _CurrentVelocity = Vector3.Scale(_CurrentVelocity, new Vector3(1f, 0f, 1f));
        }
        
        /// <summary>
        /// 가속도 테이블을 참조하여, 속도 테이블을 갱신하고 총합 속도로 취합하는 메서드
        /// </summary>
        protected void UpdateVelocity(float p_DeltaTime)
        {
            _CurrentVelocity = Vector3.zero;
            foreach (var accelerationType in PhysicsTool._AccelerationTypeEnumerator)
            {
                var accumulatedAccel = Vector3.zero;
                var targetList = _Acceleration[accelerationType];
                foreach (var accel in targetList)
                {
                    accumulatedAccel += accel;
                }
                // v = _v + dt * sum of accel
                var dAccel = p_DeltaTime * accumulatedAccel;
                _VelocityRecord[accelerationType] += dAccel;
                _CurrentVelocity += _VelocityRecord[accelerationType];
            }

            var prevYVelocityType = _Current_Y_VelocityType;
            Update_Y_VelocityType();
            if (prevYVelocityType != _Current_Y_VelocityType)
            {
                _MasterNode.OnReversed_Y_Velocity();
            }
        }

        /// <summary>
        /// 연산된 총 속도를 기준으로 물리 모듈에 힘을 가하는 메서드
        /// </summary>
        protected abstract void ApplyVelocity(float p_DeltaTime);

        /// <summary>
        /// 가속도 테이블에 감쇄를 적용하여, 일정 크기 미만이된 가속도를 제거하는 메서드
        /// 중력 속도에는 감쇄가 적용되지 않는다.
        /// </summary>
        protected void ApplyDampingRate(float p_Factor)
        {
            foreach (var accelerationType in PhysicsTool._DampingTarget_AccelerationTypeEnumerator)
            {
                _VelocityRecord[accelerationType] = (p_Factor * UnitInteractManager.__DampenRate * _VelocityRecord[accelerationType]).FloorVectorValue(UnitInteractManager.__VelocityElementLowerBound);
                if (_ForceValidationTable[accelerationType])
                {
                    if (_VelocityRecord[accelerationType].IsReachedZero())
                    {
                        ClearVelocity(accelerationType, false);
                    }
                    else
                    {
                        _DampingTimer[accelerationType].OnTick();
                    }
                }
                _Acceleration[accelerationType].Clear();
            }
            _Acceleration[PhysicsTool.AccelerationType.Gravity].Clear();
        }
        
        #endregion
    }
}