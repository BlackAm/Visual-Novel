using UnityEngine;
using Random = UnityEngine.Random;

namespace k514
{
    public partial class AutonomyAIBase
    {
        #region <Consts>

        /// <summary>
        /// AI pivot 전이 간격 하한 값, 단위는 Dsec
        /// </summary>
        private const int __AI_Wandering_Interval_Lowerbound = 4;

        #endregion

        #region <Fields>

        /// <summary>
        /// 랜덤 이동 반경
        /// </summary>
        protected float _WanderingRadius;

        /// <summary>
        /// 방황 타입
        /// </summary>
        protected ThinkableTool.AIWanderingType _WanderingType;
                
        /// <summary>
        /// 방황하는 유닛의 활동 개시 다운카운터
        /// </summary>
        protected int CurrentWanderingCounter;

        #endregion

        #region <Callbacks>

        protected void OnModuleNotifyWander()
        {
            if (_WanderingType.HasAnyFlagExceptNone(ThinkableTool.AIWanderingType.InstantWanderWhenAwakeAI))
            {
                Wander(true);
            }
            else
            {
                RandomizeWanderingCount();
            }
        }

        private void OnUpdateWanderingPivot()
        {
            if (_MasterNode.IsGround() && _AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.Wandering))
            {
                if (CurrentWanderingCounter < 1)
                {
                    RandomizeWanderingCount();
                    switch (_CurrentAIState)
                    {
                        case ThinkableTool.AIState.Idle :
                            Wander(false);
                            break;
                    }
                }
                else if(CurrentWanderingCounter > 0)
                {
                    CurrentWanderingCounter--;
                }
            }
        }

        #endregion

        #region <Methods>
        
        public void AddWanderingFlag(ThinkableTool.AIWanderingType p_Type)
        {
            _WanderingType.AddFlag(p_Type);
        }

        public void RemoveWanderingFlag(ThinkableTool.AIWanderingType p_Type)
        {
            _WanderingType.RemoveFlag(p_Type);
        }
        
        private void RandomizeWanderingCount()
        {
            CurrentWanderingCounter = Random.Range(__AI_Wandering_Interval_Lowerbound, _MindRecord.WanderingIntervalDsec);
        }

        protected void Wander(bool p_ForceMoveFlag)
        {
            if (!_AIExtraFlagMask.HasAnyFlagExceptNone(ThinkableTool.AIExtraFlag.RemoteOrder))
            {
                if (!_WanderingType.HasAnyFlagExceptNone(ThinkableTool.AIWanderingType.Disable))
                {
                    var tryCount = 0;
                    var hasWorldWanderingFlag = _WanderingType.HasAnyFlagExceptNone(ThinkableTool.AIWanderingType.WorldWander);
                    while (tryCount < 5)
                    {
                        var resultTuple = GetWanderTargetPosition(hasWorldWanderingFlag, tryCount);
                        if (resultTuple.Item1 && SwitchStateMove(resultTuple.Item2, p_ForceMoveFlag, true))
                        {
#if SERVER_DRIVE
                            (_MasterNode as LamiereUnit).OnSendSyncState(0, resultTuple.Item2);
#endif
                            return;
                        }
                        else
                        {
                            tryCount++;
                        }
                    }
                }
            }
        }
        
        protected void ResetWander()
        {
            var radius = _MasterNode.GetRadius();
            _WanderingRadius = _MindRecord.WanderingRadius < radius
                ? _MasterNode.GetRadius(2f) 
                : _MindRecord.WanderingRadius;
            _WanderingType = ThinkableTool.AIWanderingType.None;
            CurrentWanderingCounter = -1;
        }

        public (bool, Vector3) GetWanderTargetPosition(bool p_BaseCurrentPositionFlag, int p_RadiusScaleFactor)
        {
            var seedRadius = _WanderingRadius * (1f + 0.5f * p_RadiusScaleFactor);
            var xzRandomOffset = CustomMath.RandomSymmetricVector(CustomMath.XYZType.ZX, seedRadius * 0.2f, seedRadius);
            var targetRandomPos = p_BaseCurrentPositionFlag ? _MasterNode._Transform.position + xzRandomOffset : _OriginPivot + xzRandomOffset;
            var (valid, result) = PhysicsTool.GetHighestObjectPosition_RayCast(targetRandomPos, GameManager.Terrain_LayerMask, QueryTriggerInteraction.Ignore);
            
            if (valid)
            {
                return (valid, result);
            }
            else
            {
                return (valid, _OriginPivot);
            }
        }
        
        #endregion
    }
}