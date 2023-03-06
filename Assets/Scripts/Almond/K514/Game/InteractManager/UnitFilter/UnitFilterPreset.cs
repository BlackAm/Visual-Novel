using UnityEngine;

namespace k514
{
    #region <Struct>

    /// <summary>
    /// 유닛을 필터링하는 경우 사용하는 데이터 프리셋
    /// </summary>
    public struct FilterParams
    {
        #region <Fields>

        /// <summary>
        /// 해당 필터링 가상 공간을 기술하는 벨류 프리셋
        /// </summary>
        public UnitFilterTool.FilterValuePreset _FilterValuePreset;

        /// <summary>
        /// 해당 필터 초기화 시, 필터 타입에 따라 같이 초기화되는 유닛 필터 플래그
        /// </summary>
        public UnitFilterTool.UnitFilterFlagType _BindedFilterFlag;
            
        /// <summary>
        /// 타겟 유닛을 필터링할 때 사용할 타겟 유닛 상태 마스크
        /// </summary>
        public Unit.UnitStateType _UnitStateFilterMask;
        
        /// <summary>
        /// 타겟 유닛을 필터링할 때 사용할 동맹 필터 마스크
        /// </summary>
        public UnitTool.UnitGroupRelateType _UnitGroupRelateFilterMask;

        /// <summary>
        /// 필터 공간 사이를 보간하여 필터링할 때 사용할 프리셋
        /// </summary>
        public UnitFilterTool.FilterInterpolatePreset _FilterInterpolatePreset;
        
        #endregion
        
        #region <Constructor>

        /// <summary>
        /// 생성자
        /// </summary>
        public FilterParams(
            UnitFilterTool.FilterValuePreset p_FilterValuePreset,
            UnitTool.UnitGroupRelateType p_UnitGroupMask = UnitTool.UnitGroupRelateType.Enemy | UnitTool.UnitGroupRelateType.Neutral, 
            UnitFilterTool.UnitFilterFlagType p_BindedFilterFlag = UnitFilterTool.UnitFilterFlagType.None,
            Unit.UnitStateType p_UnitStateFilterMask = Unit.UnitStateType.None)
        {
            _FilterValuePreset = p_FilterValuePreset;
            _BindedFilterFlag = p_BindedFilterFlag;
            
            switch (_FilterValuePreset.SpaceType)
            {
                case UnitFilterTool.FilterSpaceType.SqrDistance:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.SqrDistanceTable);
                    break;
                case UnitFilterTool.FilterSpaceType.XZPartialCircle:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.XZAngle);
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.XZCircle);
                    break;
                case UnitFilterTool.FilterSpaceType.PartialCircle:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.XZAngle);
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.Circle);
                    break;
                case UnitFilterTool.FilterSpaceType.XZCircle:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.XZCircle);
                    break;
                case UnitFilterTool.FilterSpaceType.Circle:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.Circle);
                    break;
                case UnitFilterTool.FilterSpaceType.Sphere:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.Sphere);
                    break;
                case UnitFilterTool.FilterSpaceType.XZBox:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.XZBox);
                    break;
                case UnitFilterTool.FilterSpaceType.Box:
                    _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.Box);
                    break;
                default:
                case UnitFilterTool.FilterSpaceType.NoneSpace:
                    break;
            }
            
            _UnitGroupRelateFilterMask = p_UnitGroupMask;
            _UnitStateFilterMask = p_UnitStateFilterMask;

            if (_UnitStateFilterMask == Unit.UnitStateType.None)
            {
                _BindedFilterFlag.AddFlag(UnitFilterTool.UnitFilterFlagType.HitFollowGroupRelateExceptMe);
            }

            _FilterInterpolatePreset = default;
        }
        
        public FilterParams(float p_Distance, bool p_CalculateBoundFlag) 
            : this(new UnitFilterTool.FilterValuePreset(p_Distance, p_CalculateBoundFlag ? UnitFilterTool.FilterParamsFlag.CalculateBounds : UnitFilterTool.FilterParamsFlag.None))
        {
        }
        
        #endregion

        #region <Callback>

        public void OnFilterPivotSelected(Unit p_Pivot)
        {
            _FilterInterpolatePreset.OnFilterPivotSelected(p_Pivot, _FilterValuePreset);
            _FilterValuePreset.OnFilterPivotSelected(p_Pivot);
        }

        public void OnFilterTargetSelected(Unit p_Pivot, Unit p_Target)
        {
            _FilterInterpolatePreset.OnFilterTargetSelected(p_Pivot, p_Target);
            _FilterValuePreset.OnFilterTargetSelected(p_Pivot, p_Target);
        }

        public bool OnCheckObstacleCollision()
        {
            return _FilterValuePreset.OnCheckObstacleCollision();
        }

        #endregion

        #region <Methods>

        public void SetInterpolatePivotAffine(TransformTool.AffineCachePreset p_Affine)
        {
            _FilterInterpolatePreset = new UnitFilterTool.FilterInterpolatePreset(p_Affine);
        }

        public void SetFilterAffine(Unit p_Unit, bool p_OverlapFlag)
        {
            _FilterValuePreset.SetFilterAffine(p_Unit, p_OverlapFlag);
        }
            
        public void SetFilterAffine(TransformTool.AffineCachePreset p_Affine, bool p_OverlapFlag)
        {
            _FilterValuePreset.SetFilterAffine(p_Affine, p_OverlapFlag);
        }

        public void SetFilterFlag(UnitFilterTool.FilterParamsFlag p_FilterParamsFlag, bool p_Enable)
        {
            _FilterValuePreset.SetFilterFlag(p_FilterParamsFlag, p_Enable);
        }

        #endregion
    }

    #endregion
}