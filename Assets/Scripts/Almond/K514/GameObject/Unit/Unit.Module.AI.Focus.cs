using UnityEngine;

namespace k514
{
    /*public partial class Unit
    {
        #region <Callbacks>

        protected override void OnMasterNodeChanged(Unit p_Prev, Unit p_Current)
        {
            base.OnMasterNodeChanged(p_Prev, p_Current);
            
            _MindObject.ClearEnemy();
        }
        
        protected override void OnFocusingNodeChanged(Unit p_Prev, Unit p_Current)
        {
            for (int i = 0; i < _ModuleCount; i++)
            {
                _ModuleList[i].CurrentSelectedModule.OnFocusUnitChanged(p_Prev, p_Current);
            }
            
#if !SERVER_DRIVE
            if (IsPlayer)
            {
                PlayerManager.GetInstance.OnPlayerFocusChanged(p_Prev, p_Current);
            }
#endif   
        }

        #endregion

        #region <Methods>
        
        public (bool, Unit) GetFocus()
        {
            return (FocusNode, FocusNode);
        }

        #endregion

        #region <Method/Focus/Math>
        
        public float GetDistanceLowerBoundFromFocus()
        {
            return FocusNode ? GetRadius() + FocusNode.Node.GetRadius() : GetRadius();
        }

        public Vector3 GetFocusUV()
        {
            return FocusNode
                ? _Transform.GetDirectionUnitVectorTo((Vector3) FocusNode)
                : _Transform.forward;
        }
        
        public Vector3 GetFocusPosition()
        {
            return FocusNode ? FocusNode : _Transform.position;
        }

        public void SetLookToFocus(bool p_UpdateMotionCachedRotation)
        {
            if (FocusNode)
            {
                SetLook((Vector3) FocusNode);
                if (p_UpdateMotionCachedRotation)
                {
                    _AnimationObject.CacheMasterAffine();
                }
            }
        }

        /// <summary>
        /// 거리 테이블을 갱신 후, 해당 유닛으로부터 포커스 유닛까지의 제곱거리를 리턴하는 메서드
        /// </summary>
        public float GetSqrDistanceFromFocus(bool p_UpdateDistanceFlag)
        {
            if (FocusNode)
            {
                if (p_UpdateDistanceFlag)
                {
                    return UnitInteractManager.GetInstance.GetSqrDistanceBetween_UpdateSqrDistanceWhenTargetUnitHold(this, FocusNode);
                }
                else
                {
                    return UnitInteractManager.GetInstance.GetSqrDistanceBetween(this, FocusNode);
                }
            }
            else
            {
                return 0f;
            }
        }
        
        /// <summary>
        /// 거리 테이블을 갱신 후, 해당 유닛으로부터 포커스 유닛까지의 제곱거리가 입력받은 거리보다 작은지 검증하는 메서드
        /// </summary>
        public bool CompareSqrDistanceFromFocus(bool p_UpdateDistanceFlag, float p_CompareSqrDistance)
        {
            if (FocusNode)
            {
                if (p_UpdateDistanceFlag)
                {
                    return UnitInteractManager.GetInstance.GetSqrDistanceBetween_UpdateSqrDistanceWhenTargetUnitHold(this, FocusNode) < p_CompareSqrDistance;
                }
                else
                {
                    return UnitInteractManager.GetInstance.GetSqrDistanceBetween(this, FocusNode) < p_CompareSqrDistance;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }*/
}