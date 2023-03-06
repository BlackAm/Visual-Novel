using System;
using System.Collections.Generic;
using UnityEngine;
 
namespace k514
{
    public static class UnitTool
    {
        #region <Const>

        public const float __CombatInfo_ValidTime_UpperBound = 15f;

        #endregion
        
        #region <Fields>
         
        public const int __PLAYER_PRIORITY = 500;

        #endregion
 
        #region <Enum>
        
        /// <summary>
        /// 유닛의 전투 상태 타입
        /// </summary>
        public enum UnitCombatInfoType
        {
            /// <summary>
            /// 경직 상태임
            /// </summary>
            Stuck,
            
            /// <summary>
            /// 해당 유닛을 가장 최근에 공격함
            /// </summary>
            LastAttack,
            
            /// <summary>
            /// 해당 유닛이 가장 최근에 공격함
            /// </summary>
            LastStrike,
        }
        
        public static UnitCombatInfoType[] UnitCombatInfoTypeEnumerator;
        
        [Flags]
        public enum UnitGroupRelateType
        {
            None = 0,
            Enemy = 1 << 0,
            Ally = 1 << 1,
            Neutral = 1 << 2,
        }
        
        [Flags]
        public enum UnitAuthorityFlag
        {
            /// <summary>
            /// 권한 없음
            /// </summary>
            None = 0,
             
            /// <summary>
            /// 포식자 권한
            /// 해당 권한을 가지는 유닛들은 이동시 다른 유닛과의 거리를 갱신시키며
            /// 해당 유닛을 기준으로 일정 반경 내에 존재하는 유닛들을 활성화 시킨다.
            /// </summary>
            Predator = 1 << 0,
             
            /// <summary>
            /// 플레이어 권한
            /// 기본적으로 하나의 클라이언트에 동시에 존재할 수 있는 플레이어는 하나 뿐이며,
            /// 시스템은 해당 권한을 가지는 유저를 참조한다.
            /// </summary>
            Player = 1 << 1,
    
            /// <summary>
            /// 다른 플레이어
            /// </summary>
            OtherPlayer = 1 << 2,
             
            /// <summary>
            /// 파티에 등록되어 있음
            /// </summary>
            PartyComrade = 1 << 3,
            
            /// <summary>
            /// 파티에서 파티장을 맡고 있음
            /// </summary>
            PartyLeader = 1 << 4,
            
            /// <summary>
            /// 특정한 작업을 수행한 후 파기되는 유닛 권한 타입
            /// </summary>
            Actor = 1 << 5,
            
            /// <summary>
            /// 해당 유닛이 어떤 파티에 포함되어 있음
            /// </summary>
            PartyMember = PartyComrade | PartyLeader,
            
            /// <summary>
            /// 다른 유닛의 거리 정보를 갱신시키는 권한 타입
            /// </summary>
            DistanceEventSender = Predator | Player,
            
            /// <summary>
            /// 모든 플레이어
            /// </summary>
            EveryPlayer = Player | OtherPlayer,
        }
        
        public static UnitAuthorityFlag[] UnitAuthorityFlagEnumerator;
        
        [Flags]
        public enum UnitAttributeType
        {
            /// <summary>
            /// 무속성 공격 타입
            /// </summary>
            None = 0,
             
            // 이하, 속성 공격 타입
            Fire = 1 << 0,
            Water = 1 << 1,
            Light = 1 << 2,
            Darkness = 1 << 3,
        }

        /// <summary>
        /// 유닛 속성타입 반복자
        /// </summary>
        public static UnitAttributeType[] _UnitAttributeTypeEnumerator;
        
        [Flags]
        public enum UnitEnvironmentSoundType
        {
            /// <summary>
            /// 해당 유닛은 소리를 내지 않음
            /// </summary>
            None = 0,
            
            /// <summary>
            /// 해당 유닛은 특정 상황에서 전용 음성 대사를 재생함
            ///
            /// * 예시
            ///
            ///     - 적 첫 조우시
            ///     - 오랜기간 입력이 없을 시
            ///     - 클릭 혹은 터치 시
            ///     - 좌우로 빠르게 방향을 바꿀 시
            ///     - 스킬 시전, 크리티컬, 피격, 사망 같은 전투 이벤트
            ///     - 아이템 획득이나 보스 처치 등의 던전 이벤트
            ///     - 우편, 파티 신청, 파티 오더 등의 시스템 이벤트
            /// 
            /// </summary>
            Serifu = 1 << 0,
            
            /// <summary>
            /// 해당 유닛은 이동시 발소리를 냄
            /// </summary>
            FootStep = 1 << 1,
            
            /// <summary>
            /// 해당 유닛은 피격 및 타격 전용 사운드를 재생함
            /// </summary>
            Combat = 1 << 2,

            /// <summary>
            /// 해당 플래그 보유시, 어떤 음성도 재생하지 않음
            /// </summary>
            Mute = 1 << 10,
        }
        
        public enum UnitSkinType
        {
            None = 0,
            Leather,
            Metal,
        }
         
        /// <summary>
        /// 유닛을 기준으로 벡터 연산을 수행할 때 사용하는 연산 타입
        /// </summary>
        public enum UnitAddForceType
        {
            /// <summary>
            /// 공격자로부터 타겟 방향으로 힘이 작용한다.
            /// 타겟이 없다면 StrikerForward 로 동작한다.
            /// </summary>
            TargetDirection,
            
            /// <summary>
            /// 공격 적중 당시의 공격자 기준으로 힘이 작용한다.
            /// </summary>
            StrikerForward,
            
            /// <summary>
            /// 현재 모션의 시작 아핀 값을 기준으로 힘이 작용한다.
            /// </summary>
            CachedAnimationMotionAffine,
            
            /// <summary>
            /// 공격자와 상관없이 절대 벡터로서 힘이 작용한다.
            /// </summary>
            WorldForce,
            
            /// <summary>
            /// 해당 타격 판정의 시작점을 기준으로 힘이 작용한다.
            /// </summary>
            HitStartPosition,
            
            /// <summary>
            /// 해당 타격 판정의 현재 기준점을 기준으로 힘이 작용한다.
            /// </summary>
            HitPivotPosition,
            
            /// <summary>
            /// 해당 타격 판정이 이동하는 방향을 기준으로 힘이 작용한다.
            /// </summary>
            MultiHitMoveDirection,
        }

        [Flags]
        public enum UnitAddForceProcessType
        {
            None = 0,
            BoundDistance = 1 << 0,
            Assemble = 1 << 1,
            Filter = 1 << 2,
        }
        
        public static UnitAddForceProcessType[] UnitAddForceProcessTypeEnumerator;

        [Flags]
        public enum UnitAddForceFilterEventType
        {
            None = 0,
            Drawing = 1 << 0,
            HitUnit = 1 << 1,
        }
        
        public static UnitAddForceFilterEventType[] UnitAddForceFilterEventTypeEnumerator;

        [Flags]
        public enum UnitStampResultFlag
        {
            None = 0,
            Overlapped = 1 << 0,
            UnitStamped = 1 << 1,
            TerrainStamped = 1 << 2,
            ObstacleStamped = 1 << 3,
        }

        public enum UnitMoveActionType
        {
            /// <summary>
            /// 기본 컨트롤 방식
            /// 1. 지정한 방향을 유닛이 즉시 바라보고, 해당 방향으로 외력을 받음
            /// </summary>
            Default,
            
            /// <summary>
            /// 포커스 유닛에 시점을 고정하며 멀어지는 컨트롤 방식
            /// 1. 현재 바라보는 방향 반대로 이동하려고 하면, 회전도를 바꾸지 않고 이동함.
            /// 2. 나머지는 Default 모드와 동일하게 동작함
            /// </summary>
            FixingFocus,
        }

        #endregion
 
        #region <Constructor>
 
        static UnitTool()
        {
            UnitAddForceProcessTypeEnumerator = SystemTool.GetEnumEnumerator<UnitAddForceProcessType>(SystemTool.GetEnumeratorType.ExceptNone);
            UnitAddForceFilterEventTypeEnumerator = SystemTool.GetEnumEnumerator<UnitAddForceFilterEventType>(SystemTool.GetEnumeratorType.ExceptNone);
            UnitCombatInfoTypeEnumerator = SystemTool.GetEnumEnumerator<UnitCombatInfoType>(SystemTool.GetEnumeratorType.ExceptNone);
            _UnitAttributeTypeEnumerator = SystemTool.GetEnumEnumerator<UnitAttributeType>(SystemTool.GetEnumeratorType.ExceptNone);
        }
 
        #endregion
 
        #region <Methods>

        public static bool IsInteractValid(this Unit p_TargetUnit, Unit.UnitStateType p_UnitStateFilterMask)
        {
            return p_TargetUnit.IsValid() && !p_TargetUnit.HasState_Or(p_UnitStateFilterMask);
        }

        public static bool HasUnitLayer(this GameObject p_Target)
        {
            var tryLayer = p_Target.layer;

            return tryLayer == GameManager.Unit_LayerA
                   || tryLayer == GameManager.Unit_LayerB
                   || tryLayer == GameManager.Unit_LayerC
                   || tryLayer == GameManager.Unit_LayerD
                   || tryLayer == GameManager.Unit_LayerE
                   || tryLayer == GameManager.Unit_LayerF
                   || tryLayer == GameManager.Unit_LayerG
                   || tryLayer == GameManager.Unit_LayerH;
        }
         
        public static bool IsUnitLayer(this GameManager.GameLayerType p_Target)
        {
            return p_Target == GameManager.GameLayerType.UnitA
                   || p_Target == GameManager.GameLayerType.UnitB
                   || p_Target == GameManager.GameLayerType.UnitC
                   || p_Target == GameManager.GameLayerType.UnitD
                   || p_Target == GameManager.GameLayerType.UnitE
                   || p_Target == GameManager.GameLayerType.UnitF
                   || p_Target == GameManager.GameLayerType.UnitG
                   || p_Target == GameManager.GameLayerType.UnitH;
        }

        public static Vector3 GetForceVector(Unit p_Trigger, bool p_TriggerValid, UnitAddForceType p_HitForceType, Vector3 p_ForceVector)
        {
            var focus = p_Trigger.FocusNode;
            return GetForceVector(focus, p_Trigger, p_TriggerValid, default, p_HitForceType, p_ForceVector);
        }

        public static Vector3 GetForceVector(Unit p_Target, Unit p_Trigger, bool p_TriggerValid, UnitAddForcePreset p_HitVariablePreset, UnitAddForceType p_HitForceType, Vector3 p_ForceVector)
        {
            var hasTarget = p_Target.IsValid();
            switch (p_HitForceType)
            {
                case UnitAddForceType.CachedAnimationMotionAffine:
                {
                    if (p_TriggerValid)
                    {
                        var cachedAffine = p_Trigger._AnimationObject.CachedMasterNodeUV;
                        return cachedAffine.TransformVector(p_ForceVector);
                    }
                    else
                    {
                        return Vector3.zero;
                    }
                }
                case UnitAddForceType.WorldForce:
                {
                    return p_ForceVector;
                }
                case UnitAddForceType.HitStartPosition:
                {
                    if (hasTarget)
                    {
                        var masterAffine = p_Target._Transform;
                        var hitStartPosition = p_HitVariablePreset.ForceStartPosition;
                        var forward = hitStartPosition.GetDirectionUnitVectorTo(masterAffine.position);
                        var up = masterAffine.up;
                        var right = Vector3.Cross(up, forward);
                        
                        return p_ForceVector.x * right
                               + p_ForceVector.y * up
                               + p_ForceVector.z * forward;
                    }
                    else
                    {
                        goto case UnitAddForceType.StrikerForward;
                    }
                }
                case UnitAddForceType.HitPivotPosition:
                {
                    if (hasTarget)
                    {
                        var masterAffine = p_Target._Transform;
                        var hitPivotPosition = p_HitVariablePreset.ForcePivotPosition;
                        var forward = hitPivotPosition.GetDirectionUnitVectorTo(p_Target.GetAttachPosition(Unit.UnitAttachPoint.BoneCenterNode));
                        var up = masterAffine.up;
                        var right = Vector3.Cross(up, forward);
                        
                        return p_ForceVector.x * right
                               + p_ForceVector.y * up
                               + p_ForceVector.z * forward;
                    }
                    else
                    {
                        goto case UnitAddForceType.StrikerForward;
                    }
                }
                case UnitAddForceType.MultiHitMoveDirection:
                {
                    if (hasTarget)
                    {
                        var masterAffine = p_Target._Transform;
                        var forward = p_HitVariablePreset.ForceGradientDirection;
                        var up = masterAffine.up;
                        var right = Vector3.Cross(up, forward);
                        
                        return p_ForceVector.x * right
                               + p_ForceVector.y * up
                               + p_ForceVector.z * forward;
                    }
                    else
                    {
                        goto case UnitAddForceType.StrikerForward;
                    }
                }
                case UnitAddForceType.TargetDirection:
                {
                    if (p_TriggerValid && hasTarget)
                    {
                        var masterAffine = p_Trigger._Transform;
                        var forward = masterAffine.GetDirectionUnitVectorTo(p_Target._Transform);
                        var up = masterAffine.up;
                        var right = Vector3.Cross(up, forward);
                        
                        return p_ForceVector.x * right
                               + p_ForceVector.y * up
                               + p_ForceVector.z * forward;
                    }
                    else
                    {
                        goto case UnitAddForceType.StrikerForward;
                    }
                }
                default:
                case UnitAddForceType.StrikerForward:
                {
                    if (p_TriggerValid)
                    {
                        return p_Trigger._Transform.TransformDirection(p_ForceVector);
                    }
                    else
                    {
                        return Vector3.up;
                    }
                }
            }
        }

        /// <summary>
        /// 기준 유닛의 마스터 노드를 거슬러 올라가며, p_FindUnit이 있는지 검증하는 논리 메서드
        /// </summary>
        public static bool FindMasterNodeInHierarchy(this Unit p_PivotUnit, Unit p_FindUnit)
        {
            if (ReferenceEquals(null, p_FindUnit))
            {
                return false;
            }
            else
            {
                var pivotUnit = p_PivotUnit;

                while (pivotUnit.IsValid())
                {
                    if (ReferenceEquals(pivotUnit, p_FindUnit))
                    {
                        return true;
                    }
                    else
                    {
                        if (pivotUnit.MasterNode)
                        {
                            pivotUnit = pivotUnit.MasterNode;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;
            }
        }

        #endregion

        #region <Structs>

        public struct UnitAddForcePreset
        {
            #region <Fields>

            /// <summary>
            /// 해당 외력 이벤트의 시작점
            /// </summary>
            public Vector3 ForceStartPosition;
        
            /// <summary>
            /// 해당 외력 이벤트의 현재 작용점
            /// </summary>
            public Vector3 ForcePivotPosition;
        
            /// <summary>
            /// 현재 외력 이벤트의 진행방향
            /// </summary>
            public Vector3 ForceGradientDirection;
        
            #endregion

            #region <Constructors>

            public UnitAddForcePreset(Vector3 p_ForceStartPosition, Vector3 p_ForcePivotPosition, Vector3 p_ForceGradientDirection)
            {
                ForceStartPosition = p_ForceStartPosition;
                ForcePivotPosition = p_ForcePivotPosition;
                ForceGradientDirection = p_ForceGradientDirection;
            }

            #endregion
        }

        #endregion
    }
}