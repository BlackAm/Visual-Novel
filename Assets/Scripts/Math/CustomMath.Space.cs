using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BlackAm
{
    public struct CustomLine
    {
        #region <Const/Factory>

        /// <summary>
        /// 반직선 구조체를 생성하여 리턴하는 메서드
        /// </summary>
        public static CustomLine GetHalfLine(Ray p_Ray)
        {
            return GetHalfLine(p_Ray.origin, p_Ray.direction);
        }
        
        /// <summary>
        /// 반직선 구조체를 생성하여 리턴하는 메서드
        /// </summary>
        public static CustomLine GetHalfLine(Vector3 p_StartPoint, Vector3 p_DirectionUnitVector)
        {
            var IsZeroLine = p_DirectionUnitVector.IsReachedZero();
            var result = new CustomLine();

            result.StartPoint = p_StartPoint;
            result.EndPoint =
                IsZeroLine ? p_StartPoint : p_StartPoint + p_DirectionUnitVector * CustomMath.MaxLineLength;
            result.CenterPoint =
                IsZeroLine ? p_StartPoint : (result.StartPoint + result.EndPoint) * 0.5f;
            result.DirectionVector = 
                IsZeroLine ? Vector3.zero : p_DirectionUnitVector * CustomMath.MaxLineLength;
            result.DirectionUnitVector = 
                IsZeroLine ? Vector3.zero : p_DirectionUnitVector.normalized;
            result.ReversedDirectionVector = 
                IsZeroLine ? Vector3.zero : (result.EndPoint - result.StartPoint).GetBoundedInverseVector();
            result.ThisLineType = 
                IsZeroLine ? LineType.ZeroLine : LineType.HalfLine;
            result.BasisZeroMask =
                IsZeroLine ? CustomMath.XYZType.XYZ : result.ReversedDirectionVector.GetXYZType_ReachedInfinity();
            
            return result;
        }

        /// <summary>
        /// 선분 구조체를 생성하여 리턴하는 메서드
        /// </summary>
        public static CustomLine GetSegment(Vector3 p_StartPoint, Vector3 p_EndPoint)
        {
            var directionVector = p_StartPoint.GetDirectionVectorTo(p_EndPoint);
            var IsZeroLine = directionVector.IsReachedZero();
            var result = new CustomLine();
            
            result.StartPoint = p_StartPoint;
            result.EndPoint =
                IsZeroLine ? p_StartPoint : p_EndPoint;
            result.CenterPoint =
                IsZeroLine ? p_StartPoint : (result.StartPoint + result.EndPoint) * 0.5f;
            result.DirectionVector = 
                IsZeroLine ? Vector3.zero : directionVector;
            result.DirectionUnitVector = 
                IsZeroLine ? Vector3.zero : directionVector.normalized;
            result.ReversedDirectionVector = 
                IsZeroLine ? Vector3.zero : (result.EndPoint - result.StartPoint).GetBoundedInverseVector();
            result.ThisLineType = 
                IsZeroLine ? LineType.ZeroLine : LineType.Segment;
            result.BasisZeroMask =
                IsZeroLine ? CustomMath.XYZType.XYZ : result.ReversedDirectionVector.GetXYZType_ReachedInfinity();

            return result;
        }

        /// <summary>
        /// 직선 구조체를 생성하여 리턴하는 메서드
        /// </summary>
        public static CustomLine GetLine(Vector3 p_PointOnLine, Vector3 p_DirectionUnitVector)
        {
            var IsZeroLine = p_DirectionUnitVector.IsReachedZero();
            var result = new CustomLine();
            
            result.StartPoint =
                IsZeroLine ? p_PointOnLine : p_PointOnLine - p_DirectionUnitVector * CustomMath.MaxLineLength * 0.5f;
            result.EndPoint =
                IsZeroLine ? p_PointOnLine : p_PointOnLine + p_DirectionUnitVector * CustomMath.MaxLineLength * 0.5f;
            result.CenterPoint = p_PointOnLine;
            result.DirectionVector = 
                IsZeroLine ? Vector3.zero : p_DirectionUnitVector * CustomMath.MaxLineLength;
            result.DirectionUnitVector = 
                IsZeroLine ? Vector3.zero : p_DirectionUnitVector.normalized;
            result.ReversedDirectionVector = 
                IsZeroLine ? Vector3.zero : (result.EndPoint - result.StartPoint).GetBoundedInverseVector();
            result.ThisLineType = 
                IsZeroLine ? LineType.ZeroLine : LineType.Line;
            result.BasisZeroMask =
                IsZeroLine ? CustomMath.XYZType.XYZ : result.ReversedDirectionVector.GetXYZType_ReachedInfinity();

            return result;
        }

        #endregion

        #region <Enums>

        /// <summary>
        /// 선 타입
        /// </summary>
        public enum LineType
        {
            /// <summary>
            /// 영선, 시작점과 끝점이 같은 선
            /// </summary>
            ZeroLine,
            
            /// <summary>
            /// 반직선, 시작점과 방향을 가짐
            /// </summary>
            HalfLine,
            
            /// <summary>
            /// 선분, 시작점과 끝점을 가짐
            /// </summary>
            Segment,
            
            /// <summary>
            /// 직선, 방향과 직선위의 한 점을 가짐
            /// </summary>
            Line,
        }
        
        /// <summary>
        /// 두 직선의 위치관계를 기술하는 열거형 상수
        /// </summary>
        public enum LineIntersectType
        {
            /// <summary>
            /// 두 직선은 평행하지 않고 만나지도 않음
            /// </summary>
            NoneIntersect,

            /// <summary>
            /// 두 직선은 평행함
            /// </summary>
            Parallel, 
            
            /// <summary>
            /// 두 선분은 한 경로 상에 있으면서, 겹치지 않음.
            /// </summary>
            OnePath,
            
            /// <summary>
            /// 두 직선은 일치함
            /// </summary>
            Equivalent,
            
            /// <summary>
            /// lValue 직선이 RValue 직선을 포함하고 있음
            /// </summary>
            InnerIntersect,

            /// <summary>
            /// lValue 직선이 RValue 직선에 포함되어 있음
            /// </summary>
            OutterIntersect,
            
            /// <summary>
            /// lValue 직선이 RValue 직선과 겹치는 부분이 있으면서 내접 혹은 외접이 아님
            /// </summary>
            OverlapIntersect,
            
            /// <summary>
            /// 두 직선이 한 점에서 만남
            /// </summary>
            CrossIntersect,
        }
        
        #endregion

        #region <Fields>

        /// <summary>
        /// 선 시작지점
        /// </summary>
        public Vector3 StartPoint;
        
        /// <summary>
        /// 선 끝지점
        /// </summary>
        public Vector3 EndPoint;

        /// <summary>
        /// 선 중간지점
        /// </summary>
        public Vector3 CenterPoint;
        
        /// <summary>
        /// 선 유닛벡터
        /// </summary>
        public Vector3 DirectionUnitVector;
                
        /// <summary>
        /// 선 방향벡터
        /// </summary>
        public Vector3 DirectionVector;

        /// <summary>
        /// 리앙-바스키 알고리즘에 의해 보간변수 t를 구하는데 사용할 방향벡터의 역수벡터
        /// </summary>
        private Vector3 ReversedDirectionVector;
        
        /// <summary>
        /// 선 타입
        /// </summary>
        public LineType ThisLineType;

        /// <summary>
        /// 해당 성분의 기저 특징
        ///
        /// 해당 값이 X라면 해당 선분은 X 값 변화량이 0(= YZ평면에 평행함)임을 의미한다.
        /// 해당 값이 None이라면 모든 기저성분으로 변화량을 가진다.
        /// 해당 값이 XYZ라면 해당 선분은 X, Y, Z 값 변화량이 전부 0 즉 영선임을 의미한다.
        /// </summary>
        public CustomMath.XYZType BasisZeroMask { get; private set; }
            
        #endregion

        #region <Operator>

        public override bool Equals(object obj)
        {
            var rightValue = (CustomLine) obj;
            return Equals(rightValue);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(CustomLine p_RightValue)
        {
            return ThisLineType == LineType.Line
                ? DirectionUnitVector == p_RightValue.DirectionUnitVector
                  && ThisLineType == p_RightValue.ThisLineType
                : StartPoint == p_RightValue.StartPoint
                  && DirectionUnitVector == p_RightValue.DirectionUnitVector
                  && EndPoint == p_RightValue.EndPoint
                  && ThisLineType == p_RightValue.ThisLineType;
        }

        public static bool operator==(CustomLine p_LeftLine, CustomLine p_RightLine)
        {
            return p_LeftLine.ThisLineType == LineType.Line
                ? p_LeftLine.DirectionUnitVector == p_RightLine.DirectionUnitVector
                  && p_LeftLine.ThisLineType == p_RightLine.ThisLineType
                : p_LeftLine.StartPoint == p_RightLine.StartPoint
                  && p_LeftLine.DirectionUnitVector == p_RightLine.DirectionUnitVector
                  && p_LeftLine.EndPoint == p_RightLine.EndPoint
                  && p_LeftLine.ThisLineType == p_RightLine.ThisLineType;
        }

        public static bool operator !=(CustomLine p_LeftLine, CustomLine p_RightLine)
        {
            return !(p_LeftLine == p_RightLine);
        }

        #endregion

        #region <Methods>

        public float GetSqrLength()
        {
            return DirectionVector.sqrMagnitude;
        }

        public float GetLength()
        {
            return DirectionVector.magnitude;
        }
        
        /// <summary>
        /// 리앙 바스키 알고리즘에 따라 정의되는 직선 경로상의 좌표를 리턴하는 메서드
        /// </summary>
        public Vector3 GetLBPositionOnPath(float p_LBFactor)
        {
            // 부동 소수점 곱셈에서 나와야할 0f가 나오지 않거나 하므로, 리앙바스키 좌표를 정규화 시켜서 리턴한다.
            return StartPoint + p_LBFactor * DirectionVector;
        }

        /// <summary>
        /// 해당 직선으로부터 특정한 좌표까지의 수직 제곱거리를 구하는 메서드
        /// </summary>
        public float GetSqrDistanceToPoint(Vector3 p_TargetPoint)
        {
            return Vector3.SqrMagnitude(p_TargetPoint - p_TargetPoint.GetProjectionVector(DirectionUnitVector));
        }

        /// <summary>
        /// 해당 직선으로부터 특정한 좌표까지의 수직거리를 구하는 메서드
        /// </summary>
        public float GetDistanceToPoint(Vector3 p_TargetPoint)
        {
            return Vector3.Magnitude(p_TargetPoint - p_TargetPoint.GetProjectionVector(DirectionUnitVector));
        }

        /// <summary>
        /// 해당 선 위에 특정 좌표가 포함되는지 검증하는 메서드, 포함된다면 그 떄의 리앙-바스키 계수를 리턴한다.
        /// </summary>
        public (bool, float) GetLBFactor(Vector3 p_TargetPoint, float p_Threshold)
        {
            switch (BasisZeroMask)
            {
                // 해당 선분이 영선인 경우,
                case CustomMath.XYZType.XYZ:
                    // 영선은 곧 점이므로, 타겟 좌표와 같은 점인지 검증하면 된다.
                    if (StartPoint == p_TargetPoint)
                    {
                        return (true, 1f);
                    }
                    break;
                
                // 해당 선분이 y축과 평행인 경우
                case CustomMath.XYZType.ZX:
                {
                    // p_TargetPoint이 해당 선 위에 있으려면 x, z성분이 해당 선의 것과 같아야만 한다.
                    // 단, lb팩터의 분자를 보면 두 선분을 뺀 값과 같으므로 해당 조건은 lb팩터의 x, z성분이 0인 것과 동치이다.
                    // p_TargetPoint.x.IsReachedZero(StartPoint.x, p_Threshold) ≡ lb_Factor.x.IsReachedZero(p_Threshold)
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    if (lb_Factor.x.IsReachedZero(p_Threshold) && lb_Factor.z.IsReachedZero(p_Threshold))
                    {
                        return (true, lb_Factor.y);
                    }
                }
                    break;
                // 해당 선분이 x축과 평행인 경우, 
                case CustomMath.XYZType.YZ:
                {
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    if (lb_Factor.y.IsReachedZero(p_Threshold) && lb_Factor.z.IsReachedZero(p_Threshold))
                    {
                        return (true, lb_Factor.x);
                    }
                }
                    break;
                // 해당 선분이 z축과 평행인 경우
                case CustomMath.XYZType.XY:
                {
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    if (lb_Factor.x.IsReachedZero(p_Threshold) && lb_Factor.y.IsReachedZero(p_Threshold))
                    {
                        return (true, lb_Factor.z);
                    }
                }
                    break;

                // 해당 선분이 xy평면과 평행인 경우
                case CustomMath.XYZType.Z:
                {
                    // lb팩터의 x와 y성분은 같고, p_TargetPoint이 z성분은 해당 선의 것과 같아야만 한다.
                    // 단, lb팩터의 분자를 보면 두 선분을 뺀 값과 같으므로 두번째 조건은 lb팩터의 z성분이 0인 것과 동치이다.
                    // p_TargetPoint.z.IsReachedValue(StartPoint.z, p_Threshold) ≡ lb_Factor.z.IsReachedZero(p_Threshold)
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    if (lb_Factor.x.IsReachedValue(lb_Factor.y, p_Threshold) && lb_Factor.z.IsReachedZero(p_Threshold))
                    {
                        return (true, lb_Factor.x);
                    }
                }
                    break;
                // 해당 선분이 yz평면과 평행인 경우
                case CustomMath.XYZType.X:
                {
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    if (lb_Factor.y.IsReachedValue(lb_Factor.z, p_Threshold) && lb_Factor.x.IsReachedZero(p_Threshold))
                    {
                        return (true, lb_Factor.y);
                    }
                }
                    break;
                // 해당 선분이 xz평면과 평행인 경우
                case CustomMath.XYZType.Y:
                {
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    if (lb_Factor.z.IsReachedValue(lb_Factor.x, p_Threshold) && lb_Factor.y.IsReachedZero(p_Threshold))
                    {
                        return (true, lb_Factor.z);
                    }
                }
                    break;
                // 해당 선분이 어떤 기저축이나 기저축으로 구성된 평면과도 평행하지 않은 경우
                case CustomMath.XYZType.None:
                {
                    var lb_Factor = Vector3.Scale(p_TargetPoint - StartPoint, ReversedDirectionVector);
                    // 각 성분의 배율인 lbFactor가 일벡터여야 하므로 x, y, z 성분값이 같아야한다.
                    if (lb_Factor.x.IsReachedValue(lb_Factor.y, p_Threshold) && lb_Factor.y.IsReachedValue(lb_Factor.z, p_Threshold))
                    {
                        return (true, lb_Factor.x);
                    }
                }
                    break;
            }
            
            return (false, 0f);
        }

        /// <summary>
        /// 특정한 직선과 지정한 성분에 대해 평행인지 검증하는 메서드
        /// </summary>
        public bool IsLineParallel(CustomLine p_Target, bool p_CheckReverseVector)
        {
            return DirectionUnitVector.IsParallel(p_Target.DirectionUnitVector, p_CheckReverseVector);
        }

        /// <summary>
        /// 리앙바스키 알고리즘과 외적을 이용하여, 해당 직선과 평행하지 않은 직선 p_Target에 대해
        /// 특정 성분(XYZ, XY, YZ, ZX)에 대해 하나의 교점을 가지는지 검증하는 메서드
        ///
        /// * 원리 *
        /// 
        /// 두 리앙바스키 방정식 l = sl + tl * (el - sl), r = sr + tr * (er - sr)이 교점을 가진다면 l = r이고,
        /// 이를 각 성분 x, y, z의 연립방정식으로 만들어, tl 혹은 tr을 구할 수 있다.
        /// 
        /// 예를 들어, 해당 직선의 리앙바스키 팩터인 tl에 대해 교점 연립 방정식을 정리하면
        /// 아래와 같은 외적을 포함하는 식이 된다.
        /// (이 때, p는 두 직선이 교점을 가질 때의 tl 값)
        ///
        /// p = | Cross(sr - sl, er - sr) | / | Cross(el - sl, er - sr) |
        ///
        /// 또한 외적의 절대값은 행렬값(determinant)과 같기 때문에 아래와 같은 식이 된다. 
        /// 
        /// p = det(sr - sl, er - sr) / det(el - sl, er - sr)
        ///
        /// 즉 정리된 위의 식을 통해 해당 직선과 파라미터로 받은 직선 사이에 어떤 교점이 있는지 체크할 수 있다.
        ///
        ///    1. p식의 분모가 0이되는 경우
        ///
        ///        1-1. 두 직선의 기저벡터 중에 둘 다 0벡터인 경우, 두 점이 일치하는지 검증한다.
        ///        1-2. 두 직선의 기저벡터 중에 하나가 0벡터인 경우, 0벡터가 나머지 한 선에 포함되는지 검증한다.
        ///        1-3. 두 직선의 기저벡터 el - sl, er - sr가 같은 경우, 즉 같은 기울기를 가지는 경우
        ///
        ///    2. 그 외의 경우
        ///
        ///        p식을 계산하여 리턴
        /// 
        /// </summary>
        public (bool Valid, float LBFactor) FindInterSection(CustomLine p_Target, CustomMath.XYZType p_TryXYZType, bool p_CheckParallel)
        {
            var TargetBasisType = p_Target.BasisZeroMask;
            var TargetDirection = p_Target.DirectionVector;

            switch (BasisZeroMask)
            {
                /* 1-1 */
                case CustomMath.XYZType.XYZ when TargetBasisType == CustomMath.XYZType.XYZ:
                    return GetLBFactor(p_Target.StartPoint, CustomMath.Epsilon);
                /* 1-2 */
                case CustomMath.XYZType.XYZ when TargetBasisType != CustomMath.XYZType.XYZ:
                    return p_Target.GetLBFactor(StartPoint, CustomMath.Epsilon);

                default:
                {
                    /* 1-2 */
                    if (TargetBasisType == CustomMath.XYZType.XYZ)
                    {
                        return GetLBFactor(p_Target.StartPoint, CustomMath.Epsilon);
                    }
                    else
                    {
                        /* 1-3 */
                        if (p_CheckParallel && DirectionVector.IsParallel(TargetDirection, true))
                        {
                            return default;
                        }
                        /* 2 ~ */
                        else
                        {
                            switch (p_TryXYZType)
                            {
                                case CustomMath.XYZType.None :
                                    return (true, (p_Target.StartPoint - StartPoint).DeterminantXYZ(TargetDirection) /
                                                  DirectionVector.DeterminantXYZ(TargetDirection));
                                case CustomMath.XYZType.XY:
                                    return (true, (p_Target.StartPoint - StartPoint).DeterminantXY(TargetDirection) /
                                                  DirectionVector.DeterminantXY(TargetDirection));
                                case CustomMath.XYZType.YZ:
                                    return (true, (p_Target.StartPoint - StartPoint).DeterminantYZ(TargetDirection) /
                                                  DirectionVector.DeterminantYZ(TargetDirection));
                                case CustomMath.XYZType.ZX:
                                    return (true, (p_Target.StartPoint - StartPoint).DeterminantXZ(TargetDirection) /
                                                  DirectionVector.DeterminantXZ(TargetDirection));
                            }
                        }
                    }

                    break;
                }
            }

            return default;
        }

        /// <summary>
        /// 두 선분의 위치/포함 관계를 리턴하는 메서드
        /// </summary>
        public (LineIntersectType, Vector3) GetLineIntersectType(CustomLine p_TargetLine)
        {
            // 두 선이 같은 선이라면, 중간 지점을 리턴해준다.
            if (this == p_TargetLine)
            {
                return (LineIntersectType.Equivalent, CenterPoint);
            }
            // 같은 선이 아니라면, LB 알고리즘을 통한 비교를 한다.
            else
            {
                // 타겟 라인의 시작점이 해당 라인 위에 있는지 검증한다.
                var (_HasTargetStartIntersectWithThisLine, _LBFactor_TargetStart) =
                    GetLBFactor(p_TargetLine.StartPoint,CustomMath.Epsilon);
                // 타겟 라인의 끝점이 해당 라인 위에 있는지 검증한다.
                var (_HasTargetEndIntersectWithThisLine, _LBFactor_TargetEnd) =
                    GetLBFactor(p_TargetLine.EndPoint, CustomMath.Epsilon);

                #region <CompareLine>
                
                // 해당 라인 경로 상에 타겟 라인의 시작점/끝점이 모두 위치한 경우
                if (_HasTargetStartIntersectWithThisLine && _HasTargetEndIntersectWithThisLine)
                {
                    #region <Start - End - OnPath>

                    // 타겟 라인의 시작점/끝점이 구간 [0, 1]의 어디에 위치해있는지 연산한다.
                    var TargetStartPointProgressType = _LBFactor_TargetStart.GetProgressType01();
                    var TargetEndPointProgressType = _LBFactor_TargetEnd.GetProgressType01();
                    
                    switch (TargetStartPointProgressType)
                    {
                        case CustomMath.Progress01Type.ZeroToNegative:
                            switch (TargetEndPointProgressType)
                            {
                                // 둘 다 네거티브, 즉 기울기는 같지만 접점이 없음.
                                case CustomMath.Progress01Type.ZeroToNegative:
                                    return (LineIntersectType.OnePath, default);
                                
                                // 타겟 라인 끝부분이 해당 라인 시작부분에 걸친 경우
                                case CustomMath.Progress01Type.Zero:
                                    return (LineIntersectType.CrossIntersect, p_TargetLine.EndPoint);
                                
                                // 타겟 라인의 끝부분이 해당 라인과 겹치는 경우
                                case CustomMath.Progress01Type.ZeroToOne:
                                    return (LineIntersectType.OverlapIntersect, p_TargetLine.EndPoint);
                                
                                // 타겟 라인의 끝부분이 해당 라인의 끝부분에 걸친 경우
                                case CustomMath.Progress01Type.One:
                                    return (LineIntersectType.OutterIntersect, p_TargetLine.EndPoint);
                                
                                // 타겟 라인이 해당 라인을 포함하는 경우
                                case CustomMath.Progress01Type.OneToPositive:
                                case CustomMath.Progress01Type.Infinity:
                                    return (LineIntersectType.OutterIntersect, EndPoint);
                            }
                            break;
                        case CustomMath.Progress01Type.Zero:
                            switch (TargetEndPointProgressType)
                            {
                                // 타겟 라인의 시작부분이 해당 라인과 겹치는 경우
                                case CustomMath.Progress01Type.ZeroToNegative:
                                    return (LineIntersectType.CrossIntersect, p_TargetLine.StartPoint);
                                
                                // 타겟 라인의 시작/끝 부분이 모두 해당 라인의 시작부분과 접하는 경우
                                // 경우의 허수
                                case CustomMath.Progress01Type.Zero:
                                    return (LineIntersectType.CrossIntersect, p_TargetLine.StartPoint);
                                
                                // 타겟 라인이 해당 라인에 포함되는 경우
                                case CustomMath.Progress01Type.ZeroToOne:
                                    return (LineIntersectType.InnerIntersect, p_TargetLine.EndPoint);
                                
                                // 타겟 라인과 해당 라인이 일치하는 경우
                                // 경우의 허수
                                case CustomMath.Progress01Type.One:
                                    return (LineIntersectType.Equivalent, p_TargetLine.EndPoint);
                                
                                // 타겟 라인이 해당 라인을 포함하는 경우
                                case CustomMath.Progress01Type.OneToPositive:
                                case CustomMath.Progress01Type.Infinity:
                                    return (LineIntersectType.OutterIntersect, EndPoint);
                            }
                            break;
                        case CustomMath.Progress01Type.ZeroToOne:
                            switch (TargetEndPointProgressType)
                            {
                                // 타겟 라인의 시작부분이 해당 라인의 시작부분과 겹치는 경우
                                case CustomMath.Progress01Type.ZeroToNegative:
                                    return (LineIntersectType.OverlapIntersect, p_TargetLine.StartPoint);
                                
                                // 타겟 라인이 해당 라인에 포함되는 경우
                                case CustomMath.Progress01Type.Zero:
                                case CustomMath.Progress01Type.ZeroToOne:
                                case CustomMath.Progress01Type.One:
                                    return (LineIntersectType.InnerIntersect, p_TargetLine.StartPoint);
                                
                                // 타겟 라인의 시작부분이 해당 라인의 끝부분과 겹치는 경우
                                case CustomMath.Progress01Type.OneToPositive:
                                case CustomMath.Progress01Type.Infinity:
                                    return (LineIntersectType.OverlapIntersect, p_TargetLine.StartPoint);
                            }
                            break;
                        case CustomMath.Progress01Type.One:
                            switch (TargetEndPointProgressType)
                            {
                                // 타겟 라인이 해당 라인을 포함하고 있는 경우
                                case CustomMath.Progress01Type.ZeroToNegative:
                                    return (LineIntersectType.OutterIntersect, p_TargetLine.StartPoint);
                                
                                // 타겟 라인과 해당 라인이 일치하는 경우
                                // 해당 경우는 서로 반대방향을 가리키고 있기 때문에 허수 경우가 아님
                                case CustomMath.Progress01Type.Zero:
                                    return (LineIntersectType.Equivalent, p_TargetLine.EndPoint);
                                    
                                // 타겟 라인이 해당 라인에 포함되고 있는 경우
                                case CustomMath.Progress01Type.ZeroToOne:
                                    return (LineIntersectType.InnerIntersect, p_TargetLine.EndPoint);
                                    
                                // 타겟 라인의 시작/끝 부분이 모두 해당 라인의 끝부분과 접하는 경우
                                // 경우의 허수
                                case CustomMath.Progress01Type.One:
                                    return (LineIntersectType.CrossIntersect, p_TargetLine.StartPoint);
                                
                                // 타겟 라인의 시작부분이 해당 라인의 끝부분에 걸친 경우
                                case CustomMath.Progress01Type.OneToPositive:
                                case CustomMath.Progress01Type.Infinity:
                                    return (LineIntersectType.CrossIntersect, p_TargetLine.StartPoint);
                            }
                            break;
                        case CustomMath.Progress01Type.OneToPositive:
                        case CustomMath.Progress01Type.Infinity:
                            switch (TargetEndPointProgressType)
                            {
                                // 타겟 라인이 해당 라인을 포함하는 경우
                                case CustomMath.Progress01Type.ZeroToNegative:
                                case CustomMath.Progress01Type.Zero:
                                    return (LineIntersectType.OutterIntersect, StartPoint);

                                // 타겟 라인의 끝부분이 해당 라인의 끝부분과 겹치는 경우
                                case CustomMath.Progress01Type.ZeroToOne:
                                    return (LineIntersectType.OverlapIntersect, p_TargetLine.EndPoint);
                                
                                // 타겟 라인의 끝부분이 해당 라인의 끝부분에 겹치는 경우
                                case CustomMath.Progress01Type.One:
                                    return (LineIntersectType.CrossIntersect, p_TargetLine.EndPoint);

                                // 접하지 않는 경우
                                case CustomMath.Progress01Type.OneToPositive:
                                case CustomMath.Progress01Type.Infinity:
                                    return (LineIntersectType.OnePath, default);
                            }
                            break;
                    }
                    
                    #endregion
                }
                // 해당 라인 경로 상에 타겟 라인의 시작점만 위치한 경우
                else if (_HasTargetStartIntersectWithThisLine)
                {
                    var TargetStartPointProgressType = _LBFactor_TargetStart.GetProgressType01();

                    switch (TargetStartPointProgressType)
                    {
                        case CustomMath.Progress01Type.ZeroToNegative:
                            return (LineIntersectType.NoneIntersect, default);
                        case CustomMath.Progress01Type.Zero:
                        case CustomMath.Progress01Type.ZeroToOne:
                        case CustomMath.Progress01Type.One:
                            return (LineIntersectType.CrossIntersect, p_TargetLine.StartPoint);
                        case CustomMath.Progress01Type.OneToPositive:
                        case CustomMath.Progress01Type.Infinity:
                            return (LineIntersectType.NoneIntersect, default);
                    }
                }
                // 해당 라인 경로 상에 타겟 라인의 끝점만 위치한 경우
                else if (_HasTargetEndIntersectWithThisLine)
                {
                    var TargetEndPointProgressType = _LBFactor_TargetEnd.GetProgressType01();
                    
                    switch (TargetEndPointProgressType)
                    {
                        case CustomMath.Progress01Type.ZeroToNegative:
                            return (LineIntersectType.NoneIntersect, default);
                        case CustomMath.Progress01Type.Zero:
                        case CustomMath.Progress01Type.ZeroToOne:
                        case CustomMath.Progress01Type.One:
                            return (LineIntersectType.CrossIntersect, p_TargetLine.EndPoint);
                        case CustomMath.Progress01Type.OneToPositive:
                        case CustomMath.Progress01Type.Infinity:
                            return (LineIntersectType.NoneIntersect, default);
                    }
                }
                // 해당 라인 경로 상에 타겟 라인의 시작점/끝점이 위치하지 않는 경우
                else
                {
                    // 두 라인이 평행한지 검증(일치하는 경우는 위의 블록에서 이미 처리됨.)
                    if (IsLineParallel(p_TargetLine, true))
                    {
                        return (LineIntersectType.Parallel, default);
                    }
                    // 평행하지 않은 경우 교점을 찾는다.
                    else
                    {
                        var (_HasIntersect, _LeftLineLBFactor) =
                            FindInterSection(p_TargetLine, CustomMath.XYZType.XYZ, false);
                        if (_HasIntersect)
                        {
                            // 경로상에 교점이 있었던 경우, LBFactor 값이 유효한지 검증한다.
                            switch (_LeftLineLBFactor.GetProgressType01())
                            {
                                case CustomMath.Progress01Type.ZeroToNegative:
                                    return (LineIntersectType.NoneIntersect, default);
                                case CustomMath.Progress01Type.Zero:
                                case CustomMath.Progress01Type.ZeroToOne:
                                case CustomMath.Progress01Type.One:
                                    // LBFactor가 0~1의 유효 값이라면, 해당 교점을 리턴해준다.
                                    return (LineIntersectType.CrossIntersect, GetLBPositionOnPath(_LeftLineLBFactor));
                                case CustomMath.Progress01Type.OneToPositive:
                                case CustomMath.Progress01Type.Infinity:
                                    return (LineIntersectType.NoneIntersect, default);
                            }
                        }
                        // 교점이 없는 경우
                        else
                        {
                            return (LineIntersectType.NoneIntersect, default);
                        }
                    }
                }
                #endregion
            }
            return (LineIntersectType.NoneIntersect, default);
        }
        
#if UNITY_EDITOR
        public void DrawLine()
        {
            var color = Color.white;
            switch (ThisLineType)
            {
                case LineType.Line :
                    color = Color.blue;
                    break;
                case LineType.Segment :
                    color = Color.cyan;
                    break;
                case LineType.HalfLine :
                    color = Color.green;
                    break;
            }

            Gizmos.color = color;
            Gizmos.DrawLine(StartPoint, EndPoint);
            Gizmos.DrawCube(StartPoint, 0.04f * Vector3.one);
            Gizmos.DrawCube(EndPoint, 0.04f * Vector3.one);
            Handles.Label(StartPoint, "Start");
            Handles.Label(EndPoint, "End");
        }
#endif
        
        #endregion
    }

    public struct CustomPlane
    {
        #region <Const/Factory>

        /// <summary>
        /// 평면 벡터 방정식 기술
        /// 1. 노멀 벡터와 해당 평면 위의 한 점이 주어진 경우
        /// </summary>
        public static CustomPlane GetPlane(Vector3 p_NormalVector, Vector3 p_PlaneCenterPoint)
        {
            var planeNormalVector = p_NormalVector.normalized;
            return new CustomPlane
            {
                NormalVector = planeNormalVector, 
                CenterPoint = p_PlaneCenterPoint, 
                ThisPlaneType = PlaneType.Plane
            };
        }
        
        /// <summary>
        /// 평면 벡터 방정식 기술
        /// 2. 한 직선 위에 있지 않은 세 점이 주어진 경우
        /// </summary>
        public static CustomPlane GetPlane(Vector3 p_PlaneCenterPoint, Vector3 p_Point1OnPlane, Vector3 p_Point2OnPlane)
        {
            var v01 = p_PlaneCenterPoint.GetDirectionUnitVectorTo(p_Point1OnPlane);
            var v02 = p_PlaneCenterPoint.GetDirectionUnitVectorTo(p_Point2OnPlane);
            
#if UNITY_EDITOR
            if (v01 == v02)
            {
                Debug.LogError("Plane cannot formed with 3 point which on same line");
            }
#endif
            
            // 왼손좌표계 기준으로 세 점을 지나는 평면의 법선 벡터를 구한다.
            var normalVector = Vector3.Cross(v01, v02).normalized;
            return GetPlane(normalVector, p_PlaneCenterPoint);
        }

        /// <summary>
        /// XZ 기저평면에 평행하면서 지정한 좌표를 기준으로 특정한 방향, 너비, 높이를 가지는 평면 프리셋을 리턴하는 메서드
        /// </summary>
        public static CustomPlane Get_XZ_Basis_Location(Vector3 p_CenterPos, Vector3 p_Direction, float p_Width, float p_Height)
        {
            p_Direction = p_Direction.XZUVector();
            var perpVector = p_Direction.GetXZPerpendicularUnitVector();
            var normal = Vector3.up;
            var p0 = p_CenterPos - perpVector * 0.5f * p_Width - p_Direction * 0.5f * p_Height;
            var p1 = p0 + perpVector * p_Width;
            var p2 = p1 + p_Direction * p_Height;
            var p3 = p2 - perpVector * p_Width;

            var result = GetPlane(normal, p_CenterPos);
            result.InitPoints(p0, p1, p2, p3);
            result.ThisPlaneType = PlaneType.BasisLocation;
            result.PlaneBasisType = CustomMath.XYZType.ZX;
            return result;
        }
        
        /// <summary>
        /// 임의의 기저 평면(XY, YZ, ZX)과 평행한 평면을 리턴하는 메서드
        /// </summary>
        public static CustomPlane GetBasisRegularLocation(Vector3 p_CenterPoint, Vector3 p_EdgePoint, CustomMath.XYZType p_XYZType)
        {
            switch (p_XYZType)
            {
                case CustomMath.XYZType.XY:
                {
                    var p0 = p_EdgePoint;
                    var normal = Vector3.forward;
                    var p1 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 90f);
                    var p2 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 180f);
                    var p3 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 270f);

                    var result = GetPlane(normal, p_CenterPoint);
                    result.InitPoints(p0, p1, p2, p3);
                    result.ThisPlaneType = PlaneType.BasisLocation;
                    result.PlaneBasisType = p_XYZType;
                    return result;
                }
                case CustomMath.XYZType.YZ :
                {
                    var p0 = p_EdgePoint;
                    var normal = Vector3.right;
                    var p1 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 90f);
                    var p2 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 180f);
                    var p3 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 270f);

                    var result = GetPlane(normal, p_CenterPoint);
                    result.InitPoints(p0, p1, p2, p3);
                    result.ThisPlaneType = PlaneType.BasisLocation;
                    result.PlaneBasisType = p_XYZType;
                    return result;
                }
                case CustomMath.XYZType.ZX :
                {
                    var p0 = p_EdgePoint;
                    var normal = Vector3.up;
                    var p1 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 90f);
                    var p2 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 180f);
                    var p3 = p_EdgePoint.RotationVectorByPivot(p_CenterPoint, normal, 270f);

                    var result = GetPlane(normal, p_CenterPoint);
                    result.InitPoints(p0, p1, p2, p3);
                    result.ThisPlaneType = PlaneType.BasisLocation;
                    result.PlaneBasisType = p_XYZType;
                    return result;
                }
#if UNITY_EDITOR
                default :
                    Debug.LogError("Regular Location must formed with XZ, YZ, ZX Type");
                    break;
#endif
            }
            return default;
        }

        /// <summary>
        /// 직각형의 평면 구조체를 리턴하는 메서드
        /// </summary>
        public static CustomPlane GetRegularLocation(Vector3 p_CenterPoint, Vector3 p_EdgePoint1, Vector3 p_EdgePoint2)
        {
            var p3 = p_EdgePoint1.GetSymmetricPosition(p_CenterPoint);
            var p4 = p_EdgePoint2.GetSymmetricPosition(p_CenterPoint);
            var result = GetPlane(p_CenterPoint, p_EdgePoint1, p_EdgePoint2);
            result.InitPoints(p_EdgePoint1, p_EdgePoint2, p3, p4);
            result.ThisPlaneType = PlaneType.RegularLocation;
            return result;
        }
        
        /// <summary>
        /// 임의의 네 좌표를 꼭지점으로 가지는 평면 구조체를 리턴하는 메서드
        /// </summary>
        public static CustomPlane GetLocation(Vector3 p_P0, Vector3 p_P1, Vector3 p_P2, Vector3 p_P3)
        {
            var centerPosition = 0.25f * (p_P0 + p_P1 + p_P2 + p_P3);
            var result = GetPlane(centerPosition, p_P0, p_P1);
            result.InitPoints(p_P0, p_P1, p_P2, p_P3);
            result.ThisPlaneType = PlaneType.Location;
            return result;
        }

        /// <summary>
        /// 임의의 RectTransform을 기술하는 CustomPlane
        /// </summary>
        public static CustomPlane Get_UI_Basis_Location(RectTransform p_RectTransform)
        {
            var normal = Vector3.forward;
            var centerPos = p_RectTransform.position;
            var scaleVector = p_RectTransform.lossyScale;
            var sizeVector = new Vector3(p_RectTransform.rect.width, p_RectTransform.rect.height, 0f);
            sizeVector = Vector3.Scale(sizeVector, scaleVector);

            var p0 = centerPos - 0.5f * sizeVector;
            var p2 = centerPos + 0.5f * sizeVector;
            var p1 = p0 + sizeVector.x * Vector3.right;
            var p3 = p0 + sizeVector.y * Vector3.up;
            
            var result = GetPlane(normal, centerPos);
            result.InitPoints(p0, p1, p2, p3);
            result.ThisPlaneType = PlaneType.BasisLocation;
            result.PlaneBasisType = CustomMath.XYZType.XY;
            return result;
        }

        #endregion

        #region <Enums>

        public enum PlaneType
        {
            /// <summary>
            /// 경계가 있는 평면 중에서 xy, yz, zx 평면 중에 하나와 평행인 평면,
            /// BasisLocation은 생성 메서드가 직각사각형을 생성하도록 설계되어있지만
            /// 이론적으로는 반드시 직각 사각형일 필요는 없다.
            /// </summary>
            BasisLocation,
            
            /// <summary>
            /// 경계가 있는 평면 중에서 원점 대칭인, 즉 네 모서리가 직각인 평면
            /// </summary>
            RegularLocation,
            
            /// <summary>
            /// 경계가 있는 평면
            /// </summary>
            Location,
            
            /// <summary>
            /// 경계가 없는(엄청나게 큰) 평면
            /// </summary>
            Plane,
        }

        /// <summary>
        /// 한 점과 평면의 포함관계
        /// </summary>
        public enum PlanePointIntersectType
        {
            NotOnPlane,
            In,
            Point0,
            Point1,
            Point2,
            Point3,
            Line01,
            Line12,
            Line23,
            Line30,
            Out,
        }

        #endregion
        
        #region <Fields>

        /// <summary>
        /// 평면 노말 벡터
        /// </summary>
        public Vector3 NormalVector { get; private set; }

        /// <summary>
        /// 평면의 중심
        /// </summary>
        public Vector3 CenterPoint { get; private set; }

        /// <summary>
        /// 반시계 방향으로 해당 평면을 구성하는 꼭지점 0 ~ 3
        /// </summary>
        public Vector3 Point0;
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Point3;

        /// <summary>
        /// 각 꼭지점에서 중앙으로의 단위 방향 벡터
        /// </summary>
        public Vector3 UV_0C;
        public Vector3 UV_1C;
        public Vector3 UV_2C;
        public Vector3 UV_3C;

        /// <summary>
        /// 반시계 방향으로 각 꼭지점을 연결하는 선분 0 > 1 > 2 > 3 > 0
        /// </summary>
        public CustomLine Segment01;
        public CustomLine Segment12;
        public CustomLine Segment23;
        public CustomLine Segment30;
        
        /// <summary>
        /// BiasRegularLocation 인 경우 사용
        /// </summary>
        public CustomMath.XYZType PlaneBasisType;
        
        /// <summary>
        /// 해당 평면의 타입
        /// </summary>
        public PlaneType ThisPlaneType { get; private set; }

        #endregion

        #region <Opertator>

        public static CustomPlane operator*(CustomPlane p_Plane, float p_Factor)
        {
            return p_Factor * p_Plane;
        }

        public static CustomPlane operator*(float p_Factor, CustomPlane p_Plane) {
            switch (p_Plane.ThisPlaneType)
            {
                case PlaneType.BasisLocation:
                case PlaneType.RegularLocation:
                case PlaneType.Location:
                    p_Plane.ScalePlane(p_Factor);
                    return p_Plane;
                default:
                case PlaneType.Plane:
                    return p_Plane;
            }
        }
        
        #endregion
        
        #region <Methods>

        public void MovePlane(Vector3 p_Offset)
        {
            UpdatePoints(Point0 + p_Offset, Point1 + p_Offset, Point2 + p_Offset, Point3 + p_Offset);
        }
        
        public void ScalePlane(float p_ScaleFactor)
        {
            Point0 = CustomMath.GetLBVector(CenterPoint, Point0, p_ScaleFactor);
            Point1 = CustomMath.GetLBVector(CenterPoint, Point1, p_ScaleFactor);
            Point2 = CustomMath.GetLBVector(CenterPoint, Point2, p_ScaleFactor);
            Point3 = CustomMath.GetLBVector(CenterPoint, Point3, p_ScaleFactor);
            
            UpdateEdges();
        }

        private void InitPoints(Vector3 p_P0, Vector3 p_P1, Vector3 p_P2, Vector3 p_P3)
        {
            UpdatePoints(p_P0, p_P1, p_P2, p_P3);
            
            UV_0C = Point0.GetDirectionUnitVectorTo(CenterPoint);
            UV_1C = Point1.GetDirectionUnitVectorTo(CenterPoint);
            UV_2C = Point2.GetDirectionUnitVectorTo(CenterPoint);
            UV_3C = Point3.GetDirectionUnitVectorTo(CenterPoint);
        }

        private void UpdatePoints(Vector3 p_P0, Vector3 p_P1, Vector3 p_P2, Vector3 p_P3)
        {
            Point0 = p_P0;
            Point1 = p_P1;
            Point2 = p_P2;
            Point3 = p_P3;
            CenterPoint = (Point0 + Point1 + Point2 + Point3) * 0.25f;
            
            UpdateEdges();
        }
        
        private void UpdateEdges()
        {
            Segment01 = CustomLine.GetSegment(Point0, Point1);
            Segment12 = CustomLine.GetSegment(Point1, Point2);
            Segment23 = CustomLine.GetSegment(Point2, Point3);
            Segment30 = CustomLine.GetSegment(Point3, Point0);
        }
        
        /// <summary>
        /// 특정 좌표를 해당 평면의 중심으로부터의 벡터로 취급하여 해당 평면위로 사영시킨 로컬 벡터를 리턴하는 메서드
        /// </summary>
        public Vector3 GetProjectionLocalVectorOnThisPlane(Vector3 p_TargetPosition)
        {
            return (p_TargetPosition - CenterPoint).GetPlaneProjectionVector(NormalVector);
        }
        
        /// <summary>
        /// 특정 좌표를 해당 평면의 중심으로부터의 벡터로 취급하여 해당 평면위로 사영시킨 월드 벡터를 리턴하는 메서드
        /// </summary>
        public Vector3 GetProjectionWorldVectorOnThisPlane(Vector3 p_TargetPosition)
        {
            return GetProjectionLocalVectorOnThisPlane(p_TargetPosition) + CenterPoint;
        }

        /// <summary>
        /// 평면 넓이가 무한하다고 가정했을 때(즉, 평면의 경계 안쪽에 지정한 좌표가 포함되어 있는지 여부는 고려하지 않고),
        /// 특정 좌표를 해당 평면에 사영했을 때, 그 수직 거리를 리턴하는 메서드
        /// </summary>
        public float GetPerpendicularDistance(Vector3 p_TargetPosition)
        {
            return (p_TargetPosition - GetProjectionWorldVectorOnThisPlane(p_TargetPosition)).magnitude;
        }
        
        /// <summary>
        /// 평면 넓이가 무한하다고 가정했을 때(즉, 평면의 경계 안쪽에 지정한 좌표가 포함되어 있는지 여부는 고려하지 않고),
        /// 특정 좌표를 해당 평면에 사영했을 때, 그 수직 제곱 거리를 리턴하는 메서드
        /// </summary>
        public float GetPerpendicularSqrDistance(Vector3 p_TargetPosition)
        {
            return (p_TargetPosition - GetProjectionWorldVectorOnThisPlane(p_TargetPosition)).sqrMagnitude;
        }
        
        /// <summary>
        /// 평면 넓이가 무한하다고 가정했을 때(즉, 평면의 경계 안쪽에 지정한 좌표가 포함되어 있는지 여부는 고려하지 않고),
        /// 특정 좌표가 해당 평면 위에 있는지 검증하는 논리메서드
        /// </summary>
        public bool IsOnSamePlane(Vector3 p_TargetPosition)
        {
            return GetPerpendicularSqrDistance(p_TargetPosition).IsReachedZero();
        }

        /// <summary>
        /// 특정 좌표가 해당 평면 위의 네 모서리 안쪽에 존재하는지 검증하는 논리 메서드
        /// 모서리가 없는 평면의 경우에는 무조건 inner를 리턴한다.
        /// </summary>
        public PlanePointIntersectType GetPlanePointIntersectType(Vector3 p_TargetPosition)
        {
            if (!IsOnSamePlane(p_TargetPosition))
            {
                return PlanePointIntersectType.NotOnPlane;
            }
            else
            {
                switch (ThisPlaneType)
                {
                    case PlaneType.BasisLocation:
                    {
                        if (CheckPointInnerPlane(p_TargetPosition))
                        {
                            switch (PlaneBasisType)
                            {
                                case CustomMath.XYZType.XY:
                                {
                                    var tryXReachedLeftWidth = (p_TargetPosition.x - Point0.x).IsReachedZero();
                                    var tryXReachedRightWidth = (p_TargetPosition.x - Point1.x).IsReachedZero();
                                    var tryYReachedUpperHeight = (p_TargetPosition.y - Point0.y).IsReachedZero();
                                    var tryYReachedLowerHeight = (p_TargetPosition.y - Point1.y).IsReachedZero();

                                    if (tryYReachedUpperHeight && tryXReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Point0;
                                    }
                                    if (tryYReachedUpperHeight && tryXReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Point1;
                                    }
                                    if (tryYReachedLowerHeight && tryXReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Point2;
                                    }
                                    if (tryYReachedLowerHeight && tryXReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Point3;
                                    }
                                    if (tryXReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Line30;
                                    }
                                    if (tryXReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Line12;
                                    }
                                    if (tryYReachedUpperHeight)
                                    {
                                        return PlanePointIntersectType.Line01;
                                    }
                                    if (tryYReachedLowerHeight)
                                    {
                                        return PlanePointIntersectType.Line23;
                                    }
                                }
                                    break;
                                case CustomMath.XYZType.YZ:
                                {
                                    var tryYReachedLeftWidth = (p_TargetPosition.y - Point0.y).IsReachedZero();
                                    var tryYReachedRightWidth = (p_TargetPosition.y - Point1.y).IsReachedZero();
                                    var tryZReachedUpperHeight = (p_TargetPosition.z - Point0.z).IsReachedZero();
                                    var tryZReachedLowerHeight = (p_TargetPosition.z - Point1.z).IsReachedZero();

                                    if (tryZReachedUpperHeight && tryYReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Point0;
                                    }
                                    if (tryZReachedUpperHeight && tryYReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Point1;
                                    }
                                    if (tryZReachedLowerHeight && tryYReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Point2;
                                    }
                                    if (tryZReachedLowerHeight && tryYReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Point3;
                                    }
                                    if (tryYReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Line30;
                                    }
                                    if (tryYReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Line12;
                                    }
                                    if (tryZReachedUpperHeight)
                                    {
                                        return PlanePointIntersectType.Line01;
                                    }
                                    if (tryZReachedLowerHeight)
                                    {
                                        return PlanePointIntersectType.Line23;
                                    }
                                }
                                    break;
                                case CustomMath.XYZType.ZX:
                                {
                                    var tryZReachedLeftWidth = (p_TargetPosition.z - Point0.z).IsReachedZero();
                                    var tryZReachedRightWidth = (p_TargetPosition.z - Point1.z).IsReachedZero();
                                    var tryXReachedUpperHeight = (p_TargetPosition.x - Point0.x).IsReachedZero();
                                    var tryXReachedLowerHeight = (p_TargetPosition.x - Point1.x).IsReachedZero();

                                    if (tryXReachedUpperHeight && tryZReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Point0;
                                    }
                                    if (tryXReachedUpperHeight && tryZReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Point1;
                                    }
                                    if (tryXReachedLowerHeight && tryZReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Point2;
                                    }
                                    if (tryXReachedLowerHeight && tryZReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Point3;
                                    }
                                    if (tryZReachedLeftWidth)
                                    {
                                        return PlanePointIntersectType.Line30;
                                    }
                                    if (tryZReachedRightWidth)
                                    {
                                        return PlanePointIntersectType.Line12;
                                    }
                                    if (tryXReachedUpperHeight)
                                    {
                                        return PlanePointIntersectType.Line01;
                                    }
                                    if (tryXReachedLowerHeight)
                                    {
                                        return PlanePointIntersectType.Line23;
                                    }
                                }
                                    break;
                            }
                        }
                    }
                        break;

                    case PlaneType.RegularLocation:
                    case PlaneType.Location:
                    {
                        if (CheckPointInnerPlane(p_TargetPosition))
                        {
                            var (hasSeg01Intersect, seg01LBFactor) =
                                Segment01.GetLBFactor(p_TargetPosition, CustomMath.Epsilon);
                            if (hasSeg01Intersect)
                            {
                                switch (seg01LBFactor.GetProgressType01())
                                {
                                    case CustomMath.Progress01Type.Zero:
                                        return PlanePointIntersectType.Point0;
                                    case CustomMath.Progress01Type.ZeroToOne:
                                        return PlanePointIntersectType.Line01;
                                    case CustomMath.Progress01Type.One:
                                        return PlanePointIntersectType.Point1;
                                }
                            }
                            
                            var (hasSeg12Intersect, seg12LBFactor) =
                                Segment01.GetLBFactor(p_TargetPosition, CustomMath.Epsilon);
                            if (hasSeg12Intersect)
                            {
                                switch (seg12LBFactor.GetProgressType01())
                                {
                                    case CustomMath.Progress01Type.Zero:
                                        return PlanePointIntersectType.Point1;
                                    case CustomMath.Progress01Type.ZeroToOne:
                                        return PlanePointIntersectType.Line12;
                                    case CustomMath.Progress01Type.One:
                                        return PlanePointIntersectType.Point2;
                                }
                            }

                            var (hasSeg23Intersect, seg23LBFactor) =
                                Segment01.GetLBFactor(p_TargetPosition, CustomMath.Epsilon);
                            if (hasSeg23Intersect)
                            {
                                switch (seg23LBFactor.GetProgressType01())
                                {
                                    case CustomMath.Progress01Type.Zero:
                                        return PlanePointIntersectType.Point2;
                                    case CustomMath.Progress01Type.ZeroToOne:
                                        return PlanePointIntersectType.Line23;
                                    case CustomMath.Progress01Type.One:
                                        return PlanePointIntersectType.Point3;
                                }
                            }

                            var (hasSeg30Intersect, seg30LBFactor) =
                                Segment01.GetLBFactor(p_TargetPosition, CustomMath.Epsilon);
                            if (hasSeg30Intersect)
                            {
                                switch (seg30LBFactor.GetProgressType01())
                                {
                                    case CustomMath.Progress01Type.Zero:
                                        return PlanePointIntersectType.Point3;
                                    case CustomMath.Progress01Type.ZeroToOne:
                                        return PlanePointIntersectType.Line30;
                                    case CustomMath.Progress01Type.One:
                                        return PlanePointIntersectType.Point0;
                                }
                            }
                            
                            return PlanePointIntersectType.In;
                        }
                    }
                        break;
                    
                    case PlaneType.Plane:
                        return PlanePointIntersectType.In;
                }
                return PlanePointIntersectType.Out;
            }
        }

        /// <summary>
        /// 특정 좌표가 해당 평면의 네 모서리 안쪽에 존재하는지 검증하는 논리 메서드
        /// 모서리가 없는 평면의 경우에는 무조건 true를 리턴한다.
        /// 높이를 고려하지 않는다.
        /// </summary>
        public bool CheckPointInnerPlane(Vector3 p_TargetPosition)
        {
            switch (ThisPlaneType)
            {
                case PlaneType.Location:
                case PlaneType.RegularLocation:
                {
                    var p0c = p_TargetPosition - Point0;
                    var p1c = p_TargetPosition - Point1;
                    var p2c = p_TargetPosition - Point2;
                    var p3c = p_TargetPosition - Point3;

                    var detP0 = Vector3.Cross(Segment01.DirectionUnitVector, p0c);
                    var detP1 = Vector3.Cross(Segment12.DirectionUnitVector, p1c);
                    var detP2 = Vector3.Cross(Segment23.DirectionUnitVector, p2c);
                    var detP3 = Vector3.Cross(Segment30.DirectionUnitVector, p3c);

                    if (
                        (detP0.x < 0f && detP1.x < 0f && detP2.x < 0f && detP3.x < 0f) ||
                        (detP0.y < 0f && detP1.y < 0f && detP2.y < 0f && detP3.y < 0f) ||
                        (detP0.z < 0f && detP1.z < 0f && detP2.z < 0f && detP3.z < 0f) ||
                        (detP0.x >= 0f && detP1.x >= 0f && detP2.x >= 0f && detP3.x >= 0f) ||
                        (detP0.y >= 0f && detP1.y >= 0f && detP2.y >= 0f && detP3.y >= 0f) ||
                        (detP0.z >= 0f && detP1.z >= 0f && detP2.z >= 0f && detP3.z >= 0f)
                    )
                    {
                        return true;
                    }
                }
                    break;
                case PlaneType.BasisLocation:
                {
                    var p0c = p_TargetPosition - Point0;
                    var p1c = p_TargetPosition - Point1;
                    var p2c = p_TargetPosition - Point2;
                    var p3c = p_TargetPosition - Point3;
                    
                    var detP0 = Vector3.Cross(Segment01.DirectionUnitVector, p0c);
                    var detP1 = Vector3.Cross(Segment12.DirectionUnitVector, p1c);
                    var detP2 = Vector3.Cross(Segment23.DirectionUnitVector, p2c);
                    var detP3 = Vector3.Cross(Segment30.DirectionUnitVector, p3c);

                    switch (PlaneBasisType)
                    {
                        case CustomMath.XYZType.XY:
                            if (
                                (detP0.z < 0f && detP1.z < 0f && detP2.z < 0f && detP3.z < 0f) ||
                                (detP0.z >= 0f && detP1.z >= 0f && detP2.z >= 0f && detP3.z >= 0f)
                            )
                            {
                                return true;
                            }
                            break;
                        case CustomMath.XYZType.YZ:
                            if (
                                (detP0.x < 0f && detP1.x < 0f && detP2.x < 0f && detP3.x < 0f) ||
                                (detP0.x >= 0f && detP1.x >= 0f && detP2.x >= 0f && detP3.x >= 0f)
                            )
                            {
                                return true;
                            }
                            break;
                        case CustomMath.XYZType.ZX:
                            if (
                                (detP0.y < 0f && detP1.y < 0f && detP2.y < 0f && detP3.y < 0f) ||
                                (detP0.y >= 0f && detP1.y >= 0f && detP2.y >= 0f && detP3.y >= 0f)
                            )
                            {
                                return true;
                            }
                            break;
                    }
                }
                    break;
                case PlaneType.Plane:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 특정 좌표가 해당 평면의 네 모서리 안쪽에 존재하는지 및 해당 평면으로부터 일정 거리 이내에 존재하는지
        /// 검증하는 논리 메서드
        /// </summary>
        public bool CheckPointInnerPlane(Vector3 p_TargetPosition, float p_Height)
        {
            if (CheckPointInnerPlane(p_TargetPosition))
            {
                var compareSqrHeight = Mathf.Pow(p_Height, 2);
                return GetPerpendicularSqrDistance(p_TargetPosition) <= compareSqrHeight;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// 해당 평면이 평행사변형인지 검증하는 논리메서드.
        /// </summary>
        public bool IsParallelogram()
        {
            switch (ThisPlaneType)
            {
                case PlaneType.BasisLocation:
                case PlaneType.RegularLocation:
                    return true;
                case PlaneType.Location:
                    return Segment01.IsLineParallel(Segment23, false) &&
                           Segment12.IsLineParallel(Segment30, false);
                case PlaneType.Plane:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 지정한 평면과 동일한 모양의 경계를 가지는지 검증하는 논리메서드
        /// 같은 모양이라면 해당평면에 대한 지정평면의 배율을 outmode로 리턴한다.
        /// </summary>
        public (bool, float) HasSameShape(CustomPlane p_RightPlane)
        {
            switch (ThisPlaneType)
            {
                case PlaneType.BasisLocation:
                {
                    var tryShape = Segment01.IsLineParallel(p_RightPlane.Segment01, false) &&
                                   Segment12.IsLineParallel(p_RightPlane.Segment12, false);
                    if (tryShape)
                    {
                        return (true, p_RightPlane.Segment01.GetLength() / Segment01.GetLength());
                    }
                }
                    break;
                case PlaneType.RegularLocation:
                {
                    var tryShape = Segment01.IsLineParallel(p_RightPlane.Segment01, false) &&
                                   Segment12.IsLineParallel(p_RightPlane.Segment12, false);
                    if (tryShape)
                    {
                        return (true, p_RightPlane.Segment01.GetLength() / Segment01.GetLength());
                    }
                }
                    break;

                case PlaneType.Location:
                {
                    var tryShape = Segment01.IsLineParallel(p_RightPlane.Segment01, false) &&
                                   Segment12.IsLineParallel(p_RightPlane.Segment12, false) &&
                                   Segment23.IsLineParallel(p_RightPlane.Segment23, false) &&
                                   Segment30.IsLineParallel(p_RightPlane.Segment30, false);
                    if (tryShape)
                    {
                        return (true, p_RightPlane.Segment01.GetLength() / Segment01.GetLength());
                    }
                }
                    break;
                // 경계선이 없는 경우, 두 평면은 같은 모양
                case PlaneType.Plane:
                    return (true, 1f);
            }

            return (false, 0f);
        }

        /// <summary>
        /// 유니티 엔진의 플레인 타입으로 변환하는 메서드
        /// </summary>
        public Plane ConvertToPlane()
        {
            return new Plane(NormalVector, CenterPoint);
        }

#if UNITY_EDITOR
        public void DrawPlane()
        {
            var color = Color.white;
            switch (ThisPlaneType)
            {
                case PlaneType.RegularLocation:
                    color = Color.green;
                    break;
                case PlaneType.Location:
                    color = Color.cyan;
                    break;
                case PlaneType.Plane:
                    return;
            }

            Gizmos.color = color;
            Gizmos.DrawLine(Point0, Point1);
            Gizmos.DrawLine(Point1, Point2);
            Gizmos.DrawLine(Point2, Point3);
            Gizmos.DrawLine(Point3, Point0);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(CenterPoint, Point0);
            Gizmos.DrawLine(CenterPoint, Point1);
            Gizmos.DrawLine(CenterPoint, Point2);
            Gizmos.DrawLine(CenterPoint, Point3);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(CenterPoint, NormalVector);
        }
#endif

        #endregion
    }

    /// <summary>
    /// 임의의 두 평면 간의 내부 좌표를 정규화 시키기 위한 기능을 포함한 구조체
    /// </summary>
    public struct CoordinateConverter
    {
        #region <Consts>
        
        /// <summary>
        /// 임의의 두 좌표계 간의 정규기저를 구하려면 두 좌표계 혹은 평면은 동일한 내각을 가진 평행사변형이어야 한다.
        /// </summary>
        public static (bool, CoordinateConverter) GetConverter(CustomPlane p_LeftPlane, CustomPlane p_RightPlane)
        {
            if (p_LeftPlane.ThisPlaneType != CustomPlane.PlaneType.Plane &&
                p_RightPlane.ThisPlaneType != CustomPlane.PlaneType.Plane)
            {
                var (hasSameShape, scaleFactor) = p_LeftPlane.HasSameShape(p_RightPlane);
                if (p_LeftPlane.IsParallelogram() && hasSameShape)
                {
                    var resultConverter = new CoordinateConverter();
                    resultConverter.LeftPlane = p_LeftPlane;
                    resultConverter.RightPlane = p_RightPlane;
                    resultConverter.RightScalePerLeft = scaleFactor;
                    resultConverter.BiasU = p_LeftPlane.Segment23.DirectionUnitVector;
                    resultConverter.BiasV = -p_LeftPlane.Segment12.DirectionUnitVector;
                    
                    return (true, resultConverter);
                }
            }

            return default;
        }

        #endregion

        #region <Fields>

        public CustomPlane LeftPlane;
        public CustomPlane RightPlane;
        public float RightScalePerLeft;
        public Vector3 BiasU;
        public Vector3 BiasV;

        #endregion

        #region <Methods>

        public bool TurnPosition(Vector3 p_TryPos, out Vector3 o_TranslatedPos)
        {
            if (LeftPlane.CheckPointInnerPlane(p_TryPos))
            {
                o_TranslatedPos = RightPlane.CenterPoint + (p_TryPos - LeftPlane.CenterPoint) * RightScalePerLeft;
                return true;
            }
            else
            {
                o_TranslatedPos = Vector3.zero;
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// N개의 꼭지점을 지니는 볼록다각형에 대한 포함관계를 기술하는 클래스
    /// </summary>
    public class N_PolyLocation
    {
        #region <Methods>

        public List<Vector3> EdgeList;

        #endregion

        #region <Constructors>

        public N_PolyLocation()
        {
            EdgeList = new List<Vector3>();
        }

        public N_PolyLocation(List<Vector3> p_EdgeList)
        {
            EdgeList = p_EdgeList;
        }

        #endregion

        #region <Methods>

        public bool IsContains(Vector3 p_TargetPosition)
        {
            var count = EdgeList.Count;
            var pivot = p_TargetPosition - EdgeList[count - 1];
            var nextPivot = p_TargetPosition - EdgeList[0];
            var currentFlag = Vector3.Cross(pivot, nextPivot).y > 0f;
            
            for (int i = 0; i < count - 1; i++)
            {
                pivot = nextPivot;
                nextPivot = p_TargetPosition - EdgeList[i + 1];
                if (currentFlag != Vector3.Cross(pivot, nextPivot).y > 0f)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}