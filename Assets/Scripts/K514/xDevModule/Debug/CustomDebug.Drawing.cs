#if UNITY_EDITOR
using UnityEngine;

namespace BlackAm
{
    public static partial class CustomDebug
    {
        #region <Line>

        public static void DrawLine(Vector3 p_Start, Vector3 p_End, Color p_Color, float p_Duration = 0f)
        {
            DrawCustomLine(CustomLine.GetSegment(p_Start, p_End), p_Color, p_Duration);
        }

        public static void DrawRay(Ray p_Ray, Color p_Color, float p_Duration = 0f)
        {
            DrawCustomLine(CustomLine.GetHalfLine(p_Ray), p_Color, p_Duration);
        }

        public static void DrawCustomLine(CustomLine p_Line, Color p_Color, float p_Duration = 0f)
        {
            DrawArrow(p_Line.StartPoint, p_Line.EndPoint, 0f, p_Color, p_Duration);
        }
        
        public static void DrawArrow(Vector3 p_Pivot, Color p_Color, float p_Duration = 0f)
        {
            DrawArrow(p_Pivot, p_Pivot + Vector3.up * 10f, 0.08f, p_Color, p_Duration);
        }

        public static void DrawArrow(Vector3 p_From, Vector3 p_To, float p_Radius, Color p_Color, float p_Duration = 0f)
        {
            var direction = p_From.GetDirectionVectorTo(p_To);
            if (direction == Vector3.zero || p_From == p_To)
            {
                return;
            }

            var perpDir = direction.GetXZPerpendicularUnitVector();
            var perpperpDir = Vector3.Cross(perpDir, direction).normalized;
            var groove = 4;
            var reversedGroove = 1f / groove;
            if (p_Radius < CustomMath.Epsilon)
            {
            }
            else
            {
                for (int i = 0; i < groove; i++)
                {
                    var drawingVector = perpDir.SlerpFromTo(perpperpDir, i * reversedGroove);
                    Debug.DrawLine(p_From, p_From + drawingVector * p_Radius, p_Color, p_Duration);
                    Debug.DrawLine(p_From, p_From - drawingVector * p_Radius, p_Color, p_Duration);
                    Debug.DrawLine(p_To, p_To + drawingVector * p_Radius - direction * 0.2f, p_Color, p_Duration);
                    Debug.DrawLine(p_To, p_To - drawingVector * p_Radius - direction * 0.2f, p_Color, p_Duration);
                    drawingVector = perpperpDir.SlerpFromTo(-perpDir, i * reversedGroove);
                    Debug.DrawLine(p_From, p_From + drawingVector * p_Radius, p_Color, p_Duration);
                    Debug.DrawLine(p_From, p_From - drawingVector * p_Radius, p_Color, p_Duration);
                    Debug.DrawLine(p_To, p_To + drawingVector * p_Radius - direction * 0.2f, p_Color, p_Duration);
                    Debug.DrawLine(p_To, p_To - drawingVector * p_Radius - direction * 0.2f, p_Color, p_Duration);
                }
            }

            Debug.DrawLine(p_From, p_To, p_Color, p_Duration);
        }

        #endregion

        #region <Affine>

        public static void DrawAffineCachedPreset(TransformTool.AffineCachePreset p_AffineCachedPreset, float p_Duration = 0f)
        {
            DrawAffineCachedPreset(p_AffineCachedPreset, Color.blue, Color.red, Color.green, p_Duration);
        }

        public static void DrawAffineCachedPreset(TransformTool.AffineCachePreset p_AffineCachedPreset, Color p_ColorX, Color p_ColorY, Color p_ColorZ,  float p_Duration = 0f)
        {
            DrawArrow(p_AffineCachedPreset.Position, p_AffineCachedPreset.Position + p_AffineCachedPreset.Forward * 10f, 0.1f, p_ColorX, p_Duration);
            DrawArrow(p_AffineCachedPreset.Position, p_AffineCachedPreset.Position + p_AffineCachedPreset.Right * 10f, 0.1f, p_ColorY, p_Duration);
            DrawArrow(p_AffineCachedPreset.Position, p_AffineCachedPreset.Position + p_AffineCachedPreset.Up * 10f, 0.1f, p_ColorZ, p_Duration);
        }

        #endregion

        #region <Plane>

        public static void DrawPlane(Vector3 p_Pivot0, Vector3 p_Pivot1, Vector3 p_Pivot2, Vector3 p_Pivot3, Color p_Color, int p_EdgeCount = 8, float p_Duration = 0f)
        {
            p_EdgeCount = Mathf.Max(p_EdgeCount, 1) + 1;
            var revEdge = 1f / p_EdgeCount;
            for (int i = 1; i < p_EdgeCount; i++)
            {
                var timeScale = i * revEdge;
                var intv01 = p_Pivot0.GetLBVector(p_Pivot1, timeScale);
                var intv32 = p_Pivot3.GetLBVector(p_Pivot2, timeScale);
                var intv03 = p_Pivot0.GetLBVector(p_Pivot3, timeScale);
                var intv12 = p_Pivot1.GetLBVector(p_Pivot2, timeScale);
                
                DrawLine(intv01, intv32, p_Color, p_Duration);
                DrawLine(intv03, intv12, p_Color, p_Duration);
            }
            
            DrawLine(p_Pivot0, p_Pivot1, p_Color, p_Duration);
            DrawLine(p_Pivot1, p_Pivot2, p_Color, p_Duration);
            DrawLine(p_Pivot2, p_Pivot3, p_Color, p_Duration);
            DrawLine(p_Pivot3, p_Pivot0, p_Color, p_Duration);
        }
        
        public static void DrawCustomPlane(CustomPlane p_Plane, Color p_Color, int p_EdgeCount = 8, float p_Duration = 0f)
        {
            DrawPlane(p_Plane.Point0, p_Plane.Point1, p_Plane.Point2, p_Plane.Point3, p_Color, p_EdgeCount, p_Duration);
        }
        
        public static void DrawCustomPlane(CustomPlane p_Plane, float p_HalfHeight, Color p_Color, int p_EdgeCount = 8, float p_Duration = 0f)
        {
            var upperPlane = p_Plane;
            var lowerPlane = p_Plane;
            var heightOffset = p_HalfHeight * p_Plane.NormalVector;
            upperPlane.MovePlane(heightOffset);
            lowerPlane.MovePlane(-heightOffset);
            
            var upperPivot0 = upperPlane.Point0;
            var upperPivot1 = upperPlane.Point1;
            var upperPivot2 = upperPlane.Point2;
            var upperPivot3 = upperPlane.Point3;
            
            var lowerPivot0 = lowerPlane.Point0;
            var lowerPivot1 = lowerPlane.Point1;
            var lowerPivot2 = lowerPlane.Point2;
            var lowerPivot3 = lowerPlane.Point3;

            DrawCustomPlane(upperPlane, p_Color, p_EdgeCount, p_Duration);
            DrawCustomPlane(lowerPlane, p_Color, p_EdgeCount, p_Duration);

            DrawCustomPlane(CustomPlane.GetLocation(upperPivot0, upperPivot1, lowerPivot1, lowerPivot0), p_Color, p_EdgeCount, p_Duration);
            DrawCustomPlane(CustomPlane.GetLocation(upperPivot1, upperPivot2, lowerPivot2, lowerPivot1), p_Color, p_EdgeCount, p_Duration);
            DrawCustomPlane(CustomPlane.GetLocation(upperPivot2, upperPivot3, lowerPivot3, lowerPivot2), p_Color, p_EdgeCount, p_Duration);
            DrawCustomPlane(CustomPlane.GetLocation(upperPivot3, upperPivot0, lowerPivot0, lowerPivot3), p_Color, p_EdgeCount, p_Duration);
        }

        public static void DrawBox(Vector3 p_Center, Vector3 p_HalfExtends, Quaternion p_Rotation, Color p_Color, int p_EdgeCount = 16, float p_Duration = 0f)
        {
            var biasVectorX = p_Rotation * Vector3.right;
            var biasVectorY = p_Rotation * Vector3.up;
            var biasVectorZ = p_Rotation * Vector3.forward;
            
            var scaledBiasVectorX = p_HalfExtends.x * biasVectorX;
            var scaledBiasVectorY = p_HalfExtends.y * biasVectorY;
            var scaledBiasVectorZ = p_HalfExtends.z * biasVectorZ;
            var scaledBiasVector = scaledBiasVectorX + scaledBiasVectorY + scaledBiasVectorZ;
            
            var p000 = p_Center - scaledBiasVector;
            var p001 = p000 + 2f * scaledBiasVectorZ;
            var p010 = p000 + 2f * scaledBiasVectorY;
            var p100 = p000 + 2f * scaledBiasVectorX;
            var p111 = p_Center + scaledBiasVector;
            var p110 = p111 - 2f * scaledBiasVectorZ;
            var p101 = p111 - 2f * scaledBiasVectorY;
            var p011 = p111 - 2f * scaledBiasVectorX;
            
            DrawPlane(p000, p001, p011, p010, p_Color, p_EdgeCount, p_Duration);
            DrawPlane(p001, p101, p111, p011, p_Color, p_EdgeCount, p_Duration);
            DrawPlane(p101, p100, p110, p111, p_Color, p_EdgeCount, p_Duration);
            DrawPlane(p100, p000, p010, p110, p_Color, p_EdgeCount, p_Duration);
            DrawPlane(p000, p001, p101, p100, p_Color, p_EdgeCount, p_Duration);
            DrawPlane(p010, p011, p111, p110, p_Color, p_EdgeCount, p_Duration);
        }

        #endregion

        #region <PartialCircle>

        public static void DrawPartialCircle(Vector3 p_Center, float p_LowerBoundRadius, float p_Radius, Vector3 p_Forward, float p_Angle, Vector3 p_Normal, Color p_Color,
            int p_EdgeCount = 16, float p_Duration = 0f)
        {
            p_Normal = p_Normal.normalized;

            var edgeCount = 6;
            var upperRadiusPivot = p_Center + p_Radius * p_Forward;
            var lowerRadiusPivot = p_Center + p_LowerBoundRadius * p_Forward;
            var theta = p_Angle / p_EdgeCount;
            
            for (int i = 0; i < p_EdgeCount; i++)
            {
                var targetVector = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i);
                var nextVector = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1));
                var targetVector2 = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i);
                var nextVector2 = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1));
                DrawPlane(targetVector, nextVector, nextVector2, targetVector2, p_Color, edgeCount, p_Duration);
        
                var targetRVector = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * i);
                var nextRVector = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * (i + 1));
                var targetRVector2 = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * i);
                var nextRVector2 = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * (i + 1));
                DrawPlane(targetRVector, nextRVector, nextRVector2, targetRVector2, p_Color, edgeCount, p_Duration);
            }
        }
        
        public static void DrawPartialCylinder(Vector3 p_Center, float p_LowerBoundRadius, float p_Radius, Vector3 p_Forward, float p_Angle, Vector3 p_Normal, float p_HalfHeight, Color p_Color,
            int p_EdgeCount = 16, float p_Duration = 0f)
        {
            if (p_HalfHeight < CustomMath.Epsilon)
            {
                DrawPartialCircle(p_Center, p_LowerBoundRadius, p_Radius, p_Forward, p_Angle, p_Normal, p_Color, p_EdgeCount, p_Duration);
            }
            else
            {
                p_Normal = p_Normal.normalized;

                var edgeCount = 6;
                var upperOffset = p_HalfHeight * p_Normal;
                var lowerOffset = p_HalfHeight * -p_Normal;
                var upperRadiusPivot = p_Center + p_Radius * p_Forward;
                var lowerRadiusPivot = p_Center + p_LowerBoundRadius * p_Forward;
                var theta = p_Angle / p_EdgeCount;
                
                for (int i = 0; i < p_EdgeCount; i++)
                {
                    // 부채꼴 상단면
                    var targetVectorU = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + upperOffset;
                    var nextVectorU = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + upperOffset;
                    var targetVector2U = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + upperOffset;
                    var nextVector2U = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + upperOffset;
                    DrawPlane(targetVectorU, nextVectorU, nextVector2U, targetVector2U, p_Color, edgeCount, p_Duration);
                
                    // 부채꼴 하단면
                    var targetVectorL = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + lowerOffset;
                    var nextVectorL = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + lowerOffset;
                    var targetVector2L = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + lowerOffset;
                    var nextVector2L = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + lowerOffset;
                    DrawPlane(targetVectorL, nextVectorL, nextVector2L, targetVector2L, p_Color, edgeCount, p_Duration);
                    
                    // 부채꼴 상단면
                    var targetRVectorU = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * i) + upperOffset;
                    var nextRVectorU = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * (i + 1)) + upperOffset;
                    var targetRVector2U = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * i) + upperOffset;
                    var nextRVector2U = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * (i + 1)) + upperOffset;
                    DrawPlane(targetRVectorU, nextRVectorU, nextRVector2U, targetRVector2U, p_Color, edgeCount, p_Duration);
                
                    // 부채꼴 하단면
                    var targetRVectorL = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * i) + lowerOffset;
                    var nextRVectorL = lowerRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * (i + 1)) + lowerOffset;
                    var targetRVector2L = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * i) + lowerOffset;
                    var nextRVector2L = upperRadiusPivot.RotationVectorByPivot(p_Center, p_Normal, -theta * (i + 1)) + lowerOffset;
                    DrawPlane(targetRVectorL, nextRVectorL, nextRVector2L, targetRVector2L, p_Color, edgeCount, p_Duration);

                    // 부채꼴 옆면
                    DrawPlane(targetVectorU, nextVectorU, nextVectorL, targetVectorL, p_Color, edgeCount, p_Duration);
                    DrawPlane(targetVector2U, nextVector2U, nextVector2L, targetVector2L, p_Color, edgeCount, p_Duration);
                    DrawPlane(targetRVectorU, nextRVectorU, nextRVectorL, targetRVectorL, p_Color, edgeCount, p_Duration);
                    DrawPlane(targetRVector2U, nextRVector2U, nextRVector2L, targetRVector2L, p_Color, edgeCount, p_Duration);
                    
                    // 부채꼴 단면
                    if (i == p_EdgeCount - 1)
                    {
                        DrawPlane(nextVectorU, nextVector2U, nextVector2L, nextVectorL, p_Color, edgeCount, p_Duration);
                        DrawPlane(nextRVectorU, nextRVector2U, nextRVector2L, nextRVectorL, p_Color, edgeCount, p_Duration);
                    }
                }
            }
        }

        #endregion

        #region <Circle>

        public static void DrawCircle(Vector3 p_Center, float p_LowerBoundRadius, float p_Radius, Vector3 p_Normal, Color p_Color, int p_EdgeCount = 16, float p_Duration = 0f)
        {
            p_Normal = p_Normal.normalized;
            
            var perpVector = p_Normal.GetXZPerpendicularUnitVector();
            var pivot = p_Center + p_Radius * perpVector;
            var lowerPivot = p_Center + p_LowerBoundRadius * perpVector;
            var theta = 360f / p_EdgeCount;
            
            for (int i = 0; i < p_EdgeCount; i++)
            {
                var targetVector = lowerPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i);
                var nextVector = lowerPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1));
                var targetVector2 = pivot.RotationVectorByPivot(p_Center, p_Normal, theta * i);
                var nextVector2 = pivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1));
                DrawPlane(targetVector, nextVector, nextVector2, targetVector2, p_Color, p_EdgeCount, p_Duration);
            }
        }
        
        public static void DrawCylinder(Vector3 p_Center, float p_LowerBoundRadius, float p_Radius, Vector3 p_Normal, float p_HalfHeight, Color p_Color, int p_EdgeCount = 16, float p_Duration = 0f)
        {
            if (p_HalfHeight < CustomMath.Epsilon)
            {
                DrawCircle(p_Center, p_LowerBoundRadius, p_Radius, p_Normal, p_Color, p_EdgeCount, p_Duration);
            }
            else
            {
                p_Normal = p_Normal.normalized;
                
                var perpVector = p_Normal.GetXZPerpendicularUnitVector();
                var upperOffset = p_HalfHeight * p_Normal;
                var lowerOffset = p_HalfHeight * -p_Normal;
                
                var pivot = p_Center + p_Radius * perpVector;
                var lowerPivot = p_Center + p_LowerBoundRadius * perpVector;
                var theta = 360f / p_EdgeCount;
                for (int i = 0; i < p_EdgeCount; i++)
                {
                    // 원기둥 상단면
                    var targetVectorU = lowerPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + upperOffset;
                    var nextVectorU = lowerPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + upperOffset;
                    var targetVector2U = pivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + upperOffset;
                    var nextVector2U = pivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + upperOffset;
                    DrawPlane(targetVectorU, nextVectorU, nextVector2U, targetVector2U, p_Color, p_EdgeCount, p_Duration);
                    
                    // 원기둥 하단면
                    var targetVectorL = lowerPivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + lowerOffset;
                    var nextVectorL = lowerPivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + lowerOffset;
                    var targetVector2L = pivot.RotationVectorByPivot(p_Center, p_Normal, theta * i) + lowerOffset;
                    var nextVector2L = pivot.RotationVectorByPivot(p_Center, p_Normal, theta * (i + 1)) + lowerOffset;
                    DrawPlane(targetVectorL, nextVectorL, nextVector2L, targetVector2L, p_Color, p_EdgeCount, p_Duration);
                    
                    // 원기둥 옆면
                    DrawPlane(targetVector2U, nextVector2U, nextVector2L, targetVector2L, p_Color, p_EdgeCount, p_Duration);
                }
            }
        }

        public static void DrawSphere(Vector3 p_Center, float p_LowerBoundRadius, float p_Radius, Vector3 p_Normal, Color p_Color,
            int p_Edge = 16, float p_Duration = 0f)
        {
            var perpVector = Vector3.zero;
            p_Normal = p_Normal.normalized;
            perpVector = p_Normal.GetXZPerpendicularUnitVector();
            
            var theta = 360f / p_Edge;
            for (int i = 0; i < p_Edge; i++)
            {
                var targetVector = perpVector.RotationVectorByPivot(Vector3.zero, p_Normal, theta * i);
                var targetNormal = Vector3.Cross(p_Normal, targetVector);
                DrawCircle(p_Center, p_LowerBoundRadius, p_Radius, targetNormal, p_Color, p_Edge, p_Duration);
            }
            
            for (int i = 0; i < p_Edge; i++)
            {
                var targetVector = p_Normal.RotationVectorByPivot(Vector3.zero, perpVector, theta * i);
                var targetNormal = Vector3.Cross(perpVector, targetVector);
                DrawCircle(p_Center, p_LowerBoundRadius, p_Radius, targetNormal, p_Color, p_Edge, p_Duration);
            }
        }
        
        public static void DrawCapsule(Vector3 p_BasePosition, bool p_IsReverse, float p_Radius, float p_Height, Color p_Color, int p_Edge = 16, float p_Duration = 0f)
        {
            var innerPivotLow = p_IsReverse ? p_BasePosition - Vector3.up * p_Radius : p_BasePosition + Vector3.up * p_Radius;
            var innerPivotHigh = p_IsReverse ? p_BasePosition - Vector3.up * (p_Height - p_Radius) : p_BasePosition + Vector3.up * (p_Height - p_Radius);
            
            DrawSphere(innerPivotLow, 0f, p_Radius, Vector3.up, p_Color, p_Edge, p_Duration);
            DrawSphere(innerPivotHigh, 0f, p_Radius, Vector3.up, p_Color, p_Edge, p_Duration);
            
            var theta = 360f / p_Edge;
            for (int i = 0; i < p_Edge; i++)
            {
                var curTheta = theta * i * Mathf.Deg2Rad;
                var orbitOffset = new Vector3(Mathf.Sin(curTheta), 0f, Mathf.Cos(curTheta)) * p_Radius;
                var lowerOrbitPivot = innerPivotLow + orbitOffset;
                var upperOrbitPivot = innerPivotHigh + orbitOffset;
                Debug.DrawLine(lowerOrbitPivot, upperOrbitPivot, p_Color, p_Duration);
            }
        }
        
        #endregion
    }
}
#endif