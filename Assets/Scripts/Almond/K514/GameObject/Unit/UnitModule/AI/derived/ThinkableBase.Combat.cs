using UnityEngine;
using UI2020;

namespace k514
{
    public partial class ThinkableBase
    {
        #region <Callbacks>

        public override void OnStriked(Unit p_Trigger, HitResult p_HitResult)
        {
        }

        public override void OnHitted(Unit p_Trigger, HitResult p_HitResult)
        {
        }

        #endregion
        
        #region <Methods>
        
        /*public abstract bool HasEnemy();
        public abstract void SetEnemy(Unit p_TargetUnit, PrefabInstanceTool.FocusNodeRelateType p_TraceUnitRelateType);
        public abstract void ClearEnemy();
        public abstract float GetSearchDistance();*/
        
        /// <summary>
        /// 지정한 범위 안에 있고 적대 관계이며 지정한 유닛 상태를 전부 만족하지 않는 유닛들을 필터 리스트에 갱신하는 메서드
        /// 갱신 결과 필터링된 유닛이 하나라도 존재하면 참을 리턴
        /// </summary>
        public bool FindEnemyFromPivot(float p_Distance, Unit.UnitStateType p_StateFlag)
        {
            return FindEnemyFromPivot(_MasterNode._Transform.position, p_Distance, p_StateFlag);
        }
        
        /// <summary>
        /// 지정한 범위 안에 있고 적대 관계이며 지정한 유닛 상태를 전부 만족하지 않는 유닛들을 필터 리스트에 갱신하는 메서드
        /// 갱신 결과 필터링된 유닛이 하나라도 존재하면 참을 리턴
        /// </summary>
        public bool FindEnemyFromPivot(Vector3 p_Pivot, float p_Distance, Unit.UnitStateType p_StateFlag)
        {
            var filterValuePreset 
                = new UnitFilterTool.FilterValuePreset
                (
                    UnitFilterTool.FilterSpaceType.Sphere, UnitFilterTool.FilterParamsFlag.CalculateBounds, 
                    p_Pivot, p_Distance
                );
            var filterParams 
                = new FilterParams
                (
                    filterValuePreset, UnitTool.UnitGroupRelateType.Enemy, UnitFilterTool.UnitFilterFlagType.None, p_StateFlag
                );

            return FindEnemyWithParams(filterParams);
        }
        
        /// <summary>
        /// 지정한 범위 안에 있고 적대 관계이며 지정한 유닛 상태를 전부 만족하지 않는 유닛들을 필터 리스트에 갱신하는 메서드
        /// 갱신 결과 필터링된 유닛이 하나라도 존재하면 참을 리턴
        /// </summary>
        public bool FindEnemyWithParams(FilterParams p_FilterParams)
        {
            return UnitInteractManager.GetInstance.FilterUnit_And
                (
                    _MasterNode,
                    p_FilterParams,
                    UnitFilterTool.UnitFilterFlagType.HitFollowGroupRelateExceptMe 
                    | UnitFilterTool.UnitFilterFlagType.Negative_UnitState_And,
                    _MasterNode._UnitFilterResultSet
                )
                .HasAnyFlagExceptNone(UnitFilterTool.FilterResultType.Unit);
        }
        
        /// <summary>
        /// 해당 유닛을 기준으로 주변적을 탐색하는 메서드
        /// </summary>
        /*public (bool, Unit) FindEnemy(UnitActionTool.UnitAction p_UnitAction, Unit.UnitStateType p_StateFlag)
        {
            var tryRecord = p_UnitAction._UnitActionPresetRecord;
            var tryFindType = tryRecord.UnitFindType;
            var findType = tryFindType == ThinkableTool.AIUnitFindType.None
                ? _MindRecord.DefaultUnitFindType
                : tryFindType;
            
            return FindEnemy(tryRecord.AttackRange, findType, p_StateFlag);
        }
        
        /// <summary>
        /// 해당 유닛을 기준으로 주변적을 탐색하는 메서드
        /// </summary>
        public (bool, Unit) FindEnemy(float p_Distance, ThinkableTool.AIUnitFindType p_FindType, Unit.UnitStateType p_StateFlag)
        {
            /*switch (p_FindType)
            {
                case ThinkableTool.AIUnitFindType.NearestPosition:
                {
                    var result = UnitInteractManager.GetInstance.GetNearestEnemyUnitFrom(_MasterNode, p_StateFlag, p_Distance, true);
                    return (!ReferenceEquals(null, result), result);
                }
                default:
                    return FindEnemy(_MasterNode._Transform.position, p_Distance, p_FindType, p_StateFlag);
            }#1#
            
            return FindEnemy(_MasterNode._Transform.position, p_Distance, p_FindType, p_StateFlag);
        }
        
        /// <summary>
        /// 해당 유닛을 기준으로 주변적을 탐색하는 메서드
        /// </summary>
        public (bool, Unit) FindEnemy(Vector3 p_Pivot, float p_Distance, ThinkableTool.AIUnitFindType p_FindType, Unit.UnitStateType p_StateFlag)
        {
            switch (p_FindType)
            {
                case ThinkableTool.AIUnitFindType.None:
                case ThinkableTool.AIUnitFindType.UnitNotFind:
                    return default;
                case ThinkableTool.AIUnitFindType.NearestPosition:
                {
                    var hasEnemy = FindEnemyFromPivot(p_Pivot, p_Distance, p_StateFlag);
                    if (hasEnemy)
                    {
                        var targetList = _MasterNode._UnitFilterResultSet;
                        var enemyCnt = targetList.Count;
                        var minDistance = float.MaxValue;
                        var result = default(Unit);
                        
                        for (int i = 0; i < enemyCnt; i++)
                        {
                            var targetEnemy = targetList[i];
                            var trySqrDistance = (targetEnemy._Transform.position - p_Pivot).sqrMagnitude;
                            if (minDistance > trySqrDistance)
                            {
                                minDistance = trySqrDistance;
                                result = targetEnemy;
                            }
                        }

                        return (true, result);
                    }
                    else
                    {
                        return (false, null);
                    }
                }
                case ThinkableTool.AIUnitFindType.FarthestPosition:
                {
                    var hasEnemy = FindEnemyFromPivot(p_Pivot, p_Distance, p_StateFlag);
                    if (hasEnemy)
                    {
                        var targetList = _MasterNode._UnitFilterResultSet;
                        var enemyCnt = targetList.Count;
                        float minDistance = 0;
                        var result = default(Unit);
                        
                        for (int i = (enemyCnt - 1); i >= 0; i--)
                        {
                            var targetEnemy = targetList[i];
                            var trySqrDistance = (targetEnemy._Transform.position - p_Pivot).sqrMagnitude;
                            if (minDistance < trySqrDistance)
                            {
                                minDistance = trySqrDistance;
                                result = targetEnemy;
                            }
                        }

                        return (true, result);
                    }
                    else
                    {
                        return (false, null);
                    }
                }
                case ThinkableTool.AIUnitFindType.NearestAngle:
                {
                    var hasEnemy = FindEnemyFromPivot(p_Pivot, p_Distance, p_StateFlag);
                    if (hasEnemy)
                    {
                        var targetList = _MasterNode._UnitFilterResultSet;
                        var enemyCnt = targetList.Count;
                        var pivot = _MasterNode._Transform.forward;
                        var maxDot = float.MinValue;
                        var result = default(Unit);
                        
                        for (int i = 0; i < enemyCnt; i++)
                        {
                            var targetEnemy = targetList[i];
                            var uv = _MasterNode.GetDirectionUnitVectorTo(targetEnemy);

                            var tryDot = Vector3.Dot(pivot, uv);
                            if (tryDot > maxDot)
                            {
                                maxDot = tryDot;
                                result = targetEnemy;
                            }
                        }

                        return (true, result);
                    }
                    else
                    {
                        return (false, null);
                    }
                }
                case ThinkableTool.AIUnitFindType.Random:
                {
                    var hasEnemy = FindEnemyFromPivot(p_Pivot, p_Distance, p_StateFlag);
                    if (hasEnemy)
                    {
                        var targetList = _MasterNode._UnitFilterResultSet;
                        var enemyCnt = targetList.Count;
                        return (true, targetList[Random.Range(0, enemyCnt)]);
                    }
                    else
                    {
                        return (false, null);
                    }
                }
                case ThinkableTool.AIUnitFindType.PriorityHigh:
                {
                    var hasEnemy = FindEnemyFromPivot(p_Pivot, p_Distance, p_StateFlag);
                    if (hasEnemy)
                    {
                        var targetList = _MasterNode._UnitFilterResultSet;
                        var enemyCnt = targetList.Count;
                        var maxPriority = -1;
                        var minPriority = UnitTool.__PLAYER_PRIORITY;
                        var result = default(Unit);
                        
                        for (int i = 0; i < enemyCnt; i++)
                        {
                            var targetEnemy = targetList[i];
                            var targetEnemyPriority = targetEnemy.Priority;
                            if (targetEnemyPriority > maxPriority)
                            {
                                maxPriority = targetEnemyPriority;
                                result = targetEnemy;
                            }

                            if (targetEnemyPriority < minPriority)
                            {
                                minPriority = targetEnemyPriority;
                            }
                        }

                        if (maxPriority == minPriority)
                        {
                            return (true, targetList[Random.Range(0, enemyCnt)]);
                        }
                        else
                        {
                            return (true, result);
                        }
                    }
                    else
                    {
                        return (false, null);
                    }
                }
                case ThinkableTool.AIUnitFindType.PriorityLow:
                {
                    var hasEnemy = FindEnemyFromPivot(p_Pivot, p_Distance, p_StateFlag);
                    if (hasEnemy)
                    {
                        var targetList = _MasterNode._UnitFilterResultSet;
                        var enemyCnt = targetList.Count;
                        var maxPriority = -1;
                        var minPriority = UnitTool.__PLAYER_PRIORITY;
                        var result = default(Unit);
                        
                        for (int i = 0; i < enemyCnt; i++)
                        {
                            var targetEnemy = targetList[i];
                            var targetEnemyPriority = targetEnemy.Priority;
                            if (targetEnemyPriority > maxPriority)
                            {
                                maxPriority = targetEnemyPriority;
                            }

                            if (targetEnemyPriority < minPriority)
                            {
                                minPriority = targetEnemyPriority;
                                result = targetEnemy;
                            }
                        }

                        if (maxPriority == minPriority)
                        {
                            return (true, targetList[Random.Range(0, enemyCnt)]);
                        }
                        else
                        {
                            return (true, result);
                        }
                    }
                    else
                    {
                        return (false, null);
                    }
                }
            }
            
            return (false, null);
        }*/
        
        #endregion
    }
}