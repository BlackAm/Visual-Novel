using UnityEngine;

namespace k514
{
    public struct UnitStampPreset
    {
        #region <Consts>

        /// <summary>
        /// 밟는 유닛의 발밑 좌표로부터 밟혔다고 판정되는 거리
        /// </summary>
        private const float __StampCheckOffset = 0.25f;
        private const float __StampCheckOffset_Negative = -__StampCheckOffset;

        #endregion

        #region <Fields>

        /// <summary>
        /// 검증 결과
        /// </summary>
        public UnitTool.UnitStampResultFlag ResultFlag;
        
        /// <summary>
        /// 밟고 있는 유닛
        /// </summary>
        public Unit Stamping;
        
        /// <summary>
        /// 밟힌 유닛
        /// </summary>
        public Unit Stamped;
        
        #endregion

        #region <Constructors>

        public UnitStampPreset(Unit p_Pivot)
        {
            ResultFlag = UnitTool.UnitStampResultFlag.None;
            Stamping = p_Pivot;
            Stamped = null;
            
            var count = PhysicsTool.GetCount_CapsuleOverlap(Stamping, __StampCheckOffset_Negative, GameManager.Obstacle_Terrain_UnitEC_LayerMask, QueryTriggerInteraction.Collide);
            var targetColliderSet = PhysicsTool._NonAllocCollider;
            var compareAffine = Stamping.transform;
            var compareLowerHeight = Stamping._Transform.position.y;
            
            for (int i = 0; i < count; i++)
            {
                var tryCollider = targetColliderSet[i];
                var tryGameObject = tryCollider.gameObject;
                var tryLayer = (GameManager.GameLayerType) tryGameObject.layer;
                
                switch (tryLayer)
                {
                    case GameManager.GameLayerType.UnitA:    
                    case GameManager.GameLayerType.UnitB:    
                    case GameManager.GameLayerType.UnitC:    
                    case GameManager.GameLayerType.UnitD:
                    case GameManager.GameLayerType.UnitE:
                    case GameManager.GameLayerType.UnitF:
                    case GameManager.GameLayerType.UnitG:
                    case GameManager.GameLayerType.UnitH:
                    {
                        if (ReferenceEquals(null, Stamped))
                        {
                            var tryAffine = tryCollider.transform;
                            if (!ReferenceEquals(compareAffine, tryAffine))
                            {
                                var (valid, unit) = UnitInteractManager.GetInstance.TryGetUnit(tryGameObject);
                                Stamped = valid ? unit : tryAffine.GetComponent<Unit>();

                                if (!ReferenceEquals(null, Stamped))
                                {
                                    var topPos = Stamped.GetTopPosition();
                                    if (topPos.y > compareLowerHeight)
                                    {
                                        ResultFlag.AddFlag(UnitTool.UnitStampResultFlag.Overlapped | UnitTool.UnitStampResultFlag.UnitStamped);
                                    }
                                    else
                                    {
                                        ResultFlag.AddFlag(UnitTool.UnitStampResultFlag.UnitStamped);
                                    }
                                }
                            }
                        }
                    }
                        break;
                    case GameManager.GameLayerType.Terrain:
                    {
                        ResultFlag.AddFlag(UnitTool.UnitStampResultFlag.TerrainStamped);
                    }
                        break;
                    case GameManager.GameLayerType.Obstacle:
                    {
                        ResultFlag.AddFlag(UnitTool.UnitStampResultFlag.ObstacleStamped);
                    }
                        break;
                }
            }
        }

        #endregion

        #region <Methods>

        public bool IsStampedTerrainOrObstacle() => ResultFlag.HasAnyFlagExceptNone(
            UnitTool.UnitStampResultFlag.TerrainStamped | UnitTool.UnitStampResultFlag.ObstacleStamped);

        public bool IsStampedOtherUnit() => ResultFlag.HasAnyFlagExceptNone(UnitTool.UnitStampResultFlag.UnitStamped);

        public (bool, Unit) TryGetStampedUnit() => IsStampedOtherUnit() ? (true, Stamped) : default;

        #endregion
    }
}