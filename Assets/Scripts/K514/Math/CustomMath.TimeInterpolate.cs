using UnityEngine;

namespace BlackAm
{
    public static partial class CustomMath
    {
        /// <summary>
        /// 선형구간 [p_Start, p_End]으로 구성된 실수공간을 시구간 p_ElapsedTime * p_ReversedInterval : [0, 1]의 값에
        /// 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float LinearInterpolate(float p_Start, float p_End, float p_ElapsedTime, float p_ReversedInterval)
        {
            return DistanceInterpolate(p_Start, p_End - p_Start, p_ElapsedTime, p_ReversedInterval);
        }
        
        /// <summary>
        /// 선형구간 [p_Start, p_End]으로 구성된 실수공간을 시구간 ProgressTimer.progress : [0, 1]의 값에
        /// 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float LinearInterpolate(float p_Start, float p_End, ProgressTimer p_Timer)
        {
            return DistanceInterpolate(p_Start, p_End - p_Start, p_Timer);
        }
        
        /// <summary>
        /// 선형구간 [p_Start, p_Start + p_Distance]으로 구성된 실수공간을 시구간 p_ElapsedTime * p_ReversedInterval : [0, 1]의 값에
        /// 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float DistanceInterpolate(float p_Start, float p_Distance, float p_ElapsedTime, float p_ReversedInterval)
        {
            var lerp = p_ElapsedTime * p_ReversedInterval;
            return p_Distance * lerp + p_Start;
        }
        
        /// <summary>
        /// 선형구간 [p_Start, p_Start + p_Distance]으로 구성된 실수공간을 시구간 ProgressTimer.progress : [0, 1]의 값에
        /// 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float DistanceInterpolate(float p_Start, float p_Distance, float p_ProgressRate)
        {
            return p_Distance * p_ProgressRate + p_Start;
        }
        
        /// <summary>
        /// 선형구간 [p_Start, p_Start + p_Distance]으로 구성된 실수공간을 시구간 ProgressTimer.progress : [0, 1]의 값에
        /// 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float DistanceInterpolate(float p_Start, float p_Distance, ProgressTimer p_Timer)
        {
            return p_Distance * p_Timer.ProgressRate + p_Start;
        }
        
        /// <summary>
        /// 좌표구간 [p_StartPos, p_EndPos]으로 구성된 3차원공간을 시구간[0, 1]의 값에 의해 추상화된 좌표로 리턴하는 메서드
        /// </summary>
        public static Vector3 LinearInterpolate(Vector3 p_StartPos, Vector3 p_EndPos, float p_DeltaTime01)
        {
            return p_DeltaTime01 * p_EndPos + (1f - p_DeltaTime01) * p_StartPos;
        }
                
        /// <summary>
        /// 좌표구간 [p_StartPos, p_EndPos]으로 구성된 3차원공간을 시구간 ProgressTimer.progress : [0, 1]의 값에 의해 추상화된 좌표로 리턴하는 메서드
        /// </summary>
        public static Vector3 LinearInterpolate(Vector3 p_StartPos, Vector3 p_EndPos, ProgressTimer p_Timer)
        {
            return LinearInterpolate(p_StartPos, p_EndPos, p_Timer.ProgressRate);
        }
        
        /// <summary>
        /// 궤도구간 [p_Interval : 최대 타원 너비, p_Height : 최대 타원 높이]으로 구성된 타원궤도의 높이에 대한 실수 공간을
        /// 시구간[0, 1]의 값에 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float ElipseHeightInterpolate(float p_Interval, float p_Height, float p_DeltaTime01)
        {
            return 2f * p_Height * Mathf.Sqrt(p_Interval * p_DeltaTime01 - p_DeltaTime01 * p_DeltaTime01) / p_Interval;
        }

        /// <summary>
        /// 궤도구간 [p_Interval : 최대 타원 너비, p_Height : 최대 타원 높이]으로 구성된 타원궤도의 높이에 대한 실수 공간을
        /// 시구간 ProgressTimer.progress : [0, 1]의 값에 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static float ElipseHeightInterpolate(float p_Interval, float p_Height, ProgressTimer p_Timer)
        {
            return ElipseHeightInterpolate(p_Interval, p_Height, p_Timer.ProgressRate);
        }
        
        /// <summary>
        /// BezierPreset에 의해 기술되는 베지어 곡선 궤도 상의 좌표를 시구간 p_ElapsedTime * p_ReversedInterval : [0, 1]에 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static Vector3 BezierCurveInterpolation(this CubicBezierPreset p_Preset, float p_ElapsedTime, float p_ReversedInterval)
        {
            p_Preset.SyncPivotSet();
            var pivotGroup = p_Preset.calculatePivotGroup;
            var pivotCount = pivotGroup.Length;
            for (int j = pivotCount - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    var tryVector = Vector3.zero;
                    tryVector.x = DistanceInterpolate(pivotGroup[i].x, pivotGroup[i + 1].x - pivotGroup[i].x, p_ElapsedTime,
                        p_ReversedInterval);
                    tryVector.y = DistanceInterpolate(pivotGroup[i].y, pivotGroup[i + 1].y - pivotGroup[i].y, p_ElapsedTime,
                        p_ReversedInterval);
                    tryVector.z = DistanceInterpolate(pivotGroup[i].z, pivotGroup[i + 1].z - pivotGroup[i].z, p_ElapsedTime,
                        p_ReversedInterval);
                    pivotGroup[i] = tryVector;
                }
            }
            return pivotGroup[0];
        }

        /// <summary>
        /// BezierPreset에 의해 기술되는 베지어 곡선 궤도 상의 좌표를 시구간 ProgressTimer.progress : [0, 1]에 의해 추상화된 값으로 리턴하는 메서드
        /// </summary>
        public static Vector3 BezierCurveInterpolation(this CubicBezierPreset p_Preset, float p_ProgressRate)
        {
            p_Preset.SyncPivotSet();
            var pivotGroup = p_Preset.calculatePivotGroup;
            var pivotCount = pivotGroup.Length;
            for (int j = pivotCount - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    var tryVector = Vector3.zero;
                    tryVector.x = DistanceInterpolate(pivotGroup[i].x, pivotGroup[i + 1].x - pivotGroup[i].x, p_ProgressRate);
                    tryVector.y = DistanceInterpolate(pivotGroup[i].y, pivotGroup[i + 1].y - pivotGroup[i].y, p_ProgressRate);
                    tryVector.z = DistanceInterpolate(pivotGroup[i].z, pivotGroup[i + 1].z - pivotGroup[i].z, p_ProgressRate);
                    pivotGroup[i] = tryVector;
                }
            }
            return pivotGroup[0];
        }
    }
}