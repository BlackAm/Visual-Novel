using UnityEngine;

namespace k514
{
    public partial class PhysicsTool
    {
        /// <summary>
        /// 레이캐스팅을 수행하여 충돌 정보를 리턴하는 메서드
        /// </summary>
        public static bool CheckAnyObject_RayCast(Vector3 p_StartPos, Vector3 p_TargetPos, int p_LayerMask, bool p_CorrectEpsilon, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var dv = p_StartPos.GetDirectionUnitVectorTo(p_TargetPos);
            var distance = Vector3.Distance(p_StartPos, p_TargetPos);
            return CheckAnyObject_RayCast(p_StartPos, dv, distance, p_LayerMask, p_CorrectEpsilon, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 레이캐스팅을 수행하여 충돌 정보를 리턴하는 메서드, 캐스팅할 거리/방향을 아는 경우
        /// </summary>
        public static bool CheckAnyObject_RayCast(Vector3 p_StartPos, Vector3 p_UV, float p_Distance, int p_LayerMask, bool p_CorrectEpsilon, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            return GetAnyObjectCount_RayCast(p_StartPos, p_UV, p_Distance, p_LayerMask, p_CorrectEpsilon, p_QueryTriggerInteraction) > 0;
        }

        /// <summary>
        /// 레이캐스팅을 수행하여 충돌 정보를 리턴하는 메서드, 캐스팅할 거리/위치를 아는 경우
        /// </summary>
        public static bool CheckAnyObject_RayCast_WithTargetPos(Vector3 p_StartPos, Vector3 p_TargetPos, float p_Distance, int p_LayerMask, bool p_CorrectEpsilon, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var dv = p_StartPos.GetDirectionUnitVectorTo(p_TargetPos);
            return CheckAnyObject_RayCast(p_StartPos, dv, p_Distance, p_LayerMask, p_CorrectEpsilon, p_QueryTriggerInteraction);
        }

        /// <summary>
        /// 레이캐스팅을 수행하여 충돌 정보를 리턴하는 메서드, 캐스팅할 거리/방향을 아는 경우, 리턴값이 정수인 경우
        /// </summary>
        public static int GetAnyObjectCount_RayCast(Vector3 p_StartPos, Vector3 p_UV, float p_Distance, int p_LayerMask, bool p_CorrectEpsilon, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            int hitCount = Physics.RaycastNonAlloc(
                p_CorrectEpsilon ? p_StartPos - CustomMath.Epsilon * p_UV : p_StartPos, 
                p_UV, _NonAllocRayCast, 
                p_CorrectEpsilon ? p_Distance + CustomMath.Epsilon : p_Distance, 
                p_LayerMask, p_QueryTriggerInteraction);

#if UNITY_EDITOR
            if (Application.isPlaying && CustomDebug.DrawRayPhysicsCheck)
            {
                var targetPos = p_StartPos + p_Distance * p_UV;
                CustomDebug.DrawArrow(p_StartPos, targetPos, 0.1f, Color.blue, 1f);
                CustomDebug.DrawCircle(targetPos, 0f, 0.1f, p_StartPos.GetDirectionUnitVectorTo(targetPos), Color.red, 27, 1f);
            }
#endif
            return hitCount;
        }

        /// <summary>
        /// 레이캐스팅을 수행하여 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 거리를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 최대 거리를 리턴한다.
        /// </summary>
        public static (bool, float) GetNearestObjectDistance_RayCast(Vector3 p_Start, Vector3 p_UV, float p_MaxDistance, int p_LayerMask)
        {
            int hitCount = Physics.RaycastNonAlloc(p_Start, p_UV, _NonAllocRayCast, p_MaxDistance, p_LayerMask);
            if (hitCount > 0)
            {
                var resultDistance = p_MaxDistance;
                for (int i = 0; i < hitCount; i++)
                {
                    var targetRayCastHit = _NonAllocRayCast[i];
                    var targetDistance = targetRayCastHit.distance;
                    if (targetDistance < resultDistance)
                    {
                        resultDistance = targetDistance;
                    }
                }

#if UNITY_EDITOR
                if (Application.isPlaying && CustomDebug.DrawPivot)
                {
                    var endPos = p_Start + p_UV * resultDistance;
                    CustomDebug.DrawArrow(p_Start, endPos, 1f, Color.blue, 10f);
                    CustomDebug.DrawCircle(endPos, 0f, 1.5f, p_UV, Color.red, 27, 10f);
                }
#endif
                return (true, resultDistance);
            }
            else
            {
                return (false, p_MaxDistance);
            }
        }

        /// <summary>
        /// 레이캐스팅을 수행하여 가장 가까이에 있는 충돌 정보를 리턴하는 메서드
        /// </summary>
        public static (bool, RaycastHit) GetNearestObject_RayCast(Ray p_Ray, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            int hitCount = Physics.RaycastNonAlloc(p_Ray, _NonAllocRayCast, p_MaxDistance, p_LayerMask, p_QueryTriggerInteraction);
            if (hitCount > 0)
            {
                var result = default(int);
                var tryDistance = p_MaxDistance;
                for (int i = 0; i < hitCount; i++)
                {
                    var targetRayCastHit = _NonAllocRayCast[i];
                    var targetDistance = targetRayCastHit.distance;
                    if (targetDistance < tryDistance)
                    {
                        tryDistance = targetDistance;
                        result = i;
                    }
                }
#if UNITY_EDITOR
                if (Application.isPlaying && CustomDebug.DrawPivot)
                {
                    var startPos = p_Ray.origin;
                    var uv = p_Ray.direction;
                    var endPos = startPos + uv * tryDistance;
                    CustomDebug.DrawArrow(startPos, endPos, 1f, Color.blue, 10f);
                    CustomDebug.DrawCircle(endPos, 0f, 1.5f, uv, Color.red, 27, 10f);
                }
#endif
                return (true, _NonAllocRayCast[result]);
            }
            else
            {
                
#if UNITY_EDITOR
                if (Application.isPlaying && CustomDebug.DrawPivot)
                {
                    var startPos = p_Ray.origin;
                    var uv = p_Ray.direction;
                    var endPos = startPos + uv * p_MaxDistance;
                    CustomDebug.DrawArrow(startPos, endPos, 1f, Color.blue, 10f);
                }
#endif
                return default;
            }
        }
        
        /// <summary>
        /// 레이캐스팅을 수행하여 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 위치를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 시작 지점을 리턴한다.
        /// 3번째 파라미터가 true인 경우, 방향벡터를 기준으로 충돌 검증 시작 위치를 뒤로 당겨서 수행한다.
        /// </summary>
        public static (bool, Vector3) GetNearestObjectPosition_RayCast(Vector3 p_Start, Vector3 p_UV, bool p_CorrectStartPos, float p_MaxDistance, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var startPos = p_CorrectStartPos ? p_Start - p_UV * p_MaxDistance : p_Start;
            var doubleDistance = p_CorrectStartPos ? p_MaxDistance * 2f : p_MaxDistance;
            var hitCount = Physics.RaycastNonAlloc(startPos, p_UV, _NonAllocRayCast, doubleDistance, p_LayerMask, p_QueryTriggerInteraction);
            
            if (hitCount > 0)
            {
                var tryDistance = doubleDistance;
                var resultValid = false;
                var resultPosition = p_Start;
                for (int i = 0; i < hitCount; i++)
                {
                    var targetRayCastHit = _NonAllocRayCast[i];
                    var targetDistance = targetRayCastHit.distance;
                    if (targetDistance < tryDistance)
                    {
                        resultValid = true;
                        tryDistance = targetDistance;
                        resultPosition = targetRayCastHit.point;
                    }
                }

#if UNITY_EDITOR
                    if (resultValid && Application.isPlaying && CustomDebug.DrawPivot)
                    {
                        var endPos = p_Start + p_UV * tryDistance;
                        CustomDebug.DrawArrow(p_Start, endPos, 1f, Color.blue, 10f);
                        CustomDebug.DrawCircle(endPos, 0f, 1.5f, p_UV, Color.red, 27, 10f);
                    }
#endif
                return (resultValid, resultPosition);
            }
            else
            {
#if UNITY_EDITOR
                if (Application.isPlaying && CustomDebug.DrawPivot)
                {
                    var endPos = p_Start + p_UV * p_MaxDistance;
                    CustomDebug.DrawArrow(p_Start, endPos, 1f, Color.blue, 10f);
                }
#endif
                return (false, p_Start);
            }
        }
        
        /// <summary>
        /// 레이캐스팅을 수행하여 지정한 경로 내에 가장 가까운 충돌 오브젝트와의 위치를 리턴하는 메서드
        /// 충돌이 발생하지 않았다면 파라미터의 시작 지점을 리턴한다.
        /// 3번째 파라미터가 true인 경우, 방향벡터를 기준으로 충돌 검증 시작 위치를 뒤로 당겨서 수행한다.
        /// 가장 가까운 오브젝트가 선정되었고 해당 오브젝트의 레이어가 4번째 파라미터에 포함되는 경우 거짓을 리턴한다.
        /// </summary>
        public static (bool, Vector3) GetNearestObjectPosition_RayCast(Vector3 p_Start, Vector3 p_UV, bool p_CorrectStartPos, float p_MaxDistance, int p_LayerMask, int p_ExceptLayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            var startPos = p_CorrectStartPos ? p_Start - p_UV * p_MaxDistance : p_Start;
            var doubleDistance = p_CorrectStartPos ? p_MaxDistance * 2f : p_MaxDistance;
            var hitCount = Physics.RaycastNonAlloc(startPos, p_UV, _NonAllocRayCast, doubleDistance, p_LayerMask, p_QueryTriggerInteraction);
            
            if (hitCount > 0)
            {
                var tryDistance = doubleDistance;
                var resultValid = false;
                var resultPosition = p_Start;
                GameObject tryObject = null;
                
                for (int i = 0; i < hitCount; i++)
                {
                    var targetRayCastHit = _NonAllocRayCast[i];
                    var targetDistance = targetRayCastHit.distance;
                    if (targetDistance < tryDistance)
                    {
                        tryObject = targetRayCastHit.transform.gameObject;
                        resultValid = true;
                        tryDistance = targetDistance;
                        resultPosition = targetRayCastHit.point;
                    }
                }

                if ((1 << tryObject.layer | p_ExceptLayerMask) != 0)
                {
                    resultValid = false;
                }

#if UNITY_EDITOR
                    if (resultValid && Application.isPlaying && CustomDebug.DrawPivot)
                    {
                        var endPos = p_Start + p_UV * tryDistance;
                        CustomDebug.DrawArrow(p_Start, endPos, 1f, Color.blue, 10f);
                        CustomDebug.DrawCircle(endPos, 0f, 1.5f, p_UV, Color.red, 27, 10f);
                    }
#endif
                return (resultValid, resultPosition);
            }
            else
            {
#if UNITY_EDITOR
                if (Application.isPlaying && CustomDebug.DrawPivot)
                {
                    var endPos = p_Start + p_UV * p_MaxDistance;
                    CustomDebug.DrawArrow(p_Start, endPos, 1f, Color.blue, 10f);
                }
#endif
                return (false, p_Start);
            }
        }
        
        /// <summary>
        /// 지정한 위치를 기준으로 Y값만 보정하여 로직 상한 높이에서 연직아래방향으로 레이캐스트를 수행하여 충돌이 검증된 가장 높은 좌표를 리턴한다.
        /// 충돌이 발생하지 않았다면 시작 위치를 리턴한다.
        /// </summary>
        public static (bool, Vector3) GetHighestObjectPosition_RayCast(Vector3 p_Start, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            return GetNearestObjectPosition_RayCast(p_Start, Vector3.down, true, UnitInteractManager.__Out_Of_Range_Half, p_LayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 지정한 위치를 기준으로 Y값만 보정하여 로직 상한 높이에서 연직아래방향으로 레이캐스트를 수행하여 충돌이 검증된 가장 높은 좌표를 리턴한다.
        /// 충돌이 발생하지 않았다면 시작 위치를 리턴한다.
        /// </summary>
        public static (bool, Vector3) GetHighestObjectPosition_RayCast(Vector3 p_Start, int p_LayerMask, int p_ExceptLayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            return GetNearestObjectPosition_RayCast(p_Start, Vector3.down, true, UnitInteractManager.__Out_Of_Range_Half, p_LayerMask, p_ExceptLayerMask, p_QueryTriggerInteraction);
        }
        
        /// <summary>
        /// 지정한 위치를 기준으로 연직아래방향으로 레이캐스트를 수행하여 충돌이 검증된 가장 높은 좌표를 리턴한다.
        /// 충돌이 발생하지 않았다면 시작 위치를 리턴한다.
        /// 충돌이 검증된 경우, 레이캐스트 시작지점이 다른 컬라이더와 겹쳐있는지 한번 더 검증한다.
        /// </summary>
        public static (bool, Vector3) GetHighestObjectPosition_CheckOverlap_RayCast(Vector3 p_Start, int p_LayerMask, QueryTriggerInteraction p_QueryTriggerInteraction)
        {
            if (GetBoxOverlap(p_Start + CustomMath.Epsilon * Vector3.up, p_LayerMask))
            {
                return (false, p_Start);
            }
            else
            {
                return GetNearestObjectPosition_RayCast(p_Start + CustomMath.Epsilon * Vector3.up, Vector3.down, false, UnitInteractManager.__Out_Of_Range_Half, p_LayerMask, p_QueryTriggerInteraction);
            }
        }
    }
}