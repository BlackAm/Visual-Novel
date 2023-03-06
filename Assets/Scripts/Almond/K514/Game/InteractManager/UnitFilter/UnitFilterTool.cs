using System;
using UnityEngine;

namespace k514
{
    public static class UnitFilterTool
    {
        #region <Enums>

        /// <summary>
        /// 유닛을 필터링할 가상 공간 타입
        /// </summary>
        public enum FilterSpaceType
        {
            /// <summary>
            /// 기본 타입 필터링 수행안함
            /// </summary>
            NoneSpace,
            
            /// <summary>
            /// Environment Table 상 제곱거리
            /// 해당 테이블은 각 유닛의 상호작용 이벤트에 의해 특정한 타이밍에 의해 자동 갱신된 값을 참조한다.
            /// 해당 필터링에는 테이블 참조 연산만 수행해서 가볍다.
            /// </summary>
            SqrDistance,
            
            /// <summary>
            /// 높이가 무한한 부채꼴 원기둥 필터링
            /// </summary>
            XZPartialCircle,
                        
            /// <summary>
            /// 높이가 일정한 부채꼴 원기둥 필터링
            /// </summary>
            PartialCircle,
            
            /// <summary>
            /// 높이가 무한한 원기둥 필터링
            /// </summary>
            XZCircle,
            
            /// <summary>
            /// 높이가 일정한 원기둥 필터링
            /// </summary>
            Circle,
            
            /// <summary>
            /// 구 필터링 혹은 거리 필터링
            /// </summary>
            Sphere,
            
            /// <summary>
            /// 높이가 무한한 직육면체 필터링
            /// </summary>
            XZBox,
            
            /// <summary>
            /// 높이가 무한한 직육면체 필터링
            /// </summary>
            Box,
        }

        /// <summary>
        /// 필터 파라미터 구조체에서 사용할 플래그 타입
        /// </summary>
        [Flags]
        public enum FilterParamsFlag
        {
            None = 0,
            
            /// <summary>
            /// 지정한 필터 가상 공간을 전방에 설치한다.
            ///
            /// 예를 들어, XZBox의 경우에는 기준 유닛의 위치에 사각형의 중심이 오지만
            /// 해당 플래그를 가진 필터링에서는 사각형의 너비변의 중심이 기준 유닛 위치에 오고
            /// 높이변이 플레이어가 바라보는 방향을 향하게 된다.
            /// </summary>
            ForwardCast = 1 << 0,
            
            /// <summary>
            /// 지정한 필터 가상 공간이 필터링 대상이 되는 타겟 유닛의 반경을 고려하여 필터링을 수행한다.
            /// 해당 플래그가 없다면 단순히 점 대 점 필터링이 된다.
            /// </summary>
            CalculateBounds = 1 << 1,
            
            /// <summary>
            /// 해당 파라미터가 배치 프리셋 정보를 가지는지 표시하는 플래그
            /// 해당 플래그가 없다면 기본적으로 기준 유닛을 기준으로 가상 공간을 생성하지만
            /// 해당 플래그를 가지는 경우, 배치 프리셋의 아핀값을 따라서 가상 공간을 생성한다.
            /// </summary>
            UsingAffine = 1 << 2,
            
            /// <summary>
            /// 해당 필터링은 'Unit'Filter 이기 때문에 기본적으로 Unit 오브젝트만 가지고 연산을 수행하지만
            /// Obstacle 오브젝트도 필터링해야하는 경우도 있기 때문에 추가된 플래그
            /// </summary>
            FilterObstacle = 1 << 3,
            
            /// <summary>
            /// 모든 가상 공간의 필터 거리는 [최소거리, 최대거리]로 구성되어 단순한 원이 아니라 도너츠 모양처럼 필터링이 가능한데
            /// 해당 플래그가 있다면 최소거리를 거리 대신 '최대거리로부터 offset'으로 적용한다.
            /// </summary>
            UsingLowerRadiusAsOffset = 1 << 4,
            
            /// <summary>
            /// 해당 플래그 보유시, 높이값을 유닛의 높이 값만큼 더해준다.
            /// </summary>
            CorrectHeight = 1 << 5,
        }

        /// <summary>
        /// 필터링을 수행할 타입
        /// 마스크 이외의 각 타입은 필터링 함수와 1:1 관계를 가지고 있다.
        /// </summary>
        [Flags]
        public enum UnitFilterFlagType
        {
            /// <summary>
            /// 기본 상태
            /// </summary>
            None = 0,

            
            /* Filter State */
            /// <summary>
            /// FilterParam 구조체에서 지정한 상태 플래그를 전부 만족하는 오브젝트를 필터링
            /// </summary>
            UnitState_And = 1 << 0,
            
            /// <summary>
            /// FilterParam 구조체에서 지정한 상태 플래그를 하나라도 만족하는 오브젝트를 필터링
            /// </summary>
            UnitState_Or = 1 << 1,

            /// <summary>
            /// FilterParam 구조체에서 지정한 상태 플래그를 전부 만족하지 않는 오브젝트를 필터링
            /// </summary>
            Negative_UnitState_And = 1 << 2,
            
            /// <summary>
            /// FilterParam 구조체에서 지정한 상태 플래그를 하나라도 만족하지 않는 오브젝트를 필터링
            /// </summary>
            Negative_UnitState_Or = 1 << 3,

            /// <summary>
            /// FilterParam 구조체에서 지정한 유닛 동맹관계를 고려하여 필터링
            /// </summary>
            UnitGroupRelate = 1 << 4,

            /// <summary>
            /// 자기 자신을 제외하는 유닛 필터링
            /// </summary>
            ExceptMe = 1 << 5,

            /// <summary>
            /// 플레이어 유닛을 찾는 필터링
            /// </summary>
            Player = 1 << 6,
                       
            /// <summary>
            /// 거리 이벤트 송신 유닛을 찾는 필터링
            /// </summary>
            DistanceEventSender = 1 << 7,

            
            /* Virtual Space Filter */
            /// <summary>
            /// 전체 유닛 오브젝트에서 지정한 제곱 거리 안에 있는 오브젝트를 필터링
            /// Environment의 거리 테이블을 참조한다.
            /// </summary>
            SqrDistanceTable = 1 << 16,

            /// <summary>
            /// 지정한 유닛이 바라보는 방향으로 좌우 지정한 각도 이내의 유닛을 필터링
            /// </summary>
            XZAngle = 1 << 17,

            /// <summary>
            /// 지정한 유닛을 기준으로 일정 제곱거리 안에 있는 유닛 필터링, 원기둥 필터링
            /// </summary>
            XZCircle = 1 << 18,

            /// <summary>
            /// 지정한 유닛을 기준으로 일정 제곱거리 안에 있고 일정 높이 차이를 가지는 유닛 필터링, 원기둥 필터링
            /// </summary>
            Circle = 1 << 19,

            /// <summary>
            /// 지정한 유닛을 기준으로 일정 제곱거리 안에 있는 유닛 필터링, 구 필터링
            /// </summary>
            Sphere = 1 << 20,

            /// <summary>
            /// 지정한 유닛이 바라보는 방향으로 일정 너비/길이 안에 존재하는 유닛 필터링, 직육면체 필터링
            /// </summary>
            XZBox = 1 << 21,

            /// <summary>
            /// 지정한 유닛이 바라보는 방향으로 일정 너비/길이/ 높이 안에 존재하는 유닛 필터링, 직육면체 필터링
            /// </summary>
            Box = 1 << 22,

            
            /* Flag Mask */
            /// <summary>
            /// 적 공격시 자주 사용하는 필터링
            /// </summary>
            HitFollowGroupRelateExceptMe = UnitGroupRelate | ExceptMe,
        }
        
        /// <summary>
        /// 필터링 결과 타입
        /// </summary>
        [Flags]
        public enum FilterResultType
        {
            None = 0,
            Unit = 1 << 0,
            Obstacle = 1 << 1,
        }

        #endregion

        #region <Structs>

        public struct FilterInterpolatePreset
        {
            #region <Fields>

            public TransformTool.AffineCachePreset InterpolatePivot;
            public bool IsValid;
            public Vector3 MidPoint;
            public Vector3 MainDirection;
            public float Distance;
            public float HalfWidth;
            public float HalfHeight;
            private bool IsXZFilter;
            private bool CalcBoundFlag;

            #endregion

            #region <Constructors>

            public FilterInterpolatePreset(TransformTool.AffineCachePreset p_Affine)
            {
                InterpolatePivot = p_Affine;
                IsValid = true;
                MidPoint = default;
                MainDirection = default;
                Distance = default;
                HalfWidth = default;
                HalfHeight = default;
                IsXZFilter = false;
                CalcBoundFlag = false;
            }

            #endregion
            
            #region <Callbacks>

            public void OnFilterPivotSelected(Unit p_Pivot, FilterValuePreset p_FilterValuePreset)
            {
                if (IsValid)
                {
                    var filterSpaceType = p_FilterValuePreset.SpaceType;
                    switch (filterSpaceType)
                    {
                        case FilterSpaceType.NoneSpace:
                            break;
                        case FilterSpaceType.SqrDistance:
                            HalfWidth = p_FilterValuePreset.FloatValue0;
                            HalfHeight = p_FilterValuePreset.FloatValue0;
                            break;
                        case FilterSpaceType.XZPartialCircle:
                            IsXZFilter = true;
                            HalfWidth = p_FilterValuePreset.FloatValue0;
                            break;
                        case FilterSpaceType.PartialCircle:
                            HalfWidth = p_FilterValuePreset.FloatValue0;
                            HalfHeight = p_FilterValuePreset.FloatValue3;
                            break;
                        case FilterSpaceType.XZCircle:
                            IsXZFilter = true;
                            HalfWidth = p_FilterValuePreset.FloatValue0;
                            break;
                        case FilterSpaceType.Circle:
                            HalfWidth = p_FilterValuePreset.FloatValue0;
                            HalfHeight = p_FilterValuePreset.FloatValue2;
                            break;
                        case FilterSpaceType.Sphere:
                            HalfWidth = p_FilterValuePreset.FloatValue0;
                            HalfHeight = p_FilterValuePreset.FloatValue0;
                            break;
                        case FilterSpaceType.XZBox:
                            IsXZFilter = true;
                            HalfWidth = 0.5f * p_FilterValuePreset.FloatValue0;
                            break;
                        case FilterSpaceType.Box:
                            HalfWidth = 0.5f * p_FilterValuePreset.FloatValue0;
                            HalfHeight = p_FilterValuePreset.FloatValue2;
                            break;
                    }

                    MidPoint = 0.5f * (InterpolatePivot.Position + p_Pivot.GetCenterPosition());
                    MainDirection = InterpolatePivot.GetDirectionVectorTo(p_Pivot).XZVector();
                    Distance = MainDirection.magnitude;

                    if (Distance < CustomMath.Epsilon)
                    {
                        IsValid = false;
                    }
                    else
                    {
                        MainDirection = MainDirection.normalized;
                        CalcBoundFlag = p_FilterValuePreset.FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CalculateBounds);  
                    }
                }
            }

            public void OnFilterTargetSelected(Unit p_Pivot, Unit p_Target)
            {
                if (CalcBoundFlag)
                {
                    HalfWidth = HalfWidth + p_Pivot.GetRadius() + p_Target.GetRadius();
                    HalfHeight = HalfHeight + p_Pivot.GetHeight(0.5f) + p_Target.GetHeight(0.5f);
                }
            }

            #endregion

            #region <Methods>
            
            public bool FilterInterpolate(Unit p_Pivot, Unit p_Target)
            {
                if (IsValid)
                {
                    var customPlane = CustomPlane.Get_XZ_Basis_Location(MidPoint, MainDirection, 2f * HalfWidth, Distance);
                    var tryPosition = p_Target.GetCenterPosition();
                    
#if UNITY_EDITOR
                    if (CustomDebug.DrawUnitFilter)
                    {
#if !SERVER_DRIVE && ON_GUI
                        if (TestManager.GetInstanceUnSafe.IsSelected(p_Pivot))
#endif
                        {
                            CustomDebug.DrawCustomPlane(customPlane, HalfHeight, Color.green, 8, 1f);
                        }
                    }
#endif
                    
                    return customPlane.CheckPointInnerPlane(tryPosition, IsXZFilter ? CustomMath.MaxLineLength : HalfHeight);
                }
                else
                {
                    return false;
                }
            }

            #endregion
        }

        /// <summary>
        /// 필터링 가상 공간을 기술하는 벨류 프리셋
        /// </summary>
        public struct FilterValuePreset
        {
            #region <Consts>

#if UNITY_EDITOR
            private const float Gizmos_Duration = 2f;
            private static readonly Color Gizmos_NonUnitBound_Color = Color.red;
            private static readonly Color Gizmos_UnitBound_Color = Color.blue;
#endif

            #endregion
            
            #region <Fields>

            /// <summary>
            /// 해당 필터링 가상 공간 타입
            /// </summary>
            public FilterSpaceType SpaceType;
            
            /// <summary>
            /// 필터 파라미터 플래그
            /// </summary>
            public FilterParamsFlag FilterParamsFlag;
            
            /// <summary>
            /// 가상 공간 기술 벨류
            /// </summary>
            public float FloatValue0;
            
            /// <summary>
            /// 가상 공간 기술 벨류
            /// </summary>
            public float FloatValue1;
            
            /// <summary>
            /// 가상 공간 기술 벨류
            /// </summary>
            public float FloatValue2;
            
            /// <summary>
            /// 가상 공간 기술 벨류
            /// </summary>
            public float FloatValue3;
        
            /// <summary>
            /// 필터링 가상 공간을 기술할 아핀 변환 프리셋
            /// </summary>
            public TransformTool.AffineCachePreset _FilterAffine;

            /// <summary>
            /// 필터링 가상 공간 중, 다각형 공간을 정의할 평면 프리셋
            /// </summary>
            public CustomPlane _FilterPlane;
            
            #endregion

            #region <Constructors>

            public FilterValuePreset(FilterSpaceType p_SpaceType = FilterSpaceType.NoneSpace, FilterParamsFlag p_FilterParamsFlag = FilterParamsFlag.None,
                TransformTool.AffineCachePreset p_FilterAffine = default, float p_Val0 = default, float p_Val1 = default, float p_Val2 = default, float p_Val3 = default)
            {
                SpaceType = p_SpaceType;
                FilterParamsFlag = p_FilterParamsFlag;
                FloatValue0 = p_Val0;
                FloatValue1 = p_Val1;
                FloatValue2 = p_Val2;
                FloatValue3 = p_Val3;
                _FilterAffine = p_FilterAffine;
                _FilterPlane = default;
            }
            
            public FilterValuePreset(FilterSpaceType p_SpaceType = FilterSpaceType.NoneSpace, FilterParamsFlag p_FilterParamsFlag = FilterParamsFlag.None,
                float p_Val0 = default, float p_Val1 = default, float p_Val2 = default, float p_Val3 = default)
            : this(p_SpaceType, p_FilterParamsFlag, default, p_Val0, p_Val1, p_Val2, p_Val3)
            {
            }

            public FilterValuePreset(float p_Distance, FilterParamsFlag p_FilterParamsFlag = FilterParamsFlag.None)
            : this(FilterSpaceType.SqrDistance, p_FilterParamsFlag, p_Distance)
            {
            }
            
            #endregion

            #region <Callbacks>

            public void OnFilterPivotSelected(Unit p_Pivot)
            {
                // 배치 기준이 없다면, 기준 유닛을 배치 기준으로 삼는다.
                SetFilterAffine(p_Pivot, false);
                
                var checkBound = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CalculateBounds);
                switch (SpaceType)
                {
                    case FilterSpaceType.NoneSpace:
                        break;
                    // val 0 : sqrDistance UpperBound, val 1 = sqrDistance LowerBound or Offset, val 2 = Wide Half Degree, val 3 = Half Height
                    case FilterSpaceType.PartialCircle:
                    {
                        goto case FilterSpaceType.XZPartialCircle;
                    }
                    // val 0 : sqrDistance UpperBound, val 1 = sqrDistance LowerBound or Offset, val 2 = Wide Half Degree
                    case FilterSpaceType.XZPartialCircle:
                    {                        
                        if (checkBound)
                        {
                        }
                        else
                        {
                            // val2 와 val3 위치를 바꿔준다. 원 필터링은 2번째 파라미터를 높이로 사용하기 때문.
                            SystemTool.Swap(ref FloatValue2, ref FloatValue3);
                            
                            var halfAngle = FloatValue3;
                            FloatValue3 = halfAngle;
                        }
                        
                        goto case FilterSpaceType.Circle;
                    }
                    // val 0 : sqrDistance UpperBound, val 1 = sqrDistance LowerBound or Offset, val 2 = Half Height
                    case FilterSpaceType.Circle:
                    {
                        if (checkBound)
                        {
                        }
                        else
                        {
                            var halfHeight = _FilterAffine.ScaleFactor * FloatValue2;
                            
                            // 높이 보정 플래그를 처리해준다.
                            if (FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CorrectHeight))
                            {
                                FloatValue2 = halfHeight + p_Pivot.GetHeight(0.5f);
                            }
                            else
                            {
                                FloatValue2 = halfHeight;
                            }
                        }
                        
                        goto case FilterSpaceType.XZCircle;
                    }
                    // val 0 : sqrDistance UpperBound, val 1 = sqrDistance LowerBound or Offset
                    case FilterSpaceType.XZCircle:
                    {                        
                        goto case FilterSpaceType.Sphere;
                    }
                    // val 0 : sqrDistance UpperBound, val 1 = sqrDistance LowerBound or Offset
                    case FilterSpaceType.SqrDistance:
                    // val 0 : sqrDistance UpperBound, val 1 = sqrDistance LowerBound or Offset
                    case FilterSpaceType.Sphere:
                    {
                        if (checkBound)
                        {
                        }
                        else
                        {
                            var useAsOffset = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.UsingLowerRadiusAsOffset);

                            // val 0 = 제곱거리 상한
                            var upperBound = _FilterAffine.ScaleFactor * FloatValue0;
                            FloatValue0 = Mathf.Pow(upperBound, 2);

                            // val 1 = UsingLowerRadiusAsOffset 플래그 보유 시 상한으로부터 Offset / 없을 시 제곱거리 하한
                            var lowerBound = _FilterAffine.ScaleFactor * FloatValue1;
                            FloatValue1 = useAsOffset ? FloatValue0 + Mathf.Pow(lowerBound, 2) - 2f * upperBound * lowerBound : Mathf.Pow(lowerBound, 2);

                            // 필터링 가상 공간을 전방에 생성해야하는 경우
                            var castForward = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.ForwardCast);
                            if (castForward)
                            {
                                _FilterAffine.AddLocalPositionOffset(upperBound * Vector3.forward, false);
                            }
#if UNITY_EDITOR
                            DrawGizmos(p_Pivot, false);
#endif
                        }
                    }
                        break;
                    // val 0 : Box Width, val 1 = Box Length, val 2 = Half Height
                    case FilterSpaceType.Box:
                    {
                        if (checkBound)
                        {
                        }
                        else
                        {
                            var halfHeight = _FilterAffine.ScaleFactor * FloatValue2;
                            
                            // 높이 보정 플래그를 처리해준다.
                            if (FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CorrectHeight))
                            {
                                FloatValue2 = halfHeight + p_Pivot.GetHeight(0.5f);
                            }
                            else
                            {
                                FloatValue2 = halfHeight;
                            }
                        }
                        
                        goto case FilterSpaceType.XZBox;
                    }
                    // val 0 : Box Width, val 1 = Box Length
                    case FilterSpaceType.XZBox:
                    {
                        if (checkBound)
                        {
                        }
                        else
                        {
                            // val 0 = 박스 너비
                            var width = _FilterAffine.ScaleFactor * FloatValue0;
                            FloatValue0 = width;
                            
                            // val 1 = 박스 길이
                            var length = _FilterAffine.ScaleFactor * FloatValue1;
                            FloatValue1 = length;
                            
                            // 필터링 가상 공간을 전방에 생성해야하는 경우
                            var castForward = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.ForwardCast);
                            if (castForward)
                            {
                                _FilterAffine.AddLocalPositionOffset(0.5f * FloatValue1 * Vector3.forward, false);
                            }
                            
                            SetFilterPlane();
#if UNITY_EDITOR
                            DrawGizmos(p_Pivot, false);
#endif
                        }
                    }
                        break;
                }
            }

            public void OnFilterTargetSelected(Unit p_Pivot, Unit p_Target)
            {
                var checkBound = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CalculateBounds);
                var isSelfFilter = ReferenceEquals(p_Pivot, p_Target);
                switch (SpaceType)
                {
                    case FilterSpaceType.NoneSpace:
                        break;
                    case FilterSpaceType.PartialCircle:
                    {
                        goto case FilterSpaceType.XZPartialCircle;
                    }
                    case FilterSpaceType.XZPartialCircle:
                    {
                        if (checkBound)
                        {
                            // val2 와 val3 위치를 바꿔준다. 원 필터링은 2번째 파라미터를 높이로 사용하기 때문.
                            SystemTool.Swap(ref FloatValue2, ref FloatValue3);
                            
                            var halfAngle = FloatValue3;
                            FloatValue3 = halfAngle;
                        }
                        else
                        {
                        }
                        
                        goto case FilterSpaceType.Circle;
                    }
                    case FilterSpaceType.Circle:
                    {
                        if (checkBound)
                        {
                            var pivotHalfHeight = p_Pivot.GetHeight(0.5f);
                            var targetHalfHeight = isSelfFilter ? 0f : p_Target.GetHeight(0.5f);
                            var unitHalfHeightSum = pivotHalfHeight + targetHalfHeight;
                            var halfHeight = _FilterAffine.ScaleFactor * FloatValue2 + unitHalfHeightSum;
                            
                            // 높이 보정 플래그를 처리해준다.
                            if (FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CorrectHeight))
                            {
                                FloatValue2 = halfHeight + p_Pivot.GetHeight(0.5f);
                            }
                            else
                            {
                                FloatValue2 = halfHeight;
                            }
                        }
                        else
                        {
                        }
                        
                        goto case FilterSpaceType.XZCircle;
                    }
                    case FilterSpaceType.XZCircle:
                    {
                        if (checkBound)
                        {
    
                        }
                        else
                        {
                        }
                        
                        goto case FilterSpaceType.Sphere;
                    }
                    case FilterSpaceType.SqrDistance:
                    case FilterSpaceType.Sphere:
                    {
                        if (checkBound)
                        {
                            var useAsOffset = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.UsingLowerRadiusAsOffset);

                            // val 0 = 제곱거리 상한
                            var radius = p_Pivot.GetRadius();
                            var targetRadius = isSelfFilter ? 0f : p_Target.GetRadius();
                            var unitBoundSum = radius + targetRadius;
                            var scaledValue0 = _FilterAffine.ScaleFactor * FloatValue0;
                            var upperBound = scaledValue0 + unitBoundSum;
                            FloatValue0 = Mathf.Pow(upperBound, 2);

                            // val 1 = UsingLowerRadiusAsOffset 플래그 보유 시 상한으로부터 Offset / 없을 시 제곱거리 하한
                            var lowerBound = Mathf.Max(0f, _FilterAffine.ScaleFactor * FloatValue1 - unitBoundSum);
                            FloatValue1 = useAsOffset ? Mathf.Pow(upperBound - lowerBound - unitBoundSum, 2) : Mathf.Pow(lowerBound, 2);
                            
                            // 필터링 가상 공간을 전방에 생성해야하는 경우
                            var castForward = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.ForwardCast);
                            if (castForward)
                            {
                                _FilterAffine.AddLocalPositionOffset(scaledValue0 * Vector3.forward, false);
                            }
#if UNITY_EDITOR
                            DrawGizmos(p_Pivot, true);
#endif
                        }
                        else
                        {
                        }
                    }
                        break;
                    case FilterSpaceType.Box:
                    {                        
                        if (checkBound)
                        {
                            var pivotHalfHeight = p_Pivot.GetHeight(0.5f);
                            var targetHalfHeight = isSelfFilter ? 0f : p_Target.GetHeight(0.5f);
                            var unitHalfHeightSum = pivotHalfHeight + targetHalfHeight;
                            var halfHeight = _FilterAffine.ScaleFactor * FloatValue2 + unitHalfHeightSum;
                 
                            // 높이 보정 플래그를 처리해준다.
                            if (FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.CorrectHeight))
                            {
                                FloatValue2 = halfHeight + p_Pivot.GetHeight(0.5f);
                            }
                            else
                            {
                                FloatValue2 = halfHeight;
                            }
                        }
                        else
                        {
                        }
                        
                        goto case FilterSpaceType.XZBox;
                    }
                    case FilterSpaceType.XZBox:
                    {
                        if (checkBound)
                        {
                            var radius = p_Pivot.GetRadius();
                            var diameter = 2f * radius;
                            var targetRadius = isSelfFilter ? 0f : p_Target.GetRadius();
                            var targetDiameter = 2f * targetRadius;
                            var unitBoundSum = diameter + targetDiameter;

                            // val 0 = 박스 너비
                            var width = _FilterAffine.ScaleFactor * FloatValue0 + unitBoundSum;
                            FloatValue0 = width;

                            // val 1 = 박스 길이
                            var scaledValue1 = _FilterAffine.ScaleFactor * FloatValue1;
                            var length = scaledValue1 + unitBoundSum;
                            FloatValue1 = length;
                            
                            // 필터링 가상 공간을 전방에 생성해야하는 경우
                            var castForward = FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.ForwardCast);
                            if (castForward)
                            {
                                _FilterAffine.AddLocalPositionOffset(0.5f * scaledValue1 * Vector3.forward, false);
                            }

                            SetFilterPlane();
#if UNITY_EDITOR
                            DrawGizmos(p_Pivot, true);
#endif
                        }
                        else
                        {
                        }
                    }
                        break;
                }
            }

            public bool OnCheckObstacleCollision()
            {
                if (FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.FilterObstacle))
                {
                    return PhysicsTool.GetSphereOverlap(_FilterAffine.Position, FloatValue0,
                        GameManager.Obstacle_Terrain_LayerMask);
                }
                else
                {
                    return false;
                }
            }

            #endregion

            #region <Methods>

#if UNITY_EDITOR

            private void DrawGizmos(Unit p_Pivot, bool p_IsDeferred)
            {
                if (CustomDebug.DrawUnitFilter || CustomDebug.DrawUnitFilterRestrict)
                {
    #if !SERVER_DRIVE && ON_GUI
                    if (CustomDebug.DrawUnitFilterRestrict || TestManager.GetInstanceUnSafe.IsSelected(p_Pivot))
    #endif
                    {
                        var tryColor = p_IsDeferred ? Gizmos_UnitBound_Color : Gizmos_NonUnitBound_Color;
                        switch (SpaceType)
                        {
                            case FilterSpaceType.SqrDistance:
                                break;
                            case FilterSpaceType.XZPartialCircle:
                                CustomDebug.DrawPartialCircle(_FilterAffine.Position, Mathf.Sqrt(FloatValue1), Mathf.Sqrt(FloatValue0), _FilterAffine.Forward, FloatValue3, _FilterAffine.Up, tryColor, 16, Gizmos_Duration);
                                break;
                            case FilterSpaceType.PartialCircle:
                                CustomDebug.DrawPartialCylinder(_FilterAffine.Position, Mathf.Sqrt(FloatValue1), Mathf.Sqrt(FloatValue0), _FilterAffine.Forward, FloatValue3, _FilterAffine.Up, FloatValue2, tryColor, 16, Gizmos_Duration);
                                break;
                            case FilterSpaceType.XZCircle:
                                CustomDebug.DrawCircle(_FilterAffine.Position, Mathf.Sqrt(FloatValue1), Mathf.Sqrt(FloatValue0), _FilterAffine.Up, tryColor, 16, Gizmos_Duration);
                                break;
                            case FilterSpaceType.Circle:
                                CustomDebug.DrawCylinder(_FilterAffine.Position, Mathf.Sqrt(FloatValue1), Mathf.Sqrt(FloatValue0), _FilterAffine.Up, FloatValue2, tryColor, 16, Gizmos_Duration);
                                break;
                            case FilterSpaceType.Sphere:
                                CustomDebug.DrawSphere(_FilterAffine.Position, Mathf.Sqrt(FloatValue1), Mathf.Sqrt(FloatValue0), _FilterAffine.Up, tryColor, 16, Gizmos_Duration);
                                break;
                            case FilterSpaceType.XZBox:
                                CustomDebug.DrawCustomPlane(_FilterPlane, tryColor, 16, Gizmos_Duration);
                                break;
                            case FilterSpaceType.Box:
                                CustomDebug.DrawCustomPlane(_FilterPlane, FloatValue2, tryColor, 16, Gizmos_Duration);
                                break;
                        }
                    }
                }
            }

#endif
            
            public void SetFilterAffine(Unit p_Unit, bool p_OverlapFlag)
            {
                if (p_OverlapFlag || !FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.UsingAffine))
                {
                    FilterParamsFlag.AddFlag(FilterParamsFlag.UsingAffine);
                    _FilterAffine = new TransformTool.AffineCachePreset(p_Unit, true);
                }
            }
            
            public void SetFilterAffine(TransformTool.AffineCachePreset p_Affine, bool p_OverlapFlag)
            {
                if (p_OverlapFlag || !FilterParamsFlag.HasAnyFlagExceptNone(FilterParamsFlag.UsingAffine))
                {
                    FilterParamsFlag.AddFlag(FilterParamsFlag.UsingAffine);
                    _FilterAffine = p_Affine;
                }
            }

            public void SetFilterPlane()
            {
                var position = _FilterAffine.Position;
                var direction = _FilterAffine.Forward;

                _FilterPlane = 
                    CustomPlane.Get_XZ_Basis_Location
                    (
                        position, 
                        direction, 
                        FloatValue0, 
                        FloatValue1
                    );
            }

            public void SetFilterFlag(FilterParamsFlag p_FilterParamsFlag, bool p_Enable)
            {
                if (p_Enable)
                {
                    FilterParamsFlag.AddFlag(p_FilterParamsFlag);
                }
                else
                {
                    FilterParamsFlag.RemoveFlag(p_FilterParamsFlag);
                }
            }
            
            #endregion
        }

        #endregion
    }
}