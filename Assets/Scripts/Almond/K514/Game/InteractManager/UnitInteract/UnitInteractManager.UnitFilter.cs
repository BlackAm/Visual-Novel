using System;
using System.Collections.Generic;
using UnityEngine;

namespace k514
{
    public partial class UnitInteractManager
    {
        #region <Fields>

        /// <summary>
        /// 지정한 조건에 맞게 필터된 유닛들의 리스트. 해당 리스트는 필터링이 진행될 때마다 내용이 바뀌기 때문에
        /// 참조하고 여러 프레임에 걸쳐 사용할 수 없다.
        /// </summary>
        private Dictionary<UnitFilterTool.UnitFilterFlagType, Func<FilterParams, Unit, Unit, bool>> _FilterToolTable;
        
        public UnitFilterTool.UnitFilterFlagType[] _Enumerator;

        #endregion

        #region <Callbacks>

        public void OnCreated_UnitFilter()
        {
            _Enumerator = SystemTool.GetEnumEnumerator<UnitFilterTool.UnitFilterFlagType>(SystemTool.GetEnumeratorType.ExceptMaskNone);
            _FilterToolTable = new Dictionary<UnitFilterTool.UnitFilterFlagType, Func<FilterParams, Unit, Unit, bool>>();

            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.UnitState_And, (filterParams, fromUnit, targetUnit) => targetUnit.HasState_And(filterParams._UnitStateFilterMask));
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.UnitState_Or, (filterParams, fromUnit, targetUnit) => targetUnit.HasState_Or(filterParams._UnitStateFilterMask));
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.Negative_UnitState_And, (filterParams, fromUnit, targetUnit) => !targetUnit.HasState_Or(filterParams._UnitStateFilterMask));
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.Negative_UnitState_Or, (filterParams, fromUnit, targetUnit) => !targetUnit.HasState_And(filterParams._UnitStateFilterMask));
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.ExceptMe, (filterParams, fromUnit, targetUnit) => fromUnit != targetUnit);
            // _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.UnitGroupRelate, FilterEvent_UnitGroupRelate);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.Player, (filterParams, fromUnit, targetUnit) => targetUnit.IsPlayer);
    
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.SqrDistanceTable, FilterEvent_SqrDistance);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.XZAngle, FilterEvent_XZAngle);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.XZCircle, FilterEvent_XZCircle);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.Circle, FilterEvent_Circle);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.Sphere, FilterEvent_Sphere);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.XZBox, FilterEvent_XZBox);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.Box, FilterEvent_Box);
            _FilterToolTable.Add(UnitFilterTool.UnitFilterFlagType.DistanceEventSender, (filterParams, fromUnit, targetUnit) => targetUnit.IsPositionEventSender());
        }

        #endregion

        #region <Method/Filter>

        public UnitFilterTool.FilterResultType FilterUnit_And(Unit p_FromUnit, FilterParams p_FilterParams, UnitFilterTool.UnitFilterFlagType p_FilterMask,
            List<Unit> r_FilterUnitSet, bool p_FindFocusUnitFirst = true)
        {
            var result = UnitFilterTool.FilterResultType.None;
            
            r_FilterUnitSet.Clear();
            p_FilterParams.OnFilterPivotSelected(p_FromUnit);
            p_FilterMask.AddFlag(p_FilterParams._BindedFilterFlag);

            if (p_FilterParams.OnCheckObstacleCollision())
            {
                result.AddFlag(UnitFilterTool.FilterResultType.Obstacle);
            }

            // 전체 유닛 검색 이전에 검색 기준 유닛의 포커스 유닛을 우선 필터링한다.
            var tryFocusUnit = p_FromUnit.FocusNode;
            if (p_FindFocusUnitFirst)
            {
                if (tryFocusUnit)
                {
                    var tryFilterParams = p_FilterParams;
                    tryFilterParams.OnFilterTargetSelected(p_FromUnit, tryFocusUnit);
                    
                    foreach (var filterFlagType in _Enumerator)
                    {
                        if (p_FilterMask.HasAnyFlagExceptNone(filterFlagType))
                        {
                            var filterResult = _FilterToolTable[filterFlagType](tryFilterParams, p_FromUnit, tryFocusUnit);
                            if (!filterResult)
                            {
                                goto out_loop;
                            }
                        }
                    }
                    
                    r_FilterUnitSet.Add(tryFocusUnit);
                    result.AddFlag(UnitFilterTool.FilterResultType.Unit);
                    out_loop :; 
                }
            }

            var targetArray = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var eventHandler in targetArray)
            {
                var tryHandler = eventHandler;
                var targetUnit = tryHandler._ThisUnit;
                var tryFilterParams = p_FilterParams;
                tryFilterParams.OnFilterTargetSelected(p_FromUnit, targetUnit);
                
                foreach (var filterFlagType in _Enumerator)
                {
                    if (p_FilterMask.HasAnyFlagExceptNone(filterFlagType))
                    {
                        var filterResult = _FilterToolTable[filterFlagType](tryFilterParams, p_FromUnit, targetUnit);
                        if (!filterResult)
                        {
                            goto out_loop;
                        }
                    }
                }

                if (!p_FindFocusUnitFirst || !ReferenceEquals(targetUnit, tryFocusUnit))
                {
                    r_FilterUnitSet.Add(targetUnit);
                    result.AddFlag(UnitFilterTool.FilterResultType.Unit);
                }
                out_loop :; 
            }

            return result;
        }
        
        public UnitFilterTool.FilterResultType FilterUnit_Or(Unit p_FromUnit, FilterParams p_FilterParams, UnitFilterTool.UnitFilterFlagType p_FilterMask,
            List<Unit> r_FilterUnitSet, bool p_FindFocusUnitFirst = true)
        {
            var result = UnitFilterTool.FilterResultType.None;

            r_FilterUnitSet.Clear();
            p_FilterParams.OnFilterPivotSelected(p_FromUnit);
            p_FilterMask.AddFlag(p_FilterParams._BindedFilterFlag);

            if (p_FilterParams.OnCheckObstacleCollision())
            {
                result.AddFlag(UnitFilterTool.FilterResultType.Obstacle);
            }

            // 전체 유닛 검색 이전에 검색 기준 유닛의 포커스 유닛을 우선 필터링한다.
            var tryFocusUnit = p_FromUnit.FocusNode;
            if (p_FindFocusUnitFirst)
            {
                if (tryFocusUnit)
                {
                    var tryFilterParams = p_FilterParams;
                    tryFilterParams.OnFilterTargetSelected(p_FromUnit, tryFocusUnit);
                    
                    foreach (var filterFlagType in _Enumerator)
                    {
                        if ((p_FilterMask | filterFlagType) == p_FilterMask)
                        {
                            var filterResult =
                                _FilterToolTable[filterFlagType](tryFilterParams, p_FromUnit, tryFocusUnit);
                            if (filterResult)
                            {
                                r_FilterUnitSet.Add(tryFocusUnit);
                                result.AddFlag(UnitFilterTool.FilterResultType.Unit);
                                goto out_loop;
                            }
                        }
                    }
                    out_loop : ;
                }
            }
            
            var targetArray = UnitEventHandlerPool.ActivedObjectPool;
            foreach (var eventHandler in targetArray)
            {
                var tryHandler = eventHandler;
                var targetUnit = tryHandler._ThisUnit;
                var tryFilterParams = p_FilterParams;
                tryFilterParams.OnFilterTargetSelected(p_FromUnit, targetUnit);
                
                foreach (var filterFlagType in _Enumerator)
                {
                    if ((p_FilterMask | filterFlagType) == p_FilterMask)
                    {
                        var filterResult =
                            _FilterToolTable[filterFlagType](tryFilterParams, p_FromUnit, targetUnit);
                        if (filterResult)
                        {
                            if (!p_FindFocusUnitFirst || !ReferenceEquals(targetUnit, tryFocusUnit))
                            {
                                r_FilterUnitSet.Add(targetUnit);
                                result.AddFlag(UnitFilterTool.FilterResultType.Unit);
                            }
                            goto out_loop;
                        }
                    }
                }
                out_loop : ;
            }
            
            return result;
        }

        #endregion

        #region <Method/EventBind/State>

        /*private bool FilterEvent_UnitGroupRelate(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            return (p_FromUnit.GetGroupRelate(p_FilterTargetUnit) | p_FilterParams._UnitGroupRelateFilterMask) == p_FilterParams._UnitGroupRelateFilterMask;
        }

        private bool FilterEvent_CurrentFocusNode_Or_UnitGroupRelate(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var focusNode = p_FromUnit.FocusNode;
            return (focusNode && ReferenceEquals(focusNode.Node, p_FilterTargetUnit)) || FilterEvent_UnitGroupRelate(p_FilterParams, p_FromUnit, p_FilterTargetUnit);
        }*/
        
        private bool FilterEvent_SqrDistance(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var tryValuePreset = p_FilterParams._FilterValuePreset;
            var trySqr = GetSqrDistanceBetween(p_FromUnit, p_FilterTargetUnit);
            
            return tryValuePreset.FloatValue1 <= trySqr && trySqr <= tryValuePreset.FloatValue0;
        }

        #endregion

        #region <Method/EventBind/Space>

        private bool FilterEvent_XZAngle(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var tryValuePreset = p_FilterParams._FilterValuePreset;
            var baseVector = tryValuePreset._FilterAffine.Forward;
            var centerPosition = tryValuePreset._FilterAffine.Position;
            var compareVector = centerPosition.GetDirectionVectorTo(p_FilterTargetUnit._Transform.position).XZUVector();
            var dotP = Vector3.Dot(baseVector, compareVector);
            var cos = Mathf.Cos(Mathf.Deg2Rad * tryValuePreset.FloatValue3);

            return dotP >= cos;
        }

        private bool FilterEvent_XZCircle(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var interpolatePreset = p_FilterParams._FilterInterpolatePreset;
            if (interpolatePreset.FilterInterpolate(p_FromUnit, p_FilterTargetUnit))
            {
                return true;
            }
            else
            {
                var tryValuePreset = p_FilterParams._FilterValuePreset;
                var circleCenter = tryValuePreset._FilterAffine.Position;
                var trySqr = (circleCenter - p_FilterTargetUnit._Transform.position).XZVector().sqrMagnitude;
                
                return tryValuePreset.FloatValue1 <= trySqr && trySqr <= tryValuePreset.FloatValue0;
            }
        }

        private bool FilterEvent_Circle(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var interpolatePreset = p_FilterParams._FilterInterpolatePreset;
            if (interpolatePreset.FilterInterpolate(p_FromUnit, p_FilterTargetUnit))
            {
                return true;
            }
            else
            {
                var tryValuePreset = p_FilterParams._FilterValuePreset;
                var circleCenter = tryValuePreset._FilterAffine.Position;
                var trySqr = (circleCenter - p_FilterTargetUnit._Transform.position).XZVector().sqrMagnitude;

                if (tryValuePreset.FloatValue1 <= trySqr && trySqr <= tryValuePreset.FloatValue0)
                {
                    var centerY = circleCenter.y;
                    var halfHeight = tryValuePreset.FloatValue2;
                    var upperBound = centerY + halfHeight;
                    var lowerBound = centerY - halfHeight;
                    var targetUnitY = p_FilterTargetUnit.GetCenterPosition().y;
                        
                    return targetUnitY < upperBound && lowerBound < targetUnitY;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool FilterEvent_Sphere(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var interpolatePreset = p_FilterParams._FilterInterpolatePreset;
            if (interpolatePreset.FilterInterpolate(p_FromUnit, p_FilterTargetUnit))
            {
                return true;
            }
            else
            {
                var tryValuePreset = p_FilterParams._FilterValuePreset;
                var sphereCenter = tryValuePreset._FilterAffine.Position;
                var trySqr = (sphereCenter - p_FilterTargetUnit.GetCenterPosition()).sqrMagnitude;
                
                return tryValuePreset.FloatValue1 <= trySqr && trySqr <= tryValuePreset.FloatValue0;
            }
        }

        private bool FilterEvent_XZBox(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var interpolatePreset = p_FilterParams._FilterInterpolatePreset;
            if (interpolatePreset.FilterInterpolate(p_FromUnit, p_FilterTargetUnit))
            {
                return true;
            }
            else
            {
                var tryValuePreset = p_FilterParams._FilterValuePreset;
                var customPlane = tryValuePreset._FilterPlane;
                
                return customPlane.CheckPointInnerPlane(p_FilterTargetUnit.GetCenterPosition());
            }
        }
        
        private bool FilterEvent_Box(FilterParams p_FilterParams, Unit p_FromUnit, Unit p_FilterTargetUnit)
        {
            var interpolatePreset = p_FilterParams._FilterInterpolatePreset;
            if (interpolatePreset.FilterInterpolate(p_FromUnit, p_FilterTargetUnit))
            {
                return true;
            }
            else
            {
                var tryValuePreset = p_FilterParams._FilterValuePreset;
                var customPlane = tryValuePreset._FilterPlane;
                var halfHeight = tryValuePreset.FloatValue2;
                
                return customPlane.CheckPointInnerPlane(p_FilterTargetUnit.GetCenterPosition(), halfHeight);
            }
        }
        
        #endregion
    }
}